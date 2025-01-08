using System.Collections.Immutable;
using System.Data.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.IO;
using FinalProjectTrainingFTS.DataBase;
using FinalProjectTrainingFTS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Server.IIS.Core;


namespace FinalProjectTrainingFTS.CustomerService;

public class CustomerService
{
    private readonly FinalProjectTrainingFtsContext _context = new FinalProjectTrainingFtsContext();
    private readonly IConfiguration _configuration;
    private static User user_in;
    
    private readonly string _imageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Images");

    public CustomerService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    

    public Dictionary<string, string> GetLoginUser(string inputUsername, string inputPassword)
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
    
    public List<FeaturedDealsResponse> GetFeaturedDealsHotels()
    {
        var featuredDealsHotels = _context.Hotels
            .Include(h => h.Rooms) // Include related Rooms
            .Where(h => h.Rooms.Any()) // Exclude hotels with no rooms
            .OrderBy(h => h.Rooms.Min(r => r.DiscountedPrice * r.Price / 100.0)) // Sort by the minimum calculated value in Rooms
            .Select(h => new FeaturedDealsResponse
            {
                hotel = h, // Select the entire Hotel object
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

    public void Updatevisitedhoteluser(int id_hotel_visited)
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
        return new FileContentResult(fileBytes, contentType);
        // Return the file
        

       // return fileBytes;
    }
    
    
    public async Task<string> upload_image (IFormFile imageFile){
        
    if (imageFile == null || imageFile.Length == 0)
    { 
        return   "No file uploaded or file is empty.";
    }

    // Validate the file type
    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
    var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(fileExtension))
    {
        return  "Invalid file type. Only JPG, JPEG, PNG, and GIF are allowed.";
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

        return "File uploaded successfully";
    }
    
    
    
    
    
    
    
    
    // Helper method to determine MIME type
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

   
    
    ////when book room add increment in table city filed visited 
    
}

 
 
 