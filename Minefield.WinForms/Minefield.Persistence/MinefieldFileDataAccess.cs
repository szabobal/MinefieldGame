namespace Minefield.Persistence
{
    public class MinefieldFileDataAccess : IMinefieldDataAccess
    {
        public async Task<MinefieldGrid> LoadAsync(Stream stream)
        {
            MinefieldGrid grid = null;
            try
            {
                StreamReader reader = new StreamReader(stream);
                String line = await reader.ReadLineAsync() ?? String.Empty; // Rows Columns
                String[] dimensions = line.Split(' ');

                Int32 gridHeight = Int32.Parse(dimensions[0]);
                Int32 gridWidth = Int32.Parse(dimensions[1]);

                grid = new MinefieldGrid(gridWidth, gridHeight);
                line = await reader.ReadLineAsync() ?? String.Empty; // Number of ships
                grid.Ships = new Ship[Int32.Parse(line)];

                for (Int32 shipCount = 0; shipCount < grid.Ships.Length; shipCount++)
                {
                    line = await reader.ReadLineAsync() ?? String.Empty; // Y position and direction of ship
                    String[] shipData = line.Split(' ');
                    grid.Ships[shipCount] = new Ship(Int32.Parse(shipData[0]), Int32.Parse(shipData[1]));
                }

                List<Bomb> bombs = null;
                for (Int32 i = 1; i < gridHeight; i++)
                {
                    line = await reader.ReadLineAsync() ?? String.Empty; // Bombs marked as the initals of their weight
                    String[] values = line.Split(' ');

                    for (Int32 j = 0; j < gridWidth; j++)
                    {
                        Bomb b;
                        switch (values[j])
                        {
                            case "L":
                                b = new Bomb(i,j,Weight.LIGHT);
                                grid.AddBombAt(b);
                                bombs.Add(b);
                                break;
                            case "M":
                                b = new Bomb(i,j,Weight.MEDIUM);
                                grid.AddBombAt(b);
                                bombs.Add(b);
                                break;
                            case "H":
                                b = new Bomb(i,j,Weight.HEAVY);
                                grid.AddBombAt(b);
                                bombs.Add(b);
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
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not read the save file contents!", ex.Message);
            }
            return grid;
        }

        public async Task SaveAsync(string path, MinefieldGrid table)
        {
            try
            {
                StreamWriter writer = new StreamWriter(path);
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
                writer.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Could not read file", ex);
            }
        }
    }
}
