using AspNetCoreHero.ToastNotification.Abstractions;

using CsvHelper;
using CsvHelper.Configuration;

using MedicalSchedular.Models;

using MedicalScheduler.Models;
using MedicalScheduler.Service;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Data;
using System.Globalization;

namespace MedicalScheduler.Controllers
{
    [Authorize(Roles = "Admin")]
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
        public async Task<ActionResult> Create(SignUp model, IFormCollection collection)
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
                    await _userManager.AddToRoleAsync(user, "Admin");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    await _userManager.DeleteAsync(user);
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
            var students = _dbContext.Students.ToList();
            var appointments = _dbContext.Appointments.ToList();


            ViewData["Students"] = students;
            ViewData["Appoint"] = appointments;

            //var appointments = _dbContext.Students.FromSqlRaw("SELECT * FROM Appointments WHERE A ).Take(number).ToList();

            return View();
        }
        [AllowAnonymous]
        [Route("/auth")]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
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
                student.Appointment = "Scheduled";
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
                _dbContext.Students.Update(student);
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

                if (model.Date_Sched.HasValue && model.Date_Sched.Value.DayOfWeek == DayOfWeek.Saturday || model.Date_Sched.Value.DayOfWeek == DayOfWeek.Sunday)
                {
                    _notyf.Error($"Error, This date is a weekend day.");
                    return View(nameof(Appointment));
                }

                if (model.Date_Sched < DateTime.Now)
                {
                    _notyf.Error($"Error, Date cannot be less than today");
                    return View(nameof(Appointment));
                }

                if (model.Date_Sched.HasValue && model.Date_Sched.Value.Hour > 12)
                {
                    _notyf.Error($"Error, Scheduled Time should be AM");
                    return View(nameof(Appointment));
                }

                var rand = new Random();
                //= _dbContext.Students.FromSqlRaw("SELECT * FROM Students WHERE SchoolFee = {0} AND Appointment = {1}", "Paid", "Not_Scheduled").OrderBy(x => Guid.NewGuid().ToString()).Take(number).ToList();

                var paidStudents = _dbContext.Students.Where(x => x.SchoolFee == "Paid" && x.Appointment == "Not_Scheduled").OrderBy(x => x.MatricNo).Take(number).ToList();
                var appointment = new List<Appointment> { };
                foreach (var item in paidStudents)
                {
                    var student = _dbContext.Students.FirstOrDefault(x => x.StudentId == item.StudentId);

                    Appointment appointment1 = new()
                    {
                        Student = student,
                        Status = "Scheduled",
                        Date_Sched = model.Date_Sched,
                        DateCreated = DateTime.Now,
                    };
                    appointment.Add(appointment1);
                }
                //var allAppintments = _dbContext.Appointments.FromSqlRaw("SELECT * FROM Appointments").ToList();

                _dbContext.Appointments.AddRange(appointment);
                foreach (var item in appointment)
                {
                    var student = _dbContext.Students.FirstOrDefault(x => x.StudentId == item.Student.StudentId);
                    student.Appointment = "Scheduled";
                    _dbContext.SaveChanges();
                }
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
            var students = _dbContext.Students.ToList();
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
                        students.Add(new Student { FullName = item.FullName, Department = item.Department, Contact = item.Contact, Email = item.Email, SchoolFee = item.SchoolFee, Level = item.Level, Appointment = item.Appointment, MatricNo = item.MatricNo });
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
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now.AddDays(5);

            int totalDays = (int)(endDate - startDate).TotalDays;

            DateTime randomDate;
            do
            {
                int randomDays = rnd.Next(totalDays);

                randomDate = startDate.AddDays(randomDays);
            } while (randomDate.DayOfWeek == DayOfWeek.Saturday || randomDate.DayOfWeek == DayOfWeek.Sunday);

            int randomHour = rnd.Next(12);
            int randomMinute = rnd.Next(0, 60);
            int randomSecond = rnd.Next(0, 60);

            DateTime randomDateTime = new(randomDate.Year, randomDate.Month, randomDate.Day, randomHour, randomMinute, randomSecond);
            return randomDateTime;
        }

        [AllowAnonymous]

        [Route("forgotpass")]
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }



        [AllowAnonymous]
        [Route("reset")]
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            var model = new ResetPassword { Token = token, Email = email };
            return View(model);
        }


        [AllowAnonymous]
        [Route("forgotpass")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPassword model, MailRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Admin",
                    new { email = user.Email, token }, protocol: HttpContext.Request.Scheme);

                request.ToEmail = model.Email;
                request.Subject = "Reset Password Token";
                await _mail.SendEmailAsync(request, $"<a href='{callbackUrl}'>Click this link to reset your paswword,<br>");

                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }
            catch
            {
                ModelState.AddModelError("", "One or more errors occurred.");
                return RedirectToAction(nameof(Login));
            }
        }


        [AllowAnonymous]

        [Route("forgotconfirmation")]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }


        [AllowAnonymous]

        [Route("resetconfirm")]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }


        [AllowAnonymous]

        [Route("reset")]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPassword model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                RedirectToAction(nameof(ResetPasswordConfirmation));

            var resetPassResult = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (!resetPassResult.Succeeded)
            {
                foreach (var error in resetPassResult.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }

                return View();
            }
            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }
    }
}