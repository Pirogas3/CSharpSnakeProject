using CSharpSnakeProject.Structs;

namespace CSharpSnakeProject.Maps
{
    public class ObstacleMap : IMap
    {
        public int Width { get; }
        public int Height { get; }

        private readonly List<Cell> _walls;
        private readonly char _wallSymbol;
        private readonly Random _random = new Random();

        // Параметры генерации
        private const int MaxObstacles = 8; // максимум препятствий
        private const float Density = 0.03f; // плотность (доля клеток, занятых стенами, кроме границ)

        public ObstacleMap(int width, int height, char wallSymbol = '█')
        {
            Width = width;
            Height = height;
            _wallSymbol = wallSymbol;
            _walls = GenerateWalls();
        }

        private List<Cell> GenerateWalls()
        {
            var walls = new List<Cell>();

            // 1. Границы (всегда есть)
            for (int x = 0; x < Width; x++)
            {
                walls.Add(new Cell(x, 0));
                walls.Add(new Cell(x, Height - 1));
            }
            for (int y = 1; y < Height - 1; y++)
            {
                walls.Add(new Cell(0, y));
                walls.Add(new Cell(Width - 1, y));
            }

            // 2. Определяем область, где не должны появляться стены (стартовая зона змейки)
            int startX = Width / 2;
            int startY = Height / 2;
            var noSpawnZone = new HashSet<Cell>();
            for (int dx = -3; dx <= 3; dx++)
                for (int dy = -3; dy <= 3; dy++)
                {
                    int x = startX + dx;
                    int y = startY + dy;
                    if (x > 0 && x < Width - 1 && y > 0 && y < Height - 1)
                        noSpawnZone.Add(new Cell(x, y));
                }

            // 3. Генерируем случайное количество препятствий (но не больше MaxObstacles и не более Density * свободных клеток)
            var freeCells = (Width - 2) * (Height - 2) - noSpawnZone.Count;
            int maxWallsByDensity = (int)(freeCells * Density);
            int targetObstacles = _random.Next(3, Math.Min(MaxObstacles, maxWallsByDensity) + 1);

            var obstacleForms = new List<Func<Cell, List<Cell>>>()
            {
                SingleCell,
                HorizontalLine2,
                HorizontalLine3,
                VerticalLine2,
                VerticalLine3,
                CornerL2x2,
                CornerL3x3,
                Square2x2,
                Square3x3
            };

            for (int i = 0; i < targetObstacles; i++)
            {
                // Выбираем случайную форму
                var form = obstacleForms[_random.Next(obstacleForms.Count)];
                TryPlaceObstacle(walls, noSpawnZone, form);
            }

            return walls;
        }

        // Попытка разместить препятствие в случайном месте с проверкой
        private void TryPlaceObstacle(List<Cell> walls, HashSet<Cell> noSpawnZone, Func<Cell, List<Cell>> form)
        {
            const int maxAttempts = 50;
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                // Выбираем случайную базовую точку (не на границе)
                int x = _random.Next(2, Width - 2);
                int y = _random.Next(2, Height - 2);
                Cell baseCell = new Cell(x, y);

                var cells = form(baseCell);

                bool isValid = true;
                foreach (var cell in cells)
                {
                    if (cell.X <= 0 || cell.X >= Width - 1 || cell.Y <= 0 || cell.Y >= Height - 1 ||
                        noSpawnZone.Contains(cell) ||
                        walls.Contains(cell))
                    {
                        isValid = false;
                        break;
                    }
                }

                if (isValid)
                {
                    walls.AddRange(cells);
                    // Добавляем все клетки препятствия в запретную зону для следующих препятствий, чтобы они не накладывались
                    foreach (var cell in cells)
                        noSpawnZone.Add(cell);
                    return;
                }
            }
            // Если не удалось разместить, просто не добавляем препятствие
        }

        // ---------- Формы препятствий ----------
        private static List<Cell> SingleCell(Cell baseCell) => new List<Cell> { baseCell };

        private static List<Cell> HorizontalLine2(Cell baseCell) => new List<Cell>
        {
            baseCell,
            new Cell(baseCell.X + 1, baseCell.Y)
        };

        private static List<Cell> HorizontalLine3(Cell baseCell) => new List<Cell>
        {
            baseCell,
            new Cell(baseCell.X + 1, baseCell.Y),
            new Cell(baseCell.X + 2, baseCell.Y)
        };

        private static List<Cell> VerticalLine2(Cell baseCell) => new List<Cell>
        {
            baseCell,
            new Cell(baseCell.X, baseCell.Y + 1)
        };

        private static List<Cell> VerticalLine3(Cell baseCell) => new List<Cell>
        {
            baseCell,
            new Cell(baseCell.X, baseCell.Y + 1),
            new Cell(baseCell.X, baseCell.Y + 2)
        };

        private static List<Cell> CornerL2x2(Cell baseCell) => new List<Cell>
        {
            baseCell,
            new Cell(baseCell.X + 1, baseCell.Y),
            new Cell(baseCell.X, baseCell.Y + 1)
        };

        private static List<Cell> CornerL3x3(Cell baseCell) => new List<Cell>
        {
            baseCell,
            new Cell(baseCell.X + 1, baseCell.Y),
            new Cell(baseCell.X + 2, baseCell.Y),
            new Cell(baseCell.X, baseCell.Y + 1),
            new Cell(baseCell.X, baseCell.Y + 2)
        };

        private static List<Cell> Square2x2(Cell baseCell) => new List<Cell>
        {
            baseCell,
            new Cell(baseCell.X + 1, baseCell.Y),
            new Cell(baseCell.X, baseCell.Y + 1),
            new Cell(baseCell.X + 1, baseCell.Y + 1)
        };

        private static List<Cell> Square3x3(Cell baseCell) => new List<Cell>
        {
            baseCell,
            new Cell(baseCell.X + 1, baseCell.Y),
            new Cell(baseCell.X + 2, baseCell.Y),
            new Cell(baseCell.X, baseCell.Y + 1),
            new Cell(baseCell.X + 1, baseCell.Y + 1),
            new Cell(baseCell.X + 2, baseCell.Y + 1),
            new Cell(baseCell.X, baseCell.Y + 2),
            new Cell(baseCell.X + 1, baseCell.Y + 2),
            new Cell(baseCell.X + 2, baseCell.Y + 2)
        };

        public IEnumerable<Cell> GetWalls() => _walls;
        public char GetWallSymbol(Cell position) => _wallSymbol;
    }
}