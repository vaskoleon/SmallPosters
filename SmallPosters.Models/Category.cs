using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmallPosters.Models
{
    public class Category
    {
        public Category(string name)
        {
            Name = name;
            Ads = new List<Ad>();
        }

        public Guid Id { get; set; }
        [MinLength(4)]
        [MaxLength(18)]
        public string Name { get; set; }
        public ICollection<Ad> Ads { get; set; }
    }
}