using System;
using EnChanger.Infrastructure.Exceptions;
using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EnChanger.Infrastructure.Filters
{
    public class MonadicResultFilterAttribute : Attribute, IAlwaysRunResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext context)
        {
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ObjectResult objectResult)
            {
                var value = objectResult.Value;
                context.Result = value switch
                {
                    IEither either => MatchEither(either),
                    IOptional optional => MatchOptional(optional),
                    _ => context.Result
                };
            }
        }

        private static IActionResult MatchOptional(IOptional optional) =>
            optional.MatchUntyped<IActionResult>(
                o => new OkObjectResult(o),
                () => new NotFoundResult()
            );

        private static IActionResult MatchEither(IEither either) =>
            either.MatchUntyped(
                r => r is IOptional rOptional
                    ? MatchOptional(rOptional)
                    : new OkObjectResult(r),
                l => l is Exception e
                    ? MatchException(e)
                    : throw new InvalidOperationException(
                        $"{l.GetType().FullName} cannot be used as a left of an Either<> returned from controller")
            );

        // TODO: match depending on the type of the exception
        private static IActionResult MatchException(Exception exception) => exception.Match<IActionResult>()
            .With<BadRequestException>(e =>
                new BadRequestObjectResult(new ValidationProblemDetails(e.Errors)))
            .OtherwiseReThrow();
    }
}
