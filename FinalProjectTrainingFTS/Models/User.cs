using System;
using System.Collections.Generic;

namespace FinalProjectTrainingFTS.Models;

public partial class User
{
    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int Id { get; set; }

    public string? VisitedHotels { get; set; }

    public virtual ICollection<BookRoom> BookRooms { get; } = new List<BookRoom>();
}
