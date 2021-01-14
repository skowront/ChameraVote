
var testCommand = "0123456789:command:login:ts:ts";
var address = document.getElementById("ServerAddress");
var port = document.getElementById("Port");

var StatusCallback = function()
{
    document.getElementById('Status').innerHTML = voteClient.status;
} 

var OnRegisterSuccessfull = function()
{
    document.getElementById('UsernameRegister').disabled = true;
    document.getElementById('PasswordRegister').disabled = true;
    document.getElementById('TokenRegister').disabled = true;
    document.getElementById('Register').disabled = true;
} 


var OnLoginSuccessfull = function()
{
    document.getElementById('Username').disabled = true;
    document.getElementById('Password').disabled = true;
    let password = document.getElementById('Login').disabled = true;
    document.getElementById('GetVotingForm').style.visibility="visible";
} 

var OnVotingRecieved = function()
{
    //document.getElementById('VotingId').disabled = true;
    //document.getElementById('GetVoting').disabled = true;
    document.getElementById('VotingForm').style.visibility="visible";
    document.getElementById('Anonymous').checked = voteClient.voting.anonymous;
    document.getElementById('MutuallyExclusive').checked = voteClient.voting.mutuallyExclusive;
    document.getElementById('AllowUnregisteredUsers').checked = voteClient.voting.allowUnregisteredUsers;
    document.getElementById('MaxOptions').value = voteClient.voting.maxOptions;
    document.getElementById('VotingTitle').innerHTML = voteClient.voting.title;
    document.getElementById('SendVote').disabled = false;
    var listContainer = document.getElementById('OptionsList');
    listContainer.innerHTML="";
    for(var i = 0; i<voteClient.voting.voteOptions.length;i++)
    {
        var node = document.createElement("LI"); 
        var textnode = document.createTextNode(voteClient.voting.voteOptions[i]); 
        var checkbox = document.createElement("input");
        checkbox.setAttribute("type","checkbox");
        var container = document.createElement("div");
        container.appendChild(checkbox); 
        container.appendChild(textnode); 
        node.appendChild(container);
        listContainer.appendChild(node);
    }
}

var OnVotesAccepted = function()
{
    document.getElementById('SendVote').disabled = true;
}


var voteClient = new VoteClient(address.placeholder,port.placeholder,StatusCallback);
voteClient.OnRegisterSuccessfull = OnRegisterSuccessfull;
voteClient.OnLoginSuccessfull = OnLoginSuccessfull;
voteClient.OnVotingRecieved = OnVotingRecieved;
voteClient.OnVotesAccepted = OnVotesAccepted;

document.getElementById('RegisterForm').onsubmit = function(e) {
    let username = document.getElementById('UsernameRegister').value;
    let password = document.getElementById('PasswordRegister').value;
    let token = document.getElementById('TokenRegister').value;
    voteClient.Register(username,password,token);
    return false;
};

document.getElementById('LoginForm').onsubmit = function(e) {
    let username = document.getElementById('Username').value;
    let password = document.getElementById('Password').value;
    voteClient.Login(username,password);
    return false;
};

document.getElementById('GetVotingForm').onsubmit = function(e) {
    let votingId = document.getElementById('VotingId');
    voteClient.GetVoting(votingId.value);
    return false;
};

document.getElementById('VotingForm').onsubmit = function(e) {
    var list = document.getElementById('OptionsList');
    var options = [];
    for(var i=0;i<list.children.length;i++)
    {
        if(list.children[i].children[0].children[0].checked==true)
        {
            options.push(list.children[i].children[0].childNodes[1].textContent);
        }
    }
    console.log(options);
    voteClient.SendVote(options);
    return false;
};



document.getElementById('EmptyForm').onsubmit = function(e) {
    return true;
};



