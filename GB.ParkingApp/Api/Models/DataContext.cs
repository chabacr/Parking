namespace Api.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DataContext : DbContext
    {
        public DataContext() : base("DefaultConnection")
        {
        }

        public virtual DbSet<Observacion> Observacions { get; set; }
        public virtual DbSet<Observacion_Zona> Observacion_Zona { get; set; }
        public virtual DbSet<Parqueo> Parqueos { get; set; }
        public virtual DbSet<Permiso> Permisoes { get; set; }
        public virtual DbSet<Permiso_TipoUsuario> Permiso_TipoUsuario { get; set; }
        public virtual DbSet<Reserva> Reservas { get; set; }
        public virtual DbSet<TipoUsuario> TipoUsuarios { get; set; }
        public virtual DbSet<TipoVehiculo> TipoVehiculoes { get; set; }
        public virtual DbSet<Usuario> Usuarios { get; set; }
        public virtual DbSet<Vehiculo> Vehiculoes { get; set; }
        public virtual DbSet<Zona> Zonas { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Observacion>()
                .HasMany(e => e.Observacion_Zona)
                .WithRequired(e => e.Observacion)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Parqueo>()
                .HasMany(e => e.Zonas)
                .WithRequired(e => e.Parqueo)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Permiso>()
                .HasMany(e => e.Permiso_TipoUsuario)
                .WithRequired(e => e.Permiso)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TipoUsuario>()
                .HasMany(e => e.Permiso_TipoUsuario)
                .WithRequired(e => e.TipoUsuario)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TipoUsuario>()
                .HasMany(e => e.Usuarios)
                .WithRequired(e => e.TipoUsuario)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TipoVehiculo>()
                .HasMany(e => e.Vehiculoes)
                .WithRequired(e => e.TipoVehiculo)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Usuario>()
                .HasMany(e => e.Observacion_Zona)
                .WithRequired(e => e.Usuario)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Usuario>()
                .HasMany(e => e.Reservas)
                .WithRequired(e => e.Usuario)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Usuario>()
                .HasMany(e => e.Vehiculoes)
                .WithRequired(e => e.Usuario)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Vehiculo>()
                .HasMany(e => e.Reservas)
                .WithRequired(e => e.Vehiculo)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Zona>()
                .HasMany(e => e.Observacion_Zona)
                .WithRequired(e => e.Zona)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Zona>()
                .HasMany(e => e.Reservas)
                .WithRequired(e => e.Zona)
                .WillCascadeOnDelete(false);
        }
    }
}
