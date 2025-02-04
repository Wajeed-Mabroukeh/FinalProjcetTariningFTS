using Azure.Core;
using FinalProjectTrainingFTS.Models;
using FinalProjectTrainingFTS.ModelsProject;
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
        return entity.GetFeaturedDealsHotels().Value;
    }
    #endregion
   
    #region User's Recently Visited Hotels
    [Authorize(Roles = "User")]
    [HttpGet("RecentlyVisitedHotels")] 
    public List<Hotel> GetRecentlyVisitedHotels()
    {
        return entity.GetRecentlyVisitedHotels().Value;
    }
    
    [Authorize(Roles = "User")]
    [HttpPut("UpdateVisitedHotelUser")] 
    public Response UpdateVisitedHotel([FromBody] HotelVisitedRequest request)
    {
         return entity.UpdateVisitedHotelUser(request.IdHotelVisited).Value;
    }
    #endregion
    
    #region Trending Destination Highlights
   [Authorize(Roles = "User")]
   [HttpGet("TrendingDestinationHighlights")] 
   public List<City> TrendingDestinationHighlights()
   {
       return entity.GetTrendingDestinationHighlights().Value;
   }
    #endregion
    
    #region Search Results Page
    
    [Authorize(Roles = "User")]
    [HttpGet("SearchResultsHotel")] 
    public List<Hotel> GetSearchHotels([FromBody] SearchRequest request)
    {
        return entity.SearchHotels(request).Value;
    }
    
    [Authorize(Roles = "User")]
    [HttpGet("SearchResultsPriceRange")] 
    public List<Hotel> SearchResultsPriceRange([FromBody] SearchRequest request)
    {
        return entity.GetSearchResultsPriceRange(request).Value;
    }
    
    [Authorize(Roles = "User")]
    [HttpGet("SearchResultsStarAmenities")] 
    public List<Hotel> SearchResultsStarAmenities([FromBody] SearchRequest request)
    {
        return entity.GetSearchResultsStarAmenities(request).Value;
    }
    
    [Authorize(Roles = "User")]
    [HttpGet("SearchResultsStarRating")] 
    public List<Hotel> SearchResultsStarRating([FromBody] SearchRequest request)
    {
        return entity.GetSearchResultsStarRating(request).Value;
    }
    
    #endregion
   
    #region Visual Gallery (Get Image)
    
    [Authorize(Roles = "User")]
    [HttpGet("GetImage/{imageName}")]
    public IActionResult GetImage(string imageName)
    {
        return entity.GetImage(imageName).Value;
    }
    #endregion
   
    #region Room Availability
    
    [Authorize(Roles = "User")]
    [HttpGet("RoomAvailability")]
    public List<Room> GetAvailabileRoom(AvailableRequest request)
    {
        return entity.GetAvailabileRoom(request).Value;
    }
   
    #endregion
    
    #region Get Login User
    [Authorize(Roles = "User")]
    [HttpGet("User/Login")]
    public User GetLoginUser()
    {
        return entity.GetLoginUser().Value;
    }
    #endregion
   
    #region BookRoom Payment
    
    [Authorize(Roles = "User")]
    [HttpPost("BookRoom/Payment")] 
    public async Task<Response> BookRoom_Payment([FromBody] BookRequest request)
    {
        return  entity.BookRoom_Payment(request).Result.Value;
    }
    #endregion
    
    #region CreateCheckoutSession
    [Authorize(Roles = "User")]
    [HttpPost("create-checkout-session")]
    public Dictionary<string,string> CreateCheckoutSession([FromBody]CreateSessionRequest request)
    {
        return entity.CreateCheckoutSession(request).Value;
    }
    #endregion
    
    #region Payment Stripe Webhook
    
    [HttpPost("payment/stripe-webhook")]
    public async Task<Result<string>> Webhook()
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
                var checkpayment = entity.GetCheckPayment(session.Id);
                if (checkpayment != null)
                {
                    var bookroom = new BookRequest()
                    {
                        id_room = checkpayment.RoomId,
                        book_from = checkpayment.BookFrom,
                        book_to = checkpayment.BookTo,
                        user_id = checkpayment.UserId,
                        payment_id = checkpayment.PaymentId
                    };
                    entity.BookRoom_Payment(bookroom);


                    return Result<string>.Success(session.Url);
                       
                }
            }
           
            return Result<string>.Failure("Error" , 1000);
          
        }
        catch (StripeException ex)
        {
            return Result<string>.Failure(ex.StripeError.Error  , 1001);
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
        return  entity.upload_image(imageFile).Result.Value;
    }

    #endregion
    
    #region Location

    [Authorize(Roles = "Admin")]
    [HttpPut("Location")]
    public async Task<Response> SetLocation([FromBody] SetLocation location)
    {
         return entity.SetLocation(location).Result.Value;
    }

    #endregion

    #region Add User
    [Authorize(Roles = "Admin")]
    [HttpPost("Add/User")]
    public Response SetUser([FromBody] User user)
    {
       return entity.SetUser(user).Value;
    }
    #endregion 
    
    #region Add Admin
    [Authorize(Roles = "Admin")]
    [HttpPost("Add/Admin")]
    public Response SetAdmin([FromBody] Admin admin)
    {
        return entity.SetAdmin(admin).Value;
    }
    

    #endregion
    
    #region Management Cities API 
    
    [Authorize(Roles = "Admin")]
    [HttpPost("Create/Cities")]
    public Response SetCities([FromBody] City city)
    {
        return entity.SetCity(city).Value;
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPut ("Update/City")]
    public Response UpdateCity([FromBody] City city)
    {
        return entity.UpdateCity(city).Value;
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("GetCities")]
    public List<City> GetCities()
    {
        return entity.GetCities().Value;
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("GetCity/{id}")]
    public City GetCity(int id)
    {
        return entity.GetCity(id).Value;
    }
    
    [Authorize(Roles = "Admin")]
    [HttpDelete("Delete/City/{id}")]
    public Response DeleteCity(int id)
    {
        return entity.DeleteCity(id).Value;
    }
    
    [Authorize(Roles = "Admin")]
    [HttpDelete("Delete/Cities")]
    public Response DeleteCities()
    {
         return entity.DeleteCities().Value;
    }
    
    #endregion
    
    #region Management Hotels API 
    
    [Authorize(Roles = "Admin")]
    [HttpPost("Create/Hotel")]
    public Response SetHotel([FromBody] Hotel hotel)
    {
        return entity.SetHotel(hotel).Value;
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPut ("Update/Hotel")]
    public Response UpdateHotel([FromBody] Hotel hotel)
    {
        return entity.UpdateHotel(hotel).Value;
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("GetHotels")]
    public List<Hotel> GetHotel()
    {
        return entity.GetHotels().Value;
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("GetHotel/{id}")]
    public Hotel GetHotel(int id)
    {
        return entity.GetHotel(id).Value;
    }
    
    [Authorize(Roles = "Admin")]
    [HttpDelete("Delete/Hotel/{id}")]
    public Response DeleteHotel(int id)
    {
        return entity.DeleteHotel(id).Value;
    }
    
    [Authorize(Roles = "Admin")]
    [HttpDelete("Delete/Hotels")]
    public Response DeleteHotels()
    {
       return entity.DeleteHotels().Value;
    }
    
    #endregion
    
    #region Management Rooms API 
    
    [Authorize(Roles = "Admin")]
    [HttpPost("Create/Room")]
    public Response SetRoom([FromBody] Room room)
    {
        return entity.SetRoom(room).Value;
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPut ("Update/Room")]
    public Response UpdateRoom([FromBody] Room room)
    {
        return entity.UpdateRoom(room).Value;
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("GetRoom")]
    public List<Room> GetRoom()
    {
        return entity.GetRooms().Value;
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("GetRoom/{id}")]
    public Room GetRoom(int id)
    {
        return entity.GetRoom(id).Value;
    }
    
    [Authorize(Roles = "Admin")]
    [HttpDelete("Delete/Room/{id}")]
    public Response DeleteRoom(int id)
    {
       return entity.DeleteRoom(id).Value;
    }
    
    [Authorize(Roles = "Admin")]
    [HttpDelete("Delete/Rooms")]
    public Response DeleteRooms()
    {
       return entity.DeleteRooms().Value;
    }
    
    #endregion
    
    #region Management Book Rooms API 
    
    [Authorize(Roles = "Admin")]
    [HttpPost("Create/BookRoom")]
    public Response SetBookRoom([FromBody] BookRoom bookroom)
    {
       return entity.SetBookRoom(bookroom).Result.Value;
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPut ("Update/BookRoom")]
    public Response UpdateBookRoom([FromBody] BookRoom bookroom)
    {
        return entity.UpdateBookRoom(bookroom).Value;
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("GetBookRooms")]
    public List<BookRoom> GetBookRooms()
    {
        return entity.GetBookRooms().Value;
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("GetBookRoom/{id}")]
    public BookRoom GetBokRoom(int id)
    {
        return entity.GetBookRoom(id).Value;
    }
    
    [Authorize(Roles = "Admin")]
    [HttpDelete("Delete/BookRoom/{id}")]
    public Response DeleteBookRoom(int id)
    {
        return entity.DeleteBookRoom(id).Value;
    }
    
    [Authorize(Roles = "Admin")]
    [HttpDelete("Delete/BookRooms")]
    public Response DeleteBookRooms()
    {
      return entity.DeleteBookRooms().Value;
    }
    
    #endregion
    
    
    #endregion
    
}
