using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InventarioApp.Models
{
    public class Delivery
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int OutcommingId { get; set; }
        public int ReceiptId { get; set; }
        public int Cantidad { get; set; }

        //public Receipt Receipt { get; set; }
        //public OutcomingProduct Outcoming { get; set; }
        //public Product Product { get; set; }
    }
}
