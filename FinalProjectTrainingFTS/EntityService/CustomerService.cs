using FinalProjectTrainingFTS.DataBase;
using FinalProjectTrainingFTS.Modules;

namespace FinalProjectTrainingFTS.CustomerService;

public class CustomerService
{
    private readonly ConnectionDataBase _context = new ConnectionDataBase();


    public List<User> GetUser()
    {
        // var user1 = new User
        // {
        //     ID = 10,
        //     Name = "Wajeed",
        //     Password = "dsc"
        // };
        // _context.user.Add(user1);
        var user = _context.User.ToList();
        return user;
    }

}

 
 
 