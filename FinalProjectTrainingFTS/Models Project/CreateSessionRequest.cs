namespace FinalProjectTrainingFTS.Models;

public class CreateSessionRequest
{
    public int RoomlId { get; set; }
    public string Book_From { get; set; }
    public string Book_To { get; set; }
   // public string RoomlId { get; set; }
}