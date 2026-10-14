using System;
using System.Collections.Generic;
using System.Text;

namespace CAMPUS.DOMAIN
{
    public class User
    {
        //this is subject to change, the database will need to be checked on since we are using Auth0 and it has it own db
        private int _id;
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        private string _username;
        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }
        private string _email;
        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }
        private string _password;
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
    }
}
