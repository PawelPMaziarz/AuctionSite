using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionSite.Data
{
    class ProductCategoryDTO
    {
        public int IdProductCategory { get; set; }
        public string Name { get; set; }
        public ProductCategoryDTO ParentCategory { get; set; }
        public ProductCategoryDTO(ProductCategory productCategory)
        {
            IdProductCategory = productCategory.idProductCategory;
            Name = productCategory.name;
            ParentCategory = productCategory.ParentCategory == null ? null : new ProductCategoryDTO(productCategory.ParentCategory);
        }
        public override string ToString()
        {
            return Name + (ParentCategory!=null? " w "+ParentCategory.Name : "");
        }
    }
}
