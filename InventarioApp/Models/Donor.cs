using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InventarioApp.Models
{
    public class Donor
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Este campo es requerido.")]
        [DisplayName("Nombre(s)")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Este campo es requerido.")]
        [DisplayName("Apellido Paterno")]
        public string LastName { get; set; }
        [DisplayName("Apellido Materno")]
        public string MotherLastName { get; set; }

        public string Fullname
        {
            get
            {
                return string.Format("{0} {1} {2}", Name, LastName, MotherLastName);
            }
        }

        //public List<Donations> Donations { get; set; }
    }
}
