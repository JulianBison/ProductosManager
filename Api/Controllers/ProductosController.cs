using Api.Contracts;
using Api.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private static List<Producto> productList = new List<Producto>()
        {
            new Producto(1, "Libro A", 25000, 1),
            new Producto(2, "Libro B", 32000, 2),
            new Producto(3, "Libro C", 12000, 3)
        };

        [HttpGet]
        public ActionResult<List<ProductoResponse>> GetAll()
        {
            var listaProductos = productList.ToList();

            if (listaProductos.Count < 2)
            {
                return Conflict("El numero de productos es menor a 2");
            }

            return Ok(listaProductos);
        }

        [HttpGet("{id}")]
        public ActionResult<List<ProductoResponse>> GetById([FromRoute] int id)
        {
            var producto = productList.FirstOrDefault(x => x.Id == id);

            if (producto == null)
            {
                return NotFound("Producto no encontrado");
            }

            return Ok(producto);
        }
        
        [HttpGet("buscar")]
        public ActionResult<List<ProductoResponse>> Search([FromQuery] string nombre)
        {
            var productos = productList.Where(x => x.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!productos.Any())
            {
                return NotFound("Productos no encontrados");
            }
            return Ok(productos);
        }

        [HttpGet("precio-minimo/{valor}")]
        public ActionResult<List<ProductoResponse>> Searchminicalprice([FromQuery] decimal valor)
        {
            var productos = productList.Where(x => x.Precio >=valor).ToList();
            return Ok(productos);
        }

        [HttpGet("total")]
        public ActionResult<List<ProductoResponse>> GetTotalProducts() 
        {
            var total = productList.Count;
            return Ok($"total de productos: {total}");
        }

        [HttpPost]
        public ActionResult<ProductoResponse> Create([FromBody] ProductoRequest producto)
        {
            if(producto is null)
            {
                return BadRequest("producto no puede ser nulo");
            }
            
            if (string.IsNullOrEmpty(producto.Nombre) || producto.Precio <= 0)
            {
                return BadRequest("nombre vacio y/o precio igual a 0.");
            }

            if (producto.Stock < 0)
            {
                return BadRequest("Stock no puede ser negativo");
            }

            producto.Id = productList.Any() ? productList.Max(x => x.Id) + 1 : 1;

            int stock = producto.Stock == null ? 10 : producto.Stock.Value;

            var newProducto = new Producto(producto.Id, producto.Nombre, producto.Precio, stock);

            productList.Add(newProducto);

            var returnProducto = new ProductoResponse()
            {
                Id = newProducto.Id,
                Nombre = newProducto.Nombre,
                Precio = newProducto.Precio
            };

            return CreatedAtAction(nameof(GetById), new { id = returnProducto.Id }, returnProducto);
        }

        [HttpPut("{id}")]
        public ActionResult Update([FromRoute] int id, [FromBody] ProductoRequest producto)
        {
            var productoExistente = productList.FirstOrDefault(x => x.Id == id);

            if (productoExistente == null)
            {
                return NotFound("Producto no encontrado");
            }

            if (string.IsNullOrEmpty(producto.Nombre) || producto.Precio <= 0)
            {
                return BadRequest("nombre vacio y/o precio igual a 0.");
            }

            productoExistente.Nombre = producto.Nombre + ".";
            productoExistente.Precio = producto.Precio;
            productoExistente.Stock = producto.Stock ?? productoExistente.Stock;



            return NoContent();
        }

        [HttpPatch("{id}")]
        public ActionResult UpdatePriceStock([FromRoute] int id, [FromBody] ProductoRequestStockPrice producto)
        {
            var productoExistente = productList.FirstOrDefault(x => x.Id == id);

            if (productoExistente == null)
            {
                return NotFound("Producto no encontrado");
            }

            if (producto.Precio < 0)
            {
                return BadRequest("precio igual o menor a 0.");
            }
            if (producto.Stock < 0)
            {
                return BadRequest("Stock no puede ser menor a 0");
            }

            productoExistente.Precio = producto.Precio ?? productoExistente.Precio;
            productoExistente.Stock = producto.Stock ?? productoExistente.Stock;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            var productoExistente = productList.FirstOrDefault(x => x.Id == id);

            if (productoExistente == null)
            {
                return NotFound("Producto no encontrado");
            }

            productList.Remove(productoExistente);

            return NoContent();
        }
    }
}
