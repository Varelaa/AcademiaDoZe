//Gabriel Souza Varela

namespace AcademiaDoZe.Domain.Exceptions
{
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message)
        {
        }

        public DomainException(string message, Exception innerException) : base(message, innerException)
        {
        }
        internal static void ThrowIf(bool condition, string message)
        {
            if (condition)
                throw new DomainException(message);
        }
    }
}
