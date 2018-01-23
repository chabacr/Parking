using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Api.Models;
using System.IO;
using Api.Helpers;

namespace Api.Controllers
{
    public class UsuariosController : ApiController
    {
        private DataContext db = new DataContext();

        // GET: api/Usuarios
        public IQueryable<Usuario> GetUsuarios()
        {
            return db.Usuarios;
        }

        // GET: api/Usuarios/5
        [ResponseType(typeof(Usuario))]
        public async Task<IHttpActionResult> GetUsuario(int id)
        {
            Usuario usuario = await db.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            return Ok(usuario);
        }

        [ResponseType(typeof(Usuario))]
        public  IHttpActionResult GetUsuarioByCorreo(string correo)
        {
            Usuario usuario = db.Usuarios.Where(u => u.Correo == correo).FirstOrDefault();
            if (usuario == null)
            {
                return NotFound();
            }

            var VehiculosRespuesta = new List<Vehiculo>();


            foreach (var vehiculo in usuario.Vehiculoes)
            {
                    VehiculosRespuesta.Add(new Vehiculo
                    {
                        Color = vehiculo.Color,
                        Foto = vehiculo.Foto,
                        IdTipoVehiculo = vehiculo.IdTipoVehiculo,
                        IdUsuario = vehiculo.IdUsuario,
                        IdVehiculo =vehiculo.IdVehiculo,
                        Marca = vehiculo.Marca,
                        Modelo = vehiculo.Modelo,
                        Placa = vehiculo.Placa,
                        Reservas = vehiculo.Reservas,
                        TipoVehiculo = vehiculo.TipoVehiculo,
                       // Usuario = vehiculo.Usuario,
                    });
            }
            var reservas = new List<ReservaRespuesta>();

            foreach (var reserva in usuario.Reservas)
            {
                if(reserva.FechaFinReserva >= DateTime.Today )
                {
                    reservas.Add(new ReservaRespuesta
                    {
                        IdReserva = reserva.IdReserva,
                        FechaFinReserva = reserva.FechaFinReserva,
                        FechaRegistro = reserva.FechaRegistro,
                        FechaReserva = reserva.FechaReserva,
                        Vehiculo = reserva.Vehiculo,
                        Zona = reserva.Zona,
                    });
                }
            }

            var PermisosRespuesta = new List<Permiso>();

            foreach (var permiso in usuario.TipoUsuario.Permiso_TipoUsuario)
            {
                PermisosRespuesta.Add(new Permiso
                {
                    Descripcion = permiso.Permiso.Descripcion,
                    Icono = permiso.Permiso.Icono,
                    Pagina = permiso.Permiso.Pagina,
                });
            }



            var usuarioRespuesta = new UsuarioRespuestas
                {
                IdUsuario = usuario.IdUsuario,
                IdTipoUsuario = usuario.IdTipoUsuario,
                Nombre = usuario.Nombre,
                Apellidos = usuario.Apellidos,
                Correo = usuario.Correo,
                Telefono = usuario.Telefono,
                Foto = usuario.Foto,
                Password = usuario.Password,
                Vehiculos = VehiculosRespuesta,
                Permisos= PermisosRespuesta,
                Reservas = reservas,
            };

            return Ok(usuarioRespuesta);
        }

        // PUT: api/Usuarios/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUsuario(int id, UsuarioSolicitud solicitud)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != solicitud.IdUsuario)
            {
                return BadRequest();
            }

            if (solicitud.ImageArray != null && solicitud.ImageArray.Length > 0)
            {
                var stream = new MemoryStream(solicitud.ImageArray);
                var guid = Guid.NewGuid().ToString();
                var file = string.Format("{0}.jpg", guid);
                var folder = "~/Content/Images";
                var fullPath = string.Format("{0}/{1}", folder, file);
                var response = FilesHelper.UploadPhoto(stream, folder, file);

                if (response)
                {
                    solicitud.Foto = fullPath;
                }
            }

            var usuario = ToUsuario(solicitud);
            db.Entry(usuario).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null &&
                    ex.InnerException.InnerException != null &&
                    ex.InnerException.InnerException.Message.Contains("Unique"))
                {
                    return BadRequest("Hay un registro con la misma descripción.");
                }
                else
                {
                    return BadRequest(ex.Message);
                }
            }

            return Ok(usuario);
        }

        // POST: api/Usuarios
        [ResponseType(typeof(Usuario))]
        public async Task<IHttpActionResult> PostUsuario(UsuarioSolicitud Solicitud)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (Solicitud.ImageArray != null && Solicitud.ImageArray.Length > 0)
            {
                var stream = new MemoryStream(Solicitud.ImageArray);
                var guid = Guid.NewGuid().ToString();
                var file = string.Format("{0}.jpg", guid);
                var folder = "~/Content/Images";
                var fullPath = string.Format("{0}/{1}", folder, file);
                var response = FilesHelper.UploadPhoto(stream, folder, file);

                if (response)
                {
                    Solicitud.Foto = fullPath;
                }
            }

            var usuario = ToUsuario(Solicitud);
            db.Usuarios.Add(usuario);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null &&
                    ex.InnerException.InnerException != null &&
                    ex.InnerException.InnerException.Message.Contains("Unique"))
                {
                    return BadRequest("Hay un registro con la misma descripción.");
                }
                else
                {
                    return BadRequest(ex.Message);
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = usuario.IdUsuario }, usuario);
        }

        private Usuario ToUsuario(UsuarioSolicitud solicitud)
        {
            return new Usuario
            {
                Reservas = solicitud.Reservas,
                Vehiculoes = solicitud.Vehiculoes,
                Observacion_Zona = solicitud.Observacion_Zona,
                IdUsuario = solicitud.IdUsuario,
                TipoUsuario = solicitud.TipoUsuario,
                IdTipoUsuario = solicitud.IdTipoUsuario,
                Nombre = solicitud.Nombre,
                Foto = solicitud.Foto,
                Apellidos = solicitud.Apellidos,
                Correo = solicitud.Correo,
                Telefono = solicitud.Telefono,
                Password = solicitud.Password,
            };
        }

        // DELETE: api/Usuarios/5
        [ResponseType(typeof(Usuario))]
        public async Task<IHttpActionResult> DeleteUsuario(int id)
        {
            Usuario usuario = await db.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            db.Usuarios.Remove(usuario);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null &&
                    ex.InnerException.InnerException != null &&
                    ex.InnerException.InnerException.Message.Contains("REFERENCE"))
                {
                    return BadRequest("No puede eliminar este registro, porque tiene un registro relacionado.");
                }
                else
                {
                    return BadRequest(ex.Message);
                }
            }

            return Ok(usuario);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UsuarioExists(int id)
        {
            return db.Usuarios.Count(e => e.IdUsuario == id) > 0;
        }
    }
}