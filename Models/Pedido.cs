using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VentaZapatillas.Models
{
    public class Pedido
    {
        public int Id { get; set; }

        [Required]
        public string UsuarioId { get; set; } = string.Empty;

        public DateTime FechaPedido { get; set; } = DateTime.Now;

        public decimal Total { get; set; }

        public List<DetallePedido> Detalles { get; set; } = new();
    }
}
