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
                voteClient.status = "Register successful";
                voteClient.OnRegisterSuccessfull();
                break;
            case VoteClientStates.login:
                voteClient.token = messageArray[1];
                voteClient.status = "Login successful";
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
                voteClient.status = "Voting recieved from server.";
                voteClient.OnVotingRecieved();
                voteClient.state = VoteClientStates.sendingVotes;
                break;
            case VoteClientStates.sendingVotes:
                console.log("Votes accepted");
                voteClient.status = "Votes accepted";
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
        this.BuildSocket();
        this.state = VoteClientStates.register;
        console.log("Logging in.")
        var msg = this.prefix+"command:register:"+username+":"+token+":"+password;
        this.socket.emit("message",msg);
        this.username = username;
        return;
    };

    Login(username,password)
    {
        this.BuildSocket();
        this.state = VoteClientStates.login;
        console.log("Logging in.")
        var msg = this.prefix+"command:login:"+username+":"+password;
        this.socket.emit("message",msg);
        this.username = username;
        return;
    };

    GetVoting(votingId)
    {
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
        var msg = this.prefix+"command:castVote:"+this.username+"::"+this.voting.votingId+":"+this.token;
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
    static done = "done";
}