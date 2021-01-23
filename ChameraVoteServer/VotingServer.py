import socket
import string
import select

from Logger import Logger
from Voting import Voting
from VotingContainer import VotingContainer
from UserDatabase import UserDatabase
from Errors import Errors
from Configuration import Configuration

class VotingServer:
    maxBufferSize = 1024

    class Response:
        def __init__(self,value="",errorCode=""):
            self.ok=True
            self.errorCode = errorCode
            self.value = value

    def __init__(self,port:int,createExampleVoting:bool):
        self.socket = socket.socket(socket.AF_INET,socket.SOCK_STREAM)
        self.socket.bind(('0.0.0.0',port))
        self.socket.listen(5)
        self.userDatabase = UserDatabase()
        self.userDatabase.Load()
        self.votingContainer = VotingContainer(self.userDatabase)
        if createExampleVoting==True:
            self.CreateExampleVoting()

    def CreateExampleVoting(self):
        exampleVoting = Voting(self.userDatabase)
        exampleVoting.GenerateNewId()
        exampleVoting.allowUnregisteredUsers=True
        exampleVoting.voteOptions = ["yes","no","abstain"]
        exampleVoting.voteTitle = "Test voting"
        exampleVoting.owner = "ts"
        exampleVoting.mutuallyExclusive = True
        #exampleVoting.anonymous = True
        exampleVoting.voteClients=["John","Donald","Mike","Michael","Lenny","Bob","Bobby","Sandy","Elizabeth","Jim","Lucy","Aaron","Julia","Betty"]
        exampleVoting.voteResults=["yes","abstain","abstain","yes","no","yes","abstain","yes","no","abstain","no","no","no","no"]
        exampleVoting.voteSignatures=["0","0","0","0","0","0","0","0","0","0","0","0","0","0"]
        self.votingContainer.votings.append(exampleVoting)
        exampleVoting1 = Voting(self.userDatabase)
        exampleVoting1.GenerateNewId()
        exampleVoting1.voteOptions = ["yes","no","abstain"]
        exampleVoting1.voteTitle = "Test voting"
        exampleVoting1.owner = "ts"
        exampleVoting1.anonymous = True
        exampleVoting1.mutuallyExclusive = True
        self.votingContainer.votings.append(exampleVoting1)
        exampleVoting2 = Voting(self.userDatabase)
        exampleVoting2.GenerateNewId()
        exampleVoting2.allowUnregisteredUsers=False
        exampleVoting2.anonymous=True
        exampleVoting2.voteTitle = "Obiad"
        exampleVoting2.voteOptions=["Żółtko","Kiełbasa","Kluski śląskie","Kompot","Wudeczka","Pierko"]
        exampleVoting2.owner = "ts"
        self.votingContainer.votings.append(exampleVoting2)
        exampleVoting3 = Voting(self.userDatabase)
        exampleVoting3.GenerateNewId()
        exampleVoting3.allowUnregisteredUsers=False
        exampleVoting3.anonymous=True
        exampleVoting3.voteTitle = "Obiad"
        exampleVoting3.voteOptions=["Żółtko","Kiełbasa","Kluski śląskie","Kompot","Wudeczka","Pierko"]
        exampleVoting3.owner = "ts"
        exampleVoting3.maxOptions = 3
        self.votingContainer.votings.append(exampleVoting3)

    def Run(self,lock):
        while True:
            clientsocket,address = self.socket.accept()
            Logger.Log("Connection from: "+str(address[0]))
            ready = select.select([clientsocket],[],[],5)
            if ready[0]:
                data = clientsocket.recv(1024)
                dataDecoded = str(data.decode(encoding='utf-8'))#.decode("utf-8")
                lock.acquire()
                response = self.BuildResponse(dataDecoded)
                lock.release()
                clientsocket.send(response.encode(encoding='utf-8'))
            clientsocket.close()

    def BuildResponse(self,dataDecoded):
        print("VoteServer incoming:")
        print(dataDecoded)
        if(dataDecoded[0:len(Configuration.ApplicationToken)+1]!=Configuration.ApplicationToken+':'):
            return(Errors.applicationUnauthorized)
        dataDecoded = dataDecoded[len(Configuration.ApplicationToken)+1:]
        prefix = ""
        suffix = ""
        if (self.ValidateRequest(dataDecoded)==False):
            prefix ="NOK"               
            return(Errors.wrongRequest)
        response = self.HandleClientRequest(dataDecoded)
        if response.errorCode==None:
            prefix="OK"
            if(response.value==None):
                suffix = "0"
            else:
                suffix = str(response.value)
        else:
            prefix ="NOK"
            suffix = str(response.errorCode)
        total = str(prefix+":"+suffix)
        # while len(total)>VotingServer.maxBufferSize:
        #     msg = total[0:VotingServer.maxBufferSize-1]
        #     msg += '&'
        #     return(msg)
        #     total = total[VotingServer.maxBufferSize-1:]
        return total

    def ValidateRequest(self,message)->bool:
        return True
        msg = message
        for i in range(0,len(msg)):
            if i+1>=len(msg):
                break
            if(msg[i]==':'and msg[i+1]==':'):
                return False
        return True

    def HandleClientRequest(self,message)->Response:
        messageArray = message.split(':')
        msgType = messageArray[0]
        print(messageArray)
        returnValue = ""
        if len(messageArray)<1:
            return VotingServer.Response(None,Errors.wrongRequest)
        elif msgType == "command":
            if len(messageArray)<2:
                return VotingServer.Response(None,Errors.wrongRequest)
            commandName = messageArray[1]
            if commandName == "getVotingById":
                if len(messageArray)<4:
                    return VotingServer.Response(None,Errors.wrongRequest)
                username = messageArray[2]
                token = messageArray[3]
                password = messageArray[4]
                votingId = messageArray[5]
                voting = self.votingContainer.GetVotingByIdStr(votingId)
                if voting.value==None:
                    return VotingServer.Response(voting.value,voting.errorCode)
                result = voting.value.GetEncodedVoting(username,token,password)
                returnValue = VotingServer.Response(result.value,result.errorCode)
                return returnValue

            if commandName == "getBallot":
                if len(messageArray)<3:
                    return VotingServer.Response(None,Errors.wrongRequest)
                votingId = messageArray[2]
                voting = self.votingContainer.GetVotingByIdStr(votingId)
                if voting.value==None:
                    return VotingServer.Response(voting.value,voting.errorCode)
                result = voting.value.GetBallot()
                returnValue = VotingServer.Response(result.value,result.errorCode)
                return returnValue

            if commandName == "getSignedBallot":
                if len(messageArray)<4:
                    return VotingServer.Response(None,Errors.wrongRequest)
                username = messageArray[2]
                token = messageArray[3]
                password = messageArray[4]
                votingId = messageArray[5]
                mPrime = messageArray[6]
                voting = self.votingContainer.GetVotingByIdStr(votingId)
                if voting.value==None:
                    return VotingServer.Response(voting.value,voting.errorCode)
                result = voting.value.SignBallot(username,token,password,mPrime)
                returnValue = VotingServer.Response(result.value,result.errorCode)
                return returnValue

            if commandName == "getTitle":
                if len(messageArray)<4:
                    return VotingServer.Response(None,Errors.wrongRequest)
                username = messageArray[2]
                token = messageArray[3]
                password = messageArray[4]
                votingId = messageArray[5]
                voting = self.votingContainer.GetVotingByIdStr(votingId)
                if voting.value==None:
                    return VotingServer.Response(voting.value,voting.errorCode)
                result = voting.value.GetEncodedVoteTitle(username,token,password)
                returnValue = VotingServer.Response(result.value,result.errorCode)
                return returnValue

            if commandName == "getOptions":
                if len(messageArray)<4:
                    return VotingServer.Response(None,Errors.wrongRequest)
                username = messageArray[2]
                token = messageArray[3]
                password = messageArray[4]
                votingId = messageArray[5]
                voting = self.votingContainer.GetVotingByIdStr(votingId)
                if voting.value==None:
                    return VotingServer.Response(voting.value,voting.errorCode)
                result = voting.value.GetEncodedVoteOptions(username,token,password)
                returnValue = VotingServer.Response(result.value,result.errorCode)
                return returnValue

            if commandName == "getAnonymous":
                if len(messageArray)<4:
                    return VotingServer.Response(None,Errors.wrongRequest)
                username = messageArray[2]
                token = messageArray[3]
                password = messageArray[4]
                votingId = messageArray[5]
                voting = self.votingContainer.GetVotingByIdStr(votingId)
                if voting.value==None:
                    return VotingServer.Response(voting.value,voting.errorCode)
                result = voting.value.GetEncodedVoteAnonymous(username,token,password)
                returnValue = VotingServer.Response(result.value,result.errorCode)
                return returnValue


            if commandName == "getMutuallyExclusive":
                if len(messageArray)<4:
                    return VotingServer.Response(None,Errors.wrongRequest)
                username = messageArray[2]
                token = messageArray[3]
                password = messageArray[4]
                votingId = messageArray[5]
                voting = self.votingContainer.GetVotingByIdStr(votingId)
                if voting.value==None:
                    return VotingServer.Response(voting.value,voting.errorCode)
                result = voting.value.GetEncodedVoteMutuallyExclusive(username,token,password)
                returnValue = VotingServer.Response(result.value,result.errorCode)
                return returnValue

            if commandName == "getOwner":
                if len(messageArray)<4:
                    return VotingServer.Response(None,Errors.wrongRequest)
                username = messageArray[2]
                token = messageArray[3]
                password = messageArray[4]
                votingId = messageArray[5]
                voting = self.votingContainer.GetVotingByIdStr(votingId)
                if voting.value==None:
                    return VotingServer.Response(voting.value,voting.errorCode)
                result = voting.value.GetEncodedOwner(username,token,password)
                returnValue = VotingServer.Response(result.value,result.errorCode)
                return returnValue

            if commandName == "getClients":
                if len(messageArray)<4:
                    return VotingServer.Response(None,Errors.wrongRequest)
                username = messageArray[2]
                token = messageArray[3]
                password = messageArray[4]
                votingId = messageArray[5]
                voting = self.votingContainer.GetVotingByIdStr(votingId)
                if voting.value==None:
                    return VotingServer.Response(voting.value,voting.errorCode)
                result = voting.value.GetEncodedVoteClients(username,token,password)
                returnValue = VotingServer.Response(result.value,result.errorCode)
                return returnValue

            if commandName == "getSignedClients":
                if len(messageArray)<4:
                    return VotingServer.Response(None,Errors.wrongRequest)
                username = messageArray[2]
                token = messageArray[3]
                password = messageArray[4]
                votingId = messageArray[5]
                voting = self.votingContainer.GetVotingByIdStr(votingId)
                if voting.value==None:
                    return VotingServer.Response(voting.value,voting.errorCode)
                result = voting.value.GetEncodedVoteSignedClients(username,token,password)
                returnValue = VotingServer.Response(result.value,result.errorCode)
                return returnValue

            if commandName == "getResults":
                if len(messageArray)<4:
                    return VotingServer.Response(None,Errors.wrongRequest)
                username = messageArray[2]
                token = messageArray[3]
                password = messageArray[4]
                votingId = messageArray[5]
                voting = self.votingContainer.GetVotingByIdStr(votingId)
                if voting.value==None:
                    return VotingServer.Response(voting.value,voting.errorCode)
                result = voting.value.GetEncodedVoteResults(username,token,password)
                returnValue = VotingServer.Response(result.value,result.errorCode)
                return returnValue

            if commandName == "castVote":
                if len(messageArray)<4:
                    return VotingServer.Response(None,Errors.wrongRequest)
                username = messageArray[2]
                password = messageArray[3]
                votingId = messageArray[4]
                token = messageArray[5]
                signature = messageArray[6]
                i = 0
                dots = 0
                msg = message
                while i<len(msg):
                    if msg[i]==':':
                        dots+=1
                        if(dots==7):
                            i += 1
                            break
                    i+=1
                voting = self.votingContainer.GetVotingByIdStr(votingId)
                if voting.value==None:
                    return VotingServer.Response(voting.value,voting.errorCode)
                result = voting.value.CastVotes(username,msg[i:].split(':'),password,token,signature)
                return VotingServer.Response(result.value,result.errorCode)

            if commandName == "login":
                username = messageArray[2]
                userPassword = messageArray[3]
                result = self.userDatabase.Authenticate(username,userPassword)
                if result.reason != None:
                    return VotingServer.Response(None,result.reason)
                return VotingServer.Response(result.token,None)

            if commandName == "getUserVotings":
                username = messageArray[2]
                token = messageArray[3]
                password = messageArray[4]
                result = self.votingContainer.GetUserVotings(username,token)
                if result.value ==None:
                    return VotingServer.Response(None,result.errorCode)
                response = ""
                for item in result.value:
                    brief = item.GetEncodedVotingBrief(username,token,password)
                    if brief.value == None:
                        return VotingServer.Response(None,brief.errorCode)
                    response += brief.value+":"
                response = response[:-1]
                return VotingServer.Response(response,None)

            if commandName == "register":
                username = messageArray[2]
                token = messageArray[3]
                password = messageArray[4]
                return self.userDatabase.RegisterUser(username,password,token)

            if commandName == "addNewVoting":
                username = messageArray[2]
                token = messageArray[3]
                msg = message
                i=0
                dots = 0
                while i<len(msg):
                    if msg[i]==':':
                        dots+=1
                        if(dots==4):
                            i += 1
                            break
                    i+=1
                msg = msg[i:]
                voting = Voting(self.userDatabase)
                result = voting.Decode(msg)
                if result.value==None:
                    return VotingServer.Response(result.value,result.errorCode)
                voting.GenerateNewId()
                result = self.votingContainer.AddVoting(voting,username,token)
                return VotingServer.Response(voting.id,None)

            if commandName == "removeVoting":
                username = messageArray[2]
                token = messageArray[3]
                id = int(messageArray[4])
                result = self.votingContainer.RemoveVoting(id,username,token)
                if result.value==None:
                    return VotingContainer.Response(result.value,result.errorCode)

            if commandName == "verify":
                if len(messageArray)<4:
                    return VotingServer.Response(None,Errors.wrongRequest)
                cardId = messageArray[2]
                signature = messageArray[3]
                votingId = messageArray[4]
                voting = self.votingContainer.GetVotingByIdStr(votingId)
                if voting.value==None:
                    return VotingServer.Response(voting.value,voting.errorCode)
                result = voting.value.Verify(cardId,signature)
                return VotingServer.Response(result.value,result.errorCode)
        return VotingServer.Response(Errors.nothingToReturn,None)