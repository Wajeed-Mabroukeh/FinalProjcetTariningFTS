using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinalProjectTrainingFTS.Controllers;

[ApiController]
[Route("/Login/")]
public class ControllerLogin : ControllerBase
{

    private readonly IConfiguration _configuration;
    private CustomerService.CustomerService entity;

    public ControllerLogin(IConfiguration configuration)
    {
        _configuration = configuration;
        entity = new CustomerService.CustomerService(_configuration);
    }
    
    
    [HttpGet("User")]
    public string GetLoginU([FromBody] LoginRequest loginRequest)
    {
        return entity.GetLoginUser(loginRequest.UserName, loginRequest.Password);
    }
    
    [HttpGet("Admin")]
    public string GetLoginA([FromBody] LoginRequest loginRequest)
    {
        return entity.GetLoginAdmin(loginRequest.UserName, loginRequest.Password);
    }
}

public class LoginRequest
{
    [Required]
    public string UserName { get; set; }
    
    [Required]
    public string Password { get; set; }
}