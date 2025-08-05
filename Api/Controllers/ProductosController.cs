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
            new Producto(1, "Libro A", 25000),
            new Producto(2, "Libro B", 32000),
            new Producto(3, "Libro C", 12000)
        };

        [HttpGet]
        public ActionResult<List<Producto>> GetAll()
        {
            var listaProductos = productList.ToList();

            if (listaProductos.Count < 2)
            {
                return Conflict("El numero de productos es menor a 2");
            }

            return Ok(listaProductos);
        }

        [HttpGet("{id}")]
        public ActionResult<List<Producto>> GetById([FromRoute] int id)
        {
            var producto = productList.FirstOrDefault(x => x.Id == id);

            if (producto == null)
            {
                return NotFound("Producto no encontrado");
            }

            return Ok(producto);
        }

        [HttpPost]
        public ActionResult<ProductoResponse> Create([FromBody] ProductoRequest producto)
        {
            if (string.IsNullOrEmpty(producto.Nombre) || producto.Precio <= 0)
            {
                return BadRequest("nombre vacio y/o precio igual a 0.");
            }

            if (productList.Any())
            {
                producto.Id = productList.Max(x => x.Id) + 1;
            }
            else
            {
                producto.Id = 1;
            }

            var newProducto = new Producto(producto.Id, producto.Nombre, producto.Precio);

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

            productoExistente.Nombre = producto.Nombre;
            productoExistente.Precio = producto.Precio;

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
