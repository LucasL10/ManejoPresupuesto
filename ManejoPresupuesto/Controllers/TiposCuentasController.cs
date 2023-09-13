using ManejoPresupuesto.Models;
using ManejoPresupuesto.Services;
using Microsoft.AspNetCore.Mvc;

namespace ManejoPresupuesto.Controllers
{
    public class TiposCuentasController:Controller
    {
        private readonly IRepositorioTiposCuentas repositorioTiposCuentas;
        private readonly IServicioUsuarios servicioUsuarios;

        public TiposCuentasController(IRepositorioTiposCuentas repositorioTiposCuentas,
            IServicioUsuarios servicioUsuarios)
        {
            this.repositorioTiposCuentas = repositorioTiposCuentas;
            this.servicioUsuarios = servicioUsuarios;
        }
    
        public async Task<IActionResult>Index()
        {
            var idUsuario = servicioUsuarios.ObtenerIdUsuario();
            var tiposCuenta = await repositorioTiposCuentas.Obtener(idUsuario);
            return View(tiposCuenta);
        }

        public IActionResult Crear()
        {
                
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Crear(TiposCuentas tiposCuenta) 
        {
            //CUANDO INGRESA MAL UN VALOR RETORNA LOS VALORES INGRESADOS
            //CON LA VISTA PARA NO VOLVER A CARGARLOS
            if(!ModelState.IsValid) 
            {

            return View(tiposCuenta);

            }
            tiposCuenta.IdUsuario = servicioUsuarios.ObtenerIdUsuario();

            //Validacion para retornar por pantalla
            //que el Nombre ingresado ya existe

            var yaExisteTiposCuenta = await repositorioTiposCuentas.Existe(tiposCuenta.Nombre, tiposCuenta.IdUsuario);

            if (yaExisteTiposCuenta)
            {
                ModelState.AddModelError(nameof(tiposCuenta.Nombre),
                    $"El Nombre {tiposCuenta.Nombre} ya existe.");
                return View(tiposCuenta);
            }

            await repositorioTiposCuentas.Crear(tiposCuenta);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Editar(int idTipoCuenta)
        {
            var idUsuario = servicioUsuarios.ObtenerIdUsuario();
            var tiposCuenta = await repositorioTiposCuentas.ObtenerPorId(idTipoCuenta, idUsuario);

            if (tiposCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(tiposCuenta);
        }

        [HttpPost]
        public async Task<ActionResult> Editar(TiposCuentas tiposCuenta)
        {
            //idUsuario siempre se obtiene de servicioUsuario, asi no damos
            //la posibilidad de que el usuario ingrese
            //con id de administrador y solo use el id que le corresponde
            var idUsuario = servicioUsuarios.ObtenerIdUsuario();
            var tiposCuentaExiste = await repositorioTiposCuentas.ObtenerPorId(tiposCuenta.IdTipoCuenta, idUsuario);

            if (tiposCuentaExiste is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            await repositorioTiposCuentas.Actualizar(tiposCuenta);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Borrar(int idTipoCuenta)
        {
            var idUsuario = servicioUsuarios.ObtenerIdUsuario();
            var tiposCuenta = await repositorioTiposCuentas.ObtenerPorId(idTipoCuenta, idUsuario);
            if (tiposCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(tiposCuenta);
        }
        [HttpPost]
        public async Task<IActionResult>BorrarTipoCuenta(int idTipoCuenta)
        {
            var idUsuario = servicioUsuarios.ObtenerIdUsuario();
            var tiposCuenta = await repositorioTiposCuentas.ObtenerPorId(idTipoCuenta, idUsuario);
            if (tiposCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
           await repositorioTiposCuentas.Borrar(idTipoCuenta);
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> VerificarExisteTipoCuenta(string nombre)
        {
            var idUsuario = servicioUsuarios.ObtenerIdUsuario();
            var yaExisteTiposCuenta = await repositorioTiposCuentas.Existe(nombre, idUsuario);

            if (yaExisteTiposCuenta)
            {
                return Json($"El Nombre {nombre} ya existe");
            }
            return Json(true);
        
              
        }
    }
}
