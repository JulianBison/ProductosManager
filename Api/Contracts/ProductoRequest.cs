using Api.Entities;

namespace Api.Contracts
{
    public class ProductoRequest
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int? Stock { get; set; }
    }
}
