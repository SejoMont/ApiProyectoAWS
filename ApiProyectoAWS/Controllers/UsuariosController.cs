using ApiProyectoAWS.Helpers;
using ApiProyectoAWS.Models;
using ApiProyectoAWS.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApiProyectoAWS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private RepositoryEventos repo;
        private HelperActionServicesOAuth helper;

        public UsuariosController(RepositoryEventos repo, HelperActionServicesOAuth helper)
        {
            this.repo = repo;
            this.helper = helper;
        }

        [HttpGet("GetUser/{correo}")]
        public async Task<ActionResult<Usuario>> GetUser(string correo)
        {
            Usuario user = await this.repo.GetUserAsync(correo);
            if (user == null)
                return NotFound("Usuario no encontrado");
            return Ok(user);
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<Usuario>> EmailExists(string correo)
        {
            bool emailExist = await this.repo.EmailExists(correo);
            if (!emailExist)
                return NotFound("Usuario no encontrado");
            return Ok(emailExist);
        }

        [HttpGet("Details/{iduser}")]
        public async Task<ActionResult<UsuarioDetalles>> Details(int iduser)
        {
            UsuarioDetalles usuarioDetalles = await this.repo.GetUsuarioDetalles(iduser);
            if (usuarioDetalles == null)
                return NotFound("Usuario no encontrado");
            return Ok(usuarioDetalles);
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login(Login login)
        {
            bool loginSuccess = await this.repo.LogInUserAsync(login.Correo, login.Password);
            if (loginSuccess)
            {
                SigningCredentials credentials =
                    new SigningCredentials(
                        this.helper.GetKeyToken()
                        , SecurityAlgorithms.HmacSha256);


                Usuario user = await this.repo.GetUserAsync(login.Correo);

                string jsonUser =
                    JsonConvert.SerializeObject(user);

                Claim[] informacion = new[]
                {
                    new Claim("UserData", jsonUser)
                };

                JwtSecurityToken token =
                    new JwtSecurityToken(
                        claims: informacion,
                        issuer: this.helper.Issuer,
                        audience: this.helper.Audience,
                        signingCredentials: credentials,
                        expires: DateTime.UtcNow.AddMinutes(30),
                        notBefore: DateTime.UtcNow
                        );
                return Ok(
                    new
                    {
                        response =
                        new JwtSecurityTokenHandler()
                        .WriteToken(token)
                    });
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost("Registro")]
        public async Task<ActionResult> Registro(Registro registro)
        {
            if (await repo.EmailExists(registro.Correo))
            {
                return BadRequest("El correo electrónico ya está en uso");
            }

            if (registro.Password != registro.ConfirmPassword)
            {
                return BadRequest("Las contraseñas no coinciden");
            }

            Usuario user = await repo.RegisterUserAsync(registro.Nombre, registro.Correo, registro.Password, 1);

            if (user != null)
            {
                //string serverUrl = _helperPathProvider.MapUrlServerPath() + "/Usuarios/ActivateUser/?token=" + user.TokenMail;
                //string message = $"Activa tu cuenta aquí: {serverUrl}";
                //await _helperMails.SendMailAsync(correo, "Registro Usuario", message);

                return Ok(new { Message = "Usuario registrado y correo de activación enviado", UserId = user.UsuarioID });
            }
            else
            {
                return BadRequest("No se pudo crear el usuario");
            }
        }

        //[HttpGet("ActivateUser/{token}")]
        //public async Task<ActionResult> ActivateUser(string token)
        //{
        //    bool activated = await _repo.ActivateUserAsync(token);
        //    if (!activated)
        //        return NotFound("Token inválido o usuario ya activado");

        //    return Ok("Cuenta activada correctamente");
        //}

        [HttpPut("[action]")]
        public async Task<ActionResult> Edit(Usuario user)
        {
            await this.repo.UpdateUserAsync(user);

            return Ok("Usuario actualizado correctamente");
        }
    }
}
