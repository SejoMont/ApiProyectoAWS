using ApiProyectoAWS.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiProyectoAWS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProvinciasController : ControllerBase
    {

        private RepositoryEventos repo;

        public ProvinciasController(RepositoryEventos repo)
        {
            this.repo = repo;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllProvinciasAsync()
        {
            var provincias = await repo.GetAllProvinciassAsync();

            return Ok(provincias);
        }

    }
}
