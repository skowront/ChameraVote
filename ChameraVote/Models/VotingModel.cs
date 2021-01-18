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

        public int maxOptions = 1;

        public string password = "";

        public string ballotId = "";

        public int blindFactor = 0;

        public string signature = "";

        public Collection<string> votingOptionsRaw = new Collection<string>();

        public Collection<string> votingResults = new Collection<string>();

        public Collection<string> votingClients = new Collection<string>();
    }
}
