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
    class RSA:
        P = 17
        Q = 23
        n = 391
        fi = 352
        e = 5
        d = 141
        def Load():
            Configuration.RSA.P = Configuration.RawJson["RSA"]["P"]
            Configuration.RSA.Q = Configuration.RawJson["RSA"]["Q"]
            Configuration.RSA.n = Configuration.RawJson["RSA"]["n"]
            Configuration.RSA.fi = Configuration.RawJson["RSA"]["fi"]
            Configuration.RSA.e = Configuration.RawJson["RSA"]["e"]
            Configuration.RSA.d = Configuration.RawJson["RSA"]["d"]

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
        with open('storage/configuration.json', 'w') as json_file:
            json.dump(Configuration.RawJson, json_file,indent=2)

Configuration.Load()
Configuration.RSA.Load()