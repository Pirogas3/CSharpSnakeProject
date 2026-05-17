using CSharpSnakeProject.Structs;

namespace CSharpSnakeProject.Maps
{
    public class BasicMap : IMap
    {
        public int Width { get; }
        public int Height { get; }

        private readonly List<Cell> _walls;
        private readonly char _wallSymbol;

        public BasicMap(int width, int height, char wallSymbol = '█')
        {
            Width = width;
            Height = height;
            _wallSymbol = wallSymbol;
            _walls = GenerateWalls();
        }

        private List<Cell> GenerateWalls()
        {
            var walls = new List<Cell>();

            // Верхняя и нижняя границы
            for (int x = 0; x < Width; x++)
            {
                walls.Add(new Cell(x, 0));
                walls.Add(new Cell(x, Height - 1));
            }

            // Левая и правая границы (без углов, чтобы не дублировать)
            for (int y = 1; y < Height - 1; y++)
            {
                walls.Add(new Cell(0, y));
                walls.Add(new Cell(Width - 1, y));
            }

            return walls;
        }

        public IEnumerable<Cell> GetWalls() => _walls;
        public char GetWallSymbol(Cell position) => _wallSymbol;
    }
}