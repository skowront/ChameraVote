import json
import os
#Json generated with https://beta5.objgen.com/json/local/design
class Configuration:
    ApplicationToken = "0123456789"
    RegistrationToken = "pfxQ3"
    ServerAddress = "localhost"
    ServerWebPort = 16402
    ServerPort = 16403
    AllowedRegistrationUsernames = []
    RawJson = None

    def Load():
        # f = open(os.getcwd()+'\\storage\\configuration.json')
        f = open('storage/configuration.json')
        Configuration.RawJson = json.load(f)
        f.close()
        Configuration.ApplicationToken = Configuration.RawJson["ApplicationToken"]
        Configuration.RegistrationToken = Configuration.RawJson["RegistrationToken"]
        Configuration.ServerAddress = Configuration.RawJson["ServerAddress"]
        Configuration.ServerWebPort = Configuration.RawJson["ServerWebPort"]
        Configuration.ServerPort = Configuration.RawJson["ServerPort"]
        Configuration.AllowedRegistrationUsernames = Configuration.RawJson["AllowedRegistrationUsernames"]

    def Save():
        Configuration.RawJson["ApplicationToken"] = Configuration.ApplicationToken
        Configuration.RawJson["RegistrationToken"] = Configuration.RegistrationToken
        Configuration.RawJson["ServerAddress"] = Configuration.ServerAddress
        Configuration.RawJson["ServerWebPort"] = Configuration.ServerWebPort
        Configuration.RawJson["ServerPort"] = Configuration.ServerPort
        Configuration.RawJson["AllowedRegistrationUsernames"] = Configuration.AllowedRegistrationUsernames
        print(Configuration.AllowedRegistrationUsernames)
        with open('storage/configuration.json', 'w') as json_file:
            json.dump(Configuration.RawJson, json_file,indent=2)

Configuration.Load()