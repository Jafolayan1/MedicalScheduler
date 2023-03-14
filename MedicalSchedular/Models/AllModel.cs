using CsvHelper.Configuration.Attributes;

using Microsoft.AspNetCore.Identity;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSchedular.Models
{
    public class SignUp
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string? PhoneNO { get; set; }
    }

    public class Appointment
    {
        public int AppointmentId { get; set; }
        public int? StudentId { get; set; }
        public virtual Student Student { get; set; }
        public string Status { get; set; }
        public DateTime? Date_Sched { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }

    public class Student
    {
        [Index(0)]
        public int StudentId { get; set; }

        [Index(1)]
        public string FullName { get; set; }

        [Index(2)]
        public string MatricNo { get; set; }

        [Index(3)]
        public string Department { get; set; }

        [Index(4)]
        public string Email { get; set; }

        [Index(5)]
        public string Level { get; set; }

        [Index(6)]
        public string Contact { get; set; }

        [Index(7)]
        public string SchoolFee { get; set; }

        [Index(8)]
        public string Appointment { get; set; }
    }

    public class User : IdentityUser<int>
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LatsName { get; set; }


        [NotMapped]
        public string[] Role { get; set; }
    }

    public class Role : IdentityRole<int>
    {
    }

    public class MailRequest
    {
        [Required]
        public string ToEmail { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Body { get; set; }

        public List<IFormFile> Attachments { get; set; }
    }

    public class ForgotPassword
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

    public class ResetPassword
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Email { get; set; }
        public string Token { get; set; }
    }
}