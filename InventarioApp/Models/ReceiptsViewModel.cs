using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventarioApp.Models
{
    public class ReceiptsViewModel
    {
        public Receipt Recibo { get; set; }
        public List<Product> productos { get; set; }

        public string Fecha { get; set; }
    }
}
