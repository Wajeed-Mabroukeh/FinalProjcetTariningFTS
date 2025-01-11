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

    #region User
   
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
   
    #region Room Availability
    
    [HttpGet("RoomAvailability")]
    public List<Room> GetAvailabileRoom(AvailableRequest request)
    {
        return entity.GetAvailabileRoom(request);
    }
   
    #endregion
    
    
    #region Get Login User
    
    [HttpGet("User/Login")]
    public User GetLoginUser()
    {
        return entity.GetLoginUser();
    }
    #endregion
   
    #region BookRoom Payment
   // [Authorize(Roles = "User")]
    [HttpPost("BookRoom/Payment")] 
    public async Task<string> BookRoom_Payment([FromBody] BookRequest request)
    {
        return await entity.BookRoom_Payment(request);
    }
    #endregion
    #endregion

    
    #region Admin
    
    #region Upload Image

    [HttpPost("Upload")]
    public async Task<string> UploadImage([FromForm] IFormFile imageFile)
    {
        return await entity.upload_image(imageFile);
    }

    #endregion
    
    #region Location

    [HttpPut("Location")]
    public async Task SetLocation([FromBody] SetLocation location)
    {
         await entity.SetLocation(location);
    }

    #endregion

 
    
    #region Management Cities API 
    
    [HttpPost("Create/Cities")]
    public void SetCities([FromBody] City city)
    {
        entity.SetCity(city);
    }
    
    [HttpPut ("Update/City")]
    public void UpdateCity([FromBody] City city)
    {
        entity.UpdateCity(city);
    }
    
    [HttpGet("GetCities")]
    public List<City> GetCities()
    {
        return entity.GetCities();
    }
    
    [HttpGet("GetCity/{id}")]
    public City GetCity(int id)
    {
        return entity.GetCity(id);
    }
    
    [HttpDelete("Delete/City/{id}")]
    public void DeleteCity(int id)
    {
        entity.DeleteCity(id);
    }
    
    
    [HttpDelete("Delete/Cities")]
    public void DeleteCities()
    {
         entity.DeleteCities();
    }
    
    #endregion
    
    #region Management Hotels API 
    
    
    [HttpPost("Create/Hotel")]
    public void SetHotel([FromBody] Hotel hotel)
    {
        entity.SetHotel(hotel);
    }
    
    [HttpPut ("Update/Hotel")]
    public void UpdateHotel([FromBody] Hotel hotel)
    {
        entity.UpdateHotel(hotel);
    }
    
    [HttpGet("GetHotel")]
    public List<Hotel> GetHotel()
    {
        return entity.GetHotels();
    }
    
    [HttpGet("GetHotel/{id}")]
    public Hotel GetHotel(int id)
    {
        return entity.GetHotel(id);
    }
    
    [HttpDelete("Delete/Hotel/{id}")]
    public void DeleteHotel(int id)
    {
        entity.DeleteHotel(id);
    }
    
    
    [HttpDelete("Delete/Hotels")]
    public void DeleteHotels()
    {
        entity.DeleteHotels();
    }
    
    #endregion
    
    #region Management Rooms API 
    
    
    [HttpPost("Create/Room")]
    public void SetRoom([FromBody] Room room)
    {
        entity.SetRoom(room);
    }
    
    [HttpPut ("Update/Room")]
    public void UpdateRoom([FromBody] Room room)
    {
        entity.UpdateRoom(room);
    }
    
    [HttpGet("GetRoom")]
    public List<Room> GetRoom()
    {
        return entity.GetRooms();
    }
    
    [HttpGet("GetRoom/{id}")]
    public Room GetRoom(int id)
    {
        return entity.GetRoom(id);
    }
    
    [HttpDelete("Delete/Room/{id}")]
    public void DeleteRoom(int id)
    {
        entity.DeleteRoom(id);
    }
    
    [HttpDelete("Delete/Rooms")]
    public void DeleteRooms()
    {
        entity.DeleteRooms();
    }
    
    #endregion
    
    #region Management Book Rooms API 
    
    
    [HttpPost("Create/BookRoom")]
    public void SetBookRoom([FromBody] BookRoom bookroom)
    {
        entity.SetBookRoom(bookroom);
    }
    
    [HttpPut ("Update/BookRoom")]
    public void UpdateBookRoom([FromBody] BookRoom bookroom)
    {
        entity.UpdateBookRoom(bookroom);
    }
    
    [HttpGet("GetBookRooms")]
    public List<BookRoom> GetBookRooms()
    {
        return entity.GetBookRooms();
    }
    
    [HttpGet("GetBookRoom/{id}")]
    public BookRoom GetBokRoom(int id)
    {
        return entity.GetBookRoom(id);
    }
    
    [HttpDelete("Delete/BookRoom/{id}")]
    public void DeleteBookRoom(int id)
    {
        entity.DeleteBookRoom(id);
    }
    
    [HttpDelete("Delete/BookRooms")]
    public void DeleteBookRooms()
    {
        entity.DeleteBookRooms();
    }
    
    #endregion
    
    
    #endregion
    
}
