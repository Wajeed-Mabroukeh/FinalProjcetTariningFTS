using System.Text;
using System.Net.Http.Headers;
using System.Text.Json;
using FinalProjectTrainingFTS.CustomerService;
using FinalProjectTrainingFTS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;

namespace FinalProjectTrainingFTSTest.Test;

public class Features_API_User_Test : IClassFixture<WebApplicationFactory<FinalProjectTrainingFTS.Controllers.ControllerProject>>
{
    
    private readonly HttpClient _client;

   
    public static IEnumerable<object[]> token_jwt = new List<object[]>
    {
        new object[] { "access_token", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoid2FqZWVkIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiVXNlciIsImV4cCI6MTczNjk0ODIwOSwiaXNzIjoiZmluYWxwcm9qZWN0dHJhaW5pbmdmdHMiLCJhdWQiOiJXYWplZWQifQ.XSJI1taQGqNiEGJ-Jl_aLHseJOhMTakHNJxE_ZKXnYA" }
    };

    public Features_API_User_Test(WebApplicationFactory<FinalProjectTrainingFTS.Controllers.ControllerProject> factory)
    {
        _client = factory.CreateClient();
    }
    
    #region UserTest

    
    #region GetImage
    
    [Theory]
    [MemberData(nameof(token_jwt))]
    public async Task GetImage(string key,string token)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/GetImage/2.png");
        
        //Authentication with Token Jwt
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        //Arrange
        // Send the request
        var response = await _client.SendAsync(request);
        
        //Act
        response.EnsureSuccessStatusCode(); // Status code 200-299
        
        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("image/png", response.Content.Headers.ContentType.ToString());
    }
    #endregion
    
    #region FeaturedDeals

    [Theory]
    [MemberData(nameof(token_jwt))]
    public async Task GetFeaturedDealsHotels(string key,string token)
    {
        
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/FeaturedDeals");
        
        //Authentication with Token Jwt
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        
        //Arrange
        // Send the request
        var response = await _client.SendAsync(request);
        
        //Act
        response.EnsureSuccessStatusCode(); // Status code 200-299
        var jsonString = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<List<FeaturedDealsResponse>>(jsonString);
        
        //Assert
        Assert.NotNull(data);
    }
    #endregion
    
    #region User's Recently Visited Hotels

    [Theory]
    [MemberData(nameof(token_jwt))]
    public async Task GetRecentlyVisitedHotels(string key,string token)
    {
        
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/RecentlyVisitedHotels");
        
        //Authentication with Token Jwt
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        
        //Arrange
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
    public async Task UpdateVisitedHotel(string key,string token)
    {
        
        //Arrange
        var hotel_v = new HotelVisitedRequest()
        {
            IdHotelVisited = 10
        };
        
        var content = new StringContent(
            JsonSerializer.Serialize(hotel_v), 
            Encoding.UTF8, 
            "application/json"
        );
        
        var request = new HttpRequestMessage(HttpMethod.Put, "/api/UpdateVisitedHotelUser");
        
        //Authentication with Token Jwt
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        request.Content = content;
        //Arrange
        // Send the request
        var response = await _client.SendAsync(request);
        
        //Act
        response.EnsureSuccessStatusCode(); // Status code 200-299
      
        
        //Assert
        Assert.True(response.IsSuccessStatusCode);
    }
    
    #endregion
    
    #region Trending Destination Highlights

    [Theory]
    [MemberData(nameof(token_jwt))]
    public async Task TrendingDestinationHighlights(string key,string token)
    {
        
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/TrendingDestinationHighlights");
        
        //Authentication with Token Jwt
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        
        //Arrange
        // Send the request
        var response = await _client.SendAsync(request);
        
        //Act
        response.EnsureSuccessStatusCode(); // Status code 200-299
        var jsonString = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<List<City>>(jsonString);
        
        //Assert
        Assert.NotNull(data);
    }
    
    #endregion
    
    #region Search Results Page

    [Theory]
    [MemberData(nameof(token_jwt))]
    public async Task GetSearchHotels( string key,string token)
    {

        var search = new SearchRequest()
        {
            Query = "ff",
            CheckInDate = Convert.ToDateTime("2025-01-02"),
            CheckOutDate = Convert.ToDateTime("2025-01-04"),
        };
        var content = new StringContent(
            JsonSerializer.Serialize(search), 
            Encoding.UTF8, 
            "application/json"
        );
        
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/SearchResultsHotel");
        
        //Authentication with Token Jwt
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        
        //Arrange
        // Add the content to the request body
        request.Content = content;

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
    public async Task SearchResultsPriceRange(string key,string token)
    {

        var search = new SearchRequest()
        {
            price_min = 10,
            price_max = 100
        };
        var content = new StringContent(
            JsonSerializer.Serialize(search), 
            Encoding.UTF8, 
            "application/json"
        );
        
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/SearchResultsPriceRange");
        
        //Authentication with Token Jwt
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        
        //Arrange
        // Add the content to the request body
        request.Content = content;

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
    public async Task SearchResultsStarAmenities(string key,string token)
    {

        var search = new SearchRequest()
        {
           amenities = "luxury"
        };
        var content = new StringContent(
            JsonSerializer.Serialize(search), 
            Encoding.UTF8, 
            "application/json"
        );
        
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/SearchResultsStarAmenities");
        
        //Authentication with Token Jwt
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        
        //Arrange
        // Add the content to the request body
        request.Content = content;

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
    public async Task SearchResultsStarRating(string key,string token)
    {

        var search = new SearchRequest()
        {
           star_rate = 5
        };
        var content = new StringContent(
            JsonSerializer.Serialize(search), 
            Encoding.UTF8, 
            "application/json"
        );
        
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/SearchResultsStarRating");
        
        //Authentication with Token Jwt
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        
        //Arrange
        // Add the content to the request body
        request.Content = content;

        // Send the request
        var response = await _client.SendAsync(request);
        //Act
        response.EnsureSuccessStatusCode(); // Status code 200-299
        var jsonString = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<List<Hotel>>(jsonString);
        
        //Assert
        Assert.NotNull(data);
    }
    
    #endregion
    
    #region Room Availability

    [Theory]
    [MemberData(nameof(token_jwt))]
    public async Task GetAvailabileRoom(string key,string token)
    {
        //Arrange
        var search = new AvailableRequest()
        {
            id_hotel = 10,
            bookfrom = Convert.ToDateTime("2025-07-02"),
            bookto = Convert.ToDateTime("2025-07-04"),
        };
        var content = new StringContent(
            JsonSerializer.Serialize(search), 
            Encoding.UTF8, 
            "application/json"
        );
        
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/RoomAvailability");
        
        //Authentication with Token Jwt
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        
        //Arrange
        // Add the content to the request body
        request.Content = content;

        // Send the request
        var response = await _client.SendAsync(request);
        //Act
        response.EnsureSuccessStatusCode(); // Status code 200-299
        var jsonString = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<List<Room>>(jsonString);
        
        //Assert
        Assert.NotNull(data);
    }
    #endregion
    
    #region Get Login User

    [Theory]
    [MemberData(nameof(token_jwt))]
    public async Task GetLoginUser(string key,string token)
    {
        
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/User/Login");
        
        //Authentication with Token Jwt
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        
        //Arrange
        // Send the request
        var response = await _client.SendAsync(request);
        
        //Act
        response.EnsureSuccessStatusCode(); // Status code 200-299
        var jsonString = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<User>(jsonString);
        
        //Assert
        Assert.NotNull(data);
    }
    #endregion
    
    
    #endregion
}