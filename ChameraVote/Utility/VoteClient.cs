using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using ChameraVote.Models;

namespace ChameraVote.Utility
{
    public class VoteClient
    {
        private string serverAddress = "localhost";
        //private string ServerAddress = "46.41.151.157";

        private const string defaultPassword = "0";

        private const int port = 16402;

        private const string incorrectResponseCode = "NOK";
        
        private const string correctResponseCode = "OK";

        private const string commandTemplate = "command:{0}:{1}:{2}";

        private const string castVoteTemplate = "command:castVote:{0}:{1}:{2}:{3}";

        private const int timeout = 5000;

        public VoteClient(string serverAddress)
        {
            this.serverAddress = serverAddress;
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
                while (str[i] != ':') { i++; if (i >= str.Length) { break; } }
                model.votingClients.Add(str.Substring(0, i));
                str = str.Remove(0, Math.Min(i + 1, str.Length));
                i = 0;
            }

            return model;
        }

        public VotingModel GetVotingModel(string votingId, string password = defaultPassword)
        {
            TcpClient tcpClient = new TcpClient(this.serverAddress, port);
            tcpClient.ReceiveTimeout = timeout;

            NetworkStream stream = tcpClient.GetStream();

            Byte[] data = new Byte[256];
            String responseData = String.Empty;

            string message = string.Format(commandTemplate, "getVotingById", password, votingId);
            data = System.Text.Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);

            data = new Byte[256];
            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

            while(responseData[responseData.Length-1]=='&')
            {
                responseData = this.RemoveLast(responseData, "&");
                data = new Byte[256];
                bytes = stream.Read(data, 0, data.Length);
                responseData += System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            }

            if (responseData.Split(':')[0] == incorrectResponseCode)
            {
                MessageBox.Show(responseData.Split(':')[1]);
                return null;
            }

            var noCode = string.Empty;
            for(int i=3;i<responseData.Length;i++)
            {
                noCode += responseData[i];
            }

            return this.VotingModelFromString(noCode);
        }

        public string GetTitle(string votingId, string password = defaultPassword)
        {
            TcpClient tcpClient = new TcpClient(this.serverAddress, port);
            tcpClient.ReceiveTimeout = timeout;

            NetworkStream stream = tcpClient.GetStream();

            Byte[] data = new Byte[256];
            String responseData = String.Empty;

            string message = string.Format(commandTemplate, "getTitle", password, votingId);
            data = System.Text.Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);

            data = new Byte[256];
            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

            if (responseData.Split(':')[0] == incorrectResponseCode)
            {
                MessageBox.Show(responseData.Split(':')[1]);
                return null;
            }

            return responseData.Split(':')[1];
        }

        public bool? GetAnonymous(string votingId, string password = defaultPassword)
        {
            TcpClient tcpClient = new TcpClient(this.serverAddress, port);
            tcpClient.ReceiveTimeout = timeout;

            NetworkStream stream = tcpClient.GetStream();

            Byte[] data = new Byte[256];
            String responseData = String.Empty;

            string message = string.Format(commandTemplate, "getAnonymous", password, votingId);
            data = System.Text.Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);

            data = new Byte[256];
            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

            if (responseData.Split(':')[0] == incorrectResponseCode)
            {
                MessageBox.Show(responseData.Split(':')[1]);
                return null;
            }

            if(responseData.Split(':')[1].ToLower()=="true")
            {
                return true;
            }
            return false;
        }

        public IEnumerable<string> GetOptions(string votingId, string password = defaultPassword)
        {
            TcpClient tcpClient = new TcpClient(this.serverAddress, port);
            tcpClient.ReceiveTimeout = timeout;

            NetworkStream stream = tcpClient.GetStream();

            Byte[] data = new Byte[256];
            String responseData = String.Empty;

            string message = string.Format(commandTemplate, "getOptions", password, votingId);
            data = System.Text.Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);

            data = new Byte[256];
            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

            if(responseData.Split(':')[0] == incorrectResponseCode)
            {
                MessageBox.Show(responseData.Split(':')[1]);
                return null;
            }

            var values = responseData.Split(':');
            var collection = new Collection<string>();
            for(int i=1;i<values.Length;i++)
            {
                collection.Add(values[i]);
            }

            return collection;
        }

        public void SendVote(string votingId, Collection<string> selectedOptions, string username,string token, string password = defaultPassword)
        {
            TcpClient tcpClient = new TcpClient(this.serverAddress, port);
            tcpClient.ReceiveTimeout = timeout;

            NetworkStream stream = tcpClient.GetStream();

            Byte[] data = new Byte[256];
            String responseData = String.Empty;

            string message = string.Format(castVoteTemplate, username, password, votingId, token);
            
            foreach(var item in selectedOptions)
            {
                message += ":" + item;
            }

            data = System.Text.Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);

            data = new Byte[256];
            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

            if (responseData.Split(':')[0] == incorrectResponseCode)
            {
                MessageBox.Show(responseData.Split(':')[1]);
                return;
            }
        }

        public string Login(string username,string password)
        {
            TcpClient tcpClient = new TcpClient(this.serverAddress, port);
            tcpClient.ReceiveTimeout = timeout;

            NetworkStream stream = tcpClient.GetStream();

            Byte[] data = new Byte[256];
            String responseData = String.Empty;

            string message = string.Format(commandTemplate, "login", username,password);
            data = System.Text.Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);

            data = new Byte[256];
            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

            if (responseData.Split(':')[0] == incorrectResponseCode)
            {
                MessageBox.Show(responseData.Split(':')[1]);
                return null;
            }

            return responseData.Split(':')[1];
        }

        public string RemoveLast(string text, string character)
        {
            if (text.Length < 1) return text;
            return text.Remove(text.ToString().LastIndexOf(character), character.Length);
        }
    }
}
