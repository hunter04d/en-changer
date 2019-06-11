using System;
using static LanguageExt.Prelude;
using LanguageExt;

namespace EnChanger
{
    public class GuidConverter
    {
        public static string ToBase64(Guid guid) =>
            Convert.ToBase64String(guid.ToByteArray())
                .Substring(0, 22)
                .Replace('+', '-')
                .Replace('/', '_');

        public static Result<Guid> FromBase64(Some<string> base64Opt)
        {
            var base64 = base64Opt.Value;
            if (base64.Length != 22)
            {
                return new Result<Guid>(new FormatException("Input string was not in a correct format."));
            }

            return new Guid(Convert.FromBase64String(
                base64
                    .Replace('-', '+')
                    .Replace('_', '/') + "==")
            );
        }
    }
}
