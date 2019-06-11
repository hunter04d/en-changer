using System;
using LanguageExt;

namespace EnChanger
{
    public static class ResultExtensions
    {
        public static Result<B> Bind<A, B>(this Result<A> result, Func<A, Result<B>> func)
            => result.Match(func, e => new Result<B>(e));

        public static OptionalResult<B> Bind<A, B>(this Result<A> result, Func<A, OptionalResult<B>> func)
            => result.Match(func, e => new OptionalResult<B>(e));
    }
}
