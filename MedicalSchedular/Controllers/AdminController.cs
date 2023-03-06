using AspNetCoreHero.ToastNotification.Abstractions;

using CsvHelper;
using CsvHelper.Configuration;

using MedicalSchedular.Models;

using MedicalScheduler.Models;
using MedicalScheduler.Service;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Globalization;

namespace MedicalScheduler.Controllers
{
    public class AdminController : Controller
    {
        protected static List<Student> _studentAccounts = new();
        private readonly ApplicationDbContext _dbContext;
        private readonly IFileService _file;
        private readonly IMailService _mail;
        private readonly INotyfService _notyf;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        public AdminController(SignInManager<User> signInManager, UserManager<User> userManager, INotyfService notyf, ApplicationDbContext dbContext, IMailService mail, IFileService file)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _notyf = notyf;
            _dbContext = dbContext;
            _mail = mail;
            _file = file;
        }

        [HttpGet]
        public IActionResult AddStudents(List<Student> accounts = null)
        {
            accounts ??= new List<Student>();

            return View(accounts);
        }

        [HttpPost]
        public async Task<IActionResult> AddStudents(IFormFile file)
        {
            try
            {
                await _file.UploadFile(file);

                var accounts = GetStudentList(file.FileName);
                return RedirectToAction(nameof(UploadStudents));
            }
            catch (Exception)
            {
                _notyf.Error("File is not valid, please check file type and try again");
                return View(nameof(AddStudents));
            }
        }

        [Route("/appointment")]
        [HttpGet]
        public ActionResult Appointment(int number)
        {
            if (number > 0)
            {
                _notyf.Success($"Appointment scheduled for {number} students");
            }
            var appointments = _dbContext.Appointments.Include(p => p.Student).Where(x => x.Status == "Scheduled" || x.Status == "Not_Scheduled").ToList();
            ViewData["appointment"] = appointments;
            return View();
        }


        [HttpPost]
        public ActionResult Create(SignUp model, IFormCollection collection)
        {
            try
            {
                User user = new()
                {
                    FirstName = model.FirstName,
                    LatsName = model.LastName,
                    UserName = model.Email,
                    PhoneNumber = model.PhoneNO,
                    Email = model.Email
                };

                var request = _userManager.CreateAsync(user, model.Password).Result;
                if (request.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _notyf.Error("Error Occured, Try again");
                    return View(nameof(Users));
                }

            }
            catch
            {
                _notyf.Error("Error Occured, Try again");
                return View(nameof(Users));
            }
        }

        public async Task<ActionResult> Delete(dynamic id)
        {
            var user = await _userManager.FindByIdAsync(id);
            await _userManager.DeleteAsync(user);

            return RedirectToAction(nameof(Users));
        }


