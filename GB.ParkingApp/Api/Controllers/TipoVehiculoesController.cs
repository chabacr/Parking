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
    public class TipoVehiculoesController : ApiController
    {
        private DataContext db = new DataContext();

        // GET: api/TipoVehiculoes
        public IQueryable<TipoVehiculo> GetTipoVehiculoes()
        {
            return db.TipoVehiculoes;
        }

        // GET: api/TipoVehiculoes/5
        [ResponseType(typeof(TipoVehiculo))]
        public async Task<IHttpActionResult> GetTipoVehiculo(int id)
        {
            TipoVehiculo tipoVehiculo = await db.TipoVehiculoes.FindAsync(id);
            if (tipoVehiculo == null)
            {
                return NotFound();
            }

            return Ok(tipoVehiculo);
        }

        // PUT: api/TipoVehiculoes/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutTipoVehiculo(int id, TipoVehiculo tipoVehiculo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tipoVehiculo.IdTipoVehiculo)
            {
                return BadRequest();
            }

            db.Entry(tipoVehiculo).State = EntityState.Modified;

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

            return Ok(tipoVehiculo);
        }

        // POST: api/TipoVehiculoes
        [ResponseType(typeof(TipoVehiculo))]
        public async Task<IHttpActionResult> PostTipoVehiculo(TipoVehiculo tipoVehiculo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.TipoVehiculoes.Add(tipoVehiculo);

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

            return CreatedAtRoute("DefaultApi", new { id = tipoVehiculo.IdTipoVehiculo }, tipoVehiculo);
        }

        // DELETE: api/TipoVehiculoes/5
        [ResponseType(typeof(TipoVehiculo))]
        public async Task<IHttpActionResult> DeleteTipoVehiculo(int id)
        {
            TipoVehiculo tipoVehiculo = await db.TipoVehiculoes.FindAsync(id);
            if (tipoVehiculo == null)
            {
                return NotFound();
            }

            db.TipoVehiculoes.Remove(tipoVehiculo);

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

            return Ok(tipoVehiculo);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TipoVehiculoExists(int id)
        {
            return db.TipoVehiculoes.Count(e => e.IdTipoVehiculo == id) > 0;
        }
    }
}