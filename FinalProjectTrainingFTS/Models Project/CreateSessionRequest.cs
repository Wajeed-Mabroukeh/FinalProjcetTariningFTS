namespace FinalProjectTrainingFTS.Models;

public class CreateSessionRequest
{
    public int RoomlId { get; set; }
    public DateTime Book_From { get; set; }
    public DateTime Book_To { get; set; }

}