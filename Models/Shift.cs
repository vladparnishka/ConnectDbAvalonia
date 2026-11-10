using System;
using System.Collections.Generic;

namespace ConnectDbAvalonia.Models;

public partial class Shift
{
    public int ShiftId { get; set; }

    public int HouseId { get; set; }

    public string ShiftName { get; set; } = null!;

    public int AvailableSeats { get; set; }

    public virtual ICollection<BookingItem> BookingItems { get; set; } = new List<BookingItem>();

    public virtual House House { get; set; } = null!;
}
