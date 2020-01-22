using System;
using EnChanger.Database.Entities;
using EnChanger.Infrastructure.Exceptions;
using EnChanger.Services.Models;
using LanguageExt;

namespace EnChanger.Services.Abstractions
{
    public interface IPasswordService
    {
        Guid Add(PasswordInput passwordInput);
        Either<BadRequestException, Option<PasswordDto>> Get(Guid id);
    }
}
