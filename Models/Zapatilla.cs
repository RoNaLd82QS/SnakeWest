using System.ComponentModel.DataAnnotations;

namespace VentaZapatillas.Models
{
    public class Zapatilla
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(500)]
        public string Descripcion { get; set; }

        [Required]
        public decimal Precio { get; set; }

        [Required]
        public int Stock { get; set; }

        [StringLength(10)]
        public string Talla { get; set; }

        [StringLength(50)]
        public string Marca { get; set; }

        public string ImagenUrl { get; set; }
    }
}
