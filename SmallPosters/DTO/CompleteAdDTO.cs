using SmallPosters.Models;
using SmallPosters.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmallPosters.Web.DTO
{
    public class CompleteAdDTO
    {
        [Required]
        public string AdId { get; set; }
        [Required]
        public string AdTitle { get; set; }
        [Required]
        public string CategoryName { get; set; }
        [Required]
        public string AdContent { get; set; }
        [Required]
        public string CreatorUsername { get; set; }
        public string ImageURL { get; set; }
        [Required]
        public string AdTimeframe { get; set; }
        [Required]
        public bool HasExpired { get; set; }
        [Required]
        public AdminApprovalState AdminApprovalState { get; set; }
        public CompleteAdDTO(Ad ad)
        {
            this.AdId = ad.Id.ToString();
            this.AdTitle = ad.Title;
            this.CategoryName = ad.Category.Name;
            this.AdContent = ad.Content;
            this.CreatorUsername = ad.Creator.Username;
            this.ImageURL = ad.ImageURL;
            this.AdTimeframe = ad.AdTimeframe.ToString();
            this.HasExpired = ad.HasExpired;
            this.AdminApprovalState = ad.AdminApprovalState;
        }
    }
}
