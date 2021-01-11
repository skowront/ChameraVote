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

    class Response:
        def __init__(self,value="",errorCode=""):
            self.ok=True
            self.errorCode = errorCode
            self.value = value

    def __init__(self,port:int,createExampleVoting:bool):
        self.socket = socket.socket(socket.AF_INET,socket.SOCK_STREAM)
        self.socket.bind(('0.0.0.0',port))
        self.socket.listen(5)
        self.votingContainer = VotingContainer()
        self.userDatabase = UserDatabase()
        self.userDatabase.Load()
        if createExampleVoting==True:
            self.CreateExampleVoting()

    def CreateExampleVoting(self):
        exampleVoting = Voting()
        exampleVoting.GenerateNewId()
        exampleVoting.voteOptions = ["yes","no","abstain"]
        exampleVoting.voteTitle = "Test voting"
        exampleVoting.owner = "Default owner"
        exampleVoting.mutuallyExclusive = True
        exampleVoting.voteClients=["User1","User2"]
        exampleVoting.voteResults=["yes","no"]
        self.votingContainer.AddVoting(exampleVoting)

    def Run(self):
        while True:
            clientsocket,address = self.socket.accept()
            Logger.Log("Connection from: "+str(address[0]))
            ready = select.select([clientsocket],[],[],5)
            if ready[0]:
                data = clientsocket.recv(1024)
                dataDecoded = data.decode("utf-8")
                print(dataDecoded)
                response = self.HandleClientRequest(dataDecoded)
                prefix = ""
                suffix = ""
                if response.errorCode==None:
                    prefix="OK"
                    suffix = str(response.value)
                else:
                    prefix ="NOK"
                    suffix = str(response.errorCode)
                total = str(prefix+":"+suffix)
                while len(total)>VotingServer.maxBufferSize:
                    msg = total[0:VotingServer.maxBufferSize-1]
                    print(msg)
                    msg += '&'
                    clientsocket.send(msg.encode())
                    total = total[VotingServer.maxBufferSize-1:]
                clientsocket.send(total.encode())
            clientsocket.close()

    def HandleClientRequest(self,message)->Response:
        messageArray = message.split(':')
        msgType = messageArray[0]
        returnValue = ""
        if len(messageArray)<=1:
            return VotingServer.Response(None,VotingServer.Messages.wrongRequest)
        elif msgType == "command":
            if len(messageArray)<2:
                return VotingServer.Response(None,VotingServer.Messages.wrongRequest)
            commandName = messageArray[1]
            if commandName == "getVotingById":
                print(messageArray)
                if len(messageArray)<4:
                    return VotingServer.Response(None,VotingServer.Messages.wrongRequest)
                password = messageArray[2]
                votingId = messageArray[3]
                voting = self.votingContainer.GetVotingByIdStr(votingId)
                if voting.value==None:
                    return VotingServer.Response(voting.value,voting.errorCode)
                result = voting.value.GetEncodedVoting(password)
                returnValue = VotingServer.Response(result.value,result.errorCode)
                return returnValue

            if commandName == "getTitle":
                print(messageArray)
                if len(messageArray)<4:
                    return VotingServer.Response(None,VotingServer.Messages.wrongRequest)
                password = messageArray[2]
                votingId = messageArray[3]
                voting = self.votingContainer.GetVotingByIdStr(votingId)
                if voting.value==None:
                    return VotingServer.Response(voting.value,voting.errorCode)
                result = voting.value.GetEncodedVoteTitle(password)
                returnValue = VotingServer.Response(result.value,result.errorCode)
                return returnValue

            if commandName == "getOptions":
                print(messageArray)
                if len(messageArray)<4:
                    return VotingServer.Response(None,VotingServer.Messages.wrongRequest)
                password = messageArray[2]
                votingId = messageArray[3]
                voting = self.votingContainer.GetVotingByIdStr(votingId)
                if voting.value==None:
                    return VotingServer.Response(voting.value,voting.errorCode)
                result = voting.value.GetEncodedVoteOptions(password)
                returnValue = VotingServer.Response(result.value,result.errorCode)
                return returnValue

            if commandName == "getAnonymous":
                print(messageArray)
                if len(messageArray)<4:
                    return VotingServer.Response(None,VotingServer.Messages.wrongRequest)
                password = messageArray[2]
                votingId = messageArray[3]
                voting = self.votingContainer.GetVotingByIdStr(votingId)
                if voting.value==None:
                    return VotingServer.Response(voting.value,voting.errorCode)
                result = voting.value.GetEncodedVoteAnonymous(password)
                returnValue = VotingServer.Response(result.value,result.errorCode)
                return returnValue


            if commandName == "getMutuallyExclusive":
                print(messageArray)
                if len(messageArray)<4:
                    return VotingServer.Response(None,VotingServer.Messages.wrongRequest)
                password = messageArray[2]
                votingId = messageArray[3]
                voting = self.votingContainer.GetVotingByIdStr(votingId)
                if voting.value==None:
                    return VotingServer.Response(voting.value,voting.errorCode)
                result = voting.value.GetEncodedVoteMutuallyExclusive(password)
                returnValue = VotingServer.Response(result.value,result.errorCode)
                return returnValue

            if commandName == "getOwner":
                print(messageArray)
                if len(messageArray)<4:
                    return VotingServer.Response(None,VotingServer.Messages.wrongRequest)
                password = messageArray[2]
                votingId = messageArray[3]
                voting = self.votingContainer.GetVotingByIdStr(votingId)
                if voting.value==None:
                    return VotingServer.Response(voting.value,voting.errorCode)
                result = voting.value.GetEncodedOwner(password)
                returnValue = VotingServer.Response(result.value,result.errorCode)
                return returnValue

            if commandName == "getClients":
                print(messageArray)
                if len(messageArray)<4:
                    return VotingServer.Response(None,VotingServer.Messages.wrongRequest)
                password = messageArray[2]
                votingId = messageArray[3]
                voting = self.votingContainer.GetVotingByIdStr(votingId)
                if voting.value==None:
                    return VotingServer.Response(voting.value,voting.errorCode)
                result = voting.value.GetEncodedVoteClients(password)
                returnValue = VotingServer.Response(result.value,result.errorCode)
                return returnValue

            if commandName == "getResults":
                print(messageArray)
                if len(messageArray)<4:
                    return VotingServer.Response(None,VotingServer.Messages.wrongRequest)
                password = messageArray[2]
                votingId = messageArray[3]
                voting = self.votingContainer.GetVotingByIdStr(votingId)
                if voting.value==None:
                    return VotingServer.Response(voting.value,voting.errorCode)
                result = voting.value.GetEncodedVoteResults(password)
                returnValue = VotingServer.Response(result.value,result.errorCode)
                return returnValue

            if commandName == "castVote":
                print(messageArray)
                if len(messageArray)<4:
                    return VotingServer.Response(None,VotingServer.Messages.wrongRequest)
                username = messageArray[2]
                password = messageArray[3]
                votingId = messageArray[4]
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
                print(msg[i:])
                voting = self.votingContainer.GetVotingByIdStr(votingId)
                if voting.value==None:
                    return VotingServer.Response(voting.value,voting.errorCode)
                result = voting.value.AddVotes(username,msg[i:].split(':'),password)
                return VotingServer.Response(result.value,result.errorCode)

            if commandName == "login":
                username = messageArray[2]
                userPassword = messageArray[3]
                result = self.userDatabase.Authenticate(username,userPassword)
                if result.reason != None:
                    return VotingServer.Response(None,result.reason)
                return VotingServer.Response(result.token,None)

        return VotingServer.Response(VotingServer.Messages.nothingToReturn,None)