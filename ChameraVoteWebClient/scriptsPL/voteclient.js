class VoteClient
{
    constructor(serverAddress,port,statusCallback)
    {
        this.link = 'http://'+serverAddress+':'+port+'';
        this.serverAddress = serverAddress;
        this.port = port;
        this.socket = null;
        this.applicationToken = "0123456789";
        this.prefix = this.applicationToken+':';
        this.state = VoteClientStates.login;
        this.status = "";
        this.username = "";
        this.token= "";
        this.voting = null;
        this.StatusCallback = statusCallback;
        this.OnRegisterSuccessfull = null;
        this.OnLoginSuccessfull = null;
        this.OnVotingRecieved = null;
        this.OnVotesAccepted =  null;
        this.OnBallotRecieved = null;
        this.OnBallotSigned = null;
    }

    BuildSocket = function()
    {
        console.log("Constructing socket.");
        this.socket = io(this.link); 
        this.socket.on('connect', function(){console.log('connect!')});
        this.socket.on('message', function(msg){console.log('message!', msg)});
        this.socket.on('disconnect', function(){console.log('disconnect!')});
        this.socket.on('reply',this.HandleResponse);
    }

    DestroySocket = function()
    {
        this.socket.disconnect();
    }
    
    HandleResponse = function (response)
    {
        voteClient.DestroySocket();
        console.log("Response:"+response);
        var messageArray = response.split(':');
        if(messageArray[0]!="OK")
        {
            voteClient.status = VoteErrors.Errors[messageArray[1]];
            voteClient.StatusCallback();
            return; 
        }
        voteClient.status = "OK";
        switch(voteClient.state)
        {
            case VoteClientStates.register:
                voteClient.token = messageArray[1];
                voteClient.status = "Rejestracja udana";
                voteClient.OnRegisterSuccessfull();
                break;
            case VoteClientStates.login:
                voteClient.token = messageArray[1];
                voteClient.status = "Logowanie udane";
                voteClient.OnLoginSuccessfull();
                break;
            case VoteClientStates.gettingVoting:
                var i = 0;
                var msg = response;
                var dots = 0;
                while(i<msg.length)
                {
                    if(msg[i]==':')
                    {
                        dots = dots +1;
                        if(dots == 1);
                        {
                            i += 1;
                            break;
                        }
                    }
                    i = i+ 1;
                }
                msg = msg.slice(i,msg.length);
                voteClient.voting = new Voting();
                voteClient.voting.FromString(msg);
                voteClient.status = "Otrzymano głosowanie z serwera.";
                voteClient.OnVotingRecieved();
                voteClient.state = VoteClientStates.gettingBallot;
                break;
            case VoteClientStates.gettingBallot:
                voteClient.voting.ballotID = messageArray[1];
                voteClient.status = "Twój numer karty:" + voteClient.voting.ballotID;
                voteClient.state = VoteClientStates.signingBallot;
                voteClient.OnBallotRecieved();
                break;
            case VoteClientStates.signingBallot:
                console.log(messageArray[1]);
                console.log(voteClient.voting.blindFactor);
                console.log(RSA.n);
                var s = parseInt(messageArray[1])*RSA.ModInverse(voteClient.voting.blindFactor,RSA.n);
                voteClient.voting.signature = s.toString();
                voteClient.status = "Twój numer karty: " + voteClient.voting.ballotID + " jest podpisany sygnaturą:" + voteClient.voting.signature;
                voteClient.state = VoteClientStates.sendingVotes;
                voteClient.OnBallotSigned();
                break;
            case VoteClientStates.sendingVotes:
                console.log("Votes accepted");
                voteClient.status = "Głosy przyjęte.";
                voteClient.OnVotesAccepted();
                voteClient.state = VoteClientStates.done;
                break;
            case VoteClientStates.done:
                console.log("Done");
                break;
            default:
                break;
        }
        voteClient.StatusCallback();
        return;
    };

    Register(username,password,token)
    {
        if(username.includes(':') || password.includes(':') || token.includes(':'))
        {
            this.status = "Znak ':' jest niedozwolony";
            voteClient.StatusCallback();
            return;
        }
        this.BuildSocket();
        this.state = VoteClientStates.register;
        console.log("Logging in.")
        var passhash = md5(password);
        var msg = this.prefix+"command:register:"+username+":"+token+":"+passhash;
        this.socket.emit("message",msg);
        this.username = username;
        return;
    };

    Login(username,password)
    {
        if(username.includes(':') || password.includes(':'))
        {
            this.status = "Znak ':' jest niedozwolony";
            voteClient.StatusCallback();
            return;
        }
        this.BuildSocket();
        this.state = VoteClientStates.login;
        console.log("Logging in.")
        var passhash = md5(password);
        var msg = this.prefix+"command:login:"+username+":"+passhash;
        this.socket.emit("message",msg);
        this.username = username;
        return;
    };

    GetBallot(votingId)
    {
        if(votingId.includes(':'))
        {
            this.status = "Znak ':' jest niedozwolony";
            voteClient.StatusCallback();
            return;
        }
        this.BuildSocket();
        this.state = VoteClientStates.gettingBallot;
        console.log("Getting ballot.");
        var msg = this.prefix+"command:getBallot:"+votingId;
        this.socket.emit("message",msg);
        return;
    }

    SignBallot(username,password)
    {
        if(username.includes(':') || password.includes(':'))
        {
            this.status = "Znak ':' jest niedozwolony";
            voteClient.StatusCallback();
            return;
        }
        this.BuildSocket();
        this.status = VoteClientStates.signingBallot;
        console.log("Signing ballot.");
        var passhash = md5(password);
        var mPrime = parseInt(this.voting.ballotID) * Math.pow(this.voting.blindFactor,RSA.PublicKey[0])%RSA.n;
        var msg = this.prefix+"command:getSignedBallot:"+this.username+":"+this.token+"::"+this.voting.votingId+":"+mPrime;
        this.socket.emit("message",msg);
        return;
    }

    GetVoting(votingId)
    {
        if(votingId.includes(':'))
        {
            this.status = "Znak ':' jest niedozwolony";
            voteClient.StatusCallback();
            return;
        }
        this.BuildSocket();
        this.state = VoteClientStates.gettingVoting;
        console.log("Getting voting with id: "+votingId);
        var msg = this.prefix+"command:getVotingById:"+this.username+":"+this.token+"::"+votingId;
        this.socket.emit("message",msg);
        return;
    }

    SendVote(options)
    {
        this.BuildSocket();
        this.state = VoteClientStates.sendingVotes;
        var msg = "";
        if (this.voting.anonymous)
        {
            msg = this.prefix+"command:castVote:"+this.voting.ballotID+"::"+this.voting.votingId+"::"+this.voting.signature;
        }
        else
        {
            msg = this.prefix+"command:castVote:"+this.username+"::"+this.voting.votingId+":"+this.token+":"+this.voting.signature;
        }
        
        for(var i=0;i<options.length;i++)
        {
            msg+=":"+options[i];
        }
        console.log(msg);
        var s = msg.toString();
        console.log(s);
        this.socket.emit("message",msg);
        return;
    }
}

class VoteClientStates
{
    static register = "register";
    static login = "login";
    static gettingVoting = "gettingVoting";
    static sendingVotes = "sendingVotes";
    static gettingBallot = "gettingBallot";
    static signingBallot = "signingBallot";
    static done = "done";
}