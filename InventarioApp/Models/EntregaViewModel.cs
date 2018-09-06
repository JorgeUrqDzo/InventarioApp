using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventarioApp.Models
{
    public class EntregaViewModel
    {
        public string Beneficiario { get; set; }

        public ICollection<Product> Products { get; set; }

        public DateTime Fecha { get; set; }
    }
}
