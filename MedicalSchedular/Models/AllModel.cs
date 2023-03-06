using CsvHelper.Configuration.Attributes;

using Microsoft.AspNetCore.Identity;

namespace MedicalSchedular.Models
{
    public class SignUp
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNO { get; set; }
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
        public virtual Appointment Appointment { get; set; }

    }

    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LatsName { get; set; }

    }
    public class Role : IdentityRole<int>
    {
    }

    public class MailRequest
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<IFormFile> Attachments { get; set; }
    }
}