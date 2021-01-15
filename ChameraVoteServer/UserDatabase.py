import string
import random
from User import User
from Errors import Errors
from Configuration import Configuration;

class UserDatabase:
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

    def IsUserAllowedToRegister(self,username):
        for i in range(0,len(Configuration.AllowedRegistrationUsernames)):
            item = Configuration.AllowedRegistrationUsernames[i]
            if username==item:
                return True
        return False

    def RegisterUser(self,username,password,registrationToken=""):
        for user in self.users:
            if user.Username == username:
                return UserDatabase.Response(None,Errors.userAlreadyExists)
        if registrationToken!=Configuration.RegistrationToken:
            print(registrationToken)
            return UserDatabase.Response(None,Errors.badRegistrationToken)
        if self.IsUserAllowedToRegister(username)==False:
            return UserDatabase.Response(None,Errors.thisUsernameIsNotAllowed)
        Configuration.AllowedRegistrationUsernames.remove(username)
        Configuration.Save()
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
                    return UserDatabase.AuthenticationResult(Errors.wrongPassword,None)
        return UserDatabase.AuthenticationResult(Errors.usernameNotFound,None)
         
    def ValidateUserToken(self,username,token)->bool:
        for user in self.users:
            if user.Username == username:
                if user.Token == token:
                    return True
                else:
                    return False
        return False

    def Load(self):
        file = open("storage/users.txt","r")
        lines = file.readlines()
        self.users.clear()
        for line in lines:
            self.users.append(User().FromString(line))
        file.close()


    def Store(self):
        file = open("storage/users.txt","w+")
        lines = []
        for user in self.users:
            lines.append(user.ToString()+'\n')
        file.writelines(lines)
        file.close()