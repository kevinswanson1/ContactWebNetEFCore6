using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContactWebNetEFCore6Models;
using MyContactManagerData;

namespace ContactWebNetEFCore6.Controllers
{
	/// <summary>
	/// https://www.udemy.com/course/aspnet-mvc-quick-start/learn/lecture/29599070#notes
	/// </summary>
	public class ContactsController : Controller
    {
        private readonly MyContactManagerDbContext _context;
		private static List<State> _allStates;
		private static SelectList _statesData;

        public ContactsController(MyContactManagerDbContext context)
        {
            _context = context;

			// Sneaky cache state IDs to avoid the calls to the database for
			// Create[GET | POST(upon error)] or Edit[GET | POST(upon error)]
			// Task.Run() because we're in a non-Async constructor
			_allStates = Task.Run(() => _context.States.ToListAsync()).Result;	// Load states
			_statesData = new SelectList(_allStates, "Id", "Abbreviation");	// Select list of states
		}

		private async Task UpdateStateAndResetModelState(Contact contact)
		{
			ModelState.Clear();
			// Caused different threads to concurrently use the same instance of DbContext (There can only be one).
			// • Should now be leveraging "_allStates".
			// States were already assigned to _allStates in the ctor.
			// So the following line is just retrieving the States _allStates already has.
			// • Remove await and SingleOrDefaultAssync method since _allStates is from a non-async method.
			//var state = await _context.States.SingleOrDefaultAsync(x => x.Id == contact.StateId);
			var state = _allStates.SingleOrDefault(x => x.Id == contact.StateId);
			contact.State = state;
			TryValidateModel(contact);
		}

        // GET: Contacts
        public async Task<IActionResult> Index()
        {
            var contacts = _context.Contacts.Include(c => c.State);
            return View(await contacts.ToListAsync());
        }

        // GET: Contacts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Contacts == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts
                .Include(c => c.State)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // GET: Contacts/Create
        public IActionResult Create()
        {
			// The following makes a new call to the database every time 
			// Create [GET|POST (upon error)] or Edit [GET|POST (upon error)] are called.
			//ViewData["StateId"] = new SelectList(_context.States, "Id", "Abbreviation");
			ViewData["StateId"] = _statesData;
            return View();
        }

        // POST: Contacts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Email,PhonePrimary,PhoneSecondary,Birthday,StreetAddress1,StreetAddress2,City,StateId,Zip,UserId")] Contact contact)
        {
			UpdateStateAndResetModelState(contact);
			//await UpdateStateAndResetModelState(contact);

			if (ModelState.IsValid)
            {
                _context.Add(contact);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

			// The following makes a new call to the database every time 
			// Create [GET|POST (upon error)] or Edit [GET|POST (upon error)] are called.
			//ViewData["StateId"] = new SelectList(_context.States, "Id", "Abbreviation", contact.StateId);
			ViewData["StateId"] = _statesData;
			return View(contact);
        }

        // GET: Contacts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Contacts == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }

			// The following makes a new call to the database every time 
			// Create [GET|POST (upon error)] or Edit [GET|POST (upon error)] are called.
			//ViewData["StateId"] = new SelectList(_context.States, "Id", "Abbreviation");   // Trainer scaffolding omitted "contact.StateId"
			//ViewData["StateId"] = new SelectList(_context.States, "Id", "Abbreviation", contact.StateId);	// My ver. VS2022
			ViewData["StateId"] = _statesData;
			return View(contact);
        }

        // POST: Contacts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,PhonePrimary,PhoneSecondary,Birthday,StreetAddress1,StreetAddress2,City,StateId,Zip,UserId")] Contact contact)
        {
            if (id != contact.Id)
            {
                return NotFound();
            }

			UpdateStateAndResetModelState(contact);
			//await UpdateStateAndResetModelState(contact);

			if (ModelState.IsValid)
            {
                try
                {
                    _context.Contacts.Update(contact);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactExists(contact.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

			// The following makes a new call to the database every time 
			// Create [GET|POST (upon error)] or Edit [GET|POST (upon error)] are called.
			//ViewData["StateId"] = new SelectList(_context.States, "Id", "Abbreviation");   // Trainer scaffolding omitted "contact.StateId"
			//ViewData["StateId"] = new SelectList(_context.States, "Id", "Abbreviation", contact.StateId);	// My ver. VS2022
			ViewData["StateId"] = _statesData;
			return View(contact);
        }

        // GET: Contacts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Contacts == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts
                .Include(c => c.State)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // POST: Contacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Contacts == null)
            {
                return Problem("Entity set 'MyContactManagerDbContext.Contacts'  is null.");
            }
            var contact = await _context.Contacts.FindAsync(id);
            if (contact != null)
            {
                _context.Contacts.Remove(contact);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContactExists(int id)
        {
          return (_context.Contacts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
