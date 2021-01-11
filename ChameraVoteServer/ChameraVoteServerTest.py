from VotingServer import VotingServer

votingServer = VotingServer(16402,True)

print("Http tests:")
tests:[VotingServer.Response] = []

test = votingServer.HandleClientRequest("command:getTitle:0:0"); tests.append(test)
test = votingServer.HandleClientRequest("command:getOptions:0:0"); tests.append(test)
test = votingServer.HandleClientRequest("command:getAnonymous:0:0"); tests.append(test)
test = votingServer.HandleClientRequest("command:getClients:0:0"); tests.append(test)
test = votingServer.HandleClientRequest("command:getResults:0:0"); tests.append(test)
test = votingServer.HandleClientRequest("command:getVotingById:0:0"); tests.append(test)

for i in range(0,len(tests)):
    test = tests[i]
    print("Test "+str(i)+" ")
    if test.errorCode == None:
        print("Result: " + str(test.value))
    else:
        print("Error: " + test.errorCode)

print("UserDatabase tests:")
from UserDatabase import UserDatabase
userDatabase = UserDatabase()
userDatabase.RegisterUser("User1","xaxaM1")
userDatabase.RegisterUser("User2","xaxaM1")
userDatabase.Store()
userDatabase.Load()

result = userDatabase.Authenticate("User1","xaxaM1")
if(result.reason==None):
    print("OK")
    print(result.token)
else:
    print("NOK")
    print(result.reason)

result = userDatabase.Authenticate("User1","a")
if(result.reason==None):
    print("NOK")
    print(result.token)
else:
    print("OK")
    print(result.reason)

result = userDatabase.Authenticate("Userasda1","a")
if(result.reason==None):
    print("NOK")
    print(result.token)
else:
    print("OK")
    print(result.reason)