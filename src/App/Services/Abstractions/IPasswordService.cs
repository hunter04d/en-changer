using System;
using EnChanger.Database.Entities;
using EnChanger.Infrastructure.Exceptions;
using LanguageExt;

namespace EnChanger.Services.Abstractions
{
    public interface IPasswordService
    {
        Entry Add(Some<PasswordDto> passwordInput);
        Either<BadRequestException, Option<PasswordDto>> Get(Some<Guid> id);
    }
}
