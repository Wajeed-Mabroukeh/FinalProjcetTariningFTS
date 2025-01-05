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
   
    [Authorize(Roles = "Admin")]
    [HttpGet("GetProject")]
    public List<User> GetProject()
    {
        return entity.GetUser();
    }
    
    [Authorize(Roles = "User")]
    [HttpGet("search/hotel")] 
    public List<Hotel> GetSearchHotels([FromBody] SearchRequest request)
    {
        return entity.SearchHotels(request);
    }

     
   
   
    
   
    
   
}
