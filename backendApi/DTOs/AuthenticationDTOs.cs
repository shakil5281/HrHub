using System.ComponentModel.DataAnnotations;

namespace HrHubAPI.DTOs
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Department { get; set; }

        [MaxLength(200)]
        public string? Position { get; set; }

        [Required]
        public int CompanyId { get; set; }
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public UserDto User { get; set; } = new();
    }

    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Department { get; set; }
        public string? Position { get; set; }
        public int? CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public List<string> Roles { get; set; } = new();
    }

    public class AssignRoleDto
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string RoleName { get; set; } = string.Empty;
    }

    public class RefreshTokenDto
    {
        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class AssignCompanyDto
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int CompanyId { get; set; }
    }

    public class AssignMultipleUsersToCompanyDto
    {
        [Required]
        public List<string> UserIds { get; set; } = new();

        [Required]
        public int CompanyId { get; set; }
    }

    public class TokenValidationDto
    {
        [Required]
        public string Token { get; set; } = string.Empty;
    }

    public class TokenValidationResponseDto
    {
        public bool IsValid { get; set; }
        public string? UserId { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public List<string> Roles { get; set; } = new();
        public DateTime? ExpirationDate { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class AssignMultipleCompaniesToUserDto
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [MinLength(1, ErrorMessage = "At least one company must be specified")]
        public List<int> CompanyIds { get; set; } = new();
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
