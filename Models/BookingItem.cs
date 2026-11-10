using System;
using System.Collections.Generic;

namespace ConnectDbAvalonia.Models;

public partial class BookingItem
{
    public int BookingItemId { get; set; }

    public int BookingId { get; set; }

    public int ShiftId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Shift Shift { get; set; } = null!;
}
