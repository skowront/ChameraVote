using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using ChameraVote.Models;
using ChameraVote.ViewModels;

namespace ChameraVote.Utility
{
    public class VoteClient
    {
        private string serverAddress = "localhost";
        //private string ServerAddress = "46.41.151.157";
        private string applicationToken = "";

        private const int port = 16402;

        private const int bufferSize = 1024;

        private const string incorrectResponseCode = "NOK";
        
        private const string correctResponseCode = "OK";

        private const string commandTemplate = "command:{0}:{1}:{2}:{3}:{4}";
        
        private const string getUserVotingsTemplate = "command:getUserVotings:{0}:{1}:{2}";
        
        private const string loginCommandTemplate = "command:login:{0}:{1}";
        
        private const string registerCommandTemplate = "command:register:{0}:{1}:{2}";

        private const string castVoteTemplate = "command:castVote:{0}:{1}:{2}:{3}";

        private const string addNewVotingTemplate = "command:addNewVoting:{0}:{1}:{2}";

        private const string removeVotingTemplate = "command:removeVoting:{0}:{1}:{2}";

        private const int timeout = 5000;

        public VoteClient(ConfigurationViewModel configurationModel)
        {
            this.serverAddress = configurationModel.ServerAddress;
            this.applicationToken = configurationModel.ApplicationToken;
        }

        private VotingModel VotingModelFromString(string stringified)
        {
            var model = new VotingModel();
            model.serverAddress = this.serverAddress;
            string str = stringified;
            int i = 0;
            while(str[i]!=':'){i++;}
            model.votingId = str.Substring(0, i);
            str = str.Remove(0, i+1);
            i = 0;

            while (str[i] != ':') { i++; }
            model.owner = str.Substring(0, i);
            str = str.Remove(0, i + 1);
            i = 0;

            while (str[i] != ':') { i++; }
            model.votingTitle = str.Substring(0, i);
            str = str.Remove(0, i + 1);
            i = 0;

            while (str[i] != ':') { i++; }
            model.anonymous = bool.Parse(str.Substring(0, i));
            str = str.Remove(0, i + 1);
            i = 0;

            while (str[i] != ':') { i++; }
            model.mutuallyExclusive = bool.Parse(str.Substring(0, i));
            str = str.Remove(0, i + 1);
            i = 0;

            while (str[i] != ':') { i++; }
            model.allowUnregisteredUsers = bool.Parse(str.Substring(0, i));
            str = str.Remove(0, i + 1);
            i = 0;

            while (str[i] != ':') { i++; }
            model.maxOptions = int.Parse(str.Substring(0, i));
            str = str.Remove(0, i + 1);
            i = 0;

            while (str[i] != ':') { i++; }
            var optionsCount = int.Parse(str.Substring(0, i));
            str = str.Remove(0, i + 1);
            i = 0;

            for(int j = 0 ; j < optionsCount ; j++)
            {
                while (str[i] != ':') { i++; }
                model.votingOptionsRaw.Add(str.Substring(0, i));
                str = str.Remove(0, i + 1);
                i = 0;
            }

            while (str[i] != ':') { i++; }
            var resultsCount = int.Parse(str.Substring(0, i));
            str = str.Remove(0, i + 1);
            i = 0;

            for (int j = 0; j < resultsCount; j++)
            {
                while (str[i] != ':') { i++; }
                model.votingResults.Add(str.Substring(0, i));
                str = str.Remove(0, i + 1);
                i = 0;
            }

            while (str[i] != ':') { i++; if (i >= str.Length) { break; } }
            var clientsCount = int.Parse(str.Substring(0, i));
            str = str.Remove(0, Math.Min(i + 1,str.Length));
            i = 0;

            for (int j = 0; j < clientsCount; j++)
            {
                if(i>=str.Length)
                {
                    model.votingClients.Add("");
                    continue;
                }
                while (str[i] != ':') { i++; if (i >= str.Length) { break; } }
                model.votingClients.Add(str.Substring(0, i));
                str = str.Remove(0, Math.Min(i + 1, str.Length));
                i = 0;
            }

            return model;
        }

