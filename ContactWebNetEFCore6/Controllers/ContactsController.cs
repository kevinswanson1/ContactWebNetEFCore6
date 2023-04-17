using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContactWebNetEFCore6Models;
using MyContactManagerData;
using ContactWebNetEFCore6.Models;
using Microsoft.Extensions.Caching.Memory;

namespace ContactWebNetEFCore6.Controllers
{
	/// <summary>
	/// https://www.udemy.com/course/aspnet-mvc-quick-start/learn/lecture/29599070#notes
	/// </summary>
	public class ContactsController : Controller
	{
		private readonly MyContactManagerDbContext _context;
		private static List<State>? _allStates;	// TODO: Deviation from course. Using nullable "?", warning suppression
		private static SelectList? _statesData; // TODO: Deviation from course. Using nullable "?", warning suppression
		private IMemoryCache _cache;

		public ContactsController(MyContactManagerDbContext context, IMemoryCache cache)
		{
			_context = context;
			_cache = cache;
			SetAllStatesCachingData();
			_statesData = new SelectList(_allStates, "Id", "Abbreviation");
		}

		private void SetAllStatesCachingData()
		{
			var allStates = new List<State>();
			if (!_cache.TryGetValue(ContactCacheConstants.ALL_STATES, out allStates))
			{
				var allStatesData = Task.Run(() => _context.States.ToListAsync()).Result;
				_cache.Set(ContactCacheConstants.ALL_STATES, allStatesData, TimeSpan.FromDays(1));
				//allStates = allStatesData;
				// Get states from cache...
				// Completely disconnect from database
				// No longer holding a reference to 'allStatesData' which ran against the _context
				allStates = _cache.Get(ContactCacheConstants.ALL_STATES) as List<State>;
			}
			_allStates = allStates;
		}

		private void UpdateStateAndResetModelState(Contact contact)
		{
			// TODO: Remove extraneous commentary.
			ModelState.Clear();
			#region Transistion method from async to sync.
			// Caused different threads to concurrently use the same instance of DbContext (There can only be one).
			// • Should now be leveraging "_allStates".
			// States were already assigned to _allStates in the ctor.
			// So the following line is just retrieving the States _allStates already has.
			// • Remove await and SingleOrDefaultAsync method since _allStates is from a non-async method.
			//private async Task UpdateStateAndResetModelState(Contact contact)
			//var state = await _context.States.SingleOrDefaultAsync(x => x.Id == contact.StateId);
			#endregion
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
			// TODO: Remove extraneous commentary.
			if (id == null || _context.Contacts == null)
			{
				return NotFound();
			}

			var contact = await _context.Contacts
				.Include(c => c.State)
				.FirstOrDefaultAsync(m => m.Id == id);
			#region Use 'x ? y : z' or more straight forward flow logic statements?
			// TODO: Decide on the following.
			// This code is replaced...
			//if (contact == null)
			//{
			//	return NotFound();
			//}
			//return View(contact);
			// ... by this
			#endregion
			//return contact != null ? View(contact) : Problem($"Entity set 'MyContactManagerDbContext.Contacts.Id={id}' is not found.");
			//return contact != null ? View(contact) : NotFound("");
			return contact != null ? View(contact) : NotFound($"Entity set 'MyContactManagerDbContext.Contacts[Id={id}]' is not found.");
		}

		// GET: Contacts/Create
		public IActionResult Create()
		{
			// TODO: Remove extraneous commentary.
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
			// TODO: Deal with inline warning suppressions.
			// TODO: Remove extraneous commentary.
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
			UpdateStateAndResetModelState(contact); // See UpdateStateAndResetModelState explanation for 'await' is not use.
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
			//await UpdateStateAndResetModelState(contact);

			if (ModelState.IsValid)
			{
				// HACK: To get State hydrated (Using data connected to the State record in the database
				// HACK: This makes an extra call. This code will be replaced later.
				var state = await _context.States.SingleOrDefaultAsync(x => x.Id == contact.StateId);
				//contact.State = state ?? contact.State;		// Generated code.
				contact.State = state;  // HACK

				//_context.Add(contact);	// Mistakenly changed to sync from async
				await _context.Contacts.AddAsync(contact);	// contact ID that tracks the navigation

				// ERROR: The state is a valid state with an ID, but it's disconnected from the database
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

			// TODO: Deal with inline warning suppressions.
			// TODO: Remove extraneous commentary.
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
			UpdateStateAndResetModelState(contact); // See UpdateStateAndResetModelState explanation for 'await' is not use.
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
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
