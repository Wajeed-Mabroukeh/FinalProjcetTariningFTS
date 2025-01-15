using System.ComponentModel.DataAnnotations;
using FinalProjectTrainingFTS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinalProjectTrainingFTS.Controllers;

[ApiController]
[Route("/Login/")]
public class ControllerLogin : ControllerBase
{

    private readonly IConfiguration _configuration;
    private  CustomerService.CustomerService entity;

    public ControllerLogin(IConfiguration configuration)
    {
        _configuration = configuration;
        entity = new CustomerService.CustomerService(_configuration);
    }
    
    
    [HttpPost("User")]
    public  Dictionary<string, string> GetLoginU([FromBody] LoginRequest loginRequest)
    {
        return entity.GetLoginUser(loginRequest.UserName, loginRequest.Password);
    }
    
    [HttpPost("Admin")]
    public Dictionary<string, string> GetLoginA([FromBody] LoginRequest loginRequest)
    {
        return entity.GetLoginAdmin(loginRequest.UserName, loginRequest.Password);
    }
}

