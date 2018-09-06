using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InventarioApp.Models
{
    public class Receipt
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre del beneficiario es requerido.")]
        [DisplayName("Nombre del Beneficiario")]
        public string Beneficiary { get; set; }
        [DisplayName("Fecha de Entrega")]
        public DateTime Date { get; set; }

        //public List<Delivery> Deliveries { get; set; }
    }
}
