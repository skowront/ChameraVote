import random

class Voting:
    topId=0
    defaultPassword=""

    class Messages:
        wrongPassword = "Wrong password!"
        alreadyVoted = "Already voted!"
        onlyOneOptionCanBeChosen = "Only one option may be chosen."
        accountNotValid = "Account is not valid. Try to log in once again."
        passwordRequired = "Password is required to enter this vote."
        onlyLoggedInUsers = "You must be logged in to enter this vote."

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
        self.voteOptions:[str] = []
        self.voteResults:[str] = []
        self.voteClients:[str] = []
        self.userDatabase = userDatabase

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
        msg = msg[i+1:]

        i = 0
        while msg[i]!=":":
            i += 1
        self.voteTitle = msg[0:i]
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
        return self

    def ValidateAccess(self,username,token,password):
        if (username == None or username == "") and self.allowUnregisteredUsers:
            if self.password!=password:
                return Voting.Response(None,Voting.Messages.wrongPassword)
        elif (username == None or username == ""):
            return Voting.Response(None,Voting.Messages.onlyLoggedInUsers)
        if self.userDatabase.ValidateUserToken(username,token)==False:
            if self.password!=password:
                return Voting.Response(None,Voting.Messages.wrongPassword)
        else:
            if self.password!=password:
                if username!=self.owner:
                    return Voting.Response(None,Voting.Messages.passwordRequired)
        return Voting.Response(True,None)

    def GetEncodedVoting(self,username,token,password):
        result = self.ValidateAccess(username,token,password)
        if result.value==None:
            return Voting.Response(None,result.errorCode)
        stringified = str(self.id) +":"+ self.owner +":"+ self.voteTitle +":"+ str(self.anonymous) +":"+ str(self.mutuallyExclusive)
        stringified += ":"+str(self.allowUnregisteredUsers) 
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

    def DidAlreadyVote(self,voteClient):
        for client in self.voteClients:
            if client==voteClient:
                return True
        return False

    def CastVotes(self,voteClient,voteResults:[],password,token):
        username = voteClient
        result = self.ValidateAccess(username,token,password)
        if result.value==None:
            return Voting.Response(None,result.errorCode)
        if self.DidAlreadyVote(voteClient) and self.allowUnregisteredUsers==False:
            return Voting.Response(None,Voting.Messages.alreadyVoted)                
        if self.mutuallyExclusive and len(voteResults)>1:
            return Voting.Response(None,Voting.Messages.onlyOneOptionCanBeChosen)  
        for result in voteResults:
            self.voteClients.append(voteClient)
            self.voteResults.append(result)
        if self.anonymous==True:
            random.shuffle(self.voteResults)
            random.shuffle(self.voteClients)
        return Voting.Response(None,None)