using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace SmallPosters.Models
{
    public class AuthToken
    {
        private const int HoursLimit = 5;
        private bool _renderedInvalid = false;
        public AuthToken()
        {
            this.DateOfCreation = DateTime.Now;
        }
        public AuthToken(string hashedValue,Account account)
        {
            this.DateOfCreation = DateTime.Now;
            this.HashedValue = hashedValue;
            this.Account = account;
        }
        public Guid Id { get; set; }
        [Required]
        public Guid AccountId { get; set; }
        public Account Account { get; set; }
        [Required]
        public string HashedValue { get; set; }
        [Required]
        public DateTime DateOfCreation { get; set; }
        public bool IsValid
        {
            get
            {
                if (_renderedInvalid)
                {
                    return false;
                }
                else
                {
                    if ((DateTime.Now - this.DateOfCreation).TotalHours < HoursLimit)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            set
            {
                this._renderedInvalid = !value;
            }
        }
    }
}
