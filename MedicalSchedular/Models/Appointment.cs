namespace MedicalSchedular.Models
{
	public class Appointment
	{
		public int Id { get; set; }
		public DateTime DateScheduled { get; set; }
		public string Ailment { get; set; }
		public string Status { get; set; }
		public DateTime DateCreated { get; set; } = DateTime.Now;
	}

	public class Patiens
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string Contact { get; set; }
		public string Gender { get; set; }
		public DateTime DOB { get; set; }
		public string Address { get; set; }
	}
}