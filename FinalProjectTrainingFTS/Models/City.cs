﻿using System;
using System.Collections.Generic;

namespace FinalProjectTrainingFTS.Models;

public partial class City
{
    public string Name { get; set; } = null!;

    public string Country { get; set; } = null!;

    public int PostOffice { get; set; }

    public int Id { get; set; }

    public string Image { get; set; } = null!;

    public int? VisitCount { get; set; }

    public virtual ICollection<Hotel> Hotels { get; } = new List<Hotel>();
}
