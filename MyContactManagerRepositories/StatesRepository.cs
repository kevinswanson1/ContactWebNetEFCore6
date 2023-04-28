using ContactWebNetEFCore6Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using MyContactManagerData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyContactManagerRepositories
{
	//internal class StatesReopsitory
	public class StatesRepository : IStatesRepository
	{
		private MyContactManagerDbContext _context;

		public StatesRepository(MyContactManagerDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddOrUpdateAsync(State state)
		{
			if (state is null) throw new ArgumentNullException(nameof(state));
			if (state.Id > 0)
			{
				//return await _context.SaveChangesAsync();
				return await Update(state);
			}
			//return await _context.AddAsync();
			//return await _context.SaveChangesAsync();
			return await Insert(state);
		}

		private async Task<int> Insert(State state)
		{
			await _context.States.AddAsync(state);
			await _context.SaveChangesAsync();		// Save the context
			return state.Id;
		}

		private async Task<int> Update(State state)
		{
			//var existingState = await _context.States.SingleOrDefaultAsync(x  => x.Id == state.Id);
			//if (existingState is null) throw new KeyNotFoundException($"UPDATE FAILED: State 'Id={state.Id}' not found");
			var existingState = await _context.States.FirstOrDefaultAsync(x  => x.Id == state.Id) 
				?? throw new KeyNotFoundException($"UPDATE FAILED: State 'Id={state.Id}' not found");

			existingState.Abbreviation = state.Abbreviation;
			existingState.Name = state.Name;

			await _context.SaveChangesAsync();      // Save the context
			return state.Id;
		}

		public async Task<int> DeleteAsync(State state)
		{
			// Passthrough
			return await DeleteAsync(state.Id);
		}

		public async Task<int> DeleteAsync(int id)
		{
			//var existingState = await _context.States.SingleOrDefaultAsync(x  => x.Id == id);
			//if (existingState is null) throw new KeyNotFoundException($"DELETE FAILED: State 'Id={state.Id}' not found");
			var existingState = await _context.States.FirstOrDefaultAsync(x => x.Id == id)
				?? throw new KeyNotFoundException($"DELETE FAILED: State 'Id={id}' not found");

			await Task.Run(() => { _context.States.Remove(existingState); });
			await _context.SaveChangesAsync();      // Save the context
			return id;
		}

		public async Task<bool> ExistsAsync(int id)
		{
			return await _context.States.AnyAsync(x => x.Id == id);
		}

		public async Task<IList<State>> GetAllAsync()
		{
			return await _context.States.AsNoTracking().ToListAsync();
		}

		public async Task<State?> GetAsync(int id)
		{
			return await _context.States.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
		}
	}
}
