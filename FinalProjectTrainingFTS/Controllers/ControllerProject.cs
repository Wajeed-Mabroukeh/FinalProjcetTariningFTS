using Azure.Core;
using FinalProjectTrainingFTS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

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
    [HttpPut("UpdateVisitedHotelUser")] 
    public void UpdateVisitedHotel([FromBody] HotelVisitedRequest request)
    {
         entity.UpdateVisitedHotelUser(request.IdHotelVisited);
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
    
    [Authorize(Roles = "User")]
    [HttpGet("GetImage/{imageName}")]
    public IActionResult GetImage(string imageName)
    {
        return entity.GetImage(imageName);
    }
    #endregion
   
    #region Room Availability
    
    [Authorize(Roles = "User")]
    [HttpGet("RoomAvailability")]
    public List<Room> GetAvailabileRoom(AvailableRequest request)
    {
        return entity.GetAvailabileRoom(request);
    }
   
    #endregion
    
    #region Get Login User
    [Authorize(Roles = "User")]
    [HttpGet("User/Login")]
    public User GetLoginUser()
    {
        return entity.GetLoginUser();
    }
    #endregion
   
    #region BookRoom Payment
    
    [Authorize(Roles = "User")]
    [HttpPost("BookRoom/Payment")] 
    public async Task<Response> BookRoom_Payment([FromBody] BookRequest request)
    {
        return await entity.BookRoom_Payment(request);
    }
    #endregion
    
    #region CreateCheckoutSession
    [HttpPost("create-checkout-session")]
    public Dictionary<string,string> CreateCheckoutSession([FromBody]CreateSessionRequest request)
    {
        return entity.CreateCheckoutSession(request);
    }
    #endregion

    #region Payment Stripe Webhook
    
    [HttpPost("payment/stripe-webhook")]
    public async Task<Dictionary<string,string>> Webhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
       
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                _configuration["Stripe:WebhookSecret"]
            );
           
            if (stripeEvent.Type == EventTypes.CheckoutSessionCompleted)
            {
                var session = stripeEvent.Data.Object as Session;
               
                Console.WriteLine(session.Id);
                return new Dictionary<string, string>
                {
                    {"Url",session.Url}
                };
            }
            return new Dictionary<string, string>
            {
                {"Error","Error"}
            };
          
        }
        catch (StripeException ex)
        {
            return new Dictionary<string, string>
            {
                {"Error",ex.StripeError.Error}
            };
        }
       
    }
    #endregion
    
    #endregion

    
    #region Admin
    
    #region Upload Image

    [Authorize(Roles = "Admin")]
    [HttpPost("Upload/Image")]
    public async Task<Response> UploadImage([FromForm] IFormFile imageFile)
    {
        return await entity.upload_image(imageFile);
    }

    #endregion
    
    #region Location

    [Authorize(Roles = "Admin")]
    [HttpPut("Location")]
    public async Task SetLocation([FromBody] SetLocation location)
    {
         await entity.SetLocation(location);
    }

    #endregion

    #region Add User
    [Authorize(Roles = "Admin")]
    [HttpPost("Add/User")]
    public void SetUser([FromBody] User user)
    {
        entity.SetUser(user);
    }
    #endregion 
    
    #region Add Admin
    [Authorize(Roles = "Admin")]
    [HttpPost("Add/Admin")]
    public void SetAdmin([FromBody] Admin admin)
    {
        entity.SetAdmin(admin);
    }
    

    #endregion
    
    #region Management Cities API 
    
    [Authorize(Roles = "Admin")]
    [HttpPost("Create/Cities")]
    public void SetCities([FromBody] City city)
    {
        entity.SetCity(city);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPut ("Update/City")]
    public void UpdateCity([FromBody] City city)
    {
        entity.UpdateCity(city);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("GetCities")]
    public List<City> GetCities()
    {
        return entity.GetCities();
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("GetCity/{id}")]
    public City GetCity(int id)
    {
        return entity.GetCity(id);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpDelete("Delete/City/{id}")]
    public void DeleteCity(int id)
    {
        entity.DeleteCity(id);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpDelete("Delete/Cities")]
    public void DeleteCities()
    {
         entity.DeleteCities();
    }
    
    #endregion
    
    #region Management Hotels API 
    
    [Authorize(Roles = "Admin")]
    [HttpPost("Create/Hotel")]
    public void SetHotel([FromBody] Hotel hotel)
    {
        entity.SetHotel(hotel);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPut ("Update/Hotel")]
    public void UpdateHotel([FromBody] Hotel hotel)
    {
        entity.UpdateHotel(hotel);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("GetHotels")]
    public List<Hotel> GetHotel()
    {
        return entity.GetHotels();
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("GetHotel/{id}")]
    public Hotel GetHotel(int id)
    {
        return entity.GetHotel(id);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpDelete("Delete/Hotel/{id}")]
    public void DeleteHotel(int id)
    {
        entity.DeleteHotel(id);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpDelete("Delete/Hotels")]
    public void DeleteHotels()
    {
        entity.DeleteHotels();
    }
    
    #endregion
    
    #region Management Rooms API 
    
    [Authorize(Roles = "Admin")]
    [HttpPost("Create/Room")]
    public void SetRoom([FromBody] Room room)
    {
        entity.SetRoom(room);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPut ("Update/Room")]
    public void UpdateRoom([FromBody] Room room)
    {
        entity.UpdateRoom(room);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("GetRoom")]
    public List<Room> GetRoom()
    {
        return entity.GetRooms();
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("GetRoom/{id}")]
    public Room GetRoom(int id)
    {
        return entity.GetRoom(id);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpDelete("Delete/Room/{id}")]
    public void DeleteRoom(int id)
    {
        entity.DeleteRoom(id);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpDelete("Delete/Rooms")]
    public void DeleteRooms()
    {
        entity.DeleteRooms();
    }
    
    #endregion
    
    #region Management Book Rooms API 
    
    [Authorize(Roles = "Admin")]
    [HttpPost("Create/BookRoom")]
    public void SetBookRoom([FromBody] BookRoom bookroom)
    {
        entity.SetBookRoom(bookroom);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPut ("Update/BookRoom")]
    public void UpdateBookRoom([FromBody] BookRoom bookroom)
    {
        entity.UpdateBookRoom(bookroom);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("GetBookRooms")]
    public List<BookRoom> GetBookRooms()
    {
        return entity.GetBookRooms();
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("GetBookRoom/{id}")]
    public BookRoom GetBokRoom(int id)
    {
        return entity.GetBookRoom(id);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpDelete("Delete/BookRoom/{id}")]
    public void DeleteBookRoom(int id)
    {
        entity.DeleteBookRoom(id);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpDelete("Delete/BookRooms")]
    public void DeleteBookRooms()
    {
        entity.DeleteBookRooms();
    }
    
    #endregion
    
    
    #endregion
    
}
