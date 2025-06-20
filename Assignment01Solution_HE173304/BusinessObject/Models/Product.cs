using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BusinessObject.Models;

[Table("Product")]
public partial class Product
{
    [Key]
    public int ProductId { get; set; }

    public int? CategoryId { get; set; }

    [StringLength(100)]
    public string? ProductName { get; set; }

    [StringLength(50)]
    public string? Weight { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? UnitPrice { get; set; }

    public int? UnitsInStock { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Products")]
    public virtual Category? Category { get; set; }

    [InverseProperty("Product")]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
