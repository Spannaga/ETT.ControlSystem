using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Main.Control.Web.ViewModels
{
    public class SignInUI
    {
        public string EmailAddress { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsSuccess { get; set; }
    }
}