using Auth.Application.DTOs;
using Auth.Application.DTOs.Auth;
using Auth.Domain.Entities;
using Mapster;

namespace Auth.Application.Mapping;

public class MappingConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<User, UserDto>
            .NewConfig()
            .Map(dest => dest.Status, src => src.Status.ToString());
        
        TypeAdapterConfig<RegisterRequest, User>
            .NewConfig()
            .Map(dest => dest.Email, src => src.Email != null ? src.Email.ToLower().Trim() : null)
            .Map(dest => dest.Id, src => Guid.NewGuid())
            .Map(dest => dest.CreatedAt, src => DateTime.UtcNow)
            .Map(dest => dest.UpdatedAt, src => DateTime.UtcNow)
            .Ignore(dest => dest.PasswordHash);

        TypeAdapterConfig<User, AuthResponse>
            .NewConfig()
            .Map(dest => dest.User, src => src.Adapt<UserDto>())
            .Ignore(dest => dest.Token)
            .Ignore(dest => dest.ExpiresAt);
    }
}