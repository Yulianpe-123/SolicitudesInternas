using System;

namespace SolicitudesInternas.Excepciones
{
    /// Excepción base de la aplicación. Su mensaje siempre es comprensible para el usuario final,
    /// por lo que puede mostrarse directamente en pantalla.
    public abstract class AplicacionException : Exception
    {
        protected AplicacionException(string mensaje) : base(mensaje)
        {
        }

        protected AplicacionException(string mensaje, Exception interna) : base(mensaje, interna)
        {
        }
    }

    ///Se lanza cuando una operación incumple una regla de negocio.
    public class ReglaNegocioException : AplicacionException
    {
        public ReglaNegocioException(string mensaje) : base(mensaje)
        {
        }
    }

    ///Se lanza cuando un registro solicitado no existe.
    public class RecursoNoEncontradoException : AplicacionException
    {
        public RecursoNoEncontradoException(string mensaje) : base(mensaje)
        {
        }
    }

    /// Se lanza cuando falla el acceso a datos. El mensaje es amigable; el detalle técnico
    /// queda únicamente en la excepción interna.
    public class PersistenciaException : AplicacionException
    {
        public PersistenciaException(string mensaje, Exception interna) : base(mensaje, interna)
        {
        }
    }
}
