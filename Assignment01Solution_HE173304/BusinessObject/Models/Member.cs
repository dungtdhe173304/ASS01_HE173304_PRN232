using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BusinessObject.Models;

[Table("Member")]
public partial class Member
{
    [Key]
    public int MemberId { get; set; }

    [StringLength(100)]
    public string? Email { get; set; }

    [StringLength(100)]
    public string? CompanyName { get; set; }

    [StringLength(50)]
    public string? City { get; set; }

    [StringLength(50)]
    public string? Country { get; set; }

    [StringLength(50)]
    public string? Password { get; set; }

    [InverseProperty("Member")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
