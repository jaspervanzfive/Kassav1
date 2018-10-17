﻿
using CompleetKassa.Models;
using CompleetKassa.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace CompleetKassa.ViewModels
{
    public class UsersViewModel : BaseViewModel
    {
        private IList<ProductModel> _dbProductList;

        private ObservableCollection<ProductCategoryModel> _categories;
        private ObservableCollection<ProductSubCategoryModel> _subCategories;


        private string _categoryFilter;
        public string CategoryFilter
        {
            get { return _categoryFilter; }
            set
            {
                SetProperty(ref _categoryFilter, value);
                ProductList.Refresh();
            }
        }

        private string _subCategoryFilter;
        public string SubCategoryFilter
        {
            get { return _subCategoryFilter; }
            set
            {
                SetProperty(ref _subCategoryFilter, value);
                ProductList.Refresh();
            }
        }

        private ProductSubCategoryModel _selectedSubCategory;
        public ProductSubCategoryModel SelectedSubCategory
        {
            get { return _selectedSubCategory; }
            set
            {
                if (value != null)
                {
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
                //SetSubCategories(value.Name);
            }
        }

        private ICollectionView _productList;
        public ICollectionView ProductList
        {
            get { return _productList; }
            set { SetProperty(ref _productList, value); }
        }

        private ObservableCollection<SelectedProductViewModel> _purchasedProducts;
        public ObservableCollection<SelectedProductViewModel> PurchasedProducts
        {
            get
            {
                return _purchasedProducts;
            }
            set { SetProperty(ref _purchasedProducts, value); }
        }

        public ObservableCollection<ProductCategoryModel> Categories
        {
            get { return _categories; }
            set
            {
                SetProperty(ref _categories, value);
            }
        }

        public ObservableCollection<ProductSubCategoryModel> SubCategories
        {
            get { return _subCategories; }
            set
            {
                SetProperty(ref _subCategories, value);
            }
        }
       
        
         public UsersViewModel() : base("Users", "#355C7D", "Icons/users.png")
           {
            _categories = new ObservableCollection<ProductCategoryModel>();

            _categoryFilter = string.Empty;
            _subCategoryFilter = string.Empty;

            // TODO: This is where to get data from DB
            GetProducts();
            ProductList = CollectionViewSource.GetDefaultView(_dbProductList);
            ProductList.Filter += ProductCategoryFilter;
            ProductList.Filter += ProductSubCategoryFilter;


            // Set the first product as active category
            _categoryFilter = _categories.FirstOrDefault() == null ? string.Empty : _categories.FirstOrDefault().Name;
            SelectFirstCategory();
        }
        private bool ProductCategoryFilter(object item)
        {
            var product = item as ProductModel;
            return item == null ? true : product.Category.Contains(_categoryFilter);
        }

        private bool ProductSubCategoryFilter(object item)
        {
            var product = item as ProductModel;
            return (product.Category.Contains(_categoryFilter) &&
                product.SubCategory.Contains(_subCategoryFilter));
        }
        private void GetCategories(IList<ProductModel> products)
        {
            //Dummy Colors of Categories
            String[] categories_colors = new string[] { "#D0A342", "#B422B9", "#6BB4FA", "#39985D", "#B422B9" };

            // TODO: Categories can be obtained from DB especially the color
            var categories = products.Select(x => x.Category).Distinct();

            int z = 0;
            foreach (var category in categories)
            {

                var subCategories = products.Where(x => x.Category == category)
                                    .Select(x => x.SubCategory).Distinct();

                var productSubCategories = new List<ProductSubCategoryModel>();
                foreach (var subCategory in subCategories)
                {
                    productSubCategories.Add(new ProductSubCategoryModel
                    {
                        Name = subCategory,
                        Color = categories_colors[z]
                    });
                }

                _categories.Add(new ProductCategoryModel
                {
                    Name = category,
                    Color = categories_colors[z],
                    SubCategories = productSubCategories
                });

                // Console.WriteLine("11");

                z++;
            }


        }

        private void GetProducts()
        {
            _dbProductList = new List<ProductModel> {
                 new ProductModel
                {
                    ID = 1,
                    Name = "Cheyene Hawk pen Purle with 25mm grip including spacersd dasdas das d ds ds dsas",
                    Image ="/CompleetKassa.ViewModels;component/Images/sample.png",
                    Price = 100.0m,
                    //Description = "This is sample 1",
                    Category = "Superadmin",
                    SubCategory = "Running"
                },
                new ProductModel
                {
                    ID = 2,
                    Name = "Shoes 2",
                    Image ="/CompleetKassa.ViewModels;component/Images/sample.png",
                    Price = 20.0m,
                    //Description = "This is sample 2",
                    Category = "Middle Management",
                    SubCategory = "Walking"
                },
                new ProductModel
                {
                    ID = 3,
                    Name = "Bag 1",
                    Image ="/CompleetKassa.ViewModels;component/Images/sumi_black.jpg",
                    Price = 20.0m,
                    //Description = "This is sample 2",
                    Category = "Shop keepers",
                    SubCategory = "Shoulder Bag"
                },
                new ProductModel
                {
                    ID = 5,
                    Name = "Belt 1",
                    Image ="/CompleetKassa.ViewModels;component/Images/sample.png",
                    Price = 10.0m,
                    //Description = "This is Belt 1",
                    Category = "Cashiers",
                    SubCategory = "Men's Belt"
                }
               ,
                new ProductModel
                {
                    ID = 5,
                    Name = "Belt 1",
                    Image ="/CompleetKassa.ViewModels;component/Images/sample.png",
                    Price = 10.0m,
                    //Description = "This is Belt 1",
                    Category = "Book Keepers",
                    SubCategory = "Men's Belt"
                }


            };

            GetCategories(_dbProductList);
        }

        private void SelectFirstCategory()
        {
            if (_categories != null && 0 < _categories.Count)
            {
                SelectedCategory = _categories[0];
            }
        }
        
    }
}
