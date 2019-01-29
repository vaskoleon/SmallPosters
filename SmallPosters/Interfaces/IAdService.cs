using SmallPosters.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmallPosters.Web.Interfaces
{
    public interface IAdService
    {
        Task<Category> GetCategory(string categoryName);
        Task<Ad> GetAd(Guid userId, string adIdString);
        Task<Account> GetUser(string username, string authTokenString);
        Task<Ad> GetAdAsAdmin(Account adminClaimant, string adIdString);
    }
}
