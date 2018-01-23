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
    public class ReservasController : ApiController
    {
        private DataContext db = new DataContext();

        // GET: api/Reservas
        public IQueryable<Reserva> GetReservas()
        {
            return db.Reservas;
        }

        // GET: api/Reservas/5
        [ResponseType(typeof(Reserva))]
        public async Task<IHttpActionResult> GetReserva(int id)
        {
            Reserva reserva = await db.Reservas.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }

            return Ok(reserva);
        }

        // PUT: api/Reservas/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutReserva(int id, Reserva reserva)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != reserva.IdReserva)
            {
                return BadRequest();
            }

            db.Entry(reserva).State = EntityState.Modified;

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

            return Ok(reserva);
        }

        // POST: api/Reservas
        [ResponseType(typeof(Reserva))]
        public async Task<IHttpActionResult> PostReserva(Reserva reserva)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (ReservaExists(reserva.IdZona, reserva.FechaReserva))
            {
                return BadRequest("Zona ya ha sido reservada.");
            }
            db.Reservas.Add(reserva);

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

            return CreatedAtRoute("DefaultApi", new { id = reserva.IdReserva }, reserva);
        }

        // DELETE: api/Reservas/5
        [ResponseType(typeof(Reserva))]
        public async Task<IHttpActionResult> DeleteReserva(int id)
        {
            Reserva reserva = await db.Reservas.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }
            
            db.Reservas.Remove(reserva);

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

            return Ok(reserva);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ReservaExists(int id, DateTime fechaReserva)
        {
            return db.Reservas.Count(e => e.IdZona == id && e.FechaFinReserva == fechaReserva) > 0;
        }
    }
}