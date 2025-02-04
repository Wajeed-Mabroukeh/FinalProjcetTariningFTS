using System;
using System.Collections.Generic;

namespace FinalProjectTrainingFTS.Models;

public partial class CheckPayment
{
    public string PaymentId { get; set; }

    public int RoomId { get; set; }
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateTime BookFrom { get; set; }

    public DateTime BookTo { get; set; }

    public virtual Room Room { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
