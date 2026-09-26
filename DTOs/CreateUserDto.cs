using System.ComponentModel.DataAnnotations;

namespace UserManagementAPI.DTOs
{
    public class CreateUserDto
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public required string FirstName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public required string LastName { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        [Range(18, 100)]
        public int Age { get; set; }

        [Required]
        public required string City { get; set; }

        [Required]
        public required string Country { get; set; }

        public bool IsActive { get; set; }

        public DateTime DateOfBirth { get; set; }

        [Required]
        public required string Role { get; set; }
    }
}