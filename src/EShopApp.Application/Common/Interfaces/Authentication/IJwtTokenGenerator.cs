using System;
using EShopApp.Domain.Entities;

namespace EShopApp.Application.Common.Interfaces.Authentication;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