        public VotingModel GetVotingModel(string votingId,string username,string token, string password, out int errorCode )
        {
            TcpClient tcpClient = new TcpClient(this.serverAddress, port);
            tcpClient.ReceiveTimeout = timeout;

            NetworkStream stream = tcpClient.GetStream();

            Byte[] data = new Byte[bufferSize];
            String responseData = String.Empty;

            string message = string.Format(commandTemplate, "getVotingById",username,token, password, votingId);
            data = System.Text.Encoding.UTF8.GetBytes(this.applicationToken + message);
            stream.Write(data, 0, data.Length);

            data = new Byte[bufferSize];
            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.UTF8.GetString(data, 0, bytes);

            while(responseData[responseData.Length-1]=='&')
            {
                responseData = this.RemoveLast(responseData, "&");
                data = new Byte[bufferSize];
                bytes = stream.Read(data, 0, data.Length);
                responseData += System.Text.Encoding.UTF8.GetString(data, 0, bytes);
            }

            if (responseData.Split(':')[0] == incorrectResponseCode)
            {
                MessageBox.Show(VoteClient.errors[int.Parse(responseData.Split(':')[1])].Item2,"Error");
                errorCode = int.Parse(responseData.Split(':')[1]);
                return null;
            }
            errorCode = 0;

            var noCode = string.Empty;
            for(int i=3;i<responseData.Length;i++)
            {
                noCode += responseData[i];
            }

            return this.VotingModelFromString(noCode);
        }

        public Collection<VotingBriefModel> GetUserVotingsBrief(string username, string token, string password, out int errorCode)
        {
            TcpClient tcpClient = new TcpClient(this.serverAddress, port);
            tcpClient.ReceiveTimeout = timeout;

            NetworkStream stream = tcpClient.GetStream();

            Byte[] data = new Byte[bufferSize];
            String responseData = String.Empty;

            string message = string.Format(getUserVotingsTemplate, username, token, password);
            data = System.Text.Encoding.UTF8.GetBytes(this.applicationToken + message);
            stream.Write(data, 0, data.Length);

            data = new Byte[bufferSize];
            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.UTF8.GetString(data, 0, bytes);

            if (responseData.Split(':')[0] == incorrectResponseCode)
            {
                MessageBox.Show(VoteClient.errors[int.Parse(responseData.Split(':')[1])].Item2, "Error");
                errorCode = int.Parse(responseData.Split(':')[1]);
                return null;
            }
            errorCode = 0;

            Collection<VotingBriefModel> collection = new Collection<VotingBriefModel>();

            var array = responseData.Split(':');
            for(int i=1;i<array.Length;i+=2)
            {
                var item = new VotingBriefModel() { id = array[i], title=array[i+1] };
                collection.Add(item);
            }

            return collection;
        }

        public string GetTitle(string votingId, string username, string token, string password,out int errorCode)
        {
            TcpClient tcpClient = new TcpClient(this.serverAddress, port);
            tcpClient.ReceiveTimeout = timeout;

            NetworkStream stream = tcpClient.GetStream();

            Byte[] data = new Byte[bufferSize];
            String responseData = String.Empty;

            string message = string.Format(commandTemplate, "getTitle", username, token, password, votingId);
            data = System.Text.Encoding.UTF8.GetBytes(this.applicationToken + message);
            stream.Write(data, 0, data.Length);

            data = new Byte[bufferSize];
            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.UTF8.GetString(data, 0, bytes);

            if (responseData.Split(':')[0] == incorrectResponseCode)
            {
                MessageBox.Show(VoteClient.errors[int.Parse(responseData.Split(':')[1])].Item2, "Error");
                errorCode = int.Parse(responseData.Split(':')[1]);
                return null;
            }
            errorCode = 0;

            return responseData.Split(':')[1];
        }

        public bool? GetAnonymous(string votingId, string username, string token, string password, out int errorCode)
        {
            TcpClient tcpClient = new TcpClient(this.serverAddress, port);
            tcpClient.ReceiveTimeout = timeout;

            NetworkStream stream = tcpClient.GetStream();

            Byte[] data = new Byte[bufferSize];
            String responseData = String.Empty;

            string message = string.Format(commandTemplate, "getAnonymous", username, token, password, votingId);
            data = System.Text.Encoding.UTF8.GetBytes(this.applicationToken + message);
            stream.Write(data, 0, data.Length);

            data = new Byte[bufferSize];
            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.UTF8.GetString(data, 0, bytes);

            if (responseData.Split(':')[0] == incorrectResponseCode)
            {
                MessageBox.Show(VoteClient.errors[int.Parse(responseData.Split(':')[1])].Item2, "Error");
                errorCode = int.Parse(responseData.Split(':')[1]);
                return null;
            }
            errorCode = 0;

            if (responseData.Split(':')[1].ToLower()=="true")
            {
                return true;
            }
            return false;
        }

