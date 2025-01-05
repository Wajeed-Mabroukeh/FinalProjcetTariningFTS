using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FinalProjectTrainingFTS.DataBase;
using FinalProjectTrainingFTS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace FinalProjectTrainingFTS.CustomerService;

public class CustomerService
{
    private readonly FinalProjectTrainingFtsContext _context = new FinalProjectTrainingFtsContext();
    private readonly IConfiguration _configuration;

    public CustomerService(IConfiguration configuration)
    {
        _configuration = configuration;
    }


    public List<User> GetUser()
    {
        var user = _context.Users.ToList();
        return user;
    }

    public string GetLoginUser(string inputUsername, string inputPassword)
    {
        if (string.IsNullOrEmpty(inputUsername) || string.IsNullOrEmpty(inputPassword))
        {
            return null;
        }

        var user = _context.Users
            .FirstOrDefault(u => u.UserName == inputUsername && u.Password == inputPassword);
        string result = "Bearer \n";
        if (!user.Equals(null))
        {
            var tokenService = new JwtTokenService(_configuration);
            var token = tokenService.GenerateToken(user.UserName, 1);
            result += token;
        }
        else
        {
            result = "error";
        }


        return result;
    }

    public string GetLoginAdmin(string inputUsername, string inputPassword)
    {
        if (string.IsNullOrEmpty(inputUsername) || string.IsNullOrEmpty(inputPassword))
        {
            return null;
        }

        var admin = _context.Admins
            .FirstOrDefault(u => u.UserName == inputUsername && u.Password == inputPassword);

        string result = "Bearer \n";
        if (!admin.Equals(null))
        {
            var tokenService = new JwtTokenService(_configuration);
            var token = tokenService.GenerateToken(admin.UserName, 0);
            result += token;
        }
        else
        {
            result = "error";
        }


        return result;
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
    

    
}

 
 
 