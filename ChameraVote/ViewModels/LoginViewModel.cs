using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChameraVote.Models;
using ChameraVote.Utility;

namespace ChameraVote.ViewModels
{
    public class LoginViewModel:BaseViewModel,IPropertyStringValid
    {
        private LoginModel loginModel = new LoginModel();

        public string Username
        {
            get { return this.loginModel.username; }
            set { this.loginModel.username = value; this.OnPropertyChanged(); }
        }

        public string UserPassword
        {
            get { return this.loginModel.userPassword; }
            set { this.loginModel.userPassword = value; this.OnPropertyChanged(); }
        }

        public string Token
        {
            get { return this.loginModel.token; }
            set { this.loginModel.token = value; this.OnPropertyChanged(); }
        }

        public string Status
        {
            get { return this.loginModel.status; }
            set { this.loginModel.status = value; this.OnPropertyChanged(); }
        }

        public bool PropertiesValid()
        {
            bool val = true; 
            if (this.Username.Contains(':'))
            {
                this.Status = "':' not allowed.";
                val = false;
            }
            return val;
        }
    }
}
