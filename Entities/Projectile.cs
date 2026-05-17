
using CSharpSnakeProject.Enums;
using CSharpSnakeProject.Maps;
using CSharpSnakeProject.Structs;

namespace CSharpSnakeProject.Entities
{
    public class Projectile
    {
        public Cell Position { get; set; }
        public SnakeDirection Direction { get; set; }
        public int Range { get; set; }
        public char Symbol => '*';
        public ConsoleColor Color => ConsoleColor.Green;

        public Projectile(Cell startPos, SnakeDirection direction, int maxRange = 8)
        {
            Position = startPos;
            Direction = direction;
            Range = maxRange;
        }

        public bool Update(IMap map, Snake snake, List<Mine> mines, out bool hitMine)
        {
            hitMine = false;
            if (Range <= 0) return true;

            // Перемещаем снаряд
            Position = Direction switch
            {
                SnakeDirection.Up => new Cell(Position.X, Position.Y - 1),
                SnakeDirection.Down => new Cell(Position.X, Position.Y + 1),
                SnakeDirection.Left => new Cell(Position.X - 1, Position.Y),
                SnakeDirection.Right => new Cell(Position.X + 1, Position.Y),
                _ => Position
            };
            Range--;

            // Столкновение со стеной
            if (map.GetWalls().Contains(Position)) return true;

            // Проверка столкновения с любой миной
            if (mines.Any(m => m.Position == Position))
            {
                hitMine = true;
                return true;
            }

            // Столкновение с телом змейки
            if (snake.Body.Contains(Position)) return true;

            // Исчерпана дальность
            if (Range <= 0) return true;

            return false;
        }
    }
}
