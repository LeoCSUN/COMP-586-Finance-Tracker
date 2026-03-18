using System;
using System.Collections.Generic;
using System.Text;

namespace finance_tracker_comp586.services
{
    public class UserDto
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
        public string Name { get; set; }
    }
}
