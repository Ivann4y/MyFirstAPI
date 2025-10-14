namespace MyFirstAPI.Models.DTOs.Transaction
{
    public class TransactionReadDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<TransactionItemDetailDto> Items { get; set; } = new();
    }

    public class TransactionItemDetailDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal SubTotal { get; set; }
    }
}
