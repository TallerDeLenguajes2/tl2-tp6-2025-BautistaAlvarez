using Microsoft.AspNetCore.Mvc;
using tl2_tp7_2025_BautistaAlvarez.Models;

namespace tl2_tp7_2025_BautistaAlvarez.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductoController : ControllerBase
    {
        private ProductoRepository productoRepository;//agrego el repositorio al controller
        public ProductoController()//constructor
        {
            productoRepository = new ProductoRepository();//lo inicio
        }

        [HttpPost]
        public IActionResult CrearNuevoProducto(Productos producto)
        {
            if (productoRepository.ExisteProducto(producto.IdProducto))//verifico si existe el id que quiero ingresar en la base de dato
            {
                return BadRequest("Id ocupado, ingrese otro");//si ya existe el id retorno
            }
            productoRepository.CrearNuevoProducto(producto);
            return Created($"/api/producto/{producto.IdProducto}", producto);//indico el id del producto creado
        }

        [HttpPut("{id}")]
        public IActionResult ModificarNombreProducto(int id, Productos producto)
        {
            if (!productoRepository.ExisteProducto(id))//verifico si existe el producto
            {
                return BadRequest("Id del producto inexistente");//sino existe retorno
            }
            productoRepository.ModificarProductoExistente(id, producto);
            return Ok(producto);
        }

        [HttpGet]
        public IActionResult ListarProductosExistentes()
        {
            var lista = productoRepository.ListarTodosLosProductos();
            return Ok(lista);
        }
        [HttpGet("{id}")]
        public IActionResult ObtenerDetallesProductoPorId(int id)
        {
            if (!productoRepository.ExisteProducto(id))//verifico si existe
            {
                return BadRequest("No existe el producto");//sino existe retorno
            }
            var producto = productoRepository.ObtenerDetalleProductoPorId(id);
            return Ok(producto);
        }
        [HttpDelete]
        public IActionResult EliminarProductoPorId(int id)
        {
            if (!productoRepository.ExisteProducto(id))//verifico si existe
            {
                return BadRequest("No existe el producto");//sino existe retorno
            }
            productoRepository.EliminarProductoPorId(id);
            return Ok(productoRepository.ListarTodosLosProductos());//devuelvo la lista para ver que se borro el producto
        }

    }
}