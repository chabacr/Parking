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
    public class VehiculosController : ApiController
    {
        private DataContext db = new DataContext();

        // GET: api/Vehiculos
        public IQueryable<Vehiculo> GetVehiculoes()
        {
            return db.Vehiculoes;
        }

        // GET: api/Vehiculos/5
        [ResponseType(typeof(Vehiculo))]
        public async Task<IHttpActionResult> GetVehiculo(int id)
        {
            Vehiculo vehiculo = await db.Vehiculoes.FindAsync(id);
            if (vehiculo == null)
            {
                return NotFound();
            }

            return Ok(vehiculo);
        }

        // PUT: api/Vehiculos/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutVehiculo(int id, VehiculoSolicitud solicitud)
        {
            if (!ModelState.IsValid)
            {
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        return BadRequest(error.ErrorMessage.ToString());
                    }
                }
            }

            if (id != solicitud.IdVehiculo)
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

            var vehiculo = ToVehiculo(solicitud);
            db.Entry(vehiculo).State = EntityState.Modified;

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
                    return BadRequest("Hay un registro con la misma placa.");
                }
                else
                {
                    return BadRequest(ex.Message);
                }
            }

            return Ok(vehiculo);
        }

        // POST: api/Vehiculos
        [ResponseType(typeof(Vehiculo))]
        public async Task<IHttpActionResult> PostVehiculo(VehiculoSolicitud Solicitud)
        {
            if (!ModelState.IsValid)
            {
                foreach(var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                       return BadRequest(error.ErrorMessage.ToString());
                    }
                }
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

            var vehiculo = ToVehiculo(Solicitud);
            db.Vehiculoes.Add(vehiculo);
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
                    return BadRequest("Hay un registro con la misma placa.");
                }
                else
                {
                    return BadRequest(ex.InnerException.InnerException.Message);
                }
            }
            
            return CreatedAtRoute("DefaultApi", new { id = vehiculo.IdVehiculo }, vehiculo);
        }

        // DELETE: api/Vehiculos/5
        [ResponseType(typeof(Vehiculo))]
        public async Task<IHttpActionResult> DeleteVehiculo(int id)
        {
            Vehiculo vehiculo = await db.Vehiculoes.FindAsync(id);
            if (vehiculo == null)
            {
                return NotFound();
            }

            db.Vehiculoes.Remove(vehiculo);

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

            return Ok(vehiculo);
        }


        private Vehiculo ToVehiculo(VehiculoSolicitud solicitud)
        {
            return new Vehiculo
            {
                IdVehiculo =solicitud.IdVehiculo,
                IdUsuario = solicitud.IdUsuario,
                Color = solicitud.Color,
                IdTipoVehiculo = solicitud.IdTipoVehiculo,
                Marca = solicitud.Marca,
                Modelo = solicitud.Modelo,
                Placa = solicitud.Placa,
                Reservas = solicitud.Reservas,
                TipoVehiculo =solicitud.TipoVehiculo,
                Usuario =solicitud.Usuario,
                Foto = solicitud.Foto,
            };
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool VehiculoExists(int id)
        {
            return db.Vehiculoes.Count(e => e.IdVehiculo == id) > 0;
        }
    }
}