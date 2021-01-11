import string
import random
from User import User

class UserDatabase:
    class Reasons:
        wrongPassword = "Incorrect Password!"
        usernameNotFound = "Username not found!"
        userAlreadyExists = "User already exists!"

    class AuthenticationResult:
        def __init__(self,reason,token):
            self.reason = reason
            self.token = token

    class Response:
        def __init__(self,value="",errorCode=""):
            self.ok=True
            self.errorCode = errorCode
            self.value = value

    def __init__(self):
        self.users:[User] = []

    def IsTokenTaken(self,token):
        for user in self.users:
            if user.Token == token:
                return True
        return False

    def GenerateToken(self):
        letters = string.ascii_letters
        token = ''.join(random.choice(letters) for i in range(15))
        while(self.IsTokenTaken(token)):
            token = ''.join(random.choice(letters) for i in range(15))
        return token

    def RegisterUser(self,username,password,registrationToken=""):
        for user in self.users:
            if user.Username == username:
                return UserDatabase.Response(None,UserDatabase.Reasons.userAlreadyExists)
        user = User()
        user.Username = username
        user.UserPassword = password
        user.Token = self.GenerateToken()
        self.users.append(user)
        self.Store()
        return UserDatabase.Response(user.Token,None)
        
    def Authenticate(self,username,password):
        for user in self.users:
            if user.Username == username:
                if user.UserPassword == password:
                    return UserDatabase.AuthenticationResult(None,user.Token)
                else:
                    return UserDatabase.AuthenticationResult(UserDatabase.Reasons.wrongPassword,None)
        return UserDatabase.AuthenticationResult(UserDatabase.Reasons.usernameNotFound,None)
         
    def ValidateUserToken(self,username,token)->bool:
        for user in self.users:
            if user.Username == username:
                if user.Token == token:
                    return True
                else:
                    return False
        return False

    def Load(self):
        file = open("users.txt","r")
        lines = file.readlines()
        self.users.clear()
        for line in lines:
            self.users.append(User().FromString(line))
        file.close()


    def Store(self):
        file = open("users.txt","w+")
        lines = []
        for user in self.users:
            lines.append(user.ToString()+'\n')
        file.writelines(lines)
        file.close()