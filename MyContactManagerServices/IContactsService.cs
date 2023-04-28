using ContactWebNetEFCore6Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyContactManagerServices
{
	//internal interface IContactsService
	public interface IContactsService
	{
		Task<IList<State>> GetAllAsync();
		Task<State?> GetAsync(int id);
		Task<int> AddOrUpdateAsync(State state);
		Task<int> DeleteAsync(State state);
		Task<int> DeleteAsync(int id);
		Task<bool> ExistsAsync(int id);
	}
}
