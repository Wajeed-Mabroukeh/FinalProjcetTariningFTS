using System.Text;
using System.Text.Json;
using FinalProjectTrainingFTS.Controllers;
using FinalProjectTrainingFTS.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;


namespace FinalProjectTrainingFTSTest.Test;

public class LoginTest : IClassFixture<WebApplicationFactory<FinalProjectTrainingFTS.Controllers.ControllerProject>>
{
    private readonly HttpClient _client;


    public LoginTest(WebApplicationFactory<FinalProjectTrainingFTS.Controllers.ControllerProject> factory)
    {
        _client = factory.CreateClient();
    }
    
    [Fact]
    public  async Task LoginUser()
    {
        //Arrange
        var login_User = new LoginRequest()
        {
            UserName = "wajeed",
            Password = "123"
        };
        
        var content = new StringContent(
            JsonSerializer.Serialize(login_User), 
            Encoding.UTF8, 
            "application/json"
        );
        
        // Act: Send POST request
        var response = await _client.PostAsync("/Login/User", content);

        // Assert: Verify response status and content
        response.EnsureSuccessStatusCode(); 
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var createdReservation = JsonSerializer.Deserialize<Dictionary<string,string>>(responseContent);
        
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(createdReservation);
        
      }

    [Fact]
    public  async Task LoginAdmin()
    {
        //Arrange
        var login_Admin = new LoginRequest()
        {
            UserName = "ibrahim",
            Password = "1234"
        };
        
        var content = new StringContent(
            JsonSerializer.Serialize(login_Admin), 
            Encoding.UTF8, 
            "application/json"
        );
        
        // Act: Send POST request
        var response = await _client.PostAsync("/Login/Admin", content);

        // Assert: Verify response status and content
        response.EnsureSuccessStatusCode(); 
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var createdReservation = JsonSerializer.Deserialize<Dictionary<string,string>>(responseContent);
        
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(createdReservation);
    }
    
}