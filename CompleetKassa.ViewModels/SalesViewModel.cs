﻿using CompleetKassa.Models;
using CompleetKassa.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using CompleetKassa.DataTypes.Enumerations;

namespace CompleetKassa.ViewModels
{
    public class SalesViewModel : BaseViewModel
    {
        private IList<ProductModel> _dbProductList;

        private ObservableCollection<ProductCategoryModel> _categories;
        private ObservableCollection<ProductSubCategoryModel> _subCategories;

        public string DiscountValue { get; set; }

        // Multi receipt
        private ObservableCollection<PurchasedProductViewModel> _receiptList;
        public ObservableCollection<PurchasedProductViewModel> ReceiptList
        {
            get
            {
                return _receiptList;
            }
            set { SetProperty (ref _receiptList, value); }
        }

        private string _categoryFilter;
        public string CategoryFilter
        {
            get { return _categoryFilter; }
            set
            {
                SetProperty (ref _categoryFilter, value);
                ProductList.Refresh ();
            }
        }

        private string _subCategoryFilter;
        public string SubCategoryFilter
        {
            get { return _subCategoryFilter; }
            set
            {
                SetProperty (ref _subCategoryFilter, value);
                ProductList.Refresh ();
            }
        }

        private ProductSubCategoryModel _selectedSubCategory;
        public ProductSubCategoryModel SelectedSubCategory
        {
            get { return _selectedSubCategory; }
            set
            {
                if (value != null) {
                    _selectedSubCategory = value;
                    SubCategoryFilter = value.Name;
                }
            }
        }

