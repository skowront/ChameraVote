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
using System.Security.Cryptography;
using System.Globalization;

namespace ChameraVote.Utility
{
    public class VoteClient
    {
        private string serverAddress = "localhost";
        //private string ServerAddress = "46.41.151.157";
        private string applicationToken = "";

        private int port = 16403;

        private const int bufferSize = 1024;

        private const string incorrectResponseCode = "NOK";
        
        private const string correctResponseCode = "OK";

        private const string commandTemplate = "command:{0}:{1}:{2}:{3}:{4}";

        private const string getBallotTemplate = "command:getBallot:{0}";

        private const string getSignedBallotTemplate = "command:getSignedBallot:{0}:{1}:{2}:{3}:{4}";
        
        private const string getUserVotingsTemplate = "command:getUserVotings:{0}:{1}:{2}";
        
        private const string loginCommandTemplate = "command:login:{0}:{1}";

        private const string validateCommandTemplate = "command:verify:{0}:{1}:{2}";
        
        private const string registerCommandTemplate = "command:register:{0}:{1}:{2}";

        private const string castVoteTemplate = "command:castVote:{0}:{1}:{2}:{3}:{4}";

        private const string addNewVotingTemplate = "command:addNewVoting:{0}:{1}:{2}";

        private const string removeVotingTemplate = "command:removeVoting:{0}:{1}:{2}";

        private const int timeout = 5000;

        public VoteClient(ConfigurationViewModel configurationModel)
        {
            this.serverAddress = configurationModel.ServerAddress;
            this.applicationToken = configurationModel.ApplicationToken;
            this.port = configurationModel.Port;
            VoteClient.errors = VoteClient.errorsPl;
        }

