using System;
using System.Collections.Generic;

namespace ConnectDbAvalonia.Models;

public partial class House
{
    public int HouseId { get; set; }

    public string HouseName { get; set; } = null!;

    public int CategoryId { get; set; }

    public string? ImagePath { get; set; }

    public decimal BasePrice { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Shift> Shifts { get; set; } = new List<Shift>();
}
