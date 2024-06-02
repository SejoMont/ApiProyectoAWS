using ApiProyectoAWS.Models;
using ApiProyectoAWS.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiProyectoAWS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistasEventoController : ControllerBase
    {
        private readonly RepositoryEventos repo;

        public ArtistasEventoController(RepositoryEventos repo)
        {
            this.repo = repo;
        }


        [HttpGet("[action]")]
        public async Task<ActionResult<List<Artista>>> GetArtistasTempEvento(int idevento)
        {
            var artistas = await this.repo.GetArtistasTempAsync(idevento);
            return Ok(artistas);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetArtistasEvento(int idevento)
        {
            var artistas = await this.repo.GetAllArtistasEventoAsync(idevento);
            return Ok(artistas);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllArtistas()
        {
            var artistas = await this.repo.GetAllArtistas();
            return Ok(artistas);
        }

        [HttpPost("AddArtistaToEvento/{idevento}/{idartista}")]
        public async Task<ActionResult> AddArtistaEvento(int idevento, int idartista)
        {
            try
            {
                await this.repo.AddArtistaToEvento(idevento, idartista);
                return Ok("Artista agregado al evento correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al agregar el artista al evento: " + ex.Message);
            }
        }

        [HttpPost("CrearArtista")]
        public async Task<ActionResult> CrearArtista([FromBody] Artista artista)
        {
            try
            {
                await this.repo.CrearArtistaAsync(artista);
                return Ok("Artista agregado al evento correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al agregar el artista al evento: " + ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteArtistaEvento(int idevento, int idartista)
        {
            await repo.DeleteArtistaEventoAsync(idevento, idartista);

            return Ok();
        }


        [Authorize]
        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteArtistaTemp(int idevento, int idartista)
        {
            await repo.DeleteArtistaAsync(idevento, idartista);

            return Ok();
        }
    }
}
