using System;
using EnChanger.Database.Entities;
using LanguageExt;

namespace EnChanger.Services.Abstractions
{
    public interface IPasswordService
    {
        Try<Entry> Add(Some<PasswordDto> passwordInput);
        TryOption<PasswordDto> Get(Some<Guid> id);
    }
}
