namespace webapi.DTO;

public class CreateExpenseDto
{
    public decimal TotalAmount { get; set; }

    public DateTime ExpenseDate { get; set; }

    public string Uid { get; set; } = null!;

    public int? ReceiptId { get; set; }

    public int SupplierId { get; set; }
}