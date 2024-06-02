using ApiProyectoAWS.Models;
using ApiProyectoAWS.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiProyectoAWS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventosController : ControllerBase
    {
        private RepositoryEventos repo;

        public EventosController(RepositoryEventos repo)
        {
            this.repo = repo;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetEventos()
        {
            var eventos = await repo.GetAllEventosAsync();

            return Ok(eventos);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllEventosTipo(string tipo)
        {
            var eventos = await repo.GetAllEventosTipoAsync(tipo);

            return Ok(eventos);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllEventosArtista(int iduser)
        {
            var eventos = await repo.GetAllEventosArtistaAsync(iduser);

            return Ok(eventos);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetEventosPorRecinto(int iduser)
        {
            var eventos = await repo.GetEventosPorRecintoAsync(iduser);

            return Ok(eventos);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetEventosPorFiltros([FromQuery] FiltroEvento filtro)
        {
            var eventos = await repo.GetEventosPorFiltros(filtro);

            return Ok(eventos);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetTipoEventos()
        {
            var tipoEventos = await repo.GetTipoEventosAsync();

            return Ok(tipoEventos);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> FindEvento(int id)
        {
            var evento = await repo.GetDetallesEventoAsync(id);
            if (evento == null)
                return NotFound();

            return Ok(evento);
        }

        [Authorize]
        [HttpPost("CrearEvento")]
        public async Task<IActionResult> CrearEvento([FromBody] Evento evento)
        {
            await repo.CrearEventoAsync(evento);
            return CreatedAtAction(nameof(FindEvento), new { id = evento.EventoID }, evento);

        }
    }
}
