using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReminderForOthers.Model
{
    internal class SignUpSingleton
    {
        
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string birthDate { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string rePassword { get; set; }

        public void ClearAllData() {
            firstName = "";
            lastName = "";
            birthDate = "";
            username = "";
            email = "";
            password = "";
            rePassword = "";
        }
    }
}
