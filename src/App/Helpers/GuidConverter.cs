using System;
using System.Linq;
using System.Text;
using EnChanger.Extensions;
using LanguageExt;
using Microsoft.AspNetCore.WebUtilities;
using static LanguageExt.Prelude;

namespace EnChanger.Helpers
{
    public static class GuidConverter
    {
        public static string ToBase64(this Guid guid) =>
            WebEncoders.Base64UrlEncode(guid.ToByteArray());

        public static Either<FormatException, Guid> FromBase64(this string base64)
        {
            if (base64 == null)
            {
                throw new ArgumentNullException(nameof(base64));
            }
            if (base64.Length != 22)
            {
                return Left(new FormatException("Input string was not in a correct format."));
            }
            return Try(() =>
            {
                var bytes = WebEncoders.Base64UrlDecode(base64);
                return new Guid(bytes);
            }).ToEither(e => e.CatchAs<FormatException>());
        }
    }
}
