namespace FinalProjectTrainingFTS.Models;

public class Confirmation
{
    public BookRoom BookRoom { get; set; }
    public Room Room { get; set; }
    public string HotelAddress { get; set; }

    public double TotalAmount { get; set; }
}