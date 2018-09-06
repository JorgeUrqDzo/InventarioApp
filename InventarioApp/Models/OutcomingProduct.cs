using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InventarioApp.Models
{
    public class OutcomingProduct
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Código de producto.")]
        public int ProductId { get; set; }

        [DisplayName("Cantidad de producto.")]
        public int Quantity { get; set; }

        [DisplayName("Fecha de salida")]
        [Required(ErrorMessage = "La fecha de salida es requerida.")]
        public DateTime OutcomingDate { get; set; }

        [DisplayName("Código de producto.")]
        public Product Porduct { get; set; }
    }
}
