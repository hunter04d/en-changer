using System;
using EnChanger.Extensions;
using LanguageExt;
using static LanguageExt.Prelude;

namespace EnChanger.Helpers
{
    public static class GuidConverter
    {
        public static string ToBase64(this Guid guid) =>
            Convert.ToBase64String(guid.ToByteArray())
                .Substring(0, 22)
                .Replace('+', '-')
                .Replace('/', '_');

        public static Either<FormatException, Guid> FromBase64(this Some<string> base64Opt)
        {
            var base64 = base64Opt.Value.ToCharArray();
            if (base64.Length != 22)
            {
                return Left(new FormatException("Input string was not in a correct format."));
            }

            for (var i = base64.Length - 1; i >= 0; i--)
            {
                var @char = base64[i];
                if (@char == '-')
                {
                    base64[i] = '+';
                }

                if (@char == '_')
                {
                    base64[i] = '/';
                }
            }

            return Try(() =>
            {
                var bytes = Convert.FromBase64String(string.Concat(base64.AsSpan(), "=="));
                return new Guid(bytes);
            }).ToEither(e => e.CatchAs<FormatException>());
        }
    }
}
