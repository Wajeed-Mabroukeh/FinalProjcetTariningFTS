using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FinalProjectTrainingFTS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using Moq.Protected;

namespace FinalProjectTrainingFTSTest.Test;

public class Features_API_Admin_Test : IClassFixture<WebApplicationFactory<FinalProjectTrainingFTS.Controllers.ControllerProject>>
{
    private readonly HttpClient _client;
    
    public static IEnumerable<object[]> token_jwt = new List<object[]>
    {
        new object[] { "access_token", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiaWJyYWhpbSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwiZXhwIjoxNzM2OTUwNjMyLCJpc3MiOiJmaW5hbHByb2plY3R0cmFpbmluZ2Z0cyIsImF1ZCI6IldhamVlZCJ9.gzJpNnpXyUzOiBHnZErOGMiXz_A4_sCtSVk7NYS7gIw" }
    };
    
    public Features_API_Admin_Test(WebApplicationFactory<FinalProjectTrainingFTS.Controllers.ControllerProject> factory)
    {
        _client = factory.CreateClient();
    }

    #region Admin Test
    
    #region Management Cities API 
    
    [Theory]
    [MemberData(nameof(token_jwt))]
    public async Task GetCities(string key,string token)
    {
        //Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/GetCities");
        
        //Authentication with Token Jwt
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        // Send the request
        var response = await _client.SendAsync(request);
        //Act
        response.EnsureSuccessStatusCode(); // Status code 200-299
        var jsonString = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<List<City>>(jsonString);
        
        //Assert
        Assert.NotNull(data);
     
        
    }
    
    [Theory]
    [MemberData(nameof(token_jwt))]
    public async Task SetCities(string key,string token)
    {
        //Arrange
        var city = new City()
        {
                Name = "nabluss",
                Country = "palestine",
                PostOffice = 125,
                Id = 12,
                VisitCount = 0,
                Image = "2.png"
        };
        
        var content = new StringContent(
            JsonSerializer.Serialize(city), 
            Encoding.UTF8, 
            "application/json"
        );
        
        
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/Create/Cities");
        
        //Authentication with Token Jwt
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        request.Content = content;
        // Act: Send POST request
        var response = await _client.SendAsync(request);
        
        // Assert: Verify response status and content
        response.EnsureSuccessStatusCode(); 
        
        Assert.True(response.IsSuccessStatusCode);
    }
    
    [Theory]
    [MemberData(nameof(token_jwt))]
    public async Task GetCity(string key,string token)
    {
        //Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/GetCity/10");
        
        //Authentication with Token Jwt
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        // Send the request
        var response = await _client.SendAsync(request);
        //Act
        response.EnsureSuccessStatusCode(); // Status code 200-299
        var jsonString = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<City>(jsonString);
        
        //Assert
        Assert.NotNull(data);
     
        
    }
    
    [Theory]
    [MemberData(nameof(token_jwt))]
    public async Task UpdateCity(string key,string token)
    {
        //Arrange
        var city = new City()
        {
            Name = "nablu",
            Country = "palestine",
            PostOffice = 125,
            Id = 10,
            VisitCount = 0,
            Image = "2.png"
        };
        
        var content = new StringContent(
            JsonSerializer.Serialize(city), 
            Encoding.UTF8, 
            "application/json"
        );
        
        var request = new HttpRequestMessage(HttpMethod.Put, "/api/Update/City");
        
        //Authentication with Token Jwt
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        request.Content = content;
        // Act: Send POST request
        var response = await _client.SendAsync(request);

        // Assert: Verify response status and content
        response.EnsureSuccessStatusCode(); 
        
        Assert.True(response.IsSuccessStatusCode);
    }
    
     [Theory]
    [MemberData(nameof(token_jwt))]
     public async Task DeleteCities(string key,string token)
     {
   
        //Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/Delete/Cities");
            
        //Authentication with Token Jwt
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
         //Send the request
        var response = await _client.SendAsync(request);
         //Act
         response.EnsureSuccessStatusCode(); // Status code 200-299
         
         //Assert
         Assert.True(response.IsSuccessStatusCode);
         
     }
    
     [Theory]
    [MemberData(nameof(token_jwt))]
     public async Task DeleteCity(string key,string token)
     {
         // Arrange
         var request = new HttpRequestMessage(HttpMethod.Get, "/api/Delete/City/12");
            
        // Authentication with Token Jwt
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
         //Send the request
        var response = await _client.SendAsync(request);   
         //Act
         response.EnsureSuccessStatusCode(); // Status code 200-299
         
         //Assert
         Assert.True(response.IsSuccessStatusCode);
         
     }
    
    #endregion
    
    #region Management Hotel API 
    
    [Theory]
    [MemberData(nameof(token_jwt))]
    public async Task GetHotels(string key,string token)
    {
        //Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/GetHotels");
        
        //Authentication with Token Jwt
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        // Send the request
        var response = await _client.SendAsync(request);
        //Act
        response.EnsureSuccessStatusCode(); // Status code 200-299
        var jsonString = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<List<Hotel>>(jsonString);
        
        //Assert
        Assert.NotNull(data);
        
    }
    
    [Theory]
    [MemberData(nameof(token_jwt))]
    public async Task SetHotel(string key,string token)
    {
        //Arrange
        var hotel = new Hotel()
        {
            Name = "wwww",
            StarRate = 4,
            Owner = "mohammed",
            Id =  77,
            CityId =  16,
            Image = "fhb",
            Descriptions = "fgaaa",
            Amenities = "luxury",
            Latitude = "120n",
            Longitude = "fdf4",
            City = null,
            Rooms = {}
        };
        
        var content = new StringContent(
            JsonSerializer.Serialize(hotel), 
            Encoding.UTF8, 
            "application/json"
        );
        
        //Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/Create/Hotel");
        
        //Authentication with Token Jwt
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        request.Content = content;
        // Send the request
        var response = await _client.SendAsync(request);
        // Assert: Verify response status and content
        response.EnsureSuccessStatusCode(); 
        
        Assert.True(response.IsSuccessStatusCode);
    }
    
    [Theory]
    [MemberData(nameof(token_jwt))]
    public async Task GetHotel(string key,string token)
    {
        //Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/GetHotel/20");
        
        //Authentication with Token Jwt
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        // Send the request
        var response = await _client.SendAsync(request);
        //Act
        response.EnsureSuccessStatusCode(); // Status code 200-299
        var jsonString = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<Hotel>(jsonString);
        
        //Assert
        Assert.NotNull(data);
     
        
    }
    
    [Theory]
    [MemberData(nameof(token_jwt))]
    public async Task UpdateHotel(string key,string token)
    {
        var hotel = new Hotel()
        {
            Name = "malkee",
            StarRate = 4,
            Owner = "mohammed",
            Id =  102,
            CityId =  16,
            Image = "fhb",
            Descriptions = "eddswd222",
            Amenities = "luxury",
            Latitude = "120n",
            Longitude = "fdf4",
            City = null,
            Rooms = {}
        };
        
        var content = new StringContent(
            JsonSerializer.Serialize(hotel), 
            Encoding.UTF8, 
            "application/json"
        );
        
        //Arrange
        var request = new HttpRequestMessage(HttpMethod.Put, "/api/Update/Hotel");
        
        //Authentication with Token Jwt
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        request.Content = content;
        // Send the request
        var response = await _client.SendAsync(request);
        // Assert: Verify response status and content
        response.EnsureSuccessStatusCode(); 
        
        Assert.True(response.IsSuccessStatusCode);
    }
    
    [Theory]
    [MemberData(nameof(token_jwt))]
     public async Task DeleteHotels(string key,string token)
     {
         //Arrange
         var request = new HttpRequestMessage(HttpMethod.Get, "/api/Delete/Hotels");
        
         //Authentication with Token Jwt
         _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
         // Send the request
         var response = await _client.SendAsync(request);
         //Act
         response.EnsureSuccessStatusCode(); // Status code 200-299
         
         //Assert
         Assert.True(response.IsSuccessStatusCode);
         
     }
    
    [Theory]
    [MemberData(nameof(token_jwt))]
     public async Task DeleteHotel(string key,string token)
     {
         //Arrange
         var request = new HttpRequestMessage(HttpMethod.Get, "/api/Delete/Hotel/102");
        
         //Authentication with Token Jwt
         _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
         // Send the request
         var response = await _client.SendAsync(request);
         //Act
         response.EnsureSuccessStatusCode(); // Status code 200-299
         
         //Assert
         Assert.True(response.IsSuccessStatusCode);
         
     }
    
    #endregion
    
    #region Management Rooms API 
    
    [Theory]
    [MemberData(nameof(token_jwt))]
    public async Task GetRooms(string key,string token)
    {
        //Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/GetRoom");
        
        //Authentication with Token Jwt
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        // Send the request
        var response = await _client.SendAsync(request);
        //Act
        response.EnsureSuccessStatusCode(); // Status code 200-299
        var jsonString = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<List<Room>>(jsonString);
        
        //Assert
        Assert.NotNull(data);
        
    }
    
    [Theory]
    [MemberData(nameof(token_jwt))]
    public async Task SetRoom(string key,string token)
    {
        //Arrange
        var room = new Room()
        {
            RoomId = 88,
            Adult = 3,
            Children = 1,
            HotelId = 14,
            Image = "1.jpg",
            Descriptions = "lwdkm",
            Price = 200,
            DiscountedPrice = 30,
            BookRooms = {  }
        };
        
        var content = new StringContent(
            JsonSerializer.Serialize(room), 
            Encoding.UTF8, 
            "application/json"
        );
        
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/Create/Room");
        
        //Authentication with Token Jwt
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = content;
        // Send the request
        var response = await _client.SendAsync(request);
        // Assert: Verify response status and content
        response.EnsureSuccessStatusCode(); 
        
        Assert.True(response.IsSuccessStatusCode);
    }
    
    [Theory]
    [MemberData(nameof(token_jwt))]
    public async Task GetRoom(string key,string token)
    {
        //Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/GetRoom/455");
        
        //Authentication with Token Jwt
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        // Send the request
        var response = await _client.SendAsync(request);
        //Act
        response.EnsureSuccessStatusCode(); // Status code 200-299
        var jsonString = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<Room>(jsonString);
        
        //Assert
        Assert.NotNull(data);
     
        
    }
    
    [Theory]
    [MemberData(nameof(token_jwt))]
    public async Task UpdateRoom(string key,string token)
    {
        var room = new Room()
        {
            RoomId = 4554,
            Adult = 3,
            Children = 1,
            HotelId = 14,
            Image = "1.jpg",
            Descriptions = "lwdkm",
            Price = 20,
            DiscountedPrice = 30,
            BookRooms = {  }
        };
        
        var content = new StringContent(
            JsonSerializer.Serialize(room), 
            Encoding.UTF8, 
            "application/json"
        );
        
        var request = new HttpRequestMessage(HttpMethod.Put, "/api/Update/Room");
        
        //Authentication with Token Jwt
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = content;
        // Send the request
        var response = await _client.SendAsync(request);
        // Assert: Verify response status and content
        response.EnsureSuccessStatusCode(); 
        
        Assert.True(response.IsSuccessStatusCode);
    }
    
    [Theory]
    [MemberData(nameof(token_jwt))]
     public async Task DeleteRooms(string key,string token)
     {
         //Arrange
         var request = new HttpRequestMessage(HttpMethod.Get, "/api/Delete/Rooms");
        
         //Authentication with Token Jwt
         _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
         
         // Send the request
         var response = await _client.SendAsync(request);
         //Act
         response.EnsureSuccessStatusCode(); // Status code 200-299
         
         //Assert
         Assert.True(response.IsSuccessStatusCode);
         
     }
    
    [Theory]
    [MemberData(nameof(token_jwt))]
     public async Task DeleteRoom(string key,string token)
     {
         //Arrange
         var request = new HttpRequestMessage(HttpMethod.Get, "/api/Delete/Room/510");
        
         //Authentication with Token Jwt
         _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
         
         // Send the request
         var response = await _client.SendAsync(request);
         //Act
         response.EnsureSuccessStatusCode(); // Status code 200-299
         
         //Assert
         Assert.True(response.IsSuccessStatusCode);
         
     }
    
    #endregion
    
    #region Management Book Rooms API 
    
    [Theory]
    [MemberData(nameof(token_jwt))]
    public async Task GetBookRooms(string key,string token)
    {
        //Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/GetBookRooms");
        
        //Authentication with Token Jwt
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
         
        // Send the request
        var response = await _client.SendAsync(request);
        //Act
        response.EnsureSuccessStatusCode(); // Status code 200-299
        var jsonString = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<List<BookRoom>>(jsonString);
        
        //Assert
        Assert.NotNull(data);
        
    }
    
    [Theory]
    [MemberData(nameof(token_jwt))]
    public async Task SetBookRoom(string key,string token)
    {
        //Arrange
        var bookroom = new BookRoom()
        {
            RoomId = 455,
            UserId = 1,
            BookFrom = Convert.ToDateTime("2025-09-01T00:00:00"),
            BookTo = Convert.ToDateTime("2025-09-02T00:00:00"),
            Id = 999,
            Room = null
        };
        
        var content = new StringContent(
            JsonSerializer.Serialize(bookroom), 
            Encoding.UTF8, 
            "application/json"
        );
        
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/Create/BookRoom");
        
        //Authentication with Token Jwt
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = content;
        // Send the request
        var response = await _client.SendAsync(request);
        // Assert: Verify response status and content
        response.EnsureSuccessStatusCode(); 
        
        Assert.True(response.IsSuccessStatusCode);
    }
    
    [Theory]
    [MemberData(nameof(token_jwt))]
    public async Task GetBokRoom(string key,string token)
    {
        //Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/GetBookRoom/999");
        
        //Authentication with Token Jwt
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
         
        // Send the request
        var response = await _client.SendAsync(request);
        //Act
        response.EnsureSuccessStatusCode(); // Status code 200-299
        var jsonString = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<BookRoom>(jsonString);
        
        //Assert
        Assert.NotNull(data);
     
        
    }
    
    [Theory]
    [MemberData(nameof(token_jwt))]
    public async Task UpdateBookRoom(string key,string token)
    {
        var bookroom = new BookRoom()
        {
            RoomId = 455,
            UserId = 1,
            BookFrom = Convert.ToDateTime("2025-09-01T00:00:00"),
            BookTo = Convert.ToDateTime("2025-09-06T00:00:00"),
            Id = 999,
            Room = null
        };
        
        var content = new StringContent(
            JsonSerializer.Serialize(bookroom), 
            Encoding.UTF8, 
            "application/json"
        );
        
        var request = new HttpRequestMessage(HttpMethod.Put, "/api/Update/BookRoom");
        
        //Authentication with Token Jwt
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = content;
        // Send the request
        var response = await _client.SendAsync(request);
        // Assert: Verify response status and content
        response.EnsureSuccessStatusCode(); 
        
        Assert.True(response.IsSuccessStatusCode);
    }
    
    [Theory]
    [MemberData(nameof(token_jwt))]
     public async Task DeleteBookRooms(string key,string token)
     {
         //Arrange
         var request = new HttpRequestMessage(HttpMethod.Get, "/api/DeleteBookRooms");
        
         //Authentication with Token Jwt
         _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
         // Send the request
         var response = await _client.SendAsync(request);
         
         //Act
         response.EnsureSuccessStatusCode(); // Status code 200-299
         
         //Assert
         Assert.True(response.IsSuccessStatusCode);
         
     }
    
    [Theory]
    [MemberData(nameof(token_jwt))]
     public async Task DeleteBookRoom(string key,string token)
     {
         //Arrange
         var request = new HttpRequestMessage(HttpMethod.Get, "/api/Delete/BookRoom/999");
        
         //Authentication with Token Jwt
         _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
         // Send the request
         var response = await _client.SendAsync(request);
         //Act
         response.EnsureSuccessStatusCode(); // Status code 200-299
         
         //Assert
         Assert.True(response.IsSuccessStatusCode);
         
     }
    
    #endregion

    #region Upload Image

    [Theory]
    [MemberData(nameof(token_jwt))]
    public async Task UploadImage(string key,string token)
    {

        // Arrange
        var imageFilePath = Path.Combine(Directory.GetCurrentDirectory(), "5.png");
        var fileContent = new ByteArrayContent(new byte[] { 1, 2, 3, 4 });
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");

        var content = new MultipartFormDataContent
        {
            { fileContent, "imageFile", imageFilePath }
        };
        
        //Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/Upload/Image");
        
        //Authentication with Token Jwt
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = content;
        // Send the request
        var response = await _client.SendAsync(request);
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

       
       
       
    }
    
    #endregion

    #region SetLocation
 
    [Theory]
    [MemberData(nameof(token_jwt))]
    public async Task SetLocation(string key,string token)
    {
        //Arrange
        var location = new SetLocation()
        {
            id_hotel = 510,
            Address = "ddfsf",
        };
        
        var content = new StringContent(
            JsonSerializer.Serialize(location), 
            Encoding.UTF8, 
            "application/json"
        );
        
        var request = new HttpRequestMessage(HttpMethod.Put, "/api/Location");
        
        //Authentication with Token Jwt
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = content;
        // Send the request
        var response = await _client.SendAsync(request);
        
        // Assert: Verify response status and content
        response.EnsureSuccessStatusCode(); 
        
        Assert.True(response.IsSuccessStatusCode);
    }
    
    #endregion

    #region Add User
  
    [Theory]
    [MemberData(nameof(token_jwt))]
    public async Task SetUser(string key,string token)
    {
        //Arrange
        var user = new User()
        {
            UserName = "mohammed",
            Password = "88fh",
            Id = 8,
            VisitedHotels = ""
        };
        
        var content = new StringContent(
            JsonSerializer.Serialize(user), 
            Encoding.UTF8, 
            "application/json"
        );
        
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/Add/User");
        
        //Authentication with Token Jwt
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = content;

        // Send the request
        var response = await _client.SendAsync(request);
        
        // Assert: Verify response status and content
        response.EnsureSuccessStatusCode(); 
        
        Assert.True(response.IsSuccessStatusCode);
    }
    #endregion

    #region Add Admin
  
    [Theory]
    [MemberData(nameof(token_jwt))]
    public async Task SetAdmin(string key,string token)
    {
        //Arrange
        var admin = new Admin()
        {
            UserName = "test mohammed",
            Password = "88fh",
            Id = 025,
        };
        
        var content = new StringContent(
            JsonSerializer.Serialize(admin), 
            Encoding.UTF8, 
            "application/json"
        );
        
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/Add/Admin");
        
        //Authentication with Token Jwt
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = content;

        // Send the request
        var response = await _client.SendAsync(request);
        
        // Assert: Verify response status and content
        response.EnsureSuccessStatusCode(); 
        
        Assert.True(response.IsSuccessStatusCode);
    }
    #endregion 
    
    #endregion
    
}