        private ProductCategoryModel _selectedCategory;
        public ProductCategoryModel SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                _selectedCategory = value;
                CategoryFilter = value.Name;
                SetSubCategories (value.Name);
            }
        }

        private ICollectionView _productList;
        public ICollectionView ProductList
        {
            get { return _productList; }
            set { SetProperty (ref _productList, value); }
        }

        private ObservableCollection<SelectedProductViewModel> _purchasedProducts;
        public ObservableCollection<SelectedProductViewModel> PurchasedProducts
        {
            get
            {
                return _purchasedProducts;
            }
            set { SetProperty (ref _purchasedProducts, value); }
        }

        public ObservableCollection<ProductCategoryModel> Categories
        {
            get { return _categories; }
            set
            {
                SetProperty (ref _categories, value);
            }
        }

        public ObservableCollection<ProductSubCategoryModel> SubCategories
        {
            get { return _subCategories; }
            set
            {
                SetProperty (ref _subCategories, value);
            }
        }

        private PurchasedProductViewModel _currentPurchase;
        public PurchasedProductViewModel CurrentPurchase
        {
            get { return _currentPurchase; }
            set
            {
                SetProperty (ref _currentPurchase, value);
                PurchasedProducts = value.Products;
            }
        }

        private int _receiptIndex;
        public int ReceiptIndex
        {
            get { return _receiptIndex; }
            set
            {
                if (value < 0) return;

                SetProperty (ref _receiptIndex, value);
                CurrentPurchase = _receiptList[value];
            }
        }

        public SelectedProductViewModel SelectedPurchasedProduct
        {
            get; set;
        }

        #region Commands
        public ICommand OnPurchased { get; private set; }
        public ICommand OnIncrementPurchased { get; private set; }
        public ICommand OnDecrementPurchased { get; private set; }
        public ICommand OnSelectAllPurchased { get; private set; }
        public ICommand OnNewReceipt { get; private set; }
        public ICommand OnPreviousReceipt { get; private set; }
        public ICommand OnNextReceipt { get; private set; }
        public ICommand OnPay { get; private set; }
        public ICommand OnDiscountDollar { get; private set; }
        public ICommand OnDiscountPercent { get; private set; }
        public ICommand OnDeleteProducts { get; private set; }

        #endregion

        public SalesViewModel () : base ("Sales", "#FDAC94", "Icons/product.png")
        {
            //PurchasedItems = new ObservableCollection<PurchasedProductViewModel>();
            _categories = new ObservableCollection<ProductCategoryModel> ();
            _purchasedProducts = new ObservableCollection<SelectedProductViewModel> ();
            _receiptList = new ObservableCollection<PurchasedProductViewModel> ();

            _categoryFilter = string.Empty;
            _subCategoryFilter = string.Empty;

            // TODO: This is where to get data from DB
            GetProducts ();
            ProductList = CollectionViewSource.GetDefaultView (_dbProductList);
            ProductList.Filter += ProductCategoryFilter;
            ProductList.Filter += ProductSubCategoryFilter;

            // Set the first product as active category
            _categoryFilter = _categories.FirstOrDefault () == null ? string.Empty : _categories.FirstOrDefault ().Name;
            SetSubCategories (_categoryFilter);
            SelectFirstCategory ();

            CreateNewReceipt (null);

            // Commands
            OnPurchased = new BaseCommand (Puchase);
            OnIncrementPurchased = new BaseCommand (IncrementPurchase);
            OnDecrementPurchased = new BaseCommand (DecrementPurchase);
            OnSelectAllPurchased = new BaseCommand (SelectAllPurchased);
            OnNewReceipt = new BaseCommand (CreateNewReceipt);
            OnPreviousReceipt = new BaseCommand (SelectPreviousReceipt);
            OnNextReceipt = new BaseCommand (SelectNextReceipt);
            OnPay = new BaseCommand (Pay);
            OnDiscountDollar = new BaseCommand (DiscountPurchaseByDollar);
            OnDiscountPercent = new BaseCommand (DiscountPurchaseByPercent);
            OnDeleteProducts = new BaseCommand (DeleteProducts);
        }

        private bool ProductCategoryFilter (object item)
        {
            var product = item as ProductModel;
            return item == null ? true : product.Category.Contains (_categoryFilter);
        }

        private bool ProductSubCategoryFilter (object item)
        {
            var product = item as ProductModel;
            return (product.Category.Contains (_categoryFilter) &&
                product.SubCategory.Contains (_subCategoryFilter));
        }

        private void SetSubCategories (string category)
        {
            SubCategories = new ObservableCollection<ProductSubCategoryModel> (_categories.Where (x => x.Name == category).First ().SubCategories);
            SubCategoryFilter = string.Empty;
        }

        // TODO: DATABASE, Get Categories from DB
        private void GetCategories (IList<ProductModel> products)
        {
            //Dummy Colors of Categories
            String[] categories_colors = new string[] { "#D0A342", "#B422B9", "#6BB4FA", "#39985D", "#CEBA5E", "#962525", "#7E8085" };

            // TODO: Categories can be obtained from DB especially the color
            var categories = products.Select (x => x.Category).Distinct ();

            int z = 0;
            foreach (var category in categories) {

                var subCategories = products.Where (x => x.Category == category)
                                    .Select (x => x.SubCategory).Distinct ();

                var productSubCategories = new List<ProductSubCategoryModel> ();
                foreach (var subCategory in subCategories) {
                    productSubCategories.Add (new ProductSubCategoryModel
                    {
                        Name = subCategory,
                        Color = categories_colors[z]
                    });
                }

                _categories.Add (new ProductCategoryModel
                {
                    Name = category,
                    Color = categories_colors[z],
                    SubCategories = productSubCategories
                });

                // Console.WriteLine("11");

                z++;
            }


        }

        private void GetProducts ()
        {
            _dbProductList = new List<ProductModel> {
                 new ProductModel
                {
                    ID = 1,
                    Name = "Cheyene Hawk pen Purle with 25mm grip including spacersd dasdas das d ds ds dsas",
                    Image ="/CompleetKassa.ViewModels;component/Images/sample.png",
                    Price = 100.0m,
                    // Description = "This is sample 1",
                    Category = "Shoes",
                    SubCategory = "Running"
                },
                new ProductModel
                {
                    ID = 2,
                    Name = "Shoes 2",
                    Image ="/CompleetKassa.ViewModels;component/Images/sample.png",
                    Price = 20.0m,
                    // Description = "This is sample 2",
                    Category = "Shoes",
                    SubCategory = "Walking"
                },
                new ProductModel
                {
                    ID = 3,
                   Name = "Bag 1",
                    Image ="/CompleetKassa.ViewModels;component/Images/sumi_black.jpg",
                    Price = 20.0m,
                    // Description = "This is sample 2",
                    Category = "Bag",
                    SubCategory = "Shoulder Bag"
                },
                new ProductModel
                {
                    ID = 4,
                   Name = "Bag 2",
                    Image ="/CompleetKassa.ViewModels;component/Images/sample.png",
                    Price = 20.0m,
                    // Description = "This is sample 2",
                    Category = "Bag",
                    SubCategory = "Shoulder Bag"
                },
                new ProductModel
                {
                    ID = 5,
                   Name = "Belt 1",
                    Image ="/CompleetKassa.ViewModels;component/Images/sample.png",
                    Price = 10.0m,
                    // Description = "This is Belt 1",
                    Category = "Belt",
                    SubCategory = "Men's Belt"
                },
                new ProductModel
                {
                    ID = 5,
                   Name = "Belt 11",
                    Image ="/CompleetKassa.ViewModels;component/Images/sample.png",
                    Price = 10.0m,
                    // Description = "This is Belt 1",
                    Category = "Sterfilters",
                    SubCategory = "Men's BeltXX"
                },
                new ProductModel
                {
                    ID = 6,
                   Name = "Nike Shoes",
                    Image ="/CompleetKassa.ViewModels;component/Images/nike.jpg",
                    Price = 10.0m,
                    // Description = "This is Belt 1",
                    Category = "Shoes",
                    SubCategory = "Basketball"
                }
                ,
                new ProductModel
                {
                    ID =7,
                   Name = "Nike Shoes2",
                    Image ="/CompleetKassa.ViewModels;component/Images/nike.jpg",
                    Price = 10.0m,
                    // Description = "This is Belt 1",
                    Category = "Shoes",
                    SubCategory = "Basketball"
                }
                ,
                new ProductModel
                {
                    ID =7,
                   Name = "Nike Shoes2",
                    Image ="/CompleetKassa.ViewModels;component/Images/nike.jpg",
                    Price = 10.0m,
                    // Description = "This is Belt 1",
                    Category = "SkylightFilters",
                    SubCategory = "Default"
                }
                ,
                new ProductModel
                {
                    ID =7,
                   Name = "Nike Shoes2",
                    Image ="/CompleetKassa.ViewModels;component/Images/nike.jpg",
                    Price = 10.0m,
                    // Description = "This is Belt 1",
                    Category = "Filtersets",
                        SubCategory = "Default"
                }
                ,
                new ProductModel
                {
                    ID =7,
                   Name = "Nike Shoes2",
                    Image ="/CompleetKassa.ViewModels;component/Images/nike.jpg",
                    Price = 10.0m,
                    // Description = "This is Belt 1",
                    Category = "UV Filters",
                   SubCategory = "Default"
                }

            };

            GetCategories (_dbProductList);
        }

        private void SelectFirstCategory ()
        {
            if (_categories != null && 0 < _categories.Count) {
                SelectedCategory = _categories[0];
            }
        }

        private void DiscountPurchaseByPercent (object obj)
        {
            var selectedItems = _purchasedProducts.Where (x => x.IsSelected).ToList (); ;
            foreach (var item in selectedItems) {
                DiscountedProduct (item, ProductDiscountOptions.Percent);
            }
        }

        private void DiscountPurchaseByDollar (object obj)
        {
            var selectedItems = _purchasedProducts.Where (x => x.IsSelected).ToList (); ;
            foreach (var item in selectedItems) {
                DiscountedProduct (item, ProductDiscountOptions.Dollar);
            }
        }

        private void IncrementPurchase (object obj)
        {
            var selectedItems = _purchasedProducts.Where (x => x.IsSelected).ToList (); ;
            foreach (var item in selectedItems) {
                IncrementPurchasedProduct (item);
            }
        }

        private void DeleteProducts (object obj)
        {
            var selectedItems = _purchasedProducts.Where (x => x.IsSelected).ToList (); ;
            foreach (var item in selectedItems) {
                PurchasedProducts.Remove (item);
            }

            CurrentPurchase.ComputeTotal ();
        }

        private void DecrementPurchase (object obj)
        {
            var selectedItems = _purchasedProducts.Where (x => x.IsSelected).ToList ();
            foreach (var item in selectedItems) {
                DecrementPurchasedProduct (item);
            }
        }

        public void SelectAllPurchased (object obj)
        {
            foreach (var item in _purchasedProducts) {
                item.IsSelected = true;
            }
        }

        private void Puchase (object obj)
        {
            var item = (SelectedProductViewModel)obj;

            var existItem = _purchasedProducts.FirstOrDefault (x => x.ID == item.ID);
            if (existItem == null) {
                AddPurchasedProduct (item);
            }
            else {
                IncrementPurchasedProduct (existItem);
            }
        }

        // TODO: Temporary Receipt counter
        private int _receiptCounter;

        private void CreateNewReceipt (object obj)
        {
            CurrentPurchase = new PurchasedProductViewModel ();
            CurrentPurchase.Label = $"{++_receiptCounter}";
            ReceiptList.Add (CurrentPurchase);
            ReceiptIndex = ReceiptList.Count () - 1;
        }

        private void SelectPreviousReceipt (object obj)
        {
            if (ReceiptIndex == 0) return;
            ReceiptIndex--;
        }

        private void SelectNextReceipt (object obj)
        {
            if (ReceiptIndex == _receiptList.Count () - 1) return;
            ReceiptIndex++;
        }

        private void Pay (object obj)
        {
            ReceiptList.Remove (CurrentPurchase);

            if (ReceiptList.Count == 0) {
                CreateNewReceipt (obj);
            }
            else {
                SelectPreviousReceipt (obj);
            }
        }

        private void DiscountedProduct (SelectedProductViewModel product, ProductDiscountOptions option)
        {
            decimal discount = 0.0m;
            if (Decimal.TryParse (DiscountValue, out discount) == false) {
                discount = 0.0m;
            }

            if (option == ProductDiscountOptions.Dollar) {
                product.Discount = discount;
            }
            else if (option == ProductDiscountOptions.Percent) {
                product.Discount = product.Price * (discount / 100);
            }

            CurrentPurchase.ComputeTotal ();
        }

        private void DiscountedProduct (SelectedProductViewModel product)
        {
            product.Discount = 5.50m;
            CurrentPurchase.ComputeTotal ();
        }

        private void AddPurchasedProduct (SelectedProductViewModel product)
        {
            var item = new SelectedProductViewModel
            {
                Quantity = 1,
                ID = product.ID,
                Name = product.Name,
                Price = product.Price,
                Discount = product.Discount
            };

            item.ComputeSubTotal ();

            PurchasedProducts.Add (item);

            CurrentPurchase.ComputeTotal ();
        }

        private void IncrementPurchasedProduct (SelectedProductViewModel product)
        {
            product.Quantity++;
            CurrentPurchase.ComputeTotal ();
        }

        private void DecrementPurchasedProduct (SelectedProductViewModel product)
        {
            product.Quantity--;
            if (product.Quantity == 0) {
                PurchasedProducts.Remove (product);
            }

            CurrentPurchase.ComputeTotal ();
        }
    }
}
