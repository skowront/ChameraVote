class Voting
{
    constructor()
    {
        this.votingId = null;
        this.title = "";
        this.anonymous = false;
        this.mutuallyExclusive = false;
        this.allowUnregisteredUsers = false;
        this.maxOptions = false;
        this.voteOptions = [];
        this.ballotID = "";
        this.blindFactor = Math.floor(Math.random() * RSA.n);
        var mi = RSA.ModInverse(this.blindFactor,RSA.n);
        while(Number.isNaN(mi))
        {
            this.blindFactor = Math.floor(Math.random() * RSA.n);
            mi = RSA.ModInverse(this.blindFactor,RSA.n);
        }
        this.signature = "";
    }

    FromString(data)
    {
        var msg = data;
        var i = 0;
        while(msg[i]!=":")
        {
            i+=1;
        }
        this.votingId = msg.slice(0,i);
        msg = msg.slice(i+1,msg.length);

        var i = 0;
        while(msg[i]!=":")
        {
            i+=1;
        }
        msg = msg.slice(i+1,msg.length);

        i = 0;
        while(msg[i]!=":")
        {
            i+=1;
        }
        this.title = msg.slice(0,i);
        msg = msg.slice(i+1,msg.length);
        i = 0;
        while(msg[i]!=":")
        {
            i+=1;
        }
        this.anonymous = (msg.slice(0,i).toLowerCase()=='true');
        msg = msg.slice(i+1,msg.length);
        i = 0;
        while(msg[i]!=":")
        {
            i+=1;
        }
        this.mutuallyExclusive = (msg.slice(0,i).toLowerCase()=='true');
        msg = msg.slice(i+1,msg.length);

        i = 0;
        while(msg[i]!=":")
        {
            i+=1;
        }
        
        this.allowUnregisteredUsers = (msg.slice(0,i).toLowerCase()=='true');
        msg = msg.slice(i+1,msg.length);

        i = 0;
        while(msg[i]!=":")
        {
            i+=1;
        }
        this.maxOptions = (msg.slice(0,i));
        msg = msg.slice(i+1,msg.length);

        i = 0;
        while(msg[i]!=":")
        {
            i+=1;
        }
        var count = parseInt(msg.slice(0,i));
        msg = msg.slice(i+1,msg.length);

        for(let j = 0; j < count;j++)
        {
            i = 0;
            while(msg[i]!=":")
            {
                i+=1;
            }
            var option = msg.slice(0,i);
            console.log(option);
            msg = msg.slice(i+1,msg.length);
            this.voteOptions.push(option)
        }
    }
}