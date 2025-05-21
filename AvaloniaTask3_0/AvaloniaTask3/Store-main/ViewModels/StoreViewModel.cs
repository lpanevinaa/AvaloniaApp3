using ReactiveUI;
using StoreApp.Models;
using StoreApp.Helpers;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using System.Windows.Input;
using System;

namespace StoreApp.ViewModels
{
    public class StoreViewModel : ViewModelBase
    {
        public ObservableCollection<Store> Stores { get; } = new();
        public ObservableCollection<MovableCustomerViewModel> Customers { get; } = new();
        public ObservableCollection<string> EventLog { get; } = new();

        public ObservableCollection<object> Models { get; } = new();

        public ICommand AddStoreCommand { get; }
        public ICommand AddCustomerCommand { get; }

        private readonly Timer _timer;

        public StoreViewModel()
        {
            AddStoreCommand = ReactiveCommand.Create(AddStore);
            AddCustomerCommand = ReactiveCommand.Create(AddCustomer);

            _timer = new Timer(100);
            _timer.Elapsed += (_, _) => MoveCustomers();
            _timer.Start();
        }

        private void AddStore()
        {
            if (Stores.Count >= 2) return;
            var store_1 = new Store($"Овощная лавка", 100, 100);
            store_1.Products.Add(new Product("Огурец свежий", 10, 3));
            store_1.Products.Add(new Product("Помидор черри", 20, 2));
            store_1.Products.Add(new Product("Перец болгарский", 15, 4));
            store_1.Products.Add(new Product("Укроп", 30, 1));
            var storeVm1 = new StoreModelView(store_1);
            store_1.ProductSold += OnProductSold;
            store_1.OutOfStock += OnOutOfStock;
            store_1.ProductDelivery += OnProductDelivered;

            var store_2 = new Store($"Чайная лавка", 400, 100); 
            store_2.Products.Add(new Product("Чай зелёный листовой", 12, 5));
            store_2.Products.Add(new Product("Кофе растворимый", 5, 10));
            store_2.Products.Add(new Product("Кофе молотый", 25, 3));
            store_2.Products.Add(new Product("Чай чёрный листовой", 18, 4));
            var storeVm2 = new StoreModelView(store_2);
            store_2.ProductSold += OnProductSold;
            store_2.OutOfStock += OnOutOfStock;
            store_2.ProductDelivery += OnProductDelivered;

            ReflectionHelper.PrintObjectInfo(store_1);
            ReflectionHelper.PrintObjectInfo(store_2);

            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                Stores.Add(store_1);
                Stores.Add(store_2);
                Models.Add(storeVm1);
                Models.Add(storeVm2);
            });
        }

        private void AddCustomer()
        {
            var random = new Random();
            var customer = new Customer($"Покупатель {Customers.Count + 1}", random.Next(50, 400), random.Next(50, 400));
            var vm = new MovableCustomerViewModel(customer);

            ReflectionHelper.PrintObjectInfo(customer);

            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                Customers.Add(vm);
                Models.Add(vm);
            });
            var store = Stores.FirstOrDefault();
            store?.TrySellProduct(customer);
        }



        private async void MoveCustomers()
        {
            foreach (var customer in Customers.ToList())
            {
                if (!Stores.Any()) continue;

                var nearestStore = Stores.OrderBy(s => Distance(customer.X, customer.Y, s.X, s.Y)).First();

                customer.MoveTowards(nearestStore.X, nearestStore.Y);

                if (customer.IsNear(nearestStore.X, nearestStore.Y))
                {
                    await customer.Customer.BuyAsync(nearestStore);

                    Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                    {
                        Customers.Remove(customer);
                        if (Models.Contains(customer))
                        {
                            Models.Remove(customer);
                        }
                    });
                }
            }
        }

        private double Distance(double x1, double y1, double x2, double y2)
        {
            double dx = x1 - x2;
            double dy = y1 - y2;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private void OnProductSold(object? sender, ProductEventArgs e)
        {
            if (sender == null || e == null || e.Product == null || e.Customer == null)
            {
                Console.WriteLine("Один из объектов равен null:");
                Console.WriteLine($"sender: {sender}");
                Console.WriteLine($"e: {e}");
                Console.WriteLine($"e.Product: {e?.Product}");
                Console.WriteLine($"e.Customer: {e?.Customer}");
                return;
            }

            if (EventLog == null)
            {
                Console.WriteLine("EventLog равен null");
                return;
            }

            EventLog.Insert(0, $"{e.Customer.Name} приобрёл {e.Product.Name}");
        }

        private void OnOutOfStock(object? sender, EventArgs e)
        {
            EventLog.Insert(0, "Товар кончился. Доставка уже отправлена.");
        }

        private void OnProductDelivered(object? sender, ProductEventArgs e)
        {
            if (e.Product == null) return;
            EventLog.Insert(0, $"Товар \"{e.Product.Name}\" доставлен.");
        }
    }
}