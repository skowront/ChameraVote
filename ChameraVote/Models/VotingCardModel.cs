using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace ChameraVote.Models
{
    public class VotingCardModel
    {
        public string username = "";

        public Collection<string> options = new Collection<string>();
    }
}
