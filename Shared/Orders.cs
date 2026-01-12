namespace Shared
{
    public record class Orders
    {
        public int clientId { get; set; }
        public Guid orderId = Guid.NewGuid();
        public string paymentProvider { get; set; } 
        public List<Products> items { get; set; }
    }
    public record class Products
    {
        public int productId { get; set; }
        public int quantity { get; set; }
    }
}
