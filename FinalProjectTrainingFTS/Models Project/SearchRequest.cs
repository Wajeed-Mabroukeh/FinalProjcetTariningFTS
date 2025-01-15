namespace FinalProjectTrainingFTS.Models;

public class SearchRequest
{
    public string Query { get; set; } = string.Empty; // Hotel or city name
    public DateTime CheckInDate { get; set; } = DateTime.Today; // Default: Today
    public DateTime CheckOutDate { get; set; } = DateTime.Today.AddDays(1); // Default: Tomorrow
    public int Adults { get; set; } = 2; // Default: 2 adults
    public int Children { get; set; } = 0; // Default: 0 children
    public int Rooms { get; set; } = 1; // Default: 1 room
    
    public int price_min { get; set; }
    public int price_max { get; set; } 
    public int star_rate { get; set; } 
    public string? amenities { get; set; } 
 
    
}
