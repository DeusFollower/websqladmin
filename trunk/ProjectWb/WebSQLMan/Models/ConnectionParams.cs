﻿using System.ComponentModel.DataAnnotations;

namespace WebSQLMan.Models
{
    public class ConnectionParams
    {
        [Required(ErrorMessage = "The field must be field")]
        public string ServerName { get; set; }
        [Required]
        public string DataBase { get; set; }
        [Required]
        public  Auth Authentification{ get; set; }
        [Required]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }

        public enum Auth
        {
            SqlAuthentication,
            WindowsAuthentication
        }
    }
}
