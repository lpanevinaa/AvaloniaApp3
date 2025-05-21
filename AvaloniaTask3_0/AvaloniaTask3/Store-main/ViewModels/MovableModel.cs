using ReactiveUI;
using StoreApp.Models;
using System;

namespace StoreApp.ViewModels
{
    public class MovableCustomerViewModel : ViewModelBase
    {
        private double _x;
        private double _y;
        private double _speed = 5.0;
        private double _maxSpeed = 10.0;
        private double _acceleration = 1.0;
        private bool _isMoving = false;
        public Customer Customer { get; }

        public double X
        {
            get => _x;
            set => this.RaiseAndSetIfChanged(ref _x, value);
        }

        public double Y
        {
            get => _y;
            set => this.RaiseAndSetIfChanged(ref _y, value);
        }

        public string Name => Customer.Name;

        public double Speed
        {
            get => _speed;
            set => this.RaiseAndSetIfChanged(ref _speed, value);
        }

        public double MaxSpeed
        {
            get => _maxSpeed;
            set => this.RaiseAndSetIfChanged(ref _maxSpeed, value);
        }

        public double Acceleration
        {
            get => _acceleration;
            set => this.RaiseAndSetIfChanged(ref _acceleration, value);
        }

        public bool IsMoving
        {
            get => _isMoving;
            set => this.RaiseAndSetIfChanged(ref _isMoving, value);
        }

        public MovableCustomerViewModel(Customer customer)
        {
            Customer = customer;
            X = customer.X;
            Y = customer.Y;
        }

        public void MoveTowards(double targetX, double targetY)
        {
            double dx = targetX - X;
            double dy = targetY - Y;
            double dist = Math.Sqrt(dx * dx + dy * dy);

            if (dist < Speed) return;

            X += Speed * dx / dist;
            Y += Speed * dy / dist;
            IsMoving = true;
        }

        public void Accelerate()
        {
            if (Speed < MaxSpeed)
            {
                Speed += Acceleration;
            }
        }

        public void Decelerate()
        {
            if (Speed > 0)
            {
                Speed -= Acceleration;
            }
        }

        public bool IsNear(double targetX, double targetY)
        {
            double dx = targetX - X;
            double dy = targetY - Y;
            return Math.Sqrt(dx * dx + dy * dy) < 10.0;
        }

        public void Stop()
        {
            Speed = 0;
            IsMoving = false;
        }
    }
}
