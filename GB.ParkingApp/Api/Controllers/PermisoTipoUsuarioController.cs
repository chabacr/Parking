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
    public class PermisoTipoUsuarioController : ApiController
    {
        private DataContext db = new DataContext();

        // GET: api/PermisoTipoUsuario
        public async Task<IHttpActionResult> GetPermiso_TipoUsuario()
        {
            var permisosTipoUsuario = await db.Permiso_TipoUsuario.ToListAsync();
            var permisoRespuesta = new List<PermisoRespuesta>();

            foreach (var permisoTipoUsuario in permisosTipoUsuario)
            {
                if (permisoTipoUsuario.IdTipoUsuario == 1)
                {
                    permisoRespuesta.Add(new PermisoRespuesta
                    {
                        Descripcion = permisoTipoUsuario.Permiso.Descripcion,
                        Icono = permisoTipoUsuario.Permiso.Icono,
                        Pagina = permisoTipoUsuario.Permiso.Pagina,
                    });
                }
            }
            return Ok(permisoRespuesta);
        }

        // GET: api/PermisoTipoUsuario/5
        public async Task<IHttpActionResult> GetPermiso_TipoUsuario(int id)
        {
           var permisosTipoUsuario = await db.Permiso_TipoUsuario.ToListAsync();
            var permisoRespuesta = new List<PermisoRespuesta>();

            foreach (var permisoTipoUsuario in permisosTipoUsuario)
            {
                if (permisoTipoUsuario.IdTipoUsuario == id)
                {
                    permisoRespuesta.Add(new PermisoRespuesta
                    {
                        Descripcion = permisoTipoUsuario.Permiso.Descripcion,
                        Icono = permisoTipoUsuario.Permiso.Icono,
                        Pagina = permisoTipoUsuario.Permiso.Pagina,
                    });
                }
            }
            return Ok(permisoRespuesta);
        }

        // PUT: api/PermisoTipoUsuario/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutPermiso_TipoUsuario(int id, Permiso_TipoUsuario permiso_TipoUsuario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != permiso_TipoUsuario.IdPermisoTipoUsuario)
            {
                return BadRequest();
            }

            db.Entry(permiso_TipoUsuario).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Permiso_TipoUsuarioExists(id))
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

        // POST: api/PermisoTipoUsuario
        [ResponseType(typeof(Permiso_TipoUsuario))]
        public async Task<IHttpActionResult> PostPermiso_TipoUsuario(Permiso_TipoUsuario permiso_TipoUsuario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Permiso_TipoUsuario.Add(permiso_TipoUsuario);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = permiso_TipoUsuario.IdPermisoTipoUsuario }, permiso_TipoUsuario);
        }

        // DELETE: api/PermisoTipoUsuario/5
        [ResponseType(typeof(Permiso_TipoUsuario))]
        public async Task<IHttpActionResult> DeletePermiso_TipoUsuario(int id)
        {
            Permiso_TipoUsuario permiso_TipoUsuario = await db.Permiso_TipoUsuario.FindAsync(id);
            if (permiso_TipoUsuario == null)
            {
                return NotFound();
            }

            db.Permiso_TipoUsuario.Remove(permiso_TipoUsuario);
            await db.SaveChangesAsync();

            return Ok(permiso_TipoUsuario);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool Permiso_TipoUsuarioExists(int id)
        {
            return db.Permiso_TipoUsuario.Count(e => e.IdPermisoTipoUsuario == id) > 0;
        }
    }
}