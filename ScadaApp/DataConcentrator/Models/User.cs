using System.ComponentModel.DataAnnotations;

namespace DataConcentrator.Models
{
    public enum UserRole { Admin, Operator, Student, Teacher }

    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }
    }
}