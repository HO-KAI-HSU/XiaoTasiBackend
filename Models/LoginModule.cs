﻿using System;
namespace travelManagement.Models
{
    public class LoginModule
    {
        public LoginModule() { }

        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public int Role { get; set; }
        public int Modify { get; set; }
        public int Status { get; set; }
    }
}
