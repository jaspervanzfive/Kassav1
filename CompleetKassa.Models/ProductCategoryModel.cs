﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompleetKassa.Models
{
    public class ProductCategoryModel : CategoryModel
    {
        public IList<ProductSubCategoryModel> SubCategories { get; set; }
    }
}
