using System;
using System.Collections.Generic;

namespace ConnectDbAvalonia.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public DateOnly BookingDate { get; set; }

    public int ClientId { get; set; }

    public virtual ICollection<BookingItem> BookingItems { get; set; } = new List<BookingItem>();

    public virtual User Client { get; set; } = null!;
}
