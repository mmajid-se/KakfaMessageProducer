namespace KafkaConsumer.Models
{
    public class Inventory
    {
        public Guid Id { get; set; }
        public Guid ProdcutId { get; set; }
        public int Quantity { get; set; }
    }
}
