using System;
using System.Collections.Generic;

namespace SupermarketManagmentSystem.Models;

public partial class Account
{
    public int Id { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? Phone { get; set; }

    public string? FullName { get; set; }

    public int? RoleId { get; set; }

    public virtual ICollection<Order> Orders { get; } = new List<Order>();

    public virtual Role? Role { get; set; }
}
