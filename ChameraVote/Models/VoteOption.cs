using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChameraVote.Models
{
    public class VoteOption
    {
        public string optionValue;

        public bool optionChecked = false;

        public VoteOption(string optionValue = "")
        {
            this.optionValue = optionValue;
        }
    }
}
