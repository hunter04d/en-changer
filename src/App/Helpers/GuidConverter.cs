using System;
using LanguageExt;
using static LanguageExt.Prelude;

namespace EnChanger.Helpers
{
    public static class GuidConverter
    {
        public static string ToBase64(Guid guid) =>
            Convert.ToBase64String(guid.ToByteArray())
                .Substring(0, 22)
                .Replace('+', '-')
                .Replace('/', '_');

        public static Try<Guid> FromBase64(Some<string> base64Opt)
        {
            var base64 = base64Opt.Value.ToCharArray();
            if (base64.Length != 22)
            {
                return Try<Guid>(new FormatException("Input string was not in a correct format."));
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

            return () => new Guid(Convert.FromBase64String(string.Concat(base64.AsSpan(), "==")));
        }
    }
}
