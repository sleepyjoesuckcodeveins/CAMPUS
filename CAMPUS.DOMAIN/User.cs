using System;
using System.Collections.Generic;
using System.Text;
using CAMPUS.DOMAIN.Enums;
/*
 * This is the User class, it is used to represent a user in the system.
 * It contains the following properties:
 * - Id: the unique identifier of the user
 * - Username: the username of the user
 * - Email: the email of the user
 * - Role: the role of the user (Employee, SupportTeam, FacilitiesManager, Administrator)
 * - AuthId: the Auth0 id of the user
 */
namespace CAMPUS.DOMAIN
{
    public class UserModel
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
 
        private Roles _role;
        public Roles Role
        {
            get { return _role; }
            set { _role = value; }
        }
        private string _authId;
        public string AuthId
        {
            get { return _authId; }
            set { _authId = value; }
        }
    }
}
