class Error:
    def __init__(self,code,message):
        self.code = code
        self.message = message

class Errors:
    nothingToReturn = "1"
    wrongRequest = "2"
    wrongPassword = "3"
    usernameNotFound = "4"
    userAlreadyExists = "5"
    votingNotFound = "6"
    incorrectNumberFormat = "7"
    accountNotValid = "8"
    noVotingsMeetRequirements = "9"
    alreadyVoted = "10"
    onlyOneOptionCanBeChosen = "11"
    passwordRequired = "12"
    onlyLoggedInUsers = "13"
    applicationUnauthorized = "14"
    tooManyOptionsSelected = "15"
    votingHasNoOwner = "16"
    votingHasNoTitle = "17"
    badRegistrationToken = "18"
    thisUsernameIsNotAllowed = "19"
    youMayNotRecieveAnyMoreBallots = "20"
    wrongSignature = "21"
    def __init__(self):
        Errors.ErrorDatabase:[Error] = []
        Errors.ErrorDatabase.append(Error("1","No return value."))
        Errors.ErrorDatabase.append(Error("2","Wrong request."))
        Errors.ErrorDatabase.append(Error("3","Wrong password."))
        Errors.ErrorDatabase.append(Error("4","Username not found."))
        Errors.ErrorDatabase.append(Error("5","User already exists."))
        Errors.ErrorDatabase.append(Error("6","Requested voting not found."))
        Errors.ErrorDatabase.append(Error("7","Wrong number format."))
        Errors.ErrorDatabase.append(Error("8","Account is not valid."))
        Errors.ErrorDatabase.append(Error("9","There are no votings that meet the requirements."))
        Errors.ErrorDatabase.append(Error("10","Already voted!"))
        Errors.ErrorDatabase.append(Error("11","Only one option may be chosen."))
        Errors.ErrorDatabase.append(Error("12","Password is required to enter this vote."))
        Errors.ErrorDatabase.append(Error("13","You must be logged in to enter this vote."))
        Errors.ErrorDatabase.append(Error("14","Application is not authorized."))
        Errors.ErrorDatabase.append(Error("15","Too many options selected."))
        Errors.ErrorDatabase.append(Error("16","Voting has no owner."))
        Errors.ErrorDatabase.append(Error("17","Voting has no title."))
        Errors.ErrorDatabase.append(Error("18","Bad registration token."))
        Errors.ErrorDatabase.append(Error("19","This username is not allowed."))
        Errors.ErrorDatabase.append(Error("20","You may not sign any more ballots."))
        Errors.ErrorDatabase.append(Error("21","Wrong signature."))

    def GetErrorByCode(self,code)->Error:
        for err in Errors.ErrorDatabase:
            if err.code == code:
                return err
        return None

Errors = Errors()