﻿using System.ComponentModel.DataAnnotations;

namespace Pharmaflow7.Models
{
    public class LoginViewModel
    {
        public int Id { get; set; }


        [Required] 
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
