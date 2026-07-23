using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiTransporteDb.Models;
using WebApiTransporteDb.Services;

namespace WebApiTransporteDb.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TransporteController : ControllerBase
    {
        private readonly TransporteService _transporteService;

        public TransporteController(TransporteService transporteService)
        {
            _transporteService = transporteService;
        }

        // ==================== ESTACIONES ====================

        [HttpGet("estaciones")]
        public IActionResult GetEstaciones()
        {
            var estaciones = _transporteService.ObtenerEstaciones();
            return Ok(estaciones);
        }

        [HttpGet("estaciones/{codigo}")]
        public IActionResult GetEstacionPorCodigo(string codigo)
        {
            var estacion = _transporteService.BuscarEstacionPorCodigo(codigo);
            if (estacion == null)
                return NotFound(new { mensaje = "Estación no encontrada." });
            return Ok(estacion);
        }

        [HttpPost("estaciones")]
        public IActionResult CrearEstacion([FromBody] Estacion estacion)
        {
            if (string.IsNullOrWhiteSpace(estacion.Codigo) || string.IsNullOrWhiteSpace(estacion.Nombre))
                return BadRequest(new { mensaje = "El código y nombre son obligatorios." });

            var nueva = _transporteService.CrearEstacion(estacion);
            return CreatedAtAction(nameof(GetEstacionPorCodigo), new { codigo = nueva.Codigo }, nueva);
        }

        // ==================== RUTAS ====================

        [HttpGet("rutas")]
        public IActionResult GetRutas()
        {
            var rutas = _transporteService.ObtenerRutas();
            return Ok(rutas);
        }

        [HttpPost("rutas")]
        public IActionResult CrearRuta([FromBody] Ruta ruta)
        {
            if (ruta.OrigenId <= 0 || ruta.DestinoId <= 0)
                return BadRequest(new { mensaje = "Los IDs de origen y destino son obligatorios." });

            var nueva = _transporteService.CrearRuta(ruta);
            return Created($"api/transporte/rutas/{nueva.RutaId}", nueva);
        }

        // ==================== CONEXIONES ====================

        [HttpGet("conexiones/{codigo}")]
        public IActionResult GetConexiones(string codigo)
        {
            var estacion = _transporteService.BuscarEstacionPorCodigo(codigo);
            if (estacion == null)
                return NotFound(new { mensaje = "Estación no encontrada." });

            var conexiones = _transporteService.ObtenerConexiones(codigo);
            return Ok(new { estacion = estacion.Nombre, conexiones });
        }

        // ==================== CAMINO MÁS CORTO ====================

        [HttpGet("camino-corto")]
        public IActionResult GetCaminoCorto([FromQuery] string origen, [FromQuery] string destino)
        {
            var resultado = _transporteService.CalcularCaminoCorto(origen, destino);

            if (!resultado.RutaEncontrada)
                return NotFound(new { mensaje = "No se encontró una ruta posible entre las estaciones." });

            return Ok(resultado);
        }

        // ==================== PASAJEROS (COLA) ====================

        [HttpPost("pasajeros/encolar/{codigoEstacion}")]
        public IActionResult EncolarPasajero(string codigoEstacion, [FromBody] Pasajero pasajero)
        {
            if (string.IsNullOrWhiteSpace(pasajero.Nombre))
                return BadRequest(new { mensaje = "El nombre del pasajero es obligatorio." });

            var ok = _transporteService.EncolarPasajero(codigoEstacion, pasajero);
            if (!ok)
                return NotFound(new { mensaje = "Estación no encontrada." });

            return Ok(new { mensaje = $"Pasajero '{pasajero.Nombre}' encolado en estación {codigoEstacion}." });
        }

        [HttpPost("pasajeros/desencolar/{codigoEstacion}")]
        public IActionResult DesencolarPasajero(string codigoEstacion)
        {
            var pasajero = _transporteService.DesencolarPasajero(codigoEstacion);
            if (pasajero == null)
                return NotFound(new { mensaje = "No hay pasajeros en la cola o la estación no existe." });

            return Ok(new { mensaje = $"Pasajero '{pasajero.Nombre}' atendido.", pasajero });
        }

        [HttpGet("pasajeros/{codigoEstacion}")]
        public IActionResult GetPasajeros(string codigoEstacion)
        {
            var estacion = _transporteService.BuscarEstacionPorCodigo(codigoEstacion);
            if (estacion == null)
                return NotFound(new { mensaje = "Estación no encontrada." });

            var pasajeros = _transporteService.ObtenerPasajeros(codigoEstacion);
            return Ok(new { estacion = estacion.Nombre, cantidadEnEspera = pasajeros.Count, pasajeros });
        }

        // ==================== ÁRBOL BINARIO ====================

        [HttpGet("rutas/arbol")]
        public IActionResult GetRutasArbol()
        {
            var rutasOrdenadas = _transporteService.ObtenerRutasOrdenadas();
            return Ok(new { totalRutas = rutasOrdenadas.Count, rutas = rutasOrdenadas });
        }

        // ==================== REPORTES ====================

        [HttpGet("reportes/trafico")]
        public IActionResult GetReporteTrafico()
        {
            var reporte = _transporteService.GenerarReporteTrafico();
            return Ok(reporte);
        }
    }
}
