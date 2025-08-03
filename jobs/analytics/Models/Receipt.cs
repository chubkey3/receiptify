using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace analytics.Models;

[Table("receipt")]
public partial class Receipt
{
    [Key]
    [Column("receipt_id")]
    public int ReceiptId { get; set; }

    [Column("upload_date")]
    public DateTime? UploadDate { get; set; }

    [Required, MaxLength(255)]
    [Column("receipt_url")]
    public string ReceiptUrl { get; set; } = null!;

    [Column("uploaded_by")]
    public string? UploadedBy { get; set; }

    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();

    [ForeignKey(nameof(UploadedBy))]
    public virtual User? UploadedByNavigation { get; set; }
}
