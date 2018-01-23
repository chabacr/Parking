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
    public class PermisosController : ApiController
    {
        private DataContext db = new DataContext();

        // GET: api/Permisos
        public IQueryable<Permiso> GetPermisoes()
        {
            return db.Permisoes;
        }

        // GET: api/Permisos/5
        [ResponseType(typeof(Permiso))]
        public async Task<IHttpActionResult> GetPermiso(int id)
        {
            Permiso permiso = await db.Permisoes.FindAsync(id);
            if (permiso == null)
            {
                return NotFound();
            }

            return Ok(permiso);
        }

        // PUT: api/Permisos/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutPermiso(int id, Permiso permiso)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != permiso.IdPermiso)
            {
                return BadRequest();
            }

            db.Entry(permiso).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PermisoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Permisos
        [ResponseType(typeof(Permiso))]
        public async Task<IHttpActionResult> PostPermiso(Permiso permiso)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Permisoes.Add(permiso);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = permiso.IdPermiso }, permiso);
        }

        // DELETE: api/Permisos/5
        [ResponseType(typeof(Permiso))]
        public async Task<IHttpActionResult> DeletePermiso(int id)
        {
            Permiso permiso = await db.Permisoes.FindAsync(id);
            if (permiso == null)
            {
                return NotFound();
            }

            db.Permisoes.Remove(permiso);
            await db.SaveChangesAsync();

            return Ok(permiso);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PermisoExists(int id)
        {
            return db.Permisoes.Count(e => e.IdPermiso == id) > 0;
        }
    }
}