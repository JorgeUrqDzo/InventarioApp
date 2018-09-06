using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventarioApp.Models
{
    public class ProductsViewModel
    {
        public IEnumerable<Product> products { get; set; }
        public double CantTotalProductos { get; set; }

        public Product product { get; set; }
    }
}
