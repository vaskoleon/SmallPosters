using SmallPosters.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SmallPosters.Models
{
    public class Ad
    {
        public Ad()
        {
            DateOfCreation = DateTime.Now;
            AdminApprovalState = AdminApprovalState.Pending;
        }

        public Ad(Category category, string content,string title, AdTimeframe adTimeframe, Account creator, string imageURL)
            :this()
        {
            Title = title;
            Category = category;
            Content = content;
            AdTimeframe = adTimeframe;
            Creator = creator;
            ImageURL = imageURL;
        }

        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        [Required]
        public Category Category { get; set; }
        [Required]
        [MinLength(4)]
        [MaxLength(25)]
        public string Title { get; set; }
        [Required]
        [MinLength(2)]
        [MaxLength(25)]
        public string Content { get; set; }
        [Required]
        public DateTime DateOfCreation { get; set; }
        [Required]
        public AdTimeframe AdTimeframe { get; set; }
        [Required]
        public Guid CreatorId { get; set; }
        public Account Creator { get; set; }
        public string ImageURL { get; set; }
        [Required]
        public AdminApprovalState AdminApprovalState { get; set; }
        public bool HasExpired
        {
            get
            {
                DateTime currentDate = DateTime.Now;
                if (this.AdTimeframe == AdTimeframe.OneDay)
                {
                    if ((currentDate - this.DateOfCreation).TotalDays >= 1)
                    {
                        return true;
                    }
                }
                else if (this.AdTimeframe==AdTimeframe.OneWeek)
                {
                    if ((currentDate - this.DateOfCreation).TotalDays >= 7)
                    {
                        return true;
                    }
                }
                else if (this.AdTimeframe == AdTimeframe.OneMonth)
                {
                    if ((currentDate - this.DateOfCreation).TotalDays >= 30)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

    }
}