        [HttpPost]
        public async Task<ActionResult> Edit(User model)
        {
            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            user.PhoneNumber = model.PhoneNumber;
            user.UserName = model.Email;
            user.Email = model.Email;
            user.FirstName = model.FirstName;
            user.LatsName = model.LatsName;
            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Users));
        }



        [Route("/index")]
        public ActionResult Index()
        {
            return View();
        }

        [Route("/auth")]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [Route("/auth")]
        [HttpPost]
        public ActionResult Login(SignUp model)
        {
            var result = _signInManager.PasswordSignInAsync(model.Username, model.Password, false, lockoutOnFailure: false).Result;

            if (result.Succeeded)
            {
                var user = _userManager.FindByNameAsync(model.Username).Result;
                if (user != null)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            TempData["Err"] = "Invalid credentials";
            return View();
        }

        [Route("/logout")]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("~/");
        }

        [Route("/schedule")]
        [HttpGet]
        public async Task<IActionResult> Schedule(int studentId)
        {
            if (studentId > 0)
            {
                var student = _dbContext.Students.FirstOrDefault(x => x.StudentId == studentId);
                var appointment = _dbContext.Appointments.Where(x => x.Student.MatricNo == student.MatricNo);

                Appointment appoint = new()

                {
                    Student = student,
                    Status = "Scheduled",
                    Date_Sched = GenerateRandomDate(),
                    DateCreated = DateTime.Now
                };
                _dbContext.Appointments.RemoveRange(appointment);
                _dbContext.Appointments.Add(appoint);
                await _dbContext.SaveChangesAsync();
                SendMail($"<p>You have been rescheduled for your medical on the {appoint.Date_Sched}</p> <br>" +
                    $"<p> Come along with a copy of your school fee recipt and your medical appointment schedule  <a href='http://gabrielgeestar-001-site1.atempurl.com/'>HERE</a>", student.Email);
                return RedirectToAction(nameof(Appointment));
            }
            return View();
        }

        [Route("/schedule")]
        [HttpPost]
        public async Task<ActionResult> Schedule(Appointment model, IFormCollection data)
        {
            try
            {
                int number = int.Parse(data["number"]);
                if (number <= 0)
                {
                    _notyf.Error($"Error, No of students cannot be less than 0");
                    return View(nameof(Appointment));


                }
                if (model.Date_Sched.HasValue && model.Date_Sched.Value.Hour < 14)
                {
                    _notyf.Error($"Error, Time should be between 8AM and 3PM");
                    return View(nameof(Appointment));

                }


                var paidStudents = _dbContext.Students.FromSqlRaw("SELECT * FROM Students WHERE SchoolFee = {0}", "Paid").Take(number).ToList();
                var appointment = new List<Appointment> { };
                foreach (var item in paidStudents)
                {
                    var student = _dbContext.Students.FirstOrDefault(x => x.StudentId == item.StudentId);
                    Appointment appointment1 = new()
                    {
                        Student = student,
                        Status = "Scheduled",
                        Date_Sched = model.Date_Sched,
                        DateCreated = DateTime.Now
                    };
                    appointment.Add(appointment1);
                }

                _dbContext.Appointments.AddRange(appointment);
                await _dbContext.SaveChangesAsync();
                foreach (var item in appointment)
                {
                    SendMail($"<p>You have been scheduled for your medical on the {model.Date_Sched}</p> <br>" +
                        $"<p> Come along with a copy of your school fee recipt and your medical appointment schedule  <a href='http://gabrielgeestar-001-site1.atempurl.com/'>HERE</a>", item.Student.Email);
                }
                return RedirectToAction(nameof(Appointment), number);
            }
            catch (Exception)
            {

                _notyf.Error($"Error, failed to schedule appointment");
                return View(nameof(Appointment));
            }

        }

        [Route("/settings")]
        public ActionResult Settings()
        {
            return View();
        }

        [Route("/signup")]
        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [Route("/students")]
        [HttpGet]
        public IActionResult Students()
        {
            var students = _dbContext.Students.Include(x => x.Appointment).ToList();
            var appointment = _dbContext.Appointments.Include(x => x.Student).ToList();
            ViewData["appointment"] = appointment;

            return View(students);
        }

        public ActionResult UploadStudents()
        {
            try
            {
                List<Student> students = new();

                foreach (var item in _studentAccounts)
                {
                    var request = _dbContext.Students.Where(x => x.MatricNo == item.MatricNo);
                    if (!request.Any())
                    {
                        students.Add(new Student { FullName = item.FullName, Department = item.Department, Contact = item.Contact, Email = item.Email, SchoolFee = item.SchoolFee, Level = item.Level, MatricNo = item.MatricNo });
                    }
                    else
                    {
                        TempData["Error"] = $"Duplicate User with Matric No {item.MatricNo} Found";
                    }
                }

                _dbContext.Students.AddRange(students);
                _dbContext.SaveChanges();
                return RedirectToAction(nameof(Students));
            }
            catch (Exception)
            {
                TempData["Error"] = "One or more error occured.";
                return RedirectToAction(nameof(Students));
            }

        }

        [Route("/users")]
        [HttpGet]
        public IActionResult Users()
        {
            var users = _dbContext.Users.ToList();
            return View(users);
        }

        [HttpGet]
        private List<Student> GetStudentList(string fileName)
        {
            try
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    MissingFieldFound = null
                };

                var path = Directory.GetCurrentDirectory() + "/wwwroot/uploads//" + fileName;
                using (var reader = new StreamReader(path))
                using (var csv = new CsvReader(reader, config))
                {
                    csv.Read();
                    csv.ReadHeader();

                    while (csv.Read())
                    {
                        var students = csv.GetRecord<Student>();
                        _studentAccounts.Add(students);
                    }
                }

                path = Directory.GetCurrentDirectory() + "/wwwroot/uploads";
                using (var write = new StreamWriter(path + "//NewFile.csv"))
                using (var csv = new CsvWriter(write, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(_studentAccounts);
                }
            }
            catch (BadDataException ex)
            {
                ModelState.AddModelError("", ex.Message);
                _notyf.Error("Something went wrong. Check uploaded file and fields");
            }
            return _studentAccounts;
        }

        public async void SendMail(string body, string toMail)
        {
            var email = new MailRequest()
            {
                ToEmail = toMail,
                Subject = "FPE MEDICAL SCHEDULE",
                Body = body
            };
            await _mail.SendEmailAsync(email, email.Body);
        }


        public DateTime GenerateRandomDate()
        {
            Random rnd = new();
            // Define a start and end date
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now.AddDays(5);

            // Calculate the number of days between the start and end date
            int totalDays = (int)(endDate - startDate).TotalDays;

            // Loop until a non-weekend date is generated
            DateTime randomDate;
            do
            {
                // Generate a random number of days between the start and end date
                int randomDays = rnd.Next(totalDays);

                // Add the random number of days to the start date
                randomDate = startDate.AddDays(randomDays);

            } while (randomDate.DayOfWeek == DayOfWeek.Saturday || randomDate.DayOfWeek == DayOfWeek.Sunday);

            // Generate a random time
            int randomHour = rnd.Next(8, 14);
            int randomMinute = rnd.Next(0, 60);
            int randomSecond = rnd.Next(0, 60);

            // Combine the random date and time
            DateTime randomDateTime = new DateTime(randomDate.Year, randomDate.Month, randomDate.Day, randomHour, randomMinute, randomSecond);
            return randomDateTime;
        }
    }
}