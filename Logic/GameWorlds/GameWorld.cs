using CSharpSnakeProject.Entities;
using CSharpSnakeProject.Enums;
using CSharpSnakeProject.Input;
using CSharpSnakeProject.Logic.Food;
using CSharpSnakeProject.Maps;
using CSharpSnakeProject.Renderer;
using CSharpSnakeProject.Structs;

namespace CSharpSnakeProject.Logic.GameWorlds
{
    public class GameWorld
    {
        private readonly IMap _map;
        private readonly List<IFood> _availableFoods;
        private readonly IFoodGenerator _foodGenerator;
        private readonly Random _rand = new Random();
        private readonly GamePalette _palette;
        private readonly bool _hasEnemies;

        private Snake _snake;
        private IFood _currentFood;
        private Cell _foodPosition;
        private int _score;
        private bool _isGameOver;
        private float _timeToMove;
        private float _foodTimeLeft;

        private List<Projectile> _projectiles = new List<Projectile>();
        private List<Mine> _mines = new List<Mine>();

        // Свойства для доступа извне
        public IMap Map => _map;
        public int Score => _score;
        public bool IsGameOver => _isGameOver;

        public GameWorld(IMap map, List<IFood> availableFoods, GamePalette palette, IFoodGenerator foodGenerator, float speedSnake, bool hasEnemies)
        {
            _map = map;
            _availableFoods = availableFoods;
            _palette = palette;
            _foodGenerator = foodGenerator;

            int middleY = _map.Height / 2;
            int middleX = _map.Width / 2;
            Cell startHead = new(middleX, middleY);
            Cell startTail = new(middleX + 1, middleY);

            _snake = new Snake(startHead, startTail, SnakeDirection.Left, speedSnake);
            _score = 0;
            _isGameOver = false;
            _timeToMove = 0f;
            _hasEnemies = hasEnemies;

            GenerateFood();
            if (_hasEnemies)
                EnsureMinesCount();
        }

        public void Reset()
        {
            _mines.Clear();
            if (_hasEnemies)
                EnsureMinesCount();
        }

        public void Shoot()
        {
            if (_isGameOver) return;
            if (_projectiles.Any(p => p.Range > 0)) return; // один снаряд за раз

            Cell head = _snake.GetHead();
            SnakeDirection dir = _snake.Direction;

            Cell startPos = dir switch
            {
                SnakeDirection.Up => new Cell(head.X, head.Y - 1),
                SnakeDirection.Down => new Cell(head.X, head.Y + 1),
                SnakeDirection.Left => new Cell(head.X - 1, head.Y),
                SnakeDirection.Right => new Cell(head.X + 1, head.Y),
                _ => head
            };

            if (_map.GetWalls().Contains(startPos)) return;

            _projectiles.Add(new Projectile(startPos, dir));
        }

        public void SetDirection(SnakeDirection dir)
        {
            if (_isGameOver) return;

            // Запрет разворота на 180 градусов
            if ((_snake.Direction == SnakeDirection.Up && dir == SnakeDirection.Down) ||
                (_snake.Direction == SnakeDirection.Down && dir == SnakeDirection.Up) ||
                (_snake.Direction == SnakeDirection.Left && dir == SnakeDirection.Right) ||
                (_snake.Direction == SnakeDirection.Right && dir == SnakeDirection.Left))
                return;

            _snake.Direction = dir;
        }

        private void FoodTimeLeft(float deltaTime)
        {
            _foodTimeLeft -= deltaTime;
            if (_foodTimeLeft <= 0f)
            {
                GenerateFood();
            }
        }

        private void TimeToMoveCalc(float deltaTime)
        {
            // Корректируем _timeToMove при изменении скорости
            if (_snake.LastSpeedSnake != 0 && _snake.LastSpeedSnake != _snake.CurrentSpeedSnake)
            {
                // Пропорционально корректируем оставшееся время
                float ratio = _snake.LastSpeedSnake / _snake.CurrentSpeedSnake;
                _timeToMove *= ratio;
                if (_timeToMove < 0) _timeToMove = 0;
            }
            _snake.LastSpeedSnake = _snake.CurrentSpeedSnake;
            _timeToMove -= deltaTime;
        }

        public void Update(float deltaTime)
        {
            if (_isGameOver) return; //проверка на гейм-овер

            FoodTimeLeft(deltaTime);

            // Проверка удержания Shift
            bool shiftHeld = KeyboardHelper.IsShiftHeld();
            _snake.CurrentSpeedSnake = shiftHeld ? _snake.BasicSpeedSnake * _snake.SpeedBoost : _snake.BasicSpeedSnake;

            // Обновляем снаряды
            for (int i = 0; i < _projectiles.Count; i++)
            {
                bool hitMine;
                if (_projectiles[i].Update(_map, _snake, _mines, out hitMine))
                {
                    if (hitMine)
                    {
                        var mine = _mines.FirstOrDefault(m => m.Position == _projectiles[i].Position);
                        if (mine != null)
                        {
                            _mines.Remove(mine);
                            EnsureMinesCount();
                        }
                    }
                    _projectiles.RemoveAt(i);
                    i--;
                }
            }

            // Обновление мин: жизнь и движение
            for (int i = 0; i < _mines.Count; i++)
            {
                if (_mines[i].UpdateLife(deltaTime))
                {
                    _mines.RemoveAt(i);
                    i--;
                }
            }
            EnsureMinesCount();
            MoveMines(deltaTime);

            TimeToMoveCalc(deltaTime);
            if (_timeToMove > 0f) return;
            _timeToMove = 1f / _snake.CurrentSpeedSnake; //скорость движения змейки

            SnakeUpdate();
        }

