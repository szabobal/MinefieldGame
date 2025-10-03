using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minefield.Persistence
{
	public interface IMinefieldDataAccess
	{
		Task<MinefieldGrid> LoadAsync(Stream stream);
		Task SaveAsync(String path, MinefieldGrid table);
	}
}
