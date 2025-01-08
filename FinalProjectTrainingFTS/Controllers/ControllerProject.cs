using Azure.Core;
using FinalProjectTrainingFTS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinalProjectTrainingFTS.Controllers;


[ApiController]
[Route("/api/")]
public class ControllerProject : ControllerBase
{
    
   private readonly IConfiguration _configuration;
   private CustomerService.CustomerService entity;
   public ControllerProject(IConfiguration configuration)
   {
       _configuration = configuration;
       entity = new CustomerService.CustomerService(_configuration);
   }
   
    
    #region FeaturedDeals
    [Authorize(Roles = "User")]
    [HttpGet("FeaturedDeals")] 
    public List<FeaturedDealsResponse> GetFeaturedDealsHotels()
    {
        return entity.GetFeaturedDealsHotels();
    }
    #endregion
 
    #region User's Recently Visited Hotels
    [Authorize(Roles = "User")]
    [HttpGet("RecentlyVisitedHotels")] 
    public List<Hotel> GetRecentlyVisitedHotels()
    {
        return entity.GetRecentlyVisitedHotels();
    }
    
    [Authorize(Roles = "User")]
    [HttpPut("Updatevisitedhoteluser")] 
    public void PutUpdateVisitedHotel([FromBody] HotelVisitedRequest request)
    {
         entity.Updatevisitedhoteluser(request.IdHotelVisited);
    }
    #endregion
    
    #region Trending Destination Highlights
   [Authorize(Roles = "User")]
   [HttpGet("TrendingDestinationHighlights")] 
   public List<City> TrendingDestinationHighlights()
   {
       return entity.GetTrendingDestinationHighlights();
   }
    #endregion
    
    #region Search Results Page
    
    [Authorize(Roles = "User")]
    [HttpGet("SearchResultsHotel")] 
    public List<Hotel> GetSearchHotels([FromBody] SearchRequest request)
    {
        return entity.SearchHotels(request);
    }
    
    [Authorize(Roles = "User")]
    [HttpGet("SearchResultsPriceRange")] 
    public List<Hotel> SearchResultsPriceRange([FromBody] SearchRequest request)
    {
        return entity.GetSearchResultsPriceRange(request);
    }
    
    [Authorize(Roles = "User")]
    [HttpGet("SearchResultsStarAmenities")] 
    public List<Hotel> SearchResultsStarAmenities([FromBody] SearchRequest request)
    {
        return entity.GetSearchResultsStarAmenities(request);
    }
    
    [Authorize(Roles = "User")]
    [HttpGet("SearchResultsStarRating")] 
    public List<Hotel> SearchResultsStarRating([FromBody] SearchRequest request)
    {
        return entity.GetSearchResultsStarRating(request);
    }
    
    #endregion

    #region Visual Gallery (Get Image)
    
    [HttpGet("get-image/{imageName}")]
    public IActionResult GetImage(string imageName)
    {
        return entity.GetImage(imageName);
    }
    #endregion

    #region MyRegion

    [HttpPost("Upload")]
    public async Task<string> UploadImage([FromForm] IFormFile imageFile)
    {
        return await entity.upload_image(imageFile);
    }

    #endregion
    
}
