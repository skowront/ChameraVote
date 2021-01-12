from Voting import Voting
from Errors import Errors

class VotingContainer:
    class Response:
        def __init__(self,value="",errorCode=""):
            self.ok=True
            self.errorCode = errorCode
            self.value = value

    def __init__(self,userDatabase):
        self.votings:[Voting] = []
        self.userDatabase = userDatabase

    def ValidateAccess(self,username,token):
        if (username == None or username == ""):
            return Voting.Response(None,Errors.accountNotValid)
        if self.userDatabase.ValidateUserToken(username,token)==False:
            return Voting.Response(None,Errors.accountNotValid)
        else:
            return Voting.Response(True,None)

    def GetVotingByIdStr (self,id):
        if id.isdigit() == False:
            return VotingContainer.Response(None,Errors.incorrectNumberFormat)
        intId = int(id)
        for voting in self.votings:
            if voting.id == intId:
                return VotingContainer.Response(voting,None)
        return VotingContainer.Response(None,Errors.votingNotFound)

    def GetVotingById (self,id):
        for voting in self.votings:
            if voting.id == id:
                return VotingContainer.Response(voting,None)
        return VotingContainer.Response(None,Errors.votingNotFound)

    def AddVoting (self,voting:Voting,username,token):
        result = self.ValidateAccess(username,token)
        if result.value==None:
            return VotingContainer.Response(result.value,result.errorCode)
        else: 
            self.votings.append(voting)
            return VotingContainer.Response("OK",None)

    def RemoveVoting (self,votingId:int,username,token):
        result = self.ValidateAccess(username,token)
        if result.value==None:
            return VotingContainer.Response(result.value,result.errorCode)
        for item in self.votings:
            if item.id == votingId:
                self.votings.remove(item)
                break
        return VotingContainer.Response(True,None)

    def ClearVotings (self):
        self.votings.clear()

    def GetUserVotings(self,username,token):
        collection:[Voting] = []
        for voting in self.votings:
            if voting.owner == username:
                collection.append(voting)
        if len(collection)>0:
            return VotingContainer.Response(collection,None)
        return VotingContainer.Response(None,Errors.noVotingsMeetRequirements)