using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmallPosters.Web.DTO
{
    public class AdminApproveDTO
    {
        [Required]
        public string AdId { get; set; }
        [Required]
        public string AdminUsername { get; set; }
        [Required]
        public string AuthToken { get; set; }
        [Required]
        public string AdDecision { get; set; }
    }
}
