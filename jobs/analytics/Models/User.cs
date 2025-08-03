using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace analytics.Models;

[Table("user")]
public partial class User
{
    [Key]
    [Column("uid")]
    public string Uid { get; set; } = null!;

    [Required]
    [MaxLength(255)]
    public string Username { get; set; } = null!;

    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = null!;

    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();

    public virtual ICollection<Receipt> Receipts { get; set; } = new List<Receipt>();
}
