using Microsoft.EntityFrameworkCore;
using SmallPosters.Data;
using SmallPosters.Models;
using SmallPosters.Models.Cryptography;
using SmallPosters.Web.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmallPosters.Web.Services
{
    public class AdService:IAdService
    {
        SmallPostersContext _smallPostersContext;
        public AdService(SmallPostersContext smallPostersContext)
        {
            this._smallPostersContext = smallPostersContext;
        }
        public async Task<Category> GetCategory(string categoryName)
        {
            Category category = await _smallPostersContext.Categories.
                    FirstOrDefaultAsync(a => a.Name == categoryName);
            if (category == null)
            {
                category = new Category(categoryName);
                _smallPostersContext.Categories.Add(category);
            }
            return category;
        }
        public async Task<Ad> GetAdAsAdmin(Account adminClaimant, string adIdString)
        {
            Guid adId;
            if (!Guid.TryParse(adIdString, out adId))
            {
                return null;
            }
            if (!adminClaimant.IsAdmin)
            {
                return null;
            }
            Ad adToFind = await _smallPostersContext.Ads.FirstOrDefaultAsync(a => a.Id == adId);
            if (adToFind == null)
            {
                return null;
            }
            return adToFind;
        }
        public async Task<Ad> GetAd(Guid userId, string adIdString)
        {
            Guid adId;
            if (!Guid.TryParse(adIdString, out adId))
            {
                return null;
            }
            Ad adToFind = await _smallPostersContext.Ads.FirstOrDefaultAsync(a => a.CreatorId == userId && a.Id == adId);
            if (adToFind == null)
            {
                return null;
            }
            return adToFind;
        }

        public async Task<Account> GetUser(string username, string authTokenString)
        {
            Models.Account accountToFind = _smallPostersContext.Accounts.FirstOrDefault
                (a => a.Username == username);
            if (accountToFind == null)
            {
                return null;
            }
            string hashedToken = HashPair.Generate(authTokenString, accountToFind.Salt);
            AuthToken authTokenToFind = await _smallPostersContext.AuthTokens.FirstOrDefaultAsync
                (a => a.IsValid == true && a.HashedValue == hashedToken);
            if (authTokenToFind == null)
            {
                return null;
            }
            return accountToFind;
        }
    }
}
