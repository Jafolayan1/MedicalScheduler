using Microsoft.AspNetCore.Identity;

namespace MedicalSchedular.Models
{
    public class SignUp
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNO { get; set; }
    }
    public class Appointment
    {
        public int Id { get; set; }
        public string Matric { get; set; }
        public string Status { get; set; }
        public DateTime? DateScheduled { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }

    public class Patient
    {
        public int Id { get; set; }
        public string MatricNo { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Contact { get; set; }
        public string Level { get; set; }
        public string SchoolFee { get; set; }
        public DateTime? Date_Sched { get; set; }
    }

    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LatsName { get; set; }

    }
}