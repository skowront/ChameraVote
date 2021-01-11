import random

class Voting:
    topId=0
    defaultPassword="0"

    class Messages:
        wrongPassword = "Wrong password!"
        alreadyVoted = "Already voted!"
        onlyOneOptionCanBeChosen = "Only one option may be chosen."

    class Response:
        def __init__(self,value="",errorCode=""):
            self.ok=True
            self.errorCode = errorCode
            self.value = value

    def __init__(self):
        self.owner = ""
        self.voteTitle = "Vote Title"
        self.password = Voting.defaultPassword
        self.anonymous = False
        self.mutuallyExclusive = False
        self.voteOptions:[str] = []
        self.voteResults:[str] = []
        self.voteClients:[str] = []

    def GenerateNewId(self):
        self.id = Voting.topId
        Voting.topId += 1

    def GetEncodedVoting(self,password):
        if self.password!=password:
            return Voting.Response(None,Voting.Messages.wrongPassword)
        stringified = str(self.id) +":"+ self.owner +":"+ self.voteTitle +":"+ str(self.anonymous) +":"+ str(self.mutuallyExclusive)
        stringified += ":"+str(len(self.voteOptions))+":"
        for item in self.voteOptions:
            stringified += item+":"
        stringified += str(len(self.voteResults))+":"
        for item in self.voteResults:
            stringified += item+":"
        stringified += str(len(self.voteClients))+":"
        for item in self.voteClients:
            stringified += item+":"
        stringified = stringified[:-1]
        print(stringified)
        return Voting.Response(stringified,None)

    def GetEncodedId(self,password):
        if self.password!=password:
            return Voting.Response(None,Voting.Messages.wrongPassword)
        return Voting.Response(self.id,None)
    
    def GetEncodedOwner(self,password):
        if self.password!=password:
            return Voting.Response(None,Voting.Messages.wrongPassword)
        return Voting.Response(self.owner,None)

    def GetEncodedVoteTitle(self,password):
        if self.password!=password:
            return Voting.Response(None,Voting.Messages.wrongPassword)
        return Voting.Response(self.voteTitle,None)

    def GetEncodedVoteAnonymous(self,password):
        if self.password!=password:
            return Voting.Response(None,Voting.Messages.wrongPassword)
        return Voting.Response(self.anonymous,None)

    
    def GetEncodedVoteMutuallyExclusive(self,password):
        if self.password!=password:
            return Voting.Response(None,Voting.Messages.wrongPassword)
        return Voting.Response(self.mutuallyExclusive,None)

    def GetEncodedVoteOptions(self,password):
        if self.password!=password:
            return Voting.Response(None,Voting.Messages.wrongPassword)
        ret = ""
        for option in self.voteOptions:
            ret += option + ":"
        ret = ret [:-1]
        return Voting.Response(ret,None)

    def GetEncodedVoteResults(self,password):
        if self.password!=password:
            return Voting.Response(None,Voting.Messages.wrongPassword)
        ret = ""
        for option in self.voteResults:
            ret += option + ":"
        ret = ret [:-1]
        return Voting.Response(ret,None)

    def GetEncodedVoteClients(self,password):
        if self.password!=password:
            return Voting.Response(None,Voting.Messages.wrongPassword)
        ret = ""
        for option in self.voteClients:
            ret += option + ":"
        ret = ret [:-1]
        return Voting.Response(ret,None)

    def DidAlreadyVote(self,voteClient):
        for client in self.voteClients:
            if client==voteClient:
                return True
        return False

    def AddVotes(self,voteClient,voteResults:[],password):
        if self.password!=password:
            return Voting.Response(None,Voting.Messages.wrongPassword)
        if self.DidAlreadyVote(voteClient):
            return Voting.Response(None,Voting.Messages.alreadyVoted)
        print(len(voteResults))  
        if self.mutuallyExclusive and len(voteResults)>1:
            return Voting.Response(None,Voting.Messages.onlyOneOptionCanBeChosen)  
        for result in voteResults:
            self.voteClients.append(voteClient)
            self.voteResults.append(result)
        if self.anonymous==True:
            random.shuffle(self.voteResults)
            random.shuffle(self.voteClients)
        return Voting.Response(None,None)

    