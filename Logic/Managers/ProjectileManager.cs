using CSharpSnakeProject.Entities;
using CSharpSnakeProject.Enums;
using CSharpSnakeProject.Logic.Utils;
using CSharpSnakeProject.Maps;
using CSharpSnakeProject.Renderer;
using CSharpSnakeProject.Structs;

namespace CSharpSnakeProject.Logic.Managers
{
    public class ProjectileManager : IDrawable
    {
        private List<Projectile> _projectiles = new List<Projectile>();

        public IReadOnlyList<Projectile> Projectiles => _projectiles;

        // Создать новый снаряд, если нет активных и позиция валидна
        public void Shoot(Cell head, SnakeDirection direction, IMap map)
        {
            if (_projectiles.Any(p => p.Range > 0)) return; // один снаряд за раз

            Cell startPos = direction switch
            {
                SnakeDirection.Up => new Cell(head.X, head.Y - 1),
                SnakeDirection.Down => new Cell(head.X, head.Y + 1),
                SnakeDirection.Left => new Cell(head.X - 1, head.Y),
                SnakeDirection.Right => new Cell(head.X + 1, head.Y),
                _ => head
            };

            if (map.GetWalls().Contains(startPos)) return;

            _projectiles.Add(new Projectile(startPos, direction));
        }

        // Обновление всех снарядов: перемещение, проверка столкновений
        public bool Update(IMap map, Snake snake, MineManager mineManager)
        {
            bool hitSnake = false;

            for (int i = 0; i < _projectiles.Count; i++)
            {
                var p = _projectiles[i];
                bool shouldRemove = false;

                // Перемещаем снаряд и проверяем дальность
                p.Range--;
                p.Position = p.Direction switch
                {
                    SnakeDirection.Up => new Cell(p.Position.X, p.Position.Y - 1),
                    SnakeDirection.Down => new Cell(p.Position.X, p.Position.Y + 1),
                    SnakeDirection.Left => new Cell(p.Position.X - 1, p.Position.Y),
                    SnakeDirection.Right => new Cell(p.Position.X + 1, p.Position.Y),
                    _ => p.Position
                };

                // Столкновение со стеной
                if (CollisionChecker.IsWall(p.Position, map))
                {
                    shouldRemove = true;
                }
                // Столкновение с миной (удаляем мину и снаряд)
                else if (mineManager != null && CollisionChecker.IsMine(p.Position, mineManager))
                {
                    var mineToRemove = mineManager.Mines.First(m => m.Position == p.Position);
                    mineManager.RemoveMine(mineToRemove);
                    shouldRemove = true;
                }
                // Столкновение с телом змеи (включая голову)
                else if (CollisionChecker.IsSnakeBody(p.Position, snake, true))
                {
                    hitSnake = true;
                    shouldRemove = true;
                }
                // Исчерпание дальности
                else if (p.Range <= 0)
                {
                    shouldRemove = true;
                }

                if (shouldRemove)
                {
                    _projectiles.RemoveAt(i);
                    i--;
                }
            }

            return hitSnake;
        }

        // Отрисовка всех снарядов
        public void Draw(ConsoleRenderer renderer, GamePalette palette)
        {
            foreach (var p in _projectiles)
            {
                renderer.SetPixel(p.Position.X, p.Position.Y, p.Symbol,
                    renderer.GetColorIndex(p.Color));
            }
        }

        // Сброс (очистить все снаряды)
        public void Reset()
        {
            _projectiles.Clear();
        }
    }
}
