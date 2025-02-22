﻿using System;
using System.Collections.Generic;

namespace FinalProjectTrainingFTS.Models;

public partial class Room
{
    public int RoomId { get; set; }

    public int Adult { get; set; }

    public int Children { get; set; }

    public int? HotelId { get; set; }

    public string Image { get; set; } = null!;

    public string Descriptions { get; set; } = null!;

    public double Price { get; set; }

    public double DiscountedPrice { get; set; }

    public virtual ICollection<BookRoom> BookRooms { get; } = new List<BookRoom>();

    public virtual ICollection<CheckPayment> CheckPayments { get; } = new List<CheckPayment>();

    public virtual Hotel? Hotel { get; set; }
}
