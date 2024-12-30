using FinalProjectTrainingFTS.Modules;
using Microsoft.AspNetCore.Mvc;

namespace FinalProjectTrainingFTS.Controllers;

[ApiController]
[Route("/api/")]
public class ControllerProject : ControllerBase
{

    [HttpGet("GetProject")]
    public List<User> GetProject()
    {
        var entity = new CustomerService.CustomerService();
        return entity.GetUser();
    }
   
}
