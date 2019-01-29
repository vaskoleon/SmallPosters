using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using SmallPosters.Models.Cryptography;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace SmallPosters.Models
{
    public class Account
    {
        public Account()
        {

        }
        public Account(string name, string password, bool isAdmin)
        {
            Username = name;
            HashPair hashedPassword = HashPair.Generate(password);
            PasswordHash = hashedPassword.Hash;
            Salt = hashedPassword.Salt;
            IsAdmin = isAdmin;
            Ads = new List<Ad>();
            AuthTokens = new List<AuthToken>();
        }
        public Guid Id { get; set; }
        [MinLength(4)]
        [MaxLength(15)]
        [Required]
        public string Username { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        [Required]
        public byte[] Salt { get; set; }
        [Required]
        public bool IsAdmin { get; set; }
        public ICollection<Ad> Ads { get; set; }
        public ICollection<AuthToken> AuthTokens { get; set; }
    }
}