        public IEnumerable<string> GetOptions(string votingId, string username, string token, string password, out int errorCode)
        {
            TcpClient tcpClient = new TcpClient(this.serverAddress, port);
            tcpClient.ReceiveTimeout = timeout;

            NetworkStream stream = tcpClient.GetStream();

            Byte[] data = new Byte[bufferSize];
            String responseData = String.Empty;

            string message = string.Format(commandTemplate, "getOptions", username, token, password, votingId);
            data = System.Text.Encoding.UTF8.GetBytes(this.applicationToken + message);
            stream.Write(data, 0, data.Length);

            data = new Byte[bufferSize];
            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.UTF8.GetString(data, 0, bytes);

            if (responseData.Split(':')[0] == incorrectResponseCode)
            {
                MessageBox.Show(VoteClient.errors[int.Parse(responseData.Split(':')[1])].Item2, "Error");
                errorCode = int.Parse(responseData.Split(':')[1]);
                return null;
            }
            errorCode = 0;

            var values = responseData.Split(':');
            var collection = new Collection<string>();
            for(int i=1;i<values.Length;i++)
            {
                collection.Add(values[i]);
            }

            return collection;
        }

        public bool SendVote(string votingId, Collection<string> selectedOptions, string username,string token, string password, out int errorCode)
        {
            TcpClient tcpClient = new TcpClient(this.serverAddress, port);
            tcpClient.ReceiveTimeout = timeout;

            NetworkStream stream = tcpClient.GetStream();

            Byte[] data = new Byte[bufferSize];
            String responseData = String.Empty;

            string message = string.Format(castVoteTemplate, username, password, votingId, token);
            
            foreach(var item in selectedOptions)
            {
                message += ":" + item;
            }

            data = System.Text.Encoding.UTF8.GetBytes(this.applicationToken + message);
            stream.Write(data, 0, data.Length);

            data = new Byte[bufferSize];
            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.UTF8.GetString(data, 0, bytes);

            if (responseData.Split(':')[0] == incorrectResponseCode)
            {
                MessageBox.Show(VoteClient.errors[int.Parse(responseData.Split(':')[1])].Item2, "Error");
                errorCode = int.Parse(responseData.Split(':')[1]);
                return false;
            }
            errorCode = 0;

            return true;
        }

        public string Login(string username,string password, out int errorCode)
        {
            TcpClient tcpClient = new TcpClient(this.serverAddress, port);
            tcpClient.ReceiveTimeout = timeout;

            NetworkStream stream = tcpClient.GetStream();

            Byte[] data = new Byte[bufferSize];
            String responseData = String.Empty;

            string message = string.Format(loginCommandTemplate, username,password);
            data = System.Text.Encoding.UTF8.GetBytes(this.applicationToken + message);
            stream.Write(data, 0, data.Length);

            data = new Byte[bufferSize];
            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.UTF8.GetString(data, 0, bytes);

            if (responseData.Split(':')[0] == incorrectResponseCode)
            {
                MessageBox.Show(VoteClient.errors[int.Parse(responseData.Split(':')[1])].Item2, "Error");
                errorCode = int.Parse(responseData.Split(':')[1]);
                return null;
            }
            errorCode = 0;

            return responseData.Split(':')[1];
        }

        public string Register(string username, string password,string token, out int errorCode)
        {
            TcpClient tcpClient = new TcpClient(this.serverAddress, port);
            tcpClient.ReceiveTimeout = timeout;

            NetworkStream stream = tcpClient.GetStream();

            Byte[] data = new Byte[bufferSize];
            String responseData = String.Empty;

            string message = string.Format(registerCommandTemplate, username, password, token);
            data = System.Text.Encoding.UTF8.GetBytes(this.applicationToken + message);
            stream.Write(data, 0, data.Length);

            data = new Byte[bufferSize];
            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.UTF8.GetString(data, 0, bytes);

            if (responseData.Split(':')[0] == incorrectResponseCode)
            {
                MessageBox.Show(VoteClient.errors[int.Parse(responseData.Split(':')[1])].Item2, "Error");
                errorCode = int.Parse(responseData.Split(':')[1]);
                return null;
            }
            errorCode = 0;

            return responseData.Split(':')[1];
        }

