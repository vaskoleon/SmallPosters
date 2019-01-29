using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using SmallPosters.Data;
using SmallPosters.Models;
using SmallPosters.Models.Cryptography;
using SmallPosters.Models.Enums;
using SmallPosters.Web.DTO;
using SmallPosters.Web.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmallPosters.Web.Controllers
{
    [Route("api/[controller]")]
    public class AdController : Controller
    {
        SmallPostersContext _smallPostersContext;
        IAdService _adService;
        public AdController(SmallPostersContext smallPostersContext,IAdService adService)
        {
            this._smallPostersContext = smallPostersContext;
            this._adService = adService;
        }
        [HttpGet("search")]
        public async Task<ActionResult<ICollection<CompleteAdDTO>>> SearchAds(SearchDTO searchDTO)
        {
            if (searchDTO.SearchQuery == null || searchDTO.SearchQuery == "")
            {
                return await GetAllAds();
            }
            string caseInsensitiveQuery = searchDTO.SearchQuery.ToLower();
            return await _smallPostersContext.Ads.Include(a => a.Category).Include(a => a.Creator).
                Where(a => (!a.HasExpired)&&(a.Title.ToLower().Contains(caseInsensitiveQuery))).Select(a => new CompleteAdDTO(a)).ToListAsync();
        }

        [HttpPut("judge")]
        public async Task<ActionResult<CompleteAdDTO>> JudgeAd([FromBody]AdminApproveDTO adminApproveDTO)
        {
            string username = adminApproveDTO.AdminUsername;
            string authTokenString = adminApproveDTO.AuthToken;
            Models.Account adminClaimant = await _adService.GetUser(username, authTokenString);
            if (adminClaimant == null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }
            if (!adminClaimant.IsAdmin)
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }
            Ad adToFind = await _adService.GetAdAsAdmin(adminClaimant, adminApproveDTO.AdId);
            if (adToFind == null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }
            if (adminApproveDTO.AdDecision == "approve")
            {
                adToFind.AdminApprovalState = AdminApprovalState.Approved;
            }
            else if(adminApproveDTO.AdDecision == "reject")
            {
                adToFind.AdminApprovalState = AdminApprovalState.Rejected;
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            await _smallPostersContext.SaveChangesAsync();
            return StatusCode(StatusCodes.Status200OK);
        }
        [HttpGet("all")]
        public async Task<ActionResult<ICollection<CompleteAdDTO>>> GetAllAds()
        {
            return await _smallPostersContext.Ads.Include(a => a.Category).Include(a => a.Creator).
                Where(a=>(!a.HasExpired)&&a.AdminApprovalState==AdminApprovalState.Approved)
                .Select(a=>new CompleteAdDTO(a)).ToListAsync();
        }
        [HttpGet("pending")]
        public async Task<ActionResult<ICollection<CompleteAdDTO>>> GetPendingAds(UserIdentifiedDTO userIdentifiedDTO)
        {
            string username = userIdentifiedDTO.Username;
            string authTokenString = userIdentifiedDTO.AuthToken;

            Models.Account accountToFind = await _adService.GetUser(username, authTokenString);
            if (accountToFind == null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }
            if (!accountToFind.IsAdmin)
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }
            return await _smallPostersContext.Ads.Include(a => a.Category).Include(a => a.Creator).
                Where(a => a.AdminApprovalState==AdminApprovalState.Pending).Select(a => new CompleteAdDTO(a)).ToListAsync();
        }
        [HttpGet("mine")]
        public async Task<ActionResult<ICollection<CompleteAdDTO>>> GetMyAds(UserIdentifiedDTO userIdentifiedDTO)
        {
            string username = userIdentifiedDTO.Username;
            string authTokenString = userIdentifiedDTO.AuthToken;
            Models.Account accountToFind = await _adService.GetUser(username, authTokenString);
            if (accountToFind == null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            return await _smallPostersContext.Ads.Include(a => a.Category).Include(a => a.Creator)
            .Where(a=>a.CreatorId==accountToFind.Id).Select(a => new CompleteAdDTO(a)).ToListAsync();
        }
        [HttpPost("create")]
        public async Task<ActionResult<AdCreatedDTO>> CreateAd([FromBody]CreateAdDTO createAdDTO)
        {
            AdTimeframe adTimeframe;
            if(!Enum.TryParse(createAdDTO.AdTimeframe, out adTimeframe))
            {
                return StatusCode(StatusCodes.Status417ExpectationFailed);
            }
            string username = createAdDTO.Username;
            string authTokenString = createAdDTO.AuthToken;
            Models.Account accountToFind = await _adService.GetUser(username, authTokenString);
            if (accountToFind == null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }
            Category categoryToFind = await _adService.GetCategory(createAdDTO.AdCategory);
            Ad ad = new Ad(categoryToFind, createAdDTO.AdContent, createAdDTO.AdTitle,
                adTimeframe, accountToFind, createAdDTO.AdImageURL);
            _smallPostersContext.Ads.Add(ad);
            await _smallPostersContext.SaveChangesAsync();
            return new AdCreatedDTO { AdId=ad.Id.ToString(),AdTitle=ad.Title};
        }
        [HttpPut("republish")]
        public async Task<ActionResult<CompleteAdDTO>> RepublishAd([FromBody]ModifyAdDTO modifyAdDTO)
        {
            string username = modifyAdDTO.Username;
            string authTokenString = modifyAdDTO.AuthToken;
            Models.Account accountToFind = await _adService.GetUser(username, authTokenString);
            if (accountToFind == null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }
            Ad adToFind = await _adService.GetAd(accountToFind.Id, modifyAdDTO.AdId);
            if (adToFind == null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }
            adToFind.DateOfCreation = DateTime.Now;
            await _smallPostersContext.SaveChangesAsync();
            return StatusCode(StatusCodes.Status200OK);
        }
        [HttpDelete("delete")]
        public async Task<ActionResult<CompleteAdDTO>> DeleteAd([FromBody]ModifyAdDTO modifyAdDTO)
        {
            string username = modifyAdDTO.Username;
            string authTokenString = modifyAdDTO.AuthToken;
            Models.Account accountToFind = await _adService.GetUser(username, authTokenString);
            if (accountToFind == null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }
            Ad adToFind = await _adService.GetAd(accountToFind.Id, modifyAdDTO.AdId);
            if (adToFind == null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }
            _smallPostersContext.Remove(adToFind);
            await _smallPostersContext.SaveChangesAsync();
            return StatusCode(StatusCodes.Status200OK);
        }
        [HttpPut("edit")]
        public async Task<ActionResult<CompleteAdDTO>> EditAd([FromBody]EditAdDTO editAdDTO)
        {
            string username = editAdDTO.Username;
            string authTokenString = editAdDTO.AuthToken;
            Models.Account accountToFind = await _adService.GetUser(username, authTokenString);
            if (accountToFind == null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }
            Ad adToFind = await _adService.GetAd(accountToFind.Id,editAdDTO.AdId);
            if (adToFind == null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }
            string adTitle = editAdDTO.AdTitle;
            string categoryName = editAdDTO.CategoryName;
            string adContent = editAdDTO.AdContent;
            string imageURL = editAdDTO.ImageURL;
            if (adTitle != null && adTitle != "")
            {
                adToFind.Title = adTitle;
            }
            if (categoryName != null && categoryName != "")
            {
                Category category = await _adService.GetCategory(categoryName);
                adToFind.Category = category;
            }
            if (adContent != null && adContent != "")
            {
                adToFind.Content = adContent;
            }
            if (imageURL != null && imageURL != "")
            {
                adToFind.ImageURL = imageURL;
            }
            await _smallPostersContext.SaveChangesAsync();
            return StatusCode(StatusCodes.Status200OK);
        }
        
    }
}
