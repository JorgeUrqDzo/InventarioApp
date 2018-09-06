using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventarioApp.Models
{
    public class DonacionesModelView
    {

        public string Product { get; set; }
        public int  Qty { get; set; }
        public DateTime Date { get; set; }
        public string DonorName { get; set; }

        public List<DonacionesModelView> DonationsList { get; set; }
    }
}
