using AspNetCoreHero.ToastNotification.Abstractions;

using MedicalSchedular.Models;

using MedicalScheduler.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Diagnostics;

namespace MedicalSchedular.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notyf;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, INotyfService notyf)
        {
            _logger = logger;
            _context = context;
            _notyf = notyf;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(IFormCollection data)
        {
            var matric = data["matric"].ToString();
            if (matric is not null)
            {
                var sched = _context.Appointments.Include(p => p.Student).FirstOrDefault(m => m.Student.MatricNo == matric);
                if (sched is null)
                {
                    _notyf.Error("You do not have any appointment");
                    return View(nameof(Index));
                }
                return View(nameof(Schedule), sched);
            }
            else
            {
                _notyf.Error("null, Input Matric No");
                return View(nameof(Index));
            }

            //if (!ModelState.IsValid)
            //{
            //    _notyf.Error("Error Submitting");
            //    return View();
            //}
            //_context.Students.Add(model);
            //_context.SaveChanges();
            //_notyf.Success("Appointment Booked Succesfully, Procced to print slip");
            //return View(nameof(Schedule), model);
        }

        [HttpGet]
        public IActionResult Schedule()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Schedule(IFormCollection data)
        {
            var matric = data["matric"].ToString();
            if (matric is not null)
            {
                var sched = _context.Appointments.Include(p => p.Student).FirstOrDefault(m => m.Student.MatricNo == matric);
                if (sched is null)
                {
                    _notyf.Error("You do not have any appointment");
                    return View(nameof(Index));
                }
                return View(nameof(Schedule), sched);
            }
            else
            {
                _notyf.Error("null, Input Matric No");
                return View(nameof(Index));
            }
        }



        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Route("404")]
        public IActionResult PageNotFound()
        {
            string originalPath = "unknown";
            if (HttpContext.Items.ContainsKey("originalPath"))
            {
                originalPath = HttpContext.Items["originalPath"] as string;
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}