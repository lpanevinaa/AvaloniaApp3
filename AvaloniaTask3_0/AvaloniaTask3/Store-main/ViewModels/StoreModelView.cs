using ReactiveUI;
using StoreApp.Models;
using System;

namespace StoreApp.ViewModels
{
	public class StoreModelView : ViewModelBase
	{
		private double _x;
		private double _y;
        public Store Store { get; }

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


        public string Name => Store.Name;

		public StoreModelView(Store store)
		{
			Store = store;
			X = store.X;
			Y = store.Y;
		}

        public void MoveTo(double newX, double newY)
        {
            X = newX;
            Y = newY;
        }

        public bool IsWithinArea(double areaX, double areaY, double areaWidth, double areaHeight)
        {
            return X >= areaX && X <= areaX + areaWidth && Y >= areaY && Y <= areaY + areaHeight;
        }
    }
}
