using Microsoft.AspNetCore.Mvc;
using mvc2025TermProject.Data;
using mvc2025TermProject.Models;
using System.Threading.Tasks;

namespace mvc2025TermProject.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Contact contact)
        {
            if (ModelState.IsValid)
            {
                contact.SubmittedAt = DateTime.Now;
                contact.IsRead = false;

                _context.Contacts.Add(contact);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ThankYou));
            }

            return View(contact);
        }

        [HttpGet]
        public IActionResult ThankYou()
        {
            return View();
        }
    } // END of class ContactController
}
