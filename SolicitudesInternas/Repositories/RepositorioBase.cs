using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using SolicitudesInternas.Data;
using SolicitudesInternas.Excepciones;

namespace SolicitudesInternas.Repositories
{
    /// Base común de los repositorios. Centraliza la traducción de errores de persistencia
    /// a mensajes comprensibles, para que ninguna excepción técnica llegue al usuario.
    public abstract class RepositorioBase
    {
        protected readonly SolicitudesContext Contexto;

        protected RepositorioBase(SolicitudesContext contexto)
        {
            Contexto = contexto;
        }

        protected void GuardarCambios()
        {
            try
            {
                Contexto.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                throw new PersistenciaException(
                    "Los datos no cumplen las restricciones de la base de datos. Revise la información ingresada.", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new PersistenciaException(
                    "No fue posible guardar la información. Verifique que los datos no estén duplicados o en uso e intente nuevamente.", ex);
            }
        }

        //Guarda una entidad existente; si no está en seguimiento, la adjunta como modificada
        protected void GuardarEntidad<T>(T entidad) where T : class
        {
            var entrada = Contexto.Entry(entidad);
            if (entrada.State == EntityState.Detached)
            {
                Contexto.Set<T>().Attach(entidad);
                Contexto.Entry(entidad).State = EntityState.Modified;
            }

            GuardarCambios();
        }
    }
}
