using System;
using System.Threading.Tasks;
using EnChanger.Helpers;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EnChanger.Infrastructure
{
    public class Base64GuidModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentException(nameof(bindingContext));
            }

            var modelName = bindingContext.ModelName;
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);
            var value = valueProviderResult.FirstValue;
            if (string.IsNullOrEmpty(value))
            {
                return Task.CompletedTask;
            }

            value.FromBase64().Match(
                guid => bindingContext.Result = ModelBindingResult.Success(guid),
                e => bindingContext.ModelState.TryAddModelError(modelName, e.Message));
            return Task.CompletedTask;
        }
    }
}
