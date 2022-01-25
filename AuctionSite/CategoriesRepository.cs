using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuctionSite.Data;

namespace AuctionSite.Data
{
    class CategoriesRepository
    {
        private AuctionSiteEntities db;
        public List<ProductCategoryDTO> Categories { get; private set; }
        public CategoriesRepository()
        {
            Categories = new List<ProductCategoryDTO>();
            db = new AuctionSiteEntities();
            db.ProductCategory.ToList().ForEach(x => Categories.Add(new ProductCategoryDTO(x)));
        }
        public ProductCategoryDTO GetCategoryById(int id)
        {
            return Categories.Where(x => x.IdProductCategory == id).FirstOrDefault();
        }
        public List<ProductCategoryDTO> GetSubCategories(int idParentCategory)
        {
            return Categories.Where(x => x.ParentCategory != null && x.ParentCategory.IdProductCategory == idParentCategory).ToList();
        }
        public List<ProductCategoryDTO> GetMainCategories()
        {
            return Categories.Where(x => x.ParentCategory == null).ToList();
        }

    }
}
