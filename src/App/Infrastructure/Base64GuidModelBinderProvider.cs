using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EnChanger.Infrastructure
{
    public class Base64GuidModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(Guid))
            {
                return new Base64GuidModelBinder();
            }

            return null;
        }
    }
}
