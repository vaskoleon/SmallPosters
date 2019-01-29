using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmallPosters.Web.DTO
{
    public class CreateAdDTO
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string AuthToken { get; set; }
        [Required]
        public string AdTitle { get; set; }
        [Required]
        public string AdCategory { get; set; }
        [Required]
        public string AdContent { get; set; }
        [Required]
        public string AdTimeframe { get; set; }
        public string AdImageURL { get; set; }
    }
}
