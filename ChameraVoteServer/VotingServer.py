import socket
import string
import select

from Logger import Logger
from Voting import Voting
from VotingContainer import VotingContainer
from UserDatabase import UserDatabase

class VotingServer:
    maxBufferSize = 256
    class Messages:
        wrongRequest = "Wrong reqest!"
        nothingToReturn = "No return value!"
        badRequest = "Request was invalid!"


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
        exampleVoting.owner = "Default owner"
        exampleVoting.mutuallyExclusive = True
        exampleVoting.voteClients=["User1","User2"]
        exampleVoting.voteResults=["yes","no"]
        self.votingContainer.AddVoting(exampleVoting)
        exampleVoting1 = Voting(self.userDatabase)
        exampleVoting1.GenerateNewId()
        exampleVoting1.voteOptions = ["yes","no","abstain"]
        exampleVoting1.voteTitle = "Test voting"
        exampleVoting1.owner = "Default owner"
        exampleVoting1.mutuallyExclusive = True
        exampleVoting1.voteClients=["User1","User2"]
        exampleVoting1.voteResults=["yes","no"]
        self.votingContainer.AddVoting(exampleVoting1)

    def Run(self):
        while True:
            clientsocket,address = self.socket.accept()
            Logger.Log("Connection from: "+str(address[0]))
            ready = select.select([clientsocket],[],[],5)
            if ready[0]:
                data = clientsocket.recv(1024)
                dataDecoded = data.decode("utf-8")
                print(dataDecoded)
                prefix = ""
                suffix = ""
                if (self.ValidateRequest(dataDecoded)==False):
                    prefix ="NOK"               
                    clientsocket.send(VotingServer.Messages.badRequest.encode())
                    continue
                response = self.HandleClientRequest(dataDecoded)
                if response.errorCode==None:
                    prefix="OK"
                    suffix = str(response.value)
                else:
                    prefix ="NOK"
                    suffix = str(response.errorCode)
                total = str(prefix+":"+suffix)
                while len(total)>VotingServer.maxBufferSize:
                    msg = total[0:VotingServer.maxBufferSize-1]
                    msg += '&'
                    clientsocket.send(msg.encode())
                    total = total[VotingServer.maxBufferSize-1:]
                clientsocket.send(total.encode())
            clientsocket.close()

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
        if len(messageArray)<=1:
            return VotingServer.Response(None,VotingServer.Messages.wrongRequest)
        elif msgType == "command":
            if len(messageArray)<2:
                return VotingServer.Response(None,VotingServer.Messages.wrongRequest)
            commandName = messageArray[1]
            if commandName == "getVotingById":
                if len(messageArray)<4:
                    return VotingServer.Response(None,VotingServer.Messages.wrongRequest)
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

            if commandName == "getTitle":
                if len(messageArray)<4:
                    return VotingServer.Response(None,VotingServer.Messages.wrongRequest)
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
                    return VotingServer.Response(None,VotingServer.Messages.wrongRequest)
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
                    return VotingServer.Response(None,VotingServer.Messages.wrongRequest)
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
                    return VotingServer.Response(None,VotingServer.Messages.wrongRequest)
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
                    return VotingServer.Response(None,VotingServer.Messages.wrongRequest)
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
                    return VotingServer.Response(None,VotingServer.Messages.wrongRequest)
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

            if commandName == "getResults":
                if len(messageArray)<4:
                    return VotingServer.Response(None,VotingServer.Messages.wrongRequest)
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
                    return VotingServer.Response(None,VotingServer.Messages.wrongRequest)
                username = messageArray[2]
                password = messageArray[3]
                votingId = messageArray[4]
                token = messageArray[5]
                i = 0
                dots = 0
                msg = message
                while i<len(msg):
                    if msg[i]==':':
                        dots+=1
                        if(dots==6):
                            i += 1
                            break
                    i+=1
                voting = self.votingContainer.GetVotingByIdStr(votingId)
                if voting.value==None:
                    return VotingServer.Response(voting.value,voting.errorCode)
                result = voting.value.CastVotes(username,msg[i:].split(':'),password,token)
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
                votingId = messageArray[5]
                result = self.votingContainer.GetUserVotings(username,token)
                if result.value ==None:
                    return VotingServer.Response(None,result.errorCode)
                response = ""
                for item in result:
                    brief = item.GetEncodedVotingBrief(username,token,password)
                    response += brief+":"
                response = response[:-1]
                return VotingServer.Response(response,None)

        return VotingServer.Response(VotingServer.Messages.nothingToReturn,None)