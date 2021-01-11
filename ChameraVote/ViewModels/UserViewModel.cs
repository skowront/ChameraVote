using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChameraVote.Models;

namespace ChameraVote.ViewModels
{
    public class UserViewModel:BaseViewModel
    {
        public UserModel userModel = new UserModel();

        public string Username
        {
            get { return this.userModel.username; }
            set { this.userModel.username = value; this.OnPropertyChanged(); }
        }

        public string Token
        {
            get { return this.userModel.token; }
            set { this.userModel.token = value; this.OnPropertyChanged(); }
        }
    }
}
