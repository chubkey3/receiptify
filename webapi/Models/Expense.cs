using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapi.Models;

[Table("expense")]
public partial class Expense
{
    [Key]
    [Column("expense_id")]
    public int ExpenseId { get; set; }

    [Required]
    [Column("total_amount", TypeName = "decimal(10,2)")]
    public decimal TotalAmount { get; set; }

    [Column("expense_date")]
    public DateTime ExpenseDate { get; set; }

    [Required]
    [Column("uid")]
    public string Uid { get; set; } = null!;

    [Column("receipt_id")]
    public int? ReceiptId { get; set; }

    [Required]
    [Column("supplier_id")]
    public int SupplierId { get; set; }

    [ForeignKey(nameof(ReceiptId))]
    public virtual Receipt? Receipt { get; set; }

    [ForeignKey(nameof(SupplierId))]
    public virtual Supplier Supplier { get; set; } = null!;

    [ForeignKey(nameof(Uid))]
    public virtual User UidNavigation { get; set; } = null!;
}
