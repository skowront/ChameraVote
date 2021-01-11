class User:
    def __init__(self):
        self.Username = ""
        self.UserPassword = ""
        self.Token = ""

    def ToString(self)->str:
        value = "UserBegin:"
        value += "U:"+self.Username+":"
        value += "P:"+self.UserPassword+":"
        value += "T:"+self.Token+":"
        value += "UserEnd"
        return value

    def FromString(self,data:str):
        values = data.split(':')
        self.Username = values[2]
        self.UserPassword = values[4]
        self.Token = values[6]
        return self