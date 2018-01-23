using Api.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Api.Controllers
{
    public class ParqueosController : ApiController
    {
        private DataContext db = new DataContext();

        // GET: api/Parqueos
        public async Task<IHttpActionResult> GetParqueos()
        {
            var parqueos = await db.Parqueos.ToListAsync();
            var parqueosRespuesta = new List<ParqueoRespuesta>();

            foreach(var parqueo in parqueos)
            {
                var zonasRespuesta = new List<ZonaRespuesta>();

                foreach(var zona in parqueo.Zonas)
                {
                    zonasRespuesta.Add(new ZonaRespuesta
                    {
                        IdZona = zona.IdZona,
                        IdParking = zona.IdParqueo,
                        NZona = zona.NZona,
                        Estado = zona.Estado
                    });
                }

                parqueosRespuesta.Add(new ParqueoRespuesta
                {
                    IdParqueo = parqueo.IdParqueo,
                    Descripcion = parqueo.Descripcion,
                    Direccion = parqueo.Direccion,
                    NumeroZonas = parqueo.NumeroZonas,
                    Zonas = zonasRespuesta,
                });
            }
            return Ok(parqueosRespuesta);

        }
        [HttpGet]
        [Route("api/GetParqueosZonasDisponibles")]
        public async Task<IHttpActionResult> GetParqueosZonasDisponibles(DateTime fecha)
        {
            var parqueos = await db.Parqueos.ToListAsync();
            var parqueosRespuesta = new List<ParqueoRespuesta>();

            foreach (var parqueo in parqueos)
            {
                if (parqueo.Zonas.Count > 0)
                {
                    var zonasRespuesta = new List<ZonaRespuesta>();

                    foreach (var zona in parqueo.Zonas)
                    {
                        var listaReserva = db.Reservas.Where(r => r.IdZona == zona.IdZona && r.FechaFinReserva == fecha).FirstOrDefault();

                        if (zona.Estado == 1 && listaReserva == null)
                        {
                            zonasRespuesta.Add(new ZonaRespuesta
                            {
                                IdZona = zona.IdZona,
                                IdParking = zona.IdParqueo,
                                NZona = zona.NZona,
                                Estado = zona.Estado
                            }); 
                        }
                    }

                    if (zonasRespuesta.Count > 0)
                    {
                        parqueosRespuesta.Add(new ParqueoRespuesta
                        {
                            IdParqueo = parqueo.IdParqueo,
                            Descripcion = parqueo.Descripcion,
                            Direccion = parqueo.Direccion,
                            NumeroZonas = parqueo.NumeroZonas,
                            Zonas = zonasRespuesta,
                        });
                    }
                }
            }
            return Ok(parqueosRespuesta);

        }

        // GET: api/Parqueos/5
        [ResponseType(typeof(Parqueo))]
        public async Task<IHttpActionResult> GetParqueo(int id)
        {
            Parqueo parqueo = await db.Parqueos.FindAsync(id);
            if (parqueo == null)
            {
                return NotFound();
            }

            return Ok(parqueo);
        }

        // PUT: api/Parqueos/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutParqueo(int id, Parqueo parqueo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != parqueo.IdParqueo)
            {
                return BadRequest();
            }

            db.Entry(parqueo).State = EntityState.Modified;

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

            return Ok(parqueo);
        }

        // POST: api/Parqueos
        [ResponseType(typeof(Parqueo))]
        public async Task<IHttpActionResult> PostParqueo(Parqueo parqueo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Parqueos.Add(parqueo);

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

            return CreatedAtRoute("DefaultApi", new { id = parqueo.IdParqueo }, parqueo);
        }

        // DELETE: api/Parqueos/5
        [ResponseType(typeof(Parqueo))]
        public async Task<IHttpActionResult> DeleteParqueo(int id)
        {
            Parqueo parqueo = await db.Parqueos.FindAsync(id);
            if (parqueo == null)
            {
                return NotFound();
            }

            db.Parqueos.Remove(parqueo);

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

            return Ok(parqueo);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ParqueoExists(int id)
        {
            return db.Parqueos.Count(e => e.IdParqueo == id) > 0;
        }
    }
}