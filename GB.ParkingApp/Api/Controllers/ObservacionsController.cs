using Api.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Api.Controllers
{
    public class ObservacionsController : ApiController
    {
        private DataContext db = new DataContext();

        // GET: api/Observacions
        public IQueryable<Observacion> GetObservacions()
        {
            return db.Observacions;
        }

        // GET: api/Observacions/5
        [ResponseType(typeof(Observacion))]
        public async Task<IHttpActionResult> GetObservacion(int id)
        {
            Observacion observacion = await db.Observacions.FindAsync(id);
            if (observacion == null)
            {
                return NotFound();
            }

            return Ok(observacion);
        }

        // PUT: api/Observacions/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutObservacion(int id, Observacion observacion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != observacion.IdObservacion)
            {
                return BadRequest();
            }

            db.Entry(observacion).State = EntityState.Modified;

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

            return Ok(observacion);
        }

        // POST: api/Observacions
        [ResponseType(typeof(Observacion))]
        public async Task<IHttpActionResult> PostObservacion(Observacion observacion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Observacions.Add(observacion);

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

            return CreatedAtRoute("DefaultApi", new { id = observacion.IdObservacion }, observacion);
        }

        // DELETE: api/Observacions/5
        [ResponseType(typeof(Observacion))]
        public async Task<IHttpActionResult> DeleteObservacion(int id)
        {
            Observacion observacion = await db.Observacions.FindAsync(id);
            if (observacion == null)
            {
                return NotFound();
            }

            db.Observacions.Remove(observacion);

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

            return Ok(observacion);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ObservacionExists(int id)
        {
            return db.Observacions.Count(e => e.IdObservacion == id) > 0;
        }
    }
}