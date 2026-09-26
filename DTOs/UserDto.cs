namespace UserManagementAPI.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }

        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }

        public string? PhoneNumber { get; set; }

        public int Age { get; set; }

        public required string City { get; set; }
        public required string Country { get; set; }

        public bool IsActive { get; set; }

        public DateTime DateOfBirth { get; set; }
        public DateTime CreatedAt { get; set; }

        public required string Role { get; set; }
    }
}