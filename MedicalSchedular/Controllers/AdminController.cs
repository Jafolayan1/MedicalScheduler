using AspNetCoreHero.ToastNotification.Abstractions;

using MedicalSchedular.Models;

using MedicalScheduler.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalScheduler.Controllers
{

    public class AdminController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly INotyfService _notyf;
        private readonly ApplicationDbContext _dbContext;


        public AdminController(SignInManager<User> signInManager, UserManager<User> userManager, INotyfService notyf, ApplicationDbContext dbContext)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _notyf = notyf;
            _dbContext = dbContext;
        }


        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

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
            _notyf.Error("Invalid Credentials");
            return View();
        }

        public ActionResult Appointment()
        {
            var appointments = _dbContext.Appointments.Where(x => x.Status.Equals("Scheduled")).ToList();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Schedule(Appointment model, IFormCollection data)
        {
            int number = int.Parse(data["number"]);

            var paidStudents = _dbContext.Patients.FromSqlRaw("SELECT * FROM Patients WHERE SchoolFee = {0}", "Paid").Take(number).ToList();

            var appointment = new List<Appointment> { };
            foreach (var item in paidStudents)
            {
                var student = _dbContext.Patients.FirstOrDefault(x => x.Id == item.Id);
                Appointment appointment1 = new()
                {
                    Matric = student.MatricNo,
                    Status = "Scheduled",
                    DateScheduled = model.DateScheduled,
                    DateCreated = DateTime.Now
                };
                appointment.Add(appointment1);
            }

            _dbContext.AddRange(appointment);
            await _dbContext.SaveChangesAsync();
            return View(nameof(Appointment));
        }

        [Route("logout")]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("~/");
        }

        public ActionResult Settings()
        {
            return View();
        }

        // POST: AdminController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AdminController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AdminController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}