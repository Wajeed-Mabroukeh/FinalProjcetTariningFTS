using System;
using System.Collections.Generic;

namespace FinalProjectTrainingFTS.Models;

public partial class Hotel
{
    public string Name { get; set; } = null!;

    public int StarRate { get; set; }

    public string Owner { get; set; } = null!;

    public int Id { get; set; }

    public int CityId { get; set; }

    public string Image { get; set; } = null!;

    public string Descriptions { get; set; } = null!;

    public virtual City City { get; set; } = null!;

    public virtual ICollection<Room> Rooms { get; } = new List<Room>();
}
