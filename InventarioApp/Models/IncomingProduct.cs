using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InventarioApp.Models
{
    public class IncomingProduct
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Código de producto.")]
        public int ProductId { get; set; }

        [DisplayName("Cantidad de producto.")]
        public int Quantity { get; set; }

        [DisplayName("Fecha de entrada")]
        [Required(ErrorMessage = "La fecha de entrada es requerida.")]
        public DateTime IncomingDate { get; set; }

        [DisplayName("Código de producto.")]
        public Product Porduct { get; set; }
    }
}
