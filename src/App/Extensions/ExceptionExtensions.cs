using System;

namespace EnChanger.Extensions
{
    public static class ExceptionExtensions
    {
        public static T CatchAs<T>(this Exception exception) where T : Exception
            => exception is T tException ? tException : throw exception;
    }
}
