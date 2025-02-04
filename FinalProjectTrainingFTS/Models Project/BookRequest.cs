namespace FinalProjectTrainingFTS.Models;

public class BookRequest
{
    public int id_room { get; set; }
    public int user_id { get; set; }
    
    public DateTime book_from { get; set; }

    public DateTime book_to { get; set; }
    
    public string payment_id { get; set; }

    
}