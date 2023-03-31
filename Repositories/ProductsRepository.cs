using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SimpleMVC.Models;

namespace SimpleMVC.Repositories
{
    //測試資料
    public class ProductsRepository
    {
        private List<Product> _Products = new List<Product>()
        {
            new Product()
            {
                ProductId = 1,
                ProductName = "iPhone 14",
                Cost = 27900,
                Quantity = 100,
                UnitInStock = 50,
                Discount = false,
                LaunchDate = new DateTime(2022,9,1)
            },
            new Product()
            {
                ProductId = 2,
                ProductName = "iPhone 14 Plus",
                Cost = 31900,
                Quantity = 100,
                UnitInStock = 70,
                Discount = false,
                LaunchDate = new DateTime(2022,9,1)
            },
            new Product()
            {
                ProductId = 3,
                ProductName = "iPhone 14 Pro",
                Cost = 34900,
                Quantity = 100,
                UnitInStock = 50,
                Discount = false,
                LaunchDate = new DateTime(2022,9,1)
            },
            new Product()
            {
                ProductId = 4,
                ProductName = "iPhone 14 Pro Max",
                Cost = 38900,
                Quantity = 100,
                UnitInStock = 50,
                Discount = false,
                LaunchDate = new DateTime(2022,9,1)
            },
            new Product()
            {
                ProductId = 5,
                ProductName = "iPhone 13",
                Cost = 24900,
                Quantity = 100,
                UnitInStock = 50,
                Discount = true,
                LaunchDate = new DateTime(2022,9,1)
            },
        };

        public IEnumerable<Product> Products => _Products;

        public bool Add(Product product)
        {
            try
            {
                _Products.Add(product);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(Product product)
        {
            try
            {
                var obj = _Products.FirstOrDefault(x => x.ProductId == product.ProductId);
                if (obj != null)
                {
                    obj.ProductName = product.ProductName;
                    obj.ProductName = product.ProductName;
                    obj.Cost = product.Cost;
                    obj.Quantity = product.Quantity;
                    obj.UnitInStock = product.UnitInStock;
                    obj.Discount = product.Discount;
                    obj.LaunchDate = product.LaunchDate;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(int id)
        {
            return _Products.Remove(_Products.Single(x => x.ProductId == id));
        }
    }
}

