import random
from Errors import Errors
from RSA import RSA

class Voting:
    topId=0
    defaultPassword=""

    class Response:
        def __init__(self,value="",errorCode=""):
            self.ok=True
            self.errorCode = errorCode
            self.value = value

    def __init__(self,userDatabase):
        self.owner = ""
        self.voteTitle = "Vote Title"
        self.password = Voting.defaultPassword
        self.anonymous = False
        self.mutuallyExclusive = False
        self.allowUnregisteredUsers = False
        self.maxOptions = 1
        self.voteOptions:[str] = []
        self.voteResults:[str] = []
        self.voteClients:[str] = []
        self.voteSignatures:[str] = []
        self.userDatabase = userDatabase
        self.rsa = RSA()
        self.usedSignatures = []
        self.ballotIds = [30]
        self.usersWithSignedBallots:[str] = []
        self.usersBlindSignedIds:[str] = []
        self.ballotsCasted = []

    def GenerateNewId(self):
        self.id = Voting.topId
        Voting.topId += 1

    def str2bool(self,v):
        return v.lower() in ("yes", "true", "t", "1")

    def Decode(self,encoded):
        msg = encoded
        i = 0
        
        while msg[i]!=":":
            i += 1
        self.owner = msg[0:i]
        if self.owner=="":
            return Voting.Response(None,Errors.votingHasNoOwner)
        msg = msg[i+1:]

        i = 0
        while msg[i]!=":":
            i += 1
        self.voteTitle = msg[0:i]
        if self.voteTitle=="":
            return Voting.Response(None,Errors.votingHasNoTitle)
        msg = msg[i+1:]

        i = 0
        while msg[i]!=":":
            i += 1
        self.password = msg[0:i]
        msg = msg[i+1:]

        i = 0
        while msg[i]!=":":
            i += 1
        self.anonymous = self.str2bool(msg[0:i])
        msg = msg[i+1:]

        i = 0
        while msg[i]!=":":
            i += 1
        self.mutuallyExclusive = self.str2bool(msg[0:i])
        msg = msg[i+1:]

        i = 0
        while msg[i]!=":":
            i += 1
        self.allowUnregisteredUsers = self.str2bool(msg[0:i])
        msg = msg[i+1:]

        i = 0
        while msg[i]!=":":
            i += 1
        self.maxOptions = int(msg[0:i])
        msg = msg[i+1:]

        i = 0
        while msg[i]!=":":
            i += 1
        count = int(msg[0:i])
        msg = msg[i+1:]
        for j in range (0,count):
            i =0
            while msg[i]!=":":
                i += 1
                if(i>len(msg)):
                    break
            self.voteOptions.append(msg[0:i])
            msg = msg[i+1:]
        if (count==0):
            msg = msg[1:]
        i = 0
        while msg[i]!=":":
            i += 1
        count = int(msg[0:i])
        msg = msg[i+1:]
        for j in range (0,count):
            i =0
            while msg[i]!=":":
                i += 1
                if(i>len(msg)):
                    break
            self.voteResults.append(msg[0:i])
            msg = msg[i+1:]
        if (count==0):
            msg = msg[1:]
        i = 0
        while msg[i]!=":":
            i += 1
        count = int(msg[0:i])
        msg = msg[i+1:]
        for j in range (0,count):
            i =0
            while msg[i]!=":":
                i += 1
                if(i>len(msg)):
                    break
            self.voteResults.append(msg[0:i])
            msg = msg[i+1:]
        return Voting.Response(True,None)

    def ValidateAccess(self,username,token,password):
        if (username == None or username == "") and self.allowUnregisteredUsers:
            if self.password!=password:
                return Voting.Response(None,Errors.wrongPassword)
            else:
                return Voting.Response(True,None)
        elif (username == None or username == ""):
            return Voting.Response(None,Errors.onlyLoggedInUsers)
        if self.userDatabase.ValidateUserToken(username,token)==False:
            if self.allowUnregisteredUsers == False:
                if self.password!=password:
                    return Voting.Response(None,Errors.wrongPassword)
        else :
            if self.password!=password:
                if username!=self.owner:
                    return Voting.Response(None,Errors.passwordRequired)
                if username == self.owner:
                    if self.userDatabase.ValidateUserToken(username,token)==False:
                        return Voting.Response(None,Errors.accountNotValid)
        return Voting.Response(True,None)

    def GetEncodedVoting(self,username,token,password):
        result = self.ValidateAccess(username,token,password)
        if result.value==None:
            return Voting.Response(None,result.errorCode)
        stringified = str(self.id) +":"+ self.owner +":"+ self.voteTitle +":"+ str(self.anonymous) +":"+ str(self.mutuallyExclusive)
        stringified += ":"+str(self.allowUnregisteredUsers) 
        stringified += ":"+str(self.maxOptions) 
        stringified += ":"+str(len(self.voteOptions))+":"
        for item in self.voteOptions:
            stringified += item+":"
        stringified += str(len(self.voteResults))+":"
        for item in self.voteResults:
            stringified += item+":"
        stringified += str(len(self.voteClients))+":"
        for i in range(0,len(self.voteClients)):
            stringified += self.voteClients[i]+":"
        stringified = stringified[:-1]
        return Voting.Response(stringified,None)

    def GetBallot(self):
        newId = self.ballotIds[-1]
        newId = newId + 1
        self.ballotIds.append(newId)
        return Voting.Response(newId,None)

    def SignBallot(self,username,token,password,mPrime):
        result = self.ValidateAccess(username,token,password)
        if username == "":
            return Voting.Response(None,Errors.onlyLoggedInUsers)
        if result.value==None:
            return Voting.Response(None,result.errorCode)
        if username in self.usersWithSignedBallots:
            return Voting.Response(None,Errors.youMayNotRecieveAnyMoreBallots)
        sPrime = self.rsa.Sign(int(mPrime))
        self.usersWithSignedBallots.append(username)
        self.usersBlindSignedIds.append(mPrime)
        print("Returned signature:"+str(sPrime))
        return Voting.Response(sPrime,None)

    def GetEncodedVotingBrief(self,username,token,password):
        result = self.ValidateAccess(username,token,password)
        if result.value==None:
            return Voting.Response(None,result.errorCode)
        stringified = str(self.id) +":"+ self.voteTitle
        return Voting.Response(stringified,None)

    def GetEncodedId(self,username,token,password):
        result = self.ValidateAccess(username,token,password)
        if result.value==None:
            return Voting.Response(None,result.errorCode)
        return Voting.Response(self.id,None)
    
    def GetEncodedOwner(self,username,token,password):
        result = self.ValidateAccess(username,token,password)
        if result.value==None:
            return Voting.Response(None,result.errorCode)
        return Voting.Response(self.owner,None)

    def GetEncodedVoteTitle(self,username,token,password):
        result = self.ValidateAccess(username,token,password)
        if result.value==None:
            return Voting.Response(None,result.errorCode)
        return Voting.Response(self.voteTitle,None)

    def GetEncodedVoteAnonymous(self,username,token,password):
        result = self.ValidateAccess(username,token,password)
        if result.value==None:
            return Voting.Response(None,result.errorCode)
        return Voting.Response(self.anonymous,None)
    
    def GetEncodedVoteMutuallyExclusive(self,username,token,password):
        result = self.ValidateAccess(username,token,password)
        if result.value==None:
            return Voting.Response(None,result.errorCode)
        return Voting.Response(self.mutuallyExclusive,None)

    def GetEncodedVoteOptions(self,username,token,password):
        result = self.ValidateAccess(username,token,password)
        if result.value==None:
            return Voting.Response(None,result.errorCode)
        ret = ""
        for option in self.voteOptions:
            ret += option + ":"
        ret = ret [:-1]
        return Voting.Response(ret,None)

    def GetEncodedVoteResults(self,username,token,password):
        result = self.ValidateAccess(username,token,password)
        if result.value==None:
            return Voting.Response(None,result.errorCode)
        ret = ""
        for option in self.voteResults:
            ret += option + ":"
        ret = ret [:-1]
        return Voting.Response(ret,None)

    def GetEncodedVoteClients(self,username,token,password):
        result = self.ValidateAccess(username,token,password)
        if result.value==None:
            return Voting.Response(None,result.errorCode)
        ret = ""
        for option in self.voteClients:
            ret += option + ":"
        ret = ret [:-1]
        return Voting.Response(ret,None)
        
    def GetEncodedVoteSignedClients(self,username,token,password):
        result = self.ValidateAccess(username,token,password)
        if result.value==None:
            return Voting.Response(None,result.errorCode)
        ret = ""
        for item in self.usersWithSignedBallots:
            ret += item + ":"
        ret = ret [:-1]
        return Voting.Response(ret,None)

    def DidAlreadyVote(self,voteClient):
        for client in self.voteClients:
            if client==voteClient:
                return True
        return False

    def CastVotes(self,voteClient,voteResults:[],password,token,signature):
        if self.anonymous:
            if(not signature.isdigit() or not voteClient.isdigit()):
                return Voting.Response(None,Errors.wrongSignature)
            if not self.rsa.Verify(int(signature),int(voteClient)) and self.anonymous == False:
                return Voting.Response(None,Errors.wrongSignature)
            elif self.rsa.Verify(int(signature),int(voteClient)):
                if self.DidAlreadyVote(voteClient):
                    return Voting.Response(None,Errors.alreadyVoted)   
                for result in voteResults:
                    self.voteClients.append(voteClient)
                    self.voteResults.append(result)
                    self.voteSignatures.append(signature)
                return Voting.Response(None,None)
        username = voteClient
        result = self.ValidateAccess(username,token,password)
        if len(voteResults) > self.maxOptions:
            return Voting.Response(None,Errors.tooManyOptionsSelected)
        if result.value==None:
            return Voting.Response(None,result.errorCode)
        if self.DidAlreadyVote(voteClient) and self.allowUnregisteredUsers==False:
            return Voting.Response(None,Errors.alreadyVoted)                
        if self.mutuallyExclusive and len(voteResults)>1:
            return Voting.Response(None,Errors.onlyOneOptionCanBeChosen)  
        for result in voteResults:
            self.voteClients.append(voteClient)
            self.voteResults.append(result)
            self.voteSignatures.append(signature)
        return Voting.Response(None,None)

    def Verify(self,cardId,signature):
        print(self.voteClients)
        print(self.voteResults)
        print(self.voteSignatures)
        for i in range(0, len(self.voteClients)):
            if self.voteClients[i]==cardId:
                print(signature)
                if self.voteSignatures[i] == signature:
                    return Voting.Response("True",None)
                else: 
                    return Voting.Response(None,Errors.wrongSignature)
        return Voting.Response(None,Errors.cardNotFound)
