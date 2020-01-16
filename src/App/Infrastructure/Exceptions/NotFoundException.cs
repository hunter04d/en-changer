using System;

namespace EnChanger.Infrastructure.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string entity, object key) :
            base($"{entity} with key<{key.GetType().FullName}> {key} not found")
        {
        }
    }
}
