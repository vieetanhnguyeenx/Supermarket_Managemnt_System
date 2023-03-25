using System;
using System.Collections.Generic;

namespace SupermarketManagmentSystem.Models;

public partial class Order
{
    public int Id { get; set; }

    public int? AccountId { get; set; }

    public DateTime CreatedDate { get; set; }

    public double TotalPrice { get; set; }

    public virtual Account? Account { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; } = new List<OrderDetail>();
}
