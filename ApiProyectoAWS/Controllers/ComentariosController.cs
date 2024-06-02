using ApiProyectoAWS.Models;
using ApiProyectoAWS.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiProyectoAWS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComentariosController : ControllerBase
    {
        private RepositoryEventos repo;

        public ComentariosController(RepositoryEventos repo)
        {
            this.repo = repo;
        }

        [Authorize]
        [HttpPost("[action]")]
        public async Task<IActionResult> AddComentario([FromBody] Comentario comentario)
        {
            await repo.AddComentarioAsync(comentario);

            //return CreatedAtAction(nameof(FindEvento), new { id = comentario.EventoID });
            return Ok(comentario);
        }

        [Authorize]
        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteComentario(int idcoment)
        {
            await repo.DeleteComentarioAsync(idcoment);

            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetComentariosEvento(int idevento)
        {
            var comentarios = await repo.GetComentariosByEventoIdAsync(idevento);

            return Ok(comentarios);
        }
    }
}
