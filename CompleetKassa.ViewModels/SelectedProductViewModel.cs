﻿using System.Collections.ObjectModel;
using CompleetKassa.DataTypes.Enumerations;

namespace CompleetKassa.ViewModels
{
    public class SelectedProductViewModel : BaseViewModel
    {
        public int ID { get; set; }
        public decimal Price { get; set; }

		private ProductDiscountOptions _discountOption;
		public ProductDiscountOptions DiscountOption
		{
			get
			{
				return _discountOption;
			}

			set { SetProperty (ref _discountOption, value); }
		}

		private decimal _subTotal;
        public decimal SubTotal
        {
            get
            {
                return _subTotal;
            }

            set { SetProperty(ref _subTotal, value); }
        }

        private int _quantity;
        public int Quantity
        {
            get { return _quantity; }

            set {
                SetProperty(ref _quantity, value);
                ComputeSubTotal();
            }
        }

        private decimal _discount;
        public decimal Discount
        {
            get { return _discount; }

            set
            {
                SetProperty(ref _discount, value);
                ComputeSubTotal();
            }
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }
 
        public SelectedProductViewModel() : base(string.Empty, string.Empty, string.Empty)
        {
            ID = 0;
            Name = string.Empty;
            Price = 0.0m;
            Quantity = 0;
            Discount = 0.0m;

            IsSelected = false;
        }

		public void ComputeSubTotal()
        {
            SubTotal = (Price - Discount) * Quantity;
        }
    }
}
