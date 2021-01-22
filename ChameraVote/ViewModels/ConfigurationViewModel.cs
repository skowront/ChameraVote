﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ChameraVote.Models;

namespace ChameraVote.ViewModels
{
    public class ConfigurationViewModel : BaseViewModel
    {
        private ConfigurationModel configurationModel = new ConfigurationModel();

        public string ServerAddress
        {
            get { return this.configurationModel.serverAddress; }
            set { this.configurationModel.serverAddress = value; this.OnPropertyChanged(); }
        }

        public string ApplicationToken
        {
            get { return this.configurationModel.applicationToken; }
            set { this.configurationModel.applicationToken = value; this.OnPropertyChanged(); }
        }

        public int Port
        {
            get { return this.configurationModel.port; }
            set { this.configurationModel.port = value; this.OnPropertyChanged(); }
        }

        public CultureInfo CultureInfo
        {
            get { return this.CultureInfo; }
            set { this.CultureInfo = value;this.OnPropertyChanged(); }
        }

        public EventHandler OnConfigurationChanged = null;

        protected override void OnPropertyChanged([CallerMemberName] string property = null)
        {
            base.OnPropertyChanged(property);
            this.OnConfigurationChanged?.Invoke(property, new EventArgs());
        }
    }
}
