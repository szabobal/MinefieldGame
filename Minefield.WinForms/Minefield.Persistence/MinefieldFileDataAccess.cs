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

					Int32 gridHeight = Int32.Parse(dimensions[0]);
					Int32 gridWidth = Int32.Parse(dimensions[1]);

					MinefieldGrid grid = new MinefieldGrid(gridWidth, gridHeight);
					line = await reader.ReadLineAsync() ?? String.Empty;
                    grid.Ships = new Ship[Int32.Parse(line)];
					for (Int32 shipCount = 0; shipCount < grid.Ships.Length; shipCount++)
					{
						line = await reader.ReadLineAsync() ?? String.Empty;
						String[] shipData = line.Split(' ');
						grid.Ships[shipCount] = new Ship(Int32.Parse(shipData[0]), Int32.Parse(shipData[1]));
                    }

					for (Int32 i = 1; i < gridHeight; i++)
					{
						line = await reader.ReadLineAsync() ?? String.Empty;
						String[] values = line.Split(' ');

						for (Int32 j = 0; j < gridWidth; j++)
						{
							switch (values[j])
							{
								case "L":
									grid.AddBombAt(i, j, Weight.LIGHT);
									break;
								case "M":
									grid.AddBombAt(i, j, Weight.MEDIUM);
									break;
								case "H":
									grid.AddBombAt(i, j, Weight.HEAVY);
									break;
								case "S":
									grid.Submarine = new Submarine(i, j);
									break;
                                default:
									grid[i, j] = null;
									break;
							}
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
			try
			{
				using (StreamWriter writer = new StreamWriter(path))
				{
					await writer.WriteLineAsync(table.Rows + " " + table.Columns);
					await writer.WriteLineAsync($"{table.Ships.Length}");
					for (Int32 i = 0; i < table.Ships.Length; i++)
					{
						await writer.WriteLineAsync($"{table.Ships[i].Y} {table.Ships[i].ShipDirection}");
                    }
					

                    for (Int32 i = 1; i < table.Rows; i++)
					{
						for (Int32 j = 0; j < table.Columns; j++)
						{
							var sub = table.Submarine;
                            var bomb = table.GetBombAt(i,j);
							if (bomb == null)
								if (i == sub.X && j == sub.Y)
									await writer.WriteAsync("S ");
                                else
									await writer.WriteAsync("0 ");
							else
								await writer.WriteAsync(bomb.Weight.ToString()[0] + " ");
						}
						await writer.WriteLineAsync();
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Could not read file", ex);
			}
		}
	}
}
