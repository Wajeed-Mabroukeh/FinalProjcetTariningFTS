using System;
using System.Collections.Generic;

namespace FinalProjectTrainingFTS.Models;

public partial class Hotel
{
    public string Name { get; set; } = null!;

    public int StarRate { get; set; }

    public string Owner { get; set; } = null!;

    public int Id { get; set; }

    public int? CityId { get; set; }

    public string Image { get; set; } = null!;

    public string Descriptions { get; set; } = null!;

    public string? Amenities { get; set; }

    public string? Latitude { get; set; }

    public string? Longitude { get; set; }

    public virtual City? City { get; set; }
    
    public virtual ICollection<Room> Rooms { get; } = new List<Room>();
}
