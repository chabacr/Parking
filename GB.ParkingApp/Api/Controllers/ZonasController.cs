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

namespace Api.Controllers
{
    public class ZonasController : ApiController
    {
        private DataContext db = new DataContext();

        // GET: api/Zonas
        public IQueryable<Zona> GetZonas()
        {
            return db.Zonas;
        }

        // GET: api/Zonas/5
        [ResponseType(typeof(Zona))]
        public async Task<IHttpActionResult> GetZona(int id)
        {
            Zona zona = await db.Zonas.FindAsync(id);
            if (zona == null)
            {
                return NotFound();
            }

            return Ok(zona);
        }

        // PUT: api/Zonas/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutZona(int id, Zona zona)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != zona.IdZona)
            {
                return BadRequest();
            }

            db.Entry(zona).State = EntityState.Modified;

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

            return Ok(zona);
        }

        // POST: api/Zonas
        [ResponseType(typeof(Zona))]
        public async Task<IHttpActionResult> PostZona(Zona zona)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Zonas.Add(zona);

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

            return CreatedAtRoute("DefaultApi", new { id = zona.IdZona }, zona);
        }

        // DELETE: api/Zonas/5
        [ResponseType(typeof(Zona))]
        public async Task<IHttpActionResult> DeleteZona(int id)
        {
            Zona zona = await db.Zonas.FindAsync(id);
            if (zona == null)
            {
                return NotFound();
            }

            db.Zonas.Remove(zona);
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


            return Ok(zona);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ZonaExists(int id)
        {
            return db.Zonas.Count(e => e.IdZona == id) > 0;
        }
    }
}