        private void SnakeUpdate()
        {
            Cell nextHead = _snake.GetNextHead();

            // Проверка на столкновение со стенами
            if (_map.GetWalls().Contains(nextHead))
            {
                GameOver();
                return;
            }

            var collidedMine = _mines.FirstOrDefault(m => m.Position == nextHead);
            if (collidedMine != null)
            {
                OnMineCollision(collidedMine);
                _mines.Remove(collidedMine);
                EnsureMinesCount();
                return;
            }

            // Проверка на самопересечение
            if (_snake.CollidesWithSelf())
            {
                _snake.CutTail();
                _score = Math.Max(0, _score - 10); // штраф
                if (_snake.Length == 0)
                    GameOver();
                return;   // Не двигаем змейку в этом кадре
            }

            // Движение змейки с учетом еды
            bool ateFood = (nextHead == _foodPosition);
            if (ateFood)
            {
                _snake.Move(nextHead, true);
                _score += _currentFood.ScoreValue;
                GenerateFood();
            }
            else
            {
                _snake.Move(nextHead, false);
            }
        }

        private void GenerateFood()
        {
            if (_availableFoods.Count == 0)
                throw new InvalidOperationException("Нет доступной еды");

            _currentFood = _availableFoods[_rand.Next(_availableFoods.Count)];
            _foodPosition = _foodGenerator.GenerateFood(_currentFood, _map, _snake.Body);
            _foodTimeLeft = _currentFood.LifespanSeconds;
        }

        private void GenerateMine()
        {
            if (!_hasEnemies) return;
            if (_mines.Count >= GetMaxMinesCount()) return;

            const int minDistanceFromHead = 10;
            var freeCells = new List<Cell>();

            for (int x = 1; x < _map.Width - 1; x++)
                for (int y = 1; y < _map.Height - 1; y++)
                {
                    Cell cell = new Cell(x, y);
                    if (!_map.GetWalls().Contains(cell) &&
                        !_snake.Body.Contains(cell) &&
                        _foodPosition != cell &&
                        !_mines.Any(m => m.Position == cell) &&
                        Math.Abs(cell.X - _snake.GetHead().X) + Math.Abs(cell.Y - _snake.GetHead().Y) >= minDistanceFromHead)
                    {
                        freeCells.Add(cell);
                    }
                }

            if (freeCells.Count == 0) return;
            var pos = freeCells[_rand.Next(freeCells.Count)];
            _mines.Add(new Mine(pos, 10f));
        }

        private int GetMaxMinesCount()
        {
            if (!_hasEnemies) return 0;
            if (_score >= 300) return 4;
            if (_score >= 200) return 3;
            if (_score >= 100) return 2;
            return 1;
        }

        private void EnsureMinesCount()
        {
            int target = GetMaxMinesCount();
            while (_mines.Count < target)
                GenerateMine();
        }

        private void MoveMines(float deltaTime)
        {
            for (int i = 0; i < _mines.Count; i++)
            {
                var mine = _mines[i];
                Cell oldPos = mine.Position;
                if (!mine.UpdateMovement(deltaTime, _snake.GetHead()))
                    continue;

                Cell newPos = mine.Position;

                // Столкновение со стеной или едой
                if (_map.GetWalls().Contains(newPos) || newPos == _foodPosition)
                {
                    _mines.RemoveAt(i);
                    i--;
                    EnsureMinesCount();
                    continue;
                }

                // Столкновение с телом змейки (включая голову) – штраф
                if (_snake.Body.Contains(newPos))
                {
                    OnMineCollision(mine);
                    _mines.RemoveAt(i);
                    i--;
                    continue;
                }
            }
        }

        private void OnMineCollision(Mine mine)
        {
            while (_snake.Length > 2)
                _snake.CutTail();
            _score = 0;
        }

        private void GameOver()
        {
            _isGameOver = true;
        }

        public void Draw(ConsoleRenderer renderer)
        {
            // Стены
            foreach (var wall in _map.GetWalls())
            {
                char wallSymbol = _map.GetWallSymbol(wall);
                renderer.SetPixel(wall.X, wall.Y, wallSymbol, renderer.GetColorIndex(_palette.Walls));
            }

            // Еда
            renderer.SetPixel(_foodPosition.X, _foodPosition.Y, _currentFood.Symbol, renderer.GetColorIndex(_currentFood.Color));

            // Змейка
            for (int i = 0; i < _snake.Body.Count; i++)
            {
                var cell = _snake.Body[i];
                byte colorIdx = i == 0 ? renderer.GetColorIndex(_palette.SnakeHead) : renderer.GetColorIndex(_palette.SnakeBody);
                renderer.SetPixel(cell.X, cell.Y, _snake.Symbol, colorIdx);
            }

            // Мины
            foreach (var mine in _mines)
            {
                renderer.SetPixel(mine.Position.X, mine.Position.Y, mine.Symbol, renderer.GetColorIndex(mine.Color));
            }

            // Снаряды
            foreach (var p in _projectiles)
            {
                renderer.SetPixel(p.Position.X, p.Position.Y, p.Symbol,
                    renderer.GetColorIndex(p.Color));
            }

            // Счёт
            renderer.DrawString($"Счёт: {_score}  Длина: {_snake.Length}", 1, _map.Height + 1, _palette.Score);
        }
    }
}
