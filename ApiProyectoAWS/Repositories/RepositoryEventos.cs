using ApiProyectoAWS.Data;
using ApiProyectoAWS.Helpers;
using ApiProyectoAWS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiProyectoAWS.Repositories
{
    public class RepositoryEventos
    {
        private EventosContext context;

        public RepositoryEventos(EventosContext context)
        {
            this.context = context;
        }

        #region Comentarios
        public async Task AddComentarioAsync(Comentario resena)
        {
            context.Comentarios.Add(resena);
            await context.SaveChangesAsync();
        }

        public async Task DeleteComentarioAsync(int idcoment)
        {
            var comentario = await context.Comentarios.FindAsync(idcoment);

            if (comentario != null)
            {
                context.Comentarios.Remove(comentario);
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<ComentarioDetalles>> GetComentariosByEventoIdAsync(int eventoId)
        {
            return await context.ComentariosDetalles
                                 .Where(r => r.EventoID == eventoId)
                                 .Include(r => r.Usuario)
                                 .ToListAsync();
        }
        #endregion

        #region Eventos
        public async Task<List<EventoDetalles>> GetAllEventosHoyAsync()
        {
            // Obtener la fecha de hoy
            DateTime fechaHoy = DateTime.Today;

            // Filtrar los eventos que ocurran a partir de hoy y ordenarlos por fecha
            var eventos = await this.context.EventosDetalles
                .Where(e => e.Fecha >= fechaHoy)
                .OrderBy(e => e.Fecha)
                .ToListAsync();

            return eventos;
        }

        public async Task<List<EventoDetalles>> GetAllEventosAsync()
        {
            var eventos = await this.context.EventosDetalles.ToListAsync();

            return eventos;
        }

        public async Task<EventoDetalles> GetDetallesEventoAsync(int idevento)
        {
            return await this.context.EventosDetalles.FirstOrDefaultAsync(z => z.Id == idevento);
        }

        public async Task<List<TipoEvento>> GetTipoEventosAsync()
        {
            var tiposevento = await this.context.TiposEventos.ToListAsync();

            return tiposevento;
        }

        public async Task<List<EventoDetalles>> GetAllEventosTipoAsync(string tipo)
        {
            var eventos = await this.context.EventosDetalles
                .Where(a => a.TipoEvento == tipo)
                .ToListAsync();

            return eventos;
        }

        public async Task<List<EventoDetalles>> GetAllEventosArtistaAsync(int idartista)
        {
            var eventos = await this.context.EventosDetalles
                .Where(e => this.context.ArtistasEvento.Any(ae => ae.ArtistaID == idartista && ae.EventoID == e.Id))
                .ToListAsync();

            return eventos;
        }

        public async Task<List<EventoDetalles>> GetEventosPorRecintoAsync(int idRecinto)
        {
            return await this.context.EventosDetalles
                .Where(e => e.RecintoId == idRecinto)
                .ToListAsync();
        }

        public async Task<List<EventoDetalles>> GetAllEventosProvinciasAsync(int idprovincia)
        {
            var eventos = await this.context.EventosDetalles
                .Where(a => a.ProvinciaID == idprovincia)
                .ToListAsync();

            return eventos;
        }

        public async Task CrearEventoAsync(Evento evento)
        {
            context.Eventos.Add(evento);
            await context.SaveChangesAsync();
        }

        public async Task BorrarEvento(int idevento)
        {
            // Buscar el evento por su ID
            var evento = await this.context.Eventos.FindAsync(idevento);

            // Eliminar el evento de la base de datos
            this.context.Eventos.Remove(evento);
        }

        public async Task<IActionResult> BuscarArtistas(string term)
        {
            var artistas = await context.ArtistasDetalles
                              .Where(a => a.NombreUsuario.Contains(term))
                              .Select(a => new { label = a.NombreUsuario, value = a.UsuarioID })
                              .ToListAsync();

            return new JsonResult(artistas);
        }


        public async Task<List<EventoDetalles>> GetEventosPorFiltros(FiltroEvento filtro)
        {
            IQueryable<EventoDetalles> query = this.context.EventosDetalles;

            if (!string.IsNullOrEmpty(filtro.Nombre))
            {
                query = query.Where(e => e.NombreEvento.Contains(filtro.Nombre));
            }

            if (filtro.FechaInicio.HasValue)
            {
                query = query.Where(e => e.Fecha == filtro.FechaInicio);
            }

            if (!string.IsNullOrEmpty(filtro.Provincia))
            {
                query = query.Where(e => e.Provincia == filtro.Provincia);
            }

            if (!string.IsNullOrEmpty(filtro.Tipo))
            {
                query = query.Where(e => e.TipoEvento == filtro.Tipo);
            }

            if (filtro.PrecioMayorQue.HasValue)
            {
                query = query.Where(e => e.Precio > filtro.PrecioMayorQue);
            }

            if (filtro.PrecioMenorQue.HasValue)
            {
                query = query.Where(e => e.Precio < filtro.PrecioMenorQue);
            }

            return await query.ToListAsync();
        }
        #endregion

        #region ArtistasEventos
        public async Task AddArtistaToEvento(int idevento, int idartista)
        {

            ArtistaEvento artistaEvento = new ArtistaEvento
            {
                EventoID = idevento,
                ArtistaID = idartista
            };
            this.context.ArtistasEvento.Add(artistaEvento);
            await this.context.SaveChangesAsync();

        }

        public async Task<List<Artista>> GetArtistasTempAsync(int idevento)
        {
            return await context.Artistas
                                 .Where(r => r.IdEvento == idevento)
                                 .ToListAsync();
        }

        public async Task DeleteArtistaEventoAsync(int idevento, int idartista)
        {
            ArtistaEvento artistaEvento = await this.context.ArtistasEvento
                .FirstOrDefaultAsync(ae => ae.EventoID == idevento && ae.ArtistaID == idartista);

            this.context.ArtistasEvento.Remove(artistaEvento);
            await context.SaveChangesAsync();
        }

        public async Task DeleteArtistaAsync(int idevento, int idartista)
        {
            Artista artista = await this.context.Artistas
                .FirstOrDefaultAsync(ae => ae.IdEvento == idevento && ae.IdArtista == idartista);

            this.context.Artistas.Remove(artista);
            await context.SaveChangesAsync();
        }
        #endregion

        #region Entradas
        public async Task AsignarEntradasAsync(AsistenciaEvento entrada)
        {
            context.AsistenciasEventos.Add(entrada);
            await this.context.SaveChangesAsync();
        }

        public async Task<List<EntradaDetalles>> GetAllEntradasUsuarioAsync(int iduser)
        {
            var entradas = await this.context.EntradaDetalles
                .Where(u => u.UsuarioID == iduser)
                .ToListAsync();

            return entradas;
        }
        public async Task CrearArtistaAsync(Artista artista)
        {
            this.context.Artistas.Add(artista);
            await this.context.SaveChangesAsync();
        }

        public async Task RestarEntrada(int idevento)
        {
            // Obtener el evento por su ID

            Evento evento = await this.context.Eventos.FirstOrDefaultAsync(e => e.EventoID == idevento);

            // Verificar si se encontró el evento
            if (evento != null)
            {
                // Restar una entrada
                evento.EntradasVendidas++;

                // Guardar los cambios en la base de datos
                await context.SaveChangesAsync();
            }
            else
            {
                // Manejar el caso cuando el evento no se encuentra
                throw new InvalidOperationException("El evento no existe.");
            }
        }

        #endregion

        #region Provincias
        public async Task<List<Provincia>> GetAllProvinciassAsync()
        {
            var provincias = await this.context.Provincias.ToListAsync();

            return provincias;
        }
        #endregion

        #region Usuarios
        public async Task<List<ArtistaDetalles>> GetAllArtistasEventoAsync(int idevento)
        {
            var artistas = await this.context.ArtistasDetalles
                .Where(a => a.EventoID == idevento)
                .ToListAsync();

            return artistas;
        }

        public async Task<Usuario> EditUser(Usuario user)
        {
            // Asegúrate de que el usuario existe en la base de datos
            var existingUser = await this.context.Usuarios.FindAsync(user.UsuarioID);
            if (existingUser != null)
            {
                // Actualiza las propiedades del usuario existente
                this.context.Entry(existingUser).CurrentValues.SetValues(user);

                // Guarda los cambios en la base de datos
                await this.context.SaveChangesAsync();

                return existingUser; // Retorna el usuario actualizado
            }
            else
            {
                // Opcionalmente, maneja la situación en que el usuario no se encuentra
                // Por ejemplo, podrías lanzar una excepción o retornar null
                throw new Exception("Usuario no encontrado");
            }
        }


        public async Task<List<UsuarioDetalles>> GetAllArtistas()
        {
            var artistas = await this.context.UsuariosDetalles
                .Where(r => r.RolID == 2)
                .ToListAsync();

            return artistas;
        }

        public async Task UpdateUserAsync(Usuario usuario)
        {
            this.context.Usuarios.Update(usuario);
            await this.context.SaveChangesAsync();
        }


        public async Task<UsuarioDetalles> GetUsuarioDetalles(int iduser)
        {
            return await this.context.UsuariosDetalles.FirstOrDefaultAsync(z => z.UsuarioID == iduser);
        }

        //---------------------- Registro / Login ----------------------//
        public async Task<bool> EmailExists(string email)
        {
            var consulta = from u in context.Usuarios
                           where u.Correo == email
                           select u;

            return consulta.Any();
        }

        public async Task<Usuario> RegisterUserAsync(string nombre, string email
             , string password, int rol)
        {
            Usuario user = new Usuario();
            user.NombreUsuario = nombre;
            user.Correo = email;
            user.RolID = rol;
            user.ProvinciaID = 1;
            user.Telefono = "";
            user.FotoPerfil = "default-user.png";
            user.Descripcion = "";
            user.Activo = false;
            //CADA USUARIO TENDRA UN SALT DISTINTO 
            user.Salt = HelperTools.GenerateSalt();
            //GUARDAMOS EL PASSWORD EN BYTE[] 
            user.Password =
                HelperCryptography.EncryptPassword(password, user.Salt);
            user.Activo = false;
            user.TokenMail = HelperTools.GenerateTokenMail();

            this.context.Usuarios.Add(user);
            await this.context.SaveChangesAsync();
            return user;
        }

        public async Task ActivateUserAsync(string token)
        {
            // Buscamos el usuario por su token
            Usuario user = await this.context.Usuarios.FirstOrDefaultAsync(x => x.TokenMail == token);

            user.Activo = true;

            user.TokenMail = "";

            await this.context.SaveChangesAsync();
        }

        public async Task<bool> LogInUserAsync(string correo, string password)
        {
            var usuario = await context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);

            if (usuario != null)
            {
                string salt = usuario.Salt;
                byte[] temp = HelperCryptography.EncryptPassword(password, salt);
                byte[] passUser = usuario.Password;
                bool response = HelperTools.CompareArrays(temp, passUser);

                return response;
            }
            else
            {
                return false;
            }
        }

        public async Task<Usuario> GetUserAsync(string correo)
        {
            var usuario = (from u in context.Usuarios
                           where u.Correo == correo
                           select u).FirstOrDefault();

            return usuario;
        }
        #endregion

    }
}
