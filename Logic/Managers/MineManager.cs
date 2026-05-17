using CSharpSnakeProject.Entities;
using CSharpSnakeProject.Logic.Utils;
using CSharpSnakeProject.Maps;
using CSharpSnakeProject.Renderer;
using CSharpSnakeProject.Structs;

namespace CSharpSnakeProject.Logic.Managers
{
    public class MineManager : IDrawable
    {
        private readonly IMap _map;
        private readonly Func<int> _getScore; // для доступа к текущему счёту
        private readonly Random _rand = new Random();
        private List<Mine> _mines = new List<Mine>();

        public IReadOnlyList<Mine> Mines => _mines;

        public MineManager(IMap map, Func<int> getScore)
        {
            _map = map;
            _getScore = getScore;
        }

        // Обновление всех мин (жизнь + движение) и поддержание количества
        public bool Update(float deltaTime, Snake snake, Cell foodPosition)
        {
            bool hitHappened = false;

            // 1. Обновляем время жизни мин, удаляем умершие
            for (int i = 0; i < _mines.Count; i++)
            {
                if (_mines[i].UpdateLife(deltaTime))
                {
                    _mines.RemoveAt(i);
                    i--;
                }
            }

            // 2. Движение мин и обработка столкновений (стена, еда, тело змейки)
            for (int i = 0; i < _mines.Count; i++)
            {
                var mine = _mines[i];
                Cell oldPos = mine.Position;
                if (!mine.UpdateMovement(deltaTime, snake.GetHead()))
                    continue;

                Cell newPos = mine.Position;

                // Столкновение со стеной или едой – мина уничтожается
                if (CollisionChecker.IsWall(newPos, _map) || CollisionChecker.IsFood(newPos, foodPosition))
                {
                    _mines.RemoveAt(i);
                    i--;
                    continue;
                }

                // Столкновение с телом змейки (включая голову) – мина уничтожается, будет штраф
                if (CollisionChecker.IsSnakeBody(newPos, snake, true))
                {
                    _mines.RemoveAt(i);
                    hitHappened = true;
                    i--;
                    continue;
                }
            }

            // 3. Поддерживаем нужное количество мин (добавляем, если меньше нормы)
            EnsureMinesCount(snake, foodPosition);

            return hitHappened;
        }

        public void EnsureMinesCount(Snake snake, Cell foodPosition)
        {
            int target = GetMaxMinesCount();
            while (_mines.Count < target)
            {
                GenerateMine(snake, foodPosition);
            }
        }

        // Принудительное удаление конкретной мины (например, после попадания снаряда)
        public bool RemoveMine(Mine mine)
        {
            return _mines.Remove(mine);
        }

        // Отрисовка всех мин
        public void Draw(ConsoleRenderer renderer, GamePalette palette)
        {
            foreach (var mine in _mines)
            {
                renderer.SetPixel(mine.Position.X, mine.Position.Y, mine.Symbol,
                    renderer.GetColorIndex(mine.Color));
            }
        }

        // Сброс (очистить все мины)
        public void Reset()
        {
            _mines.Clear();
        }

        private int GetMaxMinesCount()
        {
            int score = _getScore();
            if (score >= 300) return 4;
            if (score >= 200) return 3;
            if (score >= 100) return 2;
            return 1;
        }

        private void GenerateMine(Snake snake, Cell foodPosition)
        {
            const int minDistanceFromHead = 10;
            var freeCells = new List<Cell>();

            for (int x = 1; x < _map.Width - 1; x++)
                for (int y = 1; y < _map.Height - 1; y++)
                {
                    Cell cell = new Cell(x, y);
                    if (!_map.GetWalls().Contains(cell) &&
                        !snake.Body.Contains(cell) &&
                        foodPosition != cell &&
                        !_mines.Any(m => m.Position == cell) &&
                        Math.Abs(cell.X - snake.GetHead().X) + Math.Abs(cell.Y - snake.GetHead().Y) >= minDistanceFromHead)
                    {
                        freeCells.Add(cell);
                    }
                }

            if (freeCells.Count == 0) return;
            var pos = freeCells[_rand.Next(freeCells.Count)];
            _mines.Add(new Mine(pos, 10f));
        }
    }
}
