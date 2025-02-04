using System.Data.Entity;
using Azure;
using FinalProjectTrainingFTS.DataBase;
using Microsoft.AspNetCore.Mvc;
using GoogleMapsApi;
using GoogleMapsApi.Entities.Geocoding.Request;
using EFCore.BulkExtensions;
using FinalProjectTrainingFTS.Models;
using FinalProjectTrainingFTS.ModelsProject;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using MailKit.Net.Smtp;
using MimeKit;
using Stripe;
using Stripe.Checkout;
using File = System.IO.File;
using Response = FinalProjectTrainingFTS.Models.Response;


namespace FinalProjectTrainingFTS.CustomerService;

public class CustomerService
{
    private  readonly FinalProjectTrainingFtsContext _context = new FinalProjectTrainingFtsContext();
    private  readonly IConfiguration _configuration;
    private  readonly Random random = new Random();
    private  readonly Response response = new Response();
    
    private readonly string _imageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Images");
    private readonly string _PDFDirectory = Path.Combine(Directory.GetCurrentDirectory(), "PDFReportBooking");
    
    
    public static User user_in = new User()
    {
        UserName = "wajeed",
        Password = "123",
        Id =1,
        VisitedHotels = "20,14"
    };
    
    
    public CustomerService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    
    #region User
    public  Result<Dictionary<string,string>> GetLoginUser(string inputUsername, string inputPassword)
    {
        if (string.IsNullOrEmpty(inputUsername) || string.IsNullOrEmpty(inputPassword))
        {
            return null;
        }

        User? user = _context.Users
            .FirstOrDefault(u => u.UserName == inputUsername && u.Password == inputPassword);
        Dictionary<string, string> result = new();
        if (user != null)
        {
            user_in = user;
            var tokenService = new JwtTokenService(_configuration);
            var token = tokenService.GenerateToken(user.UserName, 1);
            result.Add("access_token",token);
            return Result<Dictionary<string,string>>.Success(result);
        }
        else
        {
            result.Add("access_token","Error");
            return Result<Dictionary<string,string>>.Failure( "error" ,1000);
        }
        
    }
    
    public Result<List<FeaturedDealsResponse>> GetFeaturedDealsHotels()
    {
        var featuredDealsHotels = _context.Hotels
            .Include(h => h.Rooms) // Include related Rooms
            .Where(h => h.Rooms.Any()) // Exclude hotels with no rooms
            .OrderBy(h => h.Rooms.Min(r => r.DiscountedPrice * r.Price / 100.0)) // Sort by the minimum calculated value in Rooms
            .Select(h => new FeaturedDealsResponse
            {
                hotel = h, // Select the entire Hotel object
                originalPrice = h.Rooms.Min(r =>  r.Price),
                discountedPrice = h.Rooms.Min(r => r.DiscountedPrice * r.Price / 100.0) // Add calculated discounted price
            })
            .Take(5) // Take the top 5 hotels
            .ToList();

        
        return Result<List<FeaturedDealsResponse>>.Success(featuredDealsHotels);
    }
    
    public Result<List<Hotel>> GetRecentlyVisitedHotels()
    { 
        string? visited_tag = user_in.VisitedHotels;
        
      List<int> hotel_id = visited_tag
          .Split(",", StringSplitOptions.RemoveEmptyEntries) // Split and remove empty entries
          .Select(tag => int.TryParse(tag, out var id) ? id : throw new FormatException($"Invalid hotel ID: '{tag}'"))
          .ToList();

   
     List<Hotel> recentlyVisitedHotels = new List<Hotel>();
     for (int i = 0; i < hotel_id.Count; i++)
     {
         var hotel = _context.Hotels.Where(h => h.Id == hotel_id[i]).FirstOrDefault();
         if (hotel != null) 
         {
             recentlyVisitedHotels.Add(hotel);
         }
     }
     return Result<List<Hotel>>.Success(recentlyVisitedHotels);
    }

    public Result<Response> UpdateVisitedHotelUser(int id_hotel_visited)
    {
        try
        {
        string? visited_tag = user_in.VisitedHotels;
        if (visited_tag != null)
        {
            List<int> hotel_id = visited_tag
                .Split(",", StringSplitOptions.RemoveEmptyEntries) // Split and remove empty entries
                .Select(tag => int.TryParse(tag, out var id) ? id : throw new FormatException($"Invalid hotel ID: '{tag}'"))
                .ToList();
            Queue<int> visited_hotel = new Queue<int>();
            for (int i = 0; i < hotel_id.Count; i++)
            {
                visited_hotel.Enqueue(hotel_id[i]);
            }

            if (visited_hotel.Count >= 0 && visited_hotel.Count < 5)
            {
                visited_hotel.Enqueue(id_hotel_visited);
                string visited_update = "";
                int count = visited_hotel.Count;
                for (int i = 0; i < count; i++)
                {
                    visited_update += (i == 0 || i == 4) ? visited_hotel.Dequeue() :"," + visited_hotel.Dequeue() ;
                }

                // Retrieve the user you want to update
                var user = _context.Users.FirstOrDefault(u => u.Id == user_in.Id);

                // Update the VisitedHotels property
                user_in.VisitedHotels = visited_update;
                user.VisitedHotels = visited_update;

                // Save changes to the database
                _context.SaveChanges();

                
            }
            else if (visited_hotel.Count == 5)
            {
                visited_hotel.Dequeue();
                visited_hotel.Enqueue(id_hotel_visited);
                string visited_update = "";
                int count = visited_hotel.Count;
                for (int i = 0; i < count; i++)
                {
                    visited_update += ((i == 0 || i == 4) ? visited_hotel.Dequeue() : "," + visited_hotel.Dequeue());
                }

                // Retrieve the user you want to update
                var user = _context.Users.FirstOrDefault(u => u.Id == user_in.Id);

                // Update the VisitedHotels property
                user_in.VisitedHotels = visited_update;
                user.VisitedHotels = visited_update;

                // Save changes to the database
                _context.SaveChanges();
            }

            response.MassegeResulte = "Successful";
            return Result<Response>.Success(response);
        }
        else
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == user_in.Id);

            // Update the VisitedHotels property
            user_in.VisitedHotels = id_hotel_visited + "";
            user.VisitedHotels = id_hotel_visited + "";

            // Save changes to the database
            _context.SaveChanges(); 
            response.MassegeResulte = "Successful";
            return Result<Response>.Success(response);
        }

        }
        catch (Exception e)
        {
            return Result<Response>.Failure(e.InnerException.Message , 10001);
        }
    }
    
    public Result<List<City>> GetTrendingDestinationHighlights()
    {
        var trending_city = _context.Cities.OrderByDescending(c => c.VisitCount).Take(5).ToList();
        
        return Result<List<City>>.Success(trending_city);
    }
    
    public Result<List<Hotel>> SearchHotels(SearchRequest request)
    {
        // Validate dates
        if (request.CheckOutDate <= request.CheckInDate)
        {
            return Result<List<Hotel>>.Failure("Error, DateFrom is above DateTo ", 1000);
        }

        // Filter hotels based on the query
        var filteredHotels = _context.Hotels.Where(h =>
                h.Name.Contains(request.Query) || h.City.Name.Contains(request.Query))
            .ToList();

        return  Result<List<Hotel>>.Success(filteredHotels);
    }
    
    public Result<List<Hotel>> GetSearchResultsPriceRange(SearchRequest request)
    {
        // Validate dates
        if (request.CheckOutDate <= request.CheckInDate)
        {
            return Result<List<Hotel>>.Failure("Error, DateFrom is above DateTo ", 1000);
        }

        // Filter hotels based on the query
        var filteredHotels = _context.Hotels
            .Include(h => h.Rooms)
            .Where(h =>
                h.Rooms.Any(r => r.Price >= request.price_min &&  r.Price <= request.price_max ))
            .ToList();

        return Result<List<Hotel>>.Success(filteredHotels);
    }
    
    public Result<List<Hotel>> GetSearchResultsStarRating(SearchRequest request)
    {
        // Validate dates
        if (request.CheckOutDate <= request.CheckInDate)
        {
            return Result<List<Hotel>>.Failure("Error, DateFrom is above DateTo ", 1000);
        }

        // Filter hotels based on the query
        var filteredHotels = _context.Hotels
            .Where(h =>
                h.StarRate == request.star_rate)
            .ToList();

        return Result<List<Hotel>>.Success(filteredHotels);
    }
    
    public Result<List<Hotel>> GetSearchResultsStarAmenities(SearchRequest request)
    {
        // Validate dates
        if (request.CheckOutDate <= request.CheckInDate)
        {
            return Result<List<Hotel>>.Failure("Error, DateFrom is above DateTo ", 1000);
        }

        // Filter hotels based on the query
        var filteredHotels = _context.Hotels
            .Where(h =>
                h.Amenities == request.amenities)
            .ToList();

        return Result<List<Hotel>>.Success(filteredHotels);
    }
    
    public Result<IActionResult>  GetImage(string image)
    {
        var filePath = Path.Combine(_imageDirectory, image);
        if (!System.IO.File.Exists(filePath))
        {
            throw new Exception("Image not Found");
        }
        
        // Get the file's content type
        var contentType = GetContentType(filePath);
        var fileBytes = File.ReadAllBytes(filePath);
        
        // Return the file
        return Result<IActionResult>.Success(new FileContentResult(fileBytes, contentType));
       
    }

    public Result<List<Room>> GetAvailabileRoom(AvailableRequest request)
    {
        if (request.bookfrom >= request.bookto)
        {
            return null;
        }
        var  rooms_available = _context.Rooms.Include(r=>r.BookRooms)
            .Where(r=>r.HotelId == request.id_hotel)
            .Where(r=>r.BookRooms.All(b => 
                                           b.BookTo < request.bookfrom ||
                                           b.BookFrom > request.bookto 
                                           )).ToList();
         return Result<List<Room>>.Success(rooms_available);
    }

    public Result<User> GetLoginUser()
    {
        return Result<User>.Success(user_in);
    }
    
    public async Task<Result<Response>> BookRoom_Payment(BookRequest request)
    {
        
        if (request.book_from <= request.book_to && request.book_from >= DateTime.Today)
        {
            var room = GetRoom(request.id_room).Value;
            int random_id = random.Next();
           
            if (GetBookRoom(random_id) == null)
            {

                var book_room = new BookRoom();
                book_room.Id = random_id;
                book_room.RoomId = request.id_room;
                book_room.UserId = user_in.Id;
                book_room.BookFrom = request.book_from;
                book_room.BookTo = request.book_to;
                await SetBookRoom(book_room);
                var checkpayment = GetCheckPayment(request.payment_id);
                await DeleteCheckPayment(checkpayment);

                try
                {
                    var hotel = _context.Hotels
                        .Where(h => h.Rooms.Any(r => r.RoomId == request.id_room)).FirstOrDefault();
                    var city = GetCity(hotel.CityId).Value;
                    city.VisitCount++;
                    _context.SaveChanges();


                    var confirmation = new Confirmation
                    {
                        BookRoom = book_room,
                        Room = room,
                        HotelAddress = $"Country :{city.Country} , PostOffice :{city.PostOffice}",
                        TotalAmount = (room.Price * room.DiscountedPrice) / 100.0
                    };

                    string uniqueFileName = $"{random_id}.pdf";
                    var filePath = Path.Combine(_PDFDirectory, uniqueFileName);
                    SaveConfirmationAsPdf(confirmation, filePath);

                    string paymentStatus = "Paid";
                    string recipientEmail = "wajeed.mabroukeh@gmail.com";

                    SendEmailWithInvoice(recipientEmail, paymentStatus, filePath);

                    response.MassegeResulte = $"Successfully Book Room that ID :{random_id}";
                    return Result<Response>.Success(response);
                }
                catch (Exception e)
                {
                    return Result<Response>.Failure(e.Message, 1001);
                }
            }
            else
            {
                response.MassegeResulte = "You Already Book This Room!";
                return Result<Response>.Failure(response.MassegeResulte, 1000);
            }
        } 
        else
        {
            response.MassegeResulte =
                "The Book From must be the next day to Book To after the initial booking appointment.!";
            return Result<Response>.Failure(response.MassegeResulte , 1000);
        }

    }

    public Result<Dictionary<string, string>>CreateCheckoutSession(CreateSessionRequest request)
    { 
        StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];


        var options = new SessionCreateOptions

        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = _context.Hotels
                                .Include(h => h.Rooms).FirstOrDefault(h => h.Rooms.Any(r => r.RoomId == request.RoomlId))
                                ?.Name,
                            
                        },
                        UnitAmount =
                            (long)((_context.Rooms.Where(r => r.RoomId == request.RoomlId).FirstOrDefault().Price *
                                    _context.Rooms.Where(r => r.RoomId == request.RoomlId).FirstOrDefault()
                                       .DiscountedPrice)), // Convert to cents
                    },
                    Quantity = 1,
                },
            },
            Mode = "payment",
            SuccessUrl = "http://localhost:5000/api/BookRoom/Payment1111",
            CancelUrl = "http://localhost:5000/api/BookRoom/Payment11",
        };
        
        var service = new SessionService();
        Session session = service.Create(options);
     
        int random_id = random.Next();
        var check_exist = _context.CheckPayments.Where(c => c.Id == random_id).FirstOrDefault();
        if (check_exist == null)
        {
            CheckPayment checkPayment = new CheckPayment()
            {
                RoomId = request.RoomlId,
                PaymentId = session.Id,
                BookFrom = request.Book_From,
                BookTo = request.Book_To,
                UserId = user_in.Id,
                Id = random_id

            };
            _context.CheckPayments.Add(checkPayment);
            _context.SaveChanges();
            var result = new Dictionary<string, string>
            {
                { "Result", "Successfully" },
                { "ID", session.Id },
                { "Url", session.Url }
            };
            return Result<Dictionary<string, string>>.Success(result);
        }
        return Result<Dictionary<string, string>>.Failure("Error,may be the id exist ,try again",1000);
    }
    
    #endregion
    
    #region Admin
    
    public Result<Dictionary<string,string>> GetLoginAdmin(string inputUsername, string inputPassword)
    {
        if (string.IsNullOrEmpty(inputUsername) || string.IsNullOrEmpty(inputPassword))
        {
            return null;
        }

        var admin = _context.Admins
            .FirstOrDefault(u => u.UserName == inputUsername && u.Password == inputPassword);

        Dictionary<string, string> result = new();
        if (admin != null)
        {
            var tokenService = new JwtTokenService(_configuration);
            var token = tokenService.GenerateToken(admin.UserName, 0);
            result.Add("access_token",token);
            return Result<Dictionary<string,string>>.Success(result);
        }
        else
        {
            result.Add("access_token","Error");
            return Result<Dictionary<string,string>>.Failure( "error" ,1000);
        }
        
    }

    public Result<Response> SetUser(User user)
    {
        try
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            response.MassegeResulte = "Successful";
            return Result<Response>.Success(response);
        }
        catch (Exception e)
        {
            return Result<Response>.Failure(e.Message , 1001);
        }
        
    }
    
    public Result<Response> SetAdmin(Admin admin)
    {
        try
        {
            _context.Admins.Add(admin);
            _context.SaveChanges();
            response.MassegeResulte = "Successful";
            return Result<Response>.Success(response);
        }
        catch (Exception e)
        {
            return Result<Response>.Failure(e.Message , 1001);
        }
    }
    
    public async Task<Result<Response>> upload_image (IFormFile imageFile){

        try
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                response.MassegeResulte = "No file uploaded or file is empty.";
                return Result<Response>.Failure(response.MassegeResulte, 1000);
            }

            // Validate the file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
            {
                response.MassegeResulte = "Invalid file type. Only JPG, JPEG, PNG, and GIF are allowed.";
                return Result<Response>.Failure(response.MassegeResulte, 1000);

            }

            // Generate a unique file name to avoid overwriting existing files
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";

            // Build the full file path
            var filePath = Path.Combine(_imageDirectory, uniqueFileName);
           
            // Save the file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }
            
        }
        catch (Exception e)
        {
            return Result<Response>.Failure(e.Message, 1000);
        }
        response.MassegeResulte = "File uploaded successfully";
        return Result<Response>.Success(response);
    }
    
    public async Task<Result<Response>> SetLocation( SetLocation location)
    {
        var request = new GeocodingRequest
        {
            Address = location.Address,
            ApiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiaWJyYWhpbSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwiZXhwIjoxNzM2MzYwMzIwLCJpc3MiOiJmaW5hbHByb2plY3R0cmFpbmluZ2Z0cyIsImF1ZCI6IldhamVlZCJ9.Khq4bNvmYeQxnKFEKBOEB6ioea0nnF9Ti4gxWYMvWsw"
        };
        var response = await GoogleMaps.Geocode.QueryAsync(request);
        if (response.Status == GoogleMapsApi.Entities.Geocoding.Response.Status.OK)
        {
            var location_respinse = response.Results.First().Geometry.Location;
            var hotel = _context.Hotels.Where(h => h.Id == location.id_hotel).FirstOrDefault();
            hotel.Latitude = location_respinse.Latitude.ToString();
            hotel.Longitude = location_respinse.Longitude.ToString();
            _context.SaveChanges();
            this.response.MassegeResulte =
                $"Latitude: {location_respinse.Latitude}, Longitude: {location_respinse.Longitude}";
            return Result<Response>.Success(this.response);
            
        }
        else
        {
            return Result<Response>.Failure($"Error: {response.Status}" , 1000); 
        }
    }
    
    
    #region Management Cities
    public Result<List<City>> GetCities()
    {
        if (_context.Cities.ToList() == null)
        {
            return Result<List<City>>.Success(null);
        }
        return  Result<List<City>>.Success(_context.Cities.ToList());
    }
    
    public Result<City> GetCity(int? id)
    {  
        var city = _context.Cities.Where(c=>c.Id == id).FirstOrDefault();
        return Result<City>.Success(city);
    }
    
    public Result<Response> SetCity(City city)
    {
        
        try
        {
            _context.Cities.Add(city);
            _context.SaveChanges();
            response.MassegeResulte = "Successful";
            return Result<Response>.Success(response);
        }
        catch (Exception e)
        {
            return Result<Response>.Failure("Check may be the same name is exit in Database" , 1001);
        }
    }
    
    public Result<Response> UpdateCity(City city)
    {
        try
        {
            var city_update = _context.Cities.Where(c=>c.Id == city.Id).FirstOrDefault();
            if (city_update != null)
            {
                city_update.Name = city.Name != "" ? city.Name : city_update.Name;
                city_update.Country = city.Country != "" ? city.Country : city_update.Country;
                city_update.Image = city.Image != "" ? city.Image : city_update.Image;
                city_update.PostOffice = city.PostOffice != -1 ? city.PostOffice : city_update.PostOffice;
                _context.SaveChanges();
                response.MassegeResulte = "Successful";
                return Result<Response>.Success(response);
            }
            return Result<Response>.Failure($"Not Found Bood Room for Id :{city_update.Id}",1000);
        }
        catch (Exception e)
        {
            return Result<Response>.Failure(e.Message,1001);
        }
        
        
       
    }
    
    public Result<Response> DeleteCity(int id)
    {
        try
        {
            var delete_city = _context.Cities.Where(c=>c.Id == id).FirstOrDefault();
            _context.Cities.Remove(delete_city);
            _context.SaveChanges();
            response.MassegeResulte = "Successful";
            return Result<Response>.Success(response);
        }
        catch (Exception e)
        {
            return Result<Response>.Failure(e.Message, 1001);
        }
       
    }
    
    public Result<Response> DeleteCities()
    {
        try
        {
            var delete_all_city = _context.Cities.ToList();
            _context.BulkDelete(delete_all_city); 
            response.MassegeResulte = "Successful";
            return Result<Response>.Success(response);
        }
        catch (Exception e)
        {
            return Result<Response>.Failure(e.Message, 1001);
        }
        
    }
    
    #endregion
    
    #region Management Hotels
    public Result<List<Hotel>> GetHotels()
    {
        return Result<List<Hotel>>.Success(_context.Hotels.ToList());
    }
    
    public Result<Hotel> GetHotel(int id)
    {  
        var hotel = _context.Hotels.Where(c=>c.Id == id).FirstOrDefault();
        return  Result<Hotel>.Success(hotel);
    }
    
    public Result<Response> SetHotel(Hotel hotel)
    {
        try
        {
            _context.Hotels.Add(hotel);
            _context.SaveChanges();
            response.MassegeResulte = "Successful";
            return Result<Response>.Success(response);
        }
        catch (Exception e)
        {
            return Result<Response>.Failure(e.Message , 1001);
        }
       
    }
    
    public Result<Response> UpdateHotel(Hotel hotel)
    {
        try
        {
            var hotel_update = _context.Hotels.Where(h=>h.Id == hotel.Id).FirstOrDefault();
            if (hotel_update != null)
            {
                hotel_update.Name = hotel.Name != "" ? hotel.Name : hotel_update.Name;
                hotel_update.Amenities = hotel.Amenities != "" ? hotel.Amenities : hotel_update.Amenities;
                hotel_update.Descriptions = hotel.Descriptions != "" ? hotel.Descriptions : hotel_update.Descriptions;
                hotel_update.Image = hotel.Image != "" ? hotel.Image : hotel_update.Image;
                hotel_update.Owner = hotel.Owner != "" ? hotel.Owner : hotel_update.Owner;
                hotel_update.Longitude = hotel.Longitude != "" ? hotel.Longitude : hotel_update.Longitude;
                hotel_update.Latitude = hotel.Latitude != "" ? hotel.Latitude : hotel_update.Latitude;
                hotel_update.StarRate = hotel.StarRate != -1 ? hotel.StarRate : hotel_update.StarRate;
                hotel_update.CityId = hotel.CityId != -1 ? hotel.CityId : hotel_update.CityId;

                _context.SaveChanges();
                response.MassegeResulte = "Successful";
                return Result<Response>.Success(response);
            }
            return Result<Response>.Failure($"Not Found Bood Room for Id :{hotel_update.Id}",1000);
        }
        catch (Exception e)
        {
            return Result<Response>.Failure(e.Message,1001);
        }
       
        
      
    }
    
    public Result<Response> DeleteHotel(int id)
    {
        try
        {
            var delete_hotel = _context.Hotels.Where(h=>h.Id == id).FirstOrDefault();
            _context.Hotels.Remove(delete_hotel);
            _context.SaveChanges();
            response.MassegeResulte = "Successful";
            return Result<Response>.Success(response);
        }
        catch (Exception e)
        {
            return Result<Response>.Failure(e.Message , 1001);
        }
        
    }
    
    public Result<Response> DeleteHotels()
    {
        try
        {
            var delete_all_hotel = _context.Hotels.ToList();
            _context.BulkDelete(delete_all_hotel); 
            response.MassegeResulte = "Successful";
            return Result<Response>.Success(response);
        }
        catch (Exception e)
        {
            return Result<Response>.Failure(e.Message , 1001);
        }
        
    }
    
    #endregion
    
    #region Management Rooms
    
    public Result<List<Room>> GetRooms()
    {
        return Result<List<Room>>.Success(_context.Rooms.ToList());
    }

    public Result<Room> GetRoom(int id)
    {  
        var room = _context.Rooms.Where(r=>r.RoomId == id).FirstOrDefault();
        return Result<Room>.Success(room);
    }
    
    public Result<Response> SetRoom(Room room)
    {
        try
        {
            _context.Rooms.Add(room);
            _context.SaveChanges();
            response.MassegeResulte = "Successful";
            return Result<Response>.Success(response);
        }
        catch (Exception e)
        {
            return Result<Response>.Failure(e.Message , 1001);
        }
        
    }

    public Result<Response> UpdateRoom(Room room)
    {
        try
        {
            var room_update = _context.Rooms.Where(r=>r.RoomId == room.RoomId).FirstOrDefault();
            if (room_update != null)
            {
                room_update.DiscountedPrice =
                    room.DiscountedPrice != -1 ? room.DiscountedPrice : room_update.DiscountedPrice;
                room_update.Price = room.Price != -1 ? room.Price : room_update.Price;
                room_update.HotelId = room.HotelId != -1 ? room.HotelId : room_update.HotelId;
                room_update.Adult = room.Adult != -1 ? room.Adult : room_update.Adult;
                room_update.Children = room.Children != -1 ? room.Children : room_update.Children;
                room_update.Descriptions = room.Descriptions != "" ? room.Descriptions : room_update.Descriptions;
                room_update.Image = room.Image != "" ? room.Image : room_update.Image;
                _context.SaveChanges();
                response.MassegeResulte = "Successful";
                return Result<Response>.Success(response);
            }
            return Result<Response>.Failure($"Not Found Bood Room for Id :{room.RoomId}",1000);
        }
        catch (Exception e)
        {
            return Result<Response>.Failure(e.Message,1001);
        }
        
        
       
    }
   
    public Result<Response> DeleteRoom(int id)
    {
        try
        {
            var delete_room = _context.Rooms.Where(r=>r.RoomId == id).FirstOrDefault();
            _context.Rooms.Remove(delete_room);
            _context.SaveChanges();
            response.MassegeResulte = "Successful";
            return Result<Response>.Success(response);
        }
        catch (Exception e)
        {
            return Result<Response>.Failure(e.Message , 1001);
        }
        
    }
    
    public Result<Response> DeleteRooms()
    {
        try
        {
            var delete_all_room = _context.Rooms.ToList();
            _context.BulkDelete(delete_all_room); 
            response.MassegeResulte = "Successful";
            return Result<Response>.Success(response);
        }
        catch (Exception e)
        {
            return Result<Response>.Failure(e.Message , 1001);
        }
        
    }
    
    #endregion
    
    #region Management BookRoom
    
    public Result<List<BookRoom>> GetBookRooms()
    {
        return Result<List<BookRoom>>.Success(_context.BookRooms.ToList());
    }

    public Result<BookRoom> GetBookRoom(int id)
    {  
        var bookroom = _context.BookRooms.Where(r=>r.Id == id).FirstOrDefault();
        return Result<BookRoom>.Success(bookroom);
    }
    
    public async Task<Result<Response>> SetBookRoom(BookRoom bookRoom)
    {
        try
        {
            _context.BookRooms.Add(bookRoom);
            _context.SaveChanges();
            response.MassegeResulte = "Successful";
            return Result<Response>.Success(response);
        }
        catch (Exception e)
        {
            return Result<Response>.Failure(e.Message , 1001);
        }
       
    }

    public Result<Response> UpdateBookRoom(BookRoom bookRoom)
    {
        try
        {
            var bookRoom_update = _context.BookRooms.Where(b=>b.Id == bookRoom.Id).FirstOrDefault();
            if (bookRoom_update != null)
            {
                bookRoom_update.RoomId = bookRoom.RoomId != -1 ? bookRoom.RoomId : bookRoom_update.RoomId;
                bookRoom_update.BookFrom = bookRoom.BookFrom != null ? bookRoom.BookFrom : bookRoom_update.BookFrom;
                bookRoom_update.BookTo = bookRoom.BookTo != null ? bookRoom.BookTo : bookRoom_update.BookTo;
                bookRoom_update.UserId = bookRoom.UserId != -1 ? bookRoom.UserId : bookRoom_update.UserId;
                _context.SaveChanges();
                response.MassegeResulte = "Successful";
                return Result<Response>.Success(response);
            }
            return Result<Response>.Failure($"Not Found Bood Room for Id :{bookRoom.Id}",1000);
        }
        catch (Exception e)
        {
            return Result<Response>.Failure(e.Message,1001);
        }
       
        
    }
   
    public Result<Response> DeleteBookRoom(int id)
    {
       
        try
        {
            var delete_bookroom = _context.BookRooms.Where(b=>b.Id ==  id).FirstOrDefault();
            _context.BookRooms.Remove(delete_bookroom);
            _context.SaveChanges();
            response.MassegeResulte = "Successful";
            return Result<Response>.Success(response);
        }
        catch (Exception e)
        {
            return Result<Response>.Failure(e.Message , 1001);
        }
    }
    
    public Result<Response> DeleteBookRooms()
    {
       
        try
        {
            var delete_all_bookroom = _context.BookRooms.ToList();
            _context.BulkDelete(delete_all_bookroom); 
            response.MassegeResulte = "Successful";
            return Result<Response>.Success(response);
        }
        catch (Exception e)
        {
            return Result<Response>.Failure(e.Message , 1001);
        }
    }
    
    #endregion
    
    #endregion

    #region Helper method 
    private string GetContentType(string path)
    {
        var extension = Path.GetExtension(path).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            _ => "application/octet-stream",
        };
    }

    public CheckPayment GetCheckPayment(string paymentid)
    {
        CheckPayment checkPayment = _context.CheckPayments.Where(c => c.PaymentId == paymentid).FirstOrDefault();
        return checkPayment;
    }

    public async Task DeleteCheckPayment(CheckPayment checkPayment)
    {
        _context.Remove(checkPayment);
        _context.SaveChanges();
    }
    public static void SaveConfirmationAsPdf(Confirmation confirmation, string filePath)
    {
        // Create a PDF writer instance
        using (PdfWriter writer = new PdfWriter(filePath))
        {
            // Create a PDF document
            using (PdfDocument pdf = new PdfDocument(writer))
            {
                Document document = new Document(pdf);

                // Add content to the PDF
                document.Add(new Paragraph("Confirmation Details"));
                document.Add(new Paragraph($"ID Book Room: {confirmation.BookRoom.Id}"));
                document.Add(new Paragraph($"Descriptions Room: {confirmation.Room.Descriptions}"));
                document.Add(new Paragraph($"Adult: {confirmation.Room.Adult}"));
                document.Add(new Paragraph($"Children: {confirmation.Room.Children}"));
                document.Add(new Paragraph($"HotelAddress => {confirmation.HotelAddress}"));
                document.Add(new Paragraph($"Reservation Date: {confirmation.BookRoom.BookFrom:yyyy-MM-dd HH:mm:ss}"));
                document.Add(new Paragraph($"Reservation Date: {confirmation.BookRoom.BookTo:yyyy-MM-dd HH:mm:ss}"));
                document.Add(new Paragraph($"Total Amount: ${confirmation.TotalAmount}"));

                // Close the document
                document.Close();
            }
        }
    }
    
    public static void SendEmailWithInvoice(string recipientEmail, string paymentStatus, string invoiceFilePath)
    {
        // Create a new email message
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Your Company", "projectfinalfts@gmail.com"));
        message.To.Add(new MailboxAddress("User", recipientEmail));
        message.Subject = "Payment Confirmation and Invoice";

        // Create the email body
        var bodyBuilder = new BodyBuilder
        {
            TextBody = $"Dear Customer,\n\nThank you for your payment. Your payment status is '{paymentStatus}'. Please find your invoice attached.\n\nBest regards,\nYour Company"
        };

        // Attach the PDF invoice
        bodyBuilder.Attachments.Add(invoiceFilePath);

        // Set the body
        message.Body = bodyBuilder.ToMessageBody();

        // Send the email
        using (var client = new SmtpClient())
        {
            try
            {
                // Connect to the SMTP server
                client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);

                // Authenticate (use your credentials here)
                client.Authenticate("projectfinalfts@gmail.com", "12365478900w#.");

                // Send the message
                client.Send(message);

                // Disconnect
                client.Disconnect(true);
                
                Console.WriteLine("succss");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }
    }
    
   
     
    #endregion 
   

    
}

 
 
 