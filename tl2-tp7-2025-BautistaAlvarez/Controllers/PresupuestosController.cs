using Microsoft.AspNetCore.Mvc;
using tl2_tp7_2025_BautistaAlvarez.Models;

namespace tl2_tp7_2025_BautistaAlvarez.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PresupuestosController : ControllerBase
    {
        private PresupuestosRepository presupuestosRepository;//repositorio del presupuesto
        public PresupuestosController()//constructor
        {
            presupuestosRepository = new PresupuestosRepository();//inicio
        }

        [HttpPost]
        public IActionResult CrearPresupuesto(Presupuestos presupuesto)
        {
            if (presupuestosRepository.ExistePresupuesto(presupuesto.IdPresupuesto))//verifico
            {
                return BadRequest("Ya existe un presupuesto con ese id");//si existe un presupuesto con el id que quiero ingresar retorno
            }
            presupuestosRepository.CrearPresupuesto(presupuesto);
            return Ok(presupuesto);
        }
        [HttpPost("{idPresupuesto}/ProductoDetalle")]
        public IActionResult AgregarUnProductoExistenteYCantidad(int idPresupuesto, int idProducto, int cantidad)
        {
            if (!presupuestosRepository.ExistePresupuesto(idPresupuesto) || !presupuestosRepository.ExisteProducto(idProducto)) //verifico si existen el producto y repositorio por su id
            {
                return BadRequest("Presupuesto o Producto invalido");
            }
            presupuestosRepository.AgregarProducto(idPresupuesto, idProducto, cantidad);
            return Ok(presupuestosRepository.ListarPresupuesto());
        }
        [HttpGet("{id}")]
        public IActionResult ObtenerDetallePresupuestoPorId(int id)
        {
            if (!presupuestosRepository.ExistePresupuesto(id))//sino existe presupuesto retorno, verifico por su id
            {
                return BadRequest("No existe el presupuesto");
            }
            var presupuesto = presupuestosRepository.PresupuestoPorId(id);
            return Ok(presupuesto);
        }
        [HttpGet]
        public IActionResult ListarPresupuestoExistentes()
        {
            return Ok(presupuestosRepository.ListarPresupuesto());
        }
        [HttpDelete("{id}")]
        public IActionResult EliminarPresupuestoPorId(int id)
        {
            if (!presupuestosRepository.ExistePresupuesto(id))//verifico si existe el presupuesto por su id
            {
                return BadRequest("No existe presupuesto con ese id");
            }
            presupuestosRepository.EliminarPresupuestoPorId(id);
            return Ok(presupuestosRepository.ListarPresupuesto());
        }
    }
}