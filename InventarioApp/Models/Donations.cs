using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventarioApp.Models
{
    public class Donations
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int DonorId { get; set; }
        public int Quantity { get; set; }
        public DateTime DonationDate { get; set; }

        //public Product Product { get; set; }
        //public Donor Donor { get; set; }
    }
}
