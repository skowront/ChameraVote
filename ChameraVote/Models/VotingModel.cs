using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChameraVote.Models
{
    public class VotingModel
    {
        public string serverAddress = "localhost";

        public string owner = "";

        public string votingId = "";

        public string votingTitle = "";

        public bool anonymous = false;

        public bool mutuallyExclusive = false;

        public bool allowUnregisteredUsers = false;

        public string password = "";

        public Collection<string> votingOptionsRaw = new Collection<string>();

        public Collection<string> votingResults = new Collection<string>();

        public Collection<string> votingClients = new Collection<string>();
    }
}
