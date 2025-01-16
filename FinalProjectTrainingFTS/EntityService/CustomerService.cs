using System.Data.Entity;
using FinalProjectTrainingFTS.DataBase;
using Microsoft.AspNetCore.Mvc;
using GoogleMapsApi;
using GoogleMapsApi.Entities.Geocoding.Request;
using EFCore.BulkExtensions;
using FinalProjectTrainingFTS.Models;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using MailKit.Net.Smtp;
using MimeKit;




namespace FinalProjectTrainingFTS.CustomerService;

public class CustomerService
{
    private  readonly FinalProjectTrainingFtsContext _context = new FinalProjectTrainingFtsContext();
    private  readonly IConfiguration _configuration;
    public static User user_in = new User()
    {
        UserName = "wajeed",
        Password = "123",
        Id =1,
        VisitedHotels = "20,14"
    };
    
    
    

    private readonly string _imageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Images");
    private readonly string _PDFDirectory = Path.Combine(Directory.GetCurrentDirectory(), "PDFReportBooking");

    public CustomerService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    #region User
    public  Dictionary<string, string> GetLoginUser(string inputUsername, string inputPassword)
    {
        if (string.IsNullOrEmpty(inputUsername) || string.IsNullOrEmpty(inputPassword))
        {
            return null;
        }

        var user = _context.Users
            .FirstOrDefault(u => u.UserName == inputUsername && u.Password == inputPassword);
        Dictionary<string, string> result = new();
        if (!user.Equals(null))
        {
            user_in = user;
            var tokenService = new JwtTokenService(_configuration);
            var token = tokenService.GenerateToken(user.UserName, 1);
            result.Add("access_token",token);
        }
        else
        {
            result.Add("access_token","Error");
        }


        return result;
    }
    
