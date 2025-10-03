using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minefield.Persistence
{
    public class MinefieldFileDataAccess : IMinefieldDataAccess
    {
        public async Task<MinefieldGrid> LoadAsync(Stream stream)
        {
            try
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    String line = await reader.ReadLineAsync() ?? String.Empty;
                    String[] dimensions = line.Split(' ');

                    Int32 gridWidth = Int32.Parse(dimensions[0]);
                    Int32 gridHeight = Int32.Parse(dimensions[1]);

                    MinefieldGrid grid = new MinefieldGrid(gridWidth, gridHeight);

                    for (Int32 i = 0; i < gridHeight; i++)
                    {
                        line = await reader.ReadLineAsync() ?? String.Empty;
                        String[] bombs = line.Split(' ');

                        for (Int32 j = 0; j < gridWidth; j++)
                        {
                           // table.SetValue(i, j, Int32.Parse(numbers[j]), false);
                        }
                    }
                    return grid;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not read file", ex);
            }
        }

        public async Task SaveAsync(string path, MinefieldGrid table)
        {
            throw new NotImplementedException();
        }
    }
}
