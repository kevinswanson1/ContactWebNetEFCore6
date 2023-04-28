using ContactWebNetEFCore6Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyContactManagerServices
{
	//internal class ContactsService
	public class ContactsService : IContactsService
	{
		public async Task<int> AddOrUpdateAsync(State state)
		{
			throw new NotImplementedException();
		}

		public async Task<int> DeleteAsync(State state)
		{
			throw new NotImplementedException();
		}

		public async Task<int> DeleteAsync(int id)
		{
			throw new NotImplementedException();
		}

		public async Task<bool> ExistsAsync(int id)
		{
			throw new NotImplementedException();
		}

		public async Task<IList<State>> GetAllAsync()
		{
			throw new NotImplementedException();
		}

		public async Task<State?> GetAsync(int id)
		{
			throw new NotImplementedException();
		}
	}
}
