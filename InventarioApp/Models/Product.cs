using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InventarioApp.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El código del producto es requerido.")]
        [StringLength(50)]
        [DisplayName("Código de producto")]
        public string Code { get; set; }

        [Required(ErrorMessage = "El nombre del producto es requerido.")]
        [StringLength(100)]
        [DisplayName("Nombre del producto")]
        public string Name { get; set; }

        [DisplayName("Cantidad Existente")]
        public int Total { get; set; }
        public bool IsRemoved { get; set; }

        [DisplayName("Imagen")]
        public string Image { get; set; }

        public string Fullname
        {
            get
            {
                return string.Format("{0} - {1} ", Code, Name);
            }
        }

        public List<IncomingProduct> IncomingProducts { get; set; }
        public List<OutcomingProduct> OutcomingProducts { get; set; }
        public List<Donations> Donations { get; set; }
    }
}
