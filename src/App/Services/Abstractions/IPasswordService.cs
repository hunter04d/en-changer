using System;
using EnChanger.Database.Entities;
using EnChanger.Infrastructure.Exceptions;
using LanguageExt;

namespace EnChanger.Services.Abstractions
{
    public interface IPasswordService
    {
        Entry Add(PasswordInput passwordInput);
        Either<BadRequestException, Option<PasswordDto>> Get(Guid id);
    }
}
