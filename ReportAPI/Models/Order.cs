namespace KafkaConsumer.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid ProdcutId { get; set; }
        public string Description { get; set; }
    }
}
