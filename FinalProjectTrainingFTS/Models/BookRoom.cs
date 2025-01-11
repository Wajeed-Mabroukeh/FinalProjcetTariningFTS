using System;
using System.Collections.Generic;

namespace FinalProjectTrainingFTS.Models;

public partial class BookRoom
{
    public int? RoomId { get; set; }

    public int UserId { get; set; }

    public DateTime BookFrom { get; set; }

    public DateTime BookTo { get; set; }

    public int Id { get; set; }

    public virtual Room? Room { get; set; }

   }
