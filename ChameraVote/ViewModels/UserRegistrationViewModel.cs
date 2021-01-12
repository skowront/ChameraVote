using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChameraVote.Models;
using ChameraVote.Utility;

namespace ChameraVote.ViewModels
{
    public class UserRegistrationViewModel:BaseViewModel, IPropertyStringValid
    {
        private UserRegistrationModel userRegistrationModel = new UserRegistrationModel();

        public string Username
        {
            get { return this.userRegistrationModel.Username; }
            set { this.userRegistrationModel.Username = value; this.OnPropertyChanged(); }
        }

        public string RegistrationToken
        {
            get { return this.userRegistrationModel.RegistrationToken; }
            set { this.userRegistrationModel.RegistrationToken = value; this.OnPropertyChanged(); }
        }

        public string Status
        {
            get { return this.userRegistrationModel.Status; }
            set { this.userRegistrationModel.Status = value; this.OnPropertyChanged(); }
        }

        public string CheckPasswordStrength(string password)
        {
            int level = 5;
            if(password.Length>8)
            {
                level += 1;
            }
            else if(password.Length>10)
            {
                level += 5;
            }
            else if(password.Length<4)
            {
                level -= 5;
            }
            if(level>4&&level<6)
            {
                return PasswordStrength.medium;
            }
            if(level<=4)
            {
                return PasswordStrength.bad;
            }
            else
            {
                return PasswordStrength.good;
            }
        }

        public class PasswordStrength
        {
            public const string bad = "Weak";
            public const string medium = "Medium";
            public const string good = "Good";
        }

        public bool PropertiesValid()
        {
            bool val = true;
            if (this.Username.Contains(':') || this.RegistrationToken.Contains(':'))
            {
                this.Status = "':' not allowed.";
                val = false;
            }
            return val;
        }
    }
}
