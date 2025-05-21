using System;

namespace StoreApp.Models
{
    public class DeliveryService : IDeliveryService
    {
        public void DeliverProduct(Product product, Store store)
        {
            Console.WriteLine($"Доставка: товар {product.Name} теперь доступен в магазине {store.Name}");
            store.DeliverProduct(product);
        }
    }
}