        public string CreateMD5Hash(string input)
        {
            // Step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
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

        public VotingModel GetVotingModel(string votingId,string username,string token, string password, out int errorCode)
        {
            TcpClient tcpClient = new TcpClient(this.serverAddress, port);
            tcpClient.ReceiveTimeout = timeout;

            NetworkStream stream = tcpClient.GetStream();

            Byte[] data = new Byte[bufferSize];
            String responseData = String.Empty;

            string message = string.Format(commandTemplate, "getVotingById",username,token, password, votingId);
            data = System.Text.Encoding.UTF8.GetBytes(this.applicationToken + ':' + message);
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
            var votingModel = this.VotingModelFromString(noCode);

            if(votingModel.anonymous)
            {
                var result = this.GetSignedClients(votingId, username, token, password, out errorCode);
                if (result != null)
                {
                    votingModel.votingSignedClients = new Collection<string>(result.ToList());
                }
                 
            }

            return votingModel;
        }

        public Collection<VotingBriefModel> GetUserVotingsBrief(string username, string token, string password, out int errorCode)
        {
            TcpClient tcpClient = new TcpClient(this.serverAddress, port);
            tcpClient.ReceiveTimeout = timeout;

            NetworkStream stream = tcpClient.GetStream();

            Byte[] data = new Byte[bufferSize];
            String responseData = String.Empty;

            string message = string.Format(getUserVotingsTemplate, username, token, password);
            data = System.Text.Encoding.UTF8.GetBytes(this.applicationToken + ':' + message);
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

        public string GetBallot(string votingId, out int errorCode)
        {
            TcpClient tcpClient = new TcpClient(this.serverAddress, port);
            tcpClient.ReceiveTimeout = timeout;

            NetworkStream stream = tcpClient.GetStream();

            Byte[] data = new Byte[bufferSize];
            String responseData = String.Empty;

            string message = string.Format(getBallotTemplate, votingId);
            data = System.Text.Encoding.UTF8.GetBytes(this.applicationToken + ':' + message);
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

        public string GetSignedBallot(string votingId, string username, string token, string password, string mPrime, out int errorCode)
        {
            TcpClient tcpClient = new TcpClient(this.serverAddress, port);
            tcpClient.ReceiveTimeout = timeout;

            NetworkStream stream = tcpClient.GetStream();

            Byte[] data = new Byte[bufferSize];
            String responseData = String.Empty;

            string message = string.Format(getSignedBallotTemplate, username, token, password, votingId, mPrime);
            data = System.Text.Encoding.UTF8.GetBytes(this.applicationToken + ':' + message);
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

        public string GetTitle(string votingId, string username, string token, string password,out int errorCode)
        {
            TcpClient tcpClient = new TcpClient(this.serverAddress, port);
            tcpClient.ReceiveTimeout = timeout;

            NetworkStream stream = tcpClient.GetStream();

            Byte[] data = new Byte[bufferSize];
            String responseData = String.Empty;

            string message = string.Format(commandTemplate, "getTitle", username, token, password, votingId);
            data = System.Text.Encoding.UTF8.GetBytes(this.applicationToken + ':' + message);
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
            data = System.Text.Encoding.UTF8.GetBytes(this.applicationToken + ':' + message);
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
            data = System.Text.Encoding.UTF8.GetBytes(this.applicationToken + ':' + message);
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

        public IEnumerable<string> GetSignedClients(string votingId, string username, string token, string password, out int errorCode)
        {
            TcpClient tcpClient = new TcpClient(this.serverAddress, port);
            tcpClient.ReceiveTimeout = timeout;

            NetworkStream stream = tcpClient.GetStream();

            Byte[] data = new Byte[bufferSize];
            String responseData = String.Empty;

            string message = string.Format(commandTemplate, "getSignedClients", username, token, password, votingId);
            data = System.Text.Encoding.UTF8.GetBytes(this.applicationToken + ':' + message);
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
            for (int i = 1; i < values.Length; i++)
            {
                collection.Add(values[i]);
            }

            return collection;
        }

        public bool SendVote(string votingId, Collection<string> selectedOptions, string username,string token, string password, string signature,string ballotId, bool anonymous, out int errorCode)
        {
            TcpClient tcpClient = new TcpClient(this.serverAddress, port);
            tcpClient.ReceiveTimeout = timeout;

            NetworkStream stream = tcpClient.GetStream();

            Byte[] data = new Byte[bufferSize];
            String responseData = String.Empty;
            string message = string.Empty;
            if(anonymous)
            {
                message = string.Format(castVoteTemplate, ballotId, password, votingId, string.Empty, signature);
            }
            else
            {
                message = string.Format(castVoteTemplate, username, password, votingId, token, signature);
            }
            
            foreach(var item in selectedOptions)
            {
                message += ":" + item;
            }

            data = System.Text.Encoding.UTF8.GetBytes(this.applicationToken + ':' + message);
            stream.Write(data, 0, data.Length);

            data = new Byte[bufferSize];
            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.UTF8.GetString(data, 0, bytes);

            if (responseData.Split(':')[0] == incorrectResponseCode || responseData == string.Empty)
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

            string passhash = this.CreateMD5Hash(password).ToLower();
            string message = string.Format(loginCommandTemplate, username, passhash);
            data = System.Text.Encoding.UTF8.GetBytes(this.applicationToken + ':' + message);
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

            string passhash = this.CreateMD5Hash(password).ToLower();
            string message = string.Format(registerCommandTemplate, username, token, passhash);
            data = System.Text.Encoding.UTF8.GetBytes(this.applicationToken + ':' + message);
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
            data = System.Text.Encoding.UTF8.GetBytes(this.applicationToken + ':' + message);
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
            data = System.Text.Encoding.UTF8.GetBytes(this.applicationToken + ':' + message);
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

        public bool Verify(string votingId, string cardId, string signature, out int errorCode)
        {
            TcpClient tcpClient = new TcpClient(this.serverAddress, port);
            tcpClient.ReceiveTimeout = timeout;

            NetworkStream stream = tcpClient.GetStream();

            Byte[] data = new Byte[bufferSize];
            String responseData = String.Empty;

            string message = string.Format(validateCommandTemplate, cardId, signature, votingId);
            data = System.Text.Encoding.UTF8.GetBytes(this.applicationToken + ':' + message);
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
            MessageBox.Show("Vote id and signature match.");
            return true;
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


        public static Tuple<int, string>[] errors = new Tuple<int, string>[23] 
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
            new Tuple<int,string>(18,"Bad registration token."),
            new Tuple<int,string>(19,"This username is not allowed."),
            new Tuple<int,string>(20,"You may not recieve any more ballots."),
            new Tuple<int,string>(21,"Wrong signature."),
            new Tuple<int,string>(22,"Card not found."),
        };

        public static Tuple<int, string>[] errorsPl = new Tuple<int, string>[23]
{
            new Tuple<int,string>(0,"Sukces."),
            new Tuple<int,string>(1,"Brak wartości zwrotnej."),
            new Tuple<int,string>(2,"Błędne żądanie."),
            new Tuple<int,string>(3,"Złe hasło."),
            new Tuple<int,string>(4,"Użytkownik nie odnaleziony."),
            new Tuple<int,string>(5,"Użytkownik już istnieje."),
            new Tuple<int,string>(6,"Żądane głosowanie nie zostało odnalezione."),
            new Tuple<int,string>(7,"Zły format numeru."),
            new Tuple<int,string>(8,"Konto nie jest poprawne."),
            new Tuple<int,string>(9,"Nie ma głosowań spełniających wymagania."),
            new Tuple<int,string>(10,"Już głosowano"),
            new Tuple<int,string>(11,"Tylko jedna opcja może być wybrana."),
            new Tuple<int,string>(12,"Potrzebujesz hasła by dostać się do tego głosowania."),
            new Tuple<int,string>(13,"Musisz być zalogowany by dostać się do tego głosowania"),
            new Tuple<int,string>(14,"Aplikacja nie jest autoryzowana."),
            new Tuple<int,string>(15,"Za dużo wybranych opcji."),
            new Tuple<int,string>(16,"Brak właściciela głosowania."),
            new Tuple<int,string>(17,"Głosowanie nie ma tytułu."),
            new Tuple<int,string>(18,"Zły token rejestracji."),
            new Tuple<int,string>(19,"Ta nazwa jest niedozwolona."),
            new Tuple<int,string>(20,"Nie możesz otrzymać więcej kart."),
            new Tuple<int,string>(21,"Zła sygnatura."),
            new Tuple<int,string>(22,"Nie odnaleziono karty."),
};
    }
}
