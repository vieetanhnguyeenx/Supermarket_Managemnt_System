﻿using System;
using System.Collections.Generic;

namespace SupermarketManagmentSystem.Models;

public partial class Product
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? Quantity { get; set; }

    public double? Price { get; set; }

    public int? CategoryId { get; set; }

    public virtual Category? Category { get; set; }
}
