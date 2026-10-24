using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using SolicitudesInternas.Models;

namespace SolicitudesInternas.Data
{
    /// Contexto de Entity Framework 6. La base de datos se crea con el script
    /// Database/01_CrearBaseDeDatos.sql, por eso se desactiva el inicializador automático.
    public class SolicitudesContext : DbContext
    {
        static SolicitudesContext()
        {
            Database.SetInitializer<SolicitudesContext>(null);
        }

        public SolicitudesContext() : base("name=SolicitudesContext")
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Responsable> Responsables { get; set; }
        public DbSet<TipoSolicitud> TiposSolicitud { get; set; }
        public DbSet<Solicitud> Solicitudes { get; set; }
        public DbSet<Seguimiento> Seguimientos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            modelBuilder.Entity<Responsable>().ToTable("Responsables");
            modelBuilder.Entity<TipoSolicitud>().ToTable("TiposSolicitud");
            modelBuilder.Entity<Solicitud>().ToTable("Solicitudes");
            modelBuilder.Entity<Seguimiento>().ToTable("Seguimientos");

            // Ninguna relación elimina en cascada: el historial y las solicitudes se conservan.
            modelBuilder.Entity<Solicitud>()
                .HasRequired(s => s.Solicitante)
                .WithMany(u => u.Solicitudes)
                .HasForeignKey(s => s.SolicitanteId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Solicitud>()
                .HasRequired(s => s.TipoSolicitud)
                .WithMany(t => t.Solicitudes)
                .HasForeignKey(s => s.TipoSolicitudId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Solicitud>()
                .HasOptional(s => s.Responsable)
                .WithMany(r => r.Solicitudes)
                .HasForeignKey(s => s.ResponsableId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Seguimiento>()
                .HasRequired(s => s.Solicitud)
                .WithMany(s => s.Seguimientos)
                .HasForeignKey(s => s.SolicitudId)
                .WillCascadeOnDelete(false);
        }
    }
}