        public string RemoveVoting(string username, string token, string votingId, out int errorCode)
        {
            TcpClient tcpClient = new TcpClient(this.serverAddress, port);
            tcpClient.ReceiveTimeout = timeout;

            NetworkStream stream = tcpClient.GetStream();

            Byte[] data = new Byte[bufferSize];
            String responseData = String.Empty;

            string message = string.Format(removeVotingTemplate, username, token, votingId);
            data = System.Text.Encoding.UTF8.GetBytes(this.applicationToken + message);
            stream.Write(data, 0, data.Length);

            data = new Byte[bufferSize];
            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.UTF8.GetString(data, 0, bytes);

            if (responseData.Split(':')[0] == incorrectResponseCode)
            {
                MessageBox.Show(VoteClient.errors[int.Parse(responseData.Split(':')[1])].Item2, "Error");
                errorCode = int.Parse(responseData.Split(':')[1]);
                return null;
            }
            errorCode = 0;

            return responseData.Split(':')[1];
        }

        public string AddNewVoting(string username, string token, VotingModel votingModel, out int errorCode)
        {
            TcpClient tcpClient = new TcpClient(this.serverAddress, port);
            tcpClient.ReceiveTimeout = timeout;

            NetworkStream stream = tcpClient.GetStream();

            Byte[] data = new Byte[bufferSize];
            String responseData = String.Empty;

            string message = string.Format(addNewVotingTemplate, username, token, this.EncodeNew(votingModel));
            data = System.Text.Encoding.UTF8.GetBytes(this.applicationToken + message);
            stream.Write(data, 0, data.Length);

            data = new Byte[bufferSize];
            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.UTF8.GetString(data, 0, bytes);

            if (responseData.Split(':')[0] == incorrectResponseCode)
            {
                MessageBox.Show(VoteClient.errors[int.Parse(responseData.Split(':')[1])].Item2, "Error");
                errorCode = int.Parse(responseData.Split(':')[1]);
                return null;
            }
            errorCode = 0;

            return responseData.Split(':')[1];
        }

        public string EncodeNew(VotingModel votingModel)
        {
            string value = string.Empty;
            value += votingModel.owner + ":";
            value += votingModel.votingTitle + ":";
            value += votingModel.password + ":";
            value += votingModel.anonymous + ":";
            value += votingModel.mutuallyExclusive + ":";
            value += votingModel.allowUnregisteredUsers + ":";
            value += votingModel.maxOptions + ":";
            value += votingModel.votingOptionsRaw.Count.ToString() + ":";
            foreach (var item in votingModel.votingOptionsRaw)
            {
                value += item + ':';
            }
            value += "0::";
            value += "0:";

            return value;
        }

        public string RemoveLast(string text, string character)
        {
            if (text.Length < 1) return text;
            return text.Remove(text.ToString().LastIndexOf(character), character.Length);
        }


        public static readonly Tuple<int, string>[] errors = new Tuple<int, string>[18] 
        {
            new Tuple<int,string>(0,"Succes."),
            new Tuple<int,string>(1,"No return value."),
            new Tuple<int,string>(2,"Wrong request."),
            new Tuple<int,string>(3,"Wrong password."),
            new Tuple<int,string>(4,"Username not found."),
            new Tuple<int,string>(5,"User already exists."),
            new Tuple<int,string>(6,"Requested voting not found."),
            new Tuple<int,string>(7,"Wrong number format."),
            new Tuple<int,string>(8,"Account is not valid."),
            new Tuple<int,string>(9,"There are no votings that meet the requirements."),
            new Tuple<int,string>(10,"Already voted!"),
            new Tuple<int,string>(11,"Only one option may be chosen."),
            new Tuple<int,string>(12,"Password is required to enter this vote."),
            new Tuple<int,string>(13,"You must be logged in to enter this vote."),
            new Tuple<int,string>(14,"Application is not authorized."),
            new Tuple<int,string>(15,"Too many options selected."),
            new Tuple<int,string>(16,"Voting has no owner."),
            new Tuple<int,string>(17,"Voting has no title."),
        };
    }
}
