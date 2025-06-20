namespace webapi.DTO;

public class CreateExpenseDto
{
    public decimal TotalAmount { get; set; }

    public DateTime? ExpenseDate { get; set; }

    public int? ReceiptId { get; set; }

    public int SupplierId { get; set; }
}