    public List<FeaturedDealsResponse> GetFeaturedDealsHotels()
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

        
        return featuredDealsHotels;
    }
    
    public List<Hotel> GetRecentlyVisitedHotels()
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
     return recentlyVisitedHotels;
    }

    public void UpdateVisitedHotelUser(int id_hotel_visited)
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
        }
        else
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == user_in.Id);

            // Update the VisitedHotels property
            user_in.VisitedHotels = id_hotel_visited + "";
            user.VisitedHotels = id_hotel_visited + "";

            // Save changes to the database
            _context.SaveChanges(); 
        }

    }
    
    public List<City> GetTrendingDestinationHighlights()
    {
        var trending_city = _context.Cities.OrderByDescending(c => c.VisitCount).Take(5).ToList();
        
        return trending_city;
    }
    
    public List<Hotel> SearchHotels(SearchRequest request)
    {
        // Validate dates
        if (request.CheckOutDate <= request.CheckInDate)
        {
            return null;
        }

        // Filter hotels based on the query
        var filteredHotels = _context.Hotels.Where(h =>
                h.Name.Contains(request.Query) || h.City.Name.Contains(request.Query))
            .ToList();

        return filteredHotels;
    }
    
    public List<Hotel> GetSearchResultsPriceRange(SearchRequest request)
    {
        // Validate dates
        if (request.CheckOutDate <= request.CheckInDate)
        {
            return null;
        }

        // Filter hotels based on the query
        var filteredHotels = _context.Hotels
            .Include(h => h.Rooms)
            .Where(h =>
                h.Rooms.Any(r => r.Price >= request.price_min &&  r.Price <= request.price_max ))
            .ToList();

        return filteredHotels;
    }
    
    public List<Hotel> GetSearchResultsStarRating(SearchRequest request)
    {
        // Validate dates
        if (request.CheckOutDate <= request.CheckInDate)
        {
            return null;
        }

        // Filter hotels based on the query
        var filteredHotels = _context.Hotels
            .Where(h =>
                h.StarRate == request.star_rate)
            .ToList();

        return filteredHotels;
    }
    
    public List<Hotel> GetSearchResultsStarAmenities(SearchRequest request)
    {
        // Validate dates
        if (request.CheckOutDate <= request.CheckInDate)
        {
            return null;
        }

        // Filter hotels based on the query
        var filteredHotels = _context.Hotels
            .Where(h =>
                h.Amenities == request.amenities)
            .ToList();

        return filteredHotels;
    }
    
    public IActionResult  GetImage(string image)
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
        return new FileContentResult(fileBytes, contentType);
       
    }

    public List<Room> GetAvailabileRoom(AvailableRequest request)
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
         return rooms_available;
    }

    public User GetLoginUser()
    {
        return user_in;
    }
    
    public async Task<Response> BookRoom_Payment(BookRequest request)
    {
        var room = GetRoom(request.id_room);
        //payment 
        bool Payment_made = true;
        if (Payment_made)
        {
            Random random = new Random();
            int random_id = random.Next();
            if (GetBookRoom(random_id) == null )
            {
                if (request.book_from <= request.book_to)
                {
                    var book_room = new BookRoom();
                    book_room.Id = random_id;
                    book_room.RoomId = request.id_room;
                    book_room.UserId = user_in.Id;
                    book_room.BookFrom = request.book_from;
                    book_room.BookTo = request.book_to;
                    await SetBookRoom(book_room);
                    
                    var hotel = _context.Hotels
                        .Where(h => h.Rooms.Any(r => r.RoomId == request.id_room)).FirstOrDefault();
                    var city = GetCity(hotel.CityId);
                   city.VisitCount++;
                    _context.SaveChanges();
                    
                    var confirmation = new Confirmation
                    {
                        BookRoom = book_room,
                        Room = room,
                        HotelAddress =$"Country :{city.Country} , PostOffice :{city.PostOffice}",
                        TotalAmount = (room.Price * room.DiscountedPrice)/100.0
                    };
                    string uniqueFileName = $"{random_id}.pdf";
                    var filePath = Path.Combine(_PDFDirectory, uniqueFileName);
                    SaveConfirmationAsPdf(confirmation, filePath);
                    
                    string paymentStatus = "Paid";
                    string recipientEmail = "wajeed.mabroukeh@gmail.com";
                    
                    SendEmailWithInvoice(recipientEmail, paymentStatus, filePath);
                    
                    return new Response()
                    {
                        MassegeResulte = $"Successfully Book Room that ID :{random_id}"
                    };
                }
                return new Response()
                {
                    MassegeResulte = "The Book From must be the next day to Book To after the initial booking appointment.!"
                };
            }
            else
            {
                return new Response()
                {
                    MassegeResulte = "You Already Book This Room!"
                }; 
            }
         
           
        }
        return new Response()
        {
            MassegeResulte = "Payment problem, Try Again."
        }; 
        
    }
    
    
    
    #endregion
    
    #region Admin
    
    public Dictionary<string, string> GetLoginAdmin(string inputUsername, string inputPassword)
    {
        if (string.IsNullOrEmpty(inputUsername) || string.IsNullOrEmpty(inputPassword))
        {
            return null;
        }

        var admin = _context.Admins
            .FirstOrDefault(u => u.UserName == inputUsername && u.Password == inputPassword);

        Dictionary<string, string> result = new();
        if (!admin.Equals(null))
        {
            var tokenService = new JwtTokenService(_configuration);
            var token = tokenService.GenerateToken(admin.UserName, 0);
            result.Add("access_token",token);
        }
        else
        {
            result.Add("access_token","Error");
        }


        return result;
    }

    public void SetUser(User user)
    {
        try
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }
    
    public void SetAdmin(Admin admin)
    {
        _context.Admins.Add(admin);
        _context.SaveChanges();
    }
    
    public async Task<Response> upload_image (IFormFile imageFile){

        try
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return new Response()
                {
                    MassegeResulte = "No file uploaded or file is empty."
                };
            }

            // Validate the file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
            {

                return new Response()
                {
                    MassegeResulte = "Invalid file type. Only JPG, JPEG, PNG, and GIF are allowed."
                };

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
            return new Response()
            {
                MassegeResulte = "Error"
            };
        }

        return new Response()
       {
           MassegeResulte = "File uploaded successfully"
       };
    }
    
    public   async Task SetLocation( SetLocation location)
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
            Console.WriteLine($"Latitude: {location_respinse.Latitude}, Longitude: {location_respinse.Longitude}");
        }
        else
        {
            Console.WriteLine($"Error: {response.Status}");
        }
    }
    
    
    #region Management Cities
    public List<City> GetCities()
    {
        if (_context.Cities.ToList() == null)
        {
            return null;
        }
        return _context.Cities.ToList();
    }
    
    public City GetCity(int? id)
    {  
        var city = _context.Cities.Where(c=>c.Id == id).FirstOrDefault();
        if (city == null)
        {
            return null;
        }
        return city;
    }
    
    public void SetCity(City city)
    {
        _context.Cities.Add(city);
        try
        {
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            Console.WriteLine("Check may be the same name is exit in Database");
        }
    }
    
    public void UpdateCity(City city)
    {
        var city_update = _context.Cities.Where(c=>c.Id == city.Id).FirstOrDefault();
        city_update.Name = city.Name != "" ?  city.Name : city_update.Name;
        city_update.Country = city.Country != "" ?  city.Country : city_update.Country;
        city_update.Image = city.Image != "" ?  city.Image : city_update.Image; 
        city_update.PostOffice = city.PostOffice != -1 ?  city.PostOffice : city_update.PostOffice;
        _context.SaveChanges();
    }
    
    public void DeleteCity(int id)
    {
        var delete_city = _context.Cities.Where(c=>c.Id == id).FirstOrDefault();
        _context.Cities.Remove(delete_city);
        _context.SaveChanges();
    }
    
    public void DeleteCities()
    {
        var delete_all_city = _context.Cities.ToList();
        _context.BulkDelete(delete_all_city); 
    }
    
    #endregion
    
    #region Management Hotels
    public List<Hotel> GetHotels()
    {
        return _context.Hotels.ToList();
    }
    
    public Hotel GetHotel(int id)
    {  
        var hotel = _context.Hotels.Where(c=>c.Id == id).FirstOrDefault();
        if (hotel == null)
        {
            return null;
        }
        return hotel;
    }
    
    public void SetHotel(Hotel hotel)
    {
        _context.Hotels.Add(hotel);
        _context.SaveChanges();
    }
    
    public void UpdateHotel(Hotel hotel)
    {
        var hotel_update = _context.Hotels.Where(h=>h.Id == hotel.Id).FirstOrDefault();
        hotel_update.Name = hotel.Name != ""  ?  hotel.Name : hotel_update.Name;
        hotel_update.Amenities = hotel.Amenities != ""  ?  hotel.Amenities : hotel_update.Amenities;
        hotel_update.Descriptions = hotel.Descriptions != "" ?  hotel.Descriptions : hotel_update.Descriptions;
        hotel_update.Image = hotel.Image != "" ?  hotel.Image : hotel_update.Image;
        hotel_update.Owner = hotel.Owner != "" ?  hotel.Owner : hotel_update.Owner;
        hotel_update.Longitude = hotel.Longitude != "" ?  hotel.Longitude : hotel_update.Longitude;
        hotel_update.Latitude = hotel.Latitude != "" ?  hotel.Latitude : hotel_update.Latitude;
        hotel_update.StarRate = hotel.StarRate != -1 ?  hotel.StarRate : hotel_update.StarRate;
        hotel_update.CityId = hotel.CityId != -1 ?  hotel.CityId : hotel_update.CityId;
        
        _context.SaveChanges();
    }
    
    public void DeleteHotel(int id)
    {
        var delete_hotel = _context.Hotels.Where(h=>h.Id == id).FirstOrDefault();
        _context.Hotels.Remove(delete_hotel);
        _context.SaveChanges();
    }
    
    public void DeleteHotels()
    {
        var delete_all_hotel = _context.Hotels.ToList();
        _context.BulkDelete(delete_all_hotel); 
    }
    
    #endregion
    
    #region Management Rooms
    
    public List<Room> GetRooms()
    {
        return _context.Rooms.ToList();
    }

    public Room GetRoom(int id)
    {  
        var room = _context.Rooms.Where(r=>r.RoomId == id).FirstOrDefault();
        if (room == null)
        {
            return null;
        }
        return room;
    }
    
    public void SetRoom(Room room)
    {
        _context.Rooms.Add(room);
        _context.SaveChanges();
    }

    public void UpdateRoom(Room room)
    {
        var room_update = _context.Rooms.Where(r=>r.RoomId == room.RoomId).FirstOrDefault();
        room_update.DiscountedPrice = room.DiscountedPrice != -1 ?  room.DiscountedPrice : room_update.DiscountedPrice;
        room_update.Price = room.Price != -1 ?  room.Price : room_update.Price;
        room_update.HotelId = room.HotelId != -1 ?  room.HotelId : room_update.HotelId;
        room_update.Adult = room.Adult != -1 ?  room.Adult : room_update.Adult;
        room_update.Children = room.Children != -1 ?  room.Children : room_update.Children;
        room_update.Descriptions = room.Descriptions != "" ?  room.Descriptions : room_update.Descriptions;
        room_update.Image = room.Image != "" ?  room.Image : room_update.Image;
        _context.SaveChanges();
    }
   
    public void DeleteRoom(int id)
    {
        var delete_room = _context.Rooms.Where(r=>r.RoomId == id).FirstOrDefault();
        _context.Rooms.Remove(delete_room);
        _context.SaveChanges();
    }
    
    public void DeleteRooms()
    {
        var delete_all_room = _context.Rooms.ToList();
        _context.BulkDelete(delete_all_room); 
    }
    
    #endregion
    
    #region Management BookRoom
    
    public List<BookRoom> GetBookRooms()
    {
        return _context.BookRooms.ToList();
    }

    public BookRoom GetBookRoom(int id)
    {  
        var bookroom = _context.BookRooms.Where(r=>r.Id == id).FirstOrDefault();
        if (bookroom == null)
        {
            return null;
        }
        return bookroom;
    }
    
    public async Task SetBookRoom(BookRoom bookRoom)
    {
        _context.BookRooms.Add(bookRoom);
        _context.SaveChanges();
    }

    public void UpdateBookRoom(BookRoom bookRoom)
    {
        var bookRoom_update = _context.BookRooms.Where(b=>b.Id == bookRoom.Id).FirstOrDefault();
        bookRoom_update.RoomId = bookRoom.RoomId != -1 ?  bookRoom.RoomId : bookRoom_update.RoomId;
        bookRoom_update.BookFrom = bookRoom.BookFrom != null ?  bookRoom.BookFrom : bookRoom_update.BookFrom;
        bookRoom_update.BookTo = bookRoom.BookTo != null ?  bookRoom.BookTo : bookRoom_update.BookTo;
        bookRoom_update.UserId = bookRoom.UserId != -1 ?  bookRoom.UserId : bookRoom_update.UserId;
        _context.SaveChanges();
    }
   
    public void DeleteBookRoom(int id)
    {
        var delete_bookroom = _context.BookRooms.Where(b=>b.Id ==  id).FirstOrDefault();
        _context.BookRooms.Remove(delete_bookroom);
        _context.SaveChanges();
    }
    
    public void DeleteBookRooms()
    {
        var delete_all_bookroom = _context.BookRooms.ToList();
        _context.BulkDelete(delete_all_bookroom); 
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
        message.From.Add(new MailboxAddress("Your Company", "borak.sabanje@gmail.com"));
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
                client.Connect("smtp.gmail.com", 587, false);

                // Authenticate (use your credentials here)
                client.Authenticate("borak.sabanje@gmail.com", "571833");

                // Send the message
                client.Send(message);

                // Disconnect
                client.Disconnect(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }
    }
    
    #endregion 
   

    
}

 
 
 