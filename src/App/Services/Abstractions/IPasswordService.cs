using System;
using EnChanger.Database.Entities;
using LanguageExt;

namespace EnChanger.Services.Abstractions
{
    public interface IPasswordService
    {
        Result<Entry> Add(Some<PasswordDto> passwordInput);
        OptionalResult<PasswordDto> Get(Some<Guid> id);
    }
}
