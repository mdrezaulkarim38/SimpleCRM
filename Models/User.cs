using System.ComponentModel.DataAnnotations;

namespace SimpleCRM.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "First Name is required.")]
        public string? FirstName { get; set; }
        [Required(ErrorMessage = "last Name is required.")]
        public string? LastName { get; set; }
        [Required(ErrorMessage = "userName is required.")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }                 
    }
}
