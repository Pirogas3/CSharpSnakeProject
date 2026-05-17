using CSharpSnakeProject.Entities;
using CSharpSnakeProject.Enums;
using CSharpSnakeProject.Input;
using CSharpSnakeProject.Logic.Food;
using CSharpSnakeProject.Logic.Managers;
using CSharpSnakeProject.Logic.Utils;
using CSharpSnakeProject.Maps;
using CSharpSnakeProject.Renderer;
using CSharpSnakeProject.Structs;

namespace CSharpSnakeProject.Logic.GameWorlds
{
    public class GameWorld
    {
        private readonly IMap _map;
        private readonly IGameRules _gameRules;
        private readonly List<IDrawable> _drawables = new List<IDrawable>();
        private readonly GamePalette _palette;
        private readonly bool _hasEnemies;

        private Snake _snake;
        private MovementTimer _movementTimer;
        private FoodManager _foodManager;
        private MineManager _mineManager;
        private ProjectileManager _projectileManager;

        private int _score = 0;
        private bool _isGameOver = false;
        public int Score => _score;
        public bool IsGameOver => _isGameOver;

        public GameWorld(IMap map, List<IFood> availableFoods, GamePalette palette, IFoodGenerator foodGenerator, float speedSnake, bool hasEnemies)
        {
            _gameRules = new GameRules();
            _map = map;
            _palette = palette;
            _hasEnemies = hasEnemies;

            int middleY = _map.Height / 2;
            int middleX = _map.Width / 2;
            Cell startHead = new(middleX, middleY);
            Cell startTail = new(middleX + 1, middleY);
            _snake = new Snake(startHead, startTail, SnakeDirection.Left, speedSnake);

            _projectileManager = new ProjectileManager();

            _foodManager = new FoodManager(map, availableFoods, foodGenerator);
            _foodManager.GenerateFood(_snake.Body);

            if (_hasEnemies)
            {
                _mineManager = new MineManager(_map, () => _score);
                _mineManager.EnsureMinesCount(_snake, _foodManager.Position);
            }
            else
            {
                //_mineManager = null; // или создать пустую заглушку
            }

            _drawables.Add(_snake);
            _drawables.Add(_foodManager);
            _drawables.Add(_projectileManager);
            if (_mineManager != null) _drawables.Add(_mineManager);

            _movementTimer = new MovementTimer(0f);
        }

        public void Shoot()
        {
            if (_isGameOver) return;
            _projectileManager.Shoot(_snake.GetHead(), _snake.Direction, _map);
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

        public void Update(float deltaTime)
        {
            if (_isGameOver) return; //проверка на гейм-овер

            // Обновляем таймер еды
            _foodManager.UpdateTimer(deltaTime, _snake.Body);

            // Обновляем снаряды (удаляем мины при попадании)
            _projectileManager.Update(_map, _snake, _mineManager);

            // Обновляем мины и проверяем столкновения
            if (_hasEnemies && _mineManager != null)
            {
                bool mineHit = _mineManager.Update(deltaTime, _snake, _foodManager.Position);
                if (mineHit) OnMineCollision();
            }

            // Обновляем текущую скорость змейки (зажатие Shift)
            bool shiftHeld = KeyboardHelper.IsShiftHeld();
            _snake.CurrentSpeedSnake = shiftHeld ? _snake.BasicSpeedSnake * _snake.SpeedBoost : _snake.BasicSpeedSnake;

            // Обновляем таймер и, если пора двигаться, вызываем движение
            if (_movementTimer.Update(deltaTime, _snake.CurrentSpeedSnake))
            {
                ProcessMove(); // используем переименованный метод (бывший SnakeUpdate)
            }
        }

        private void ProcessMove()
        {
            Cell nextHead = _snake.GetNextHead();

            // 1. Стена
            if (CollisionChecker.IsWall(nextHead, _map))
            {
                _isGameOver = _gameRules.ApplyWallCollisionEffect();
                return;
            }

            // 2. Проверка на мину
            if (_hasEnemies && CollisionChecker.IsMine(nextHead, _mineManager))
            {
                var mine = _mineManager.Mines.First(m => m.Position == nextHead);
                _mineManager.RemoveMine(mine);
                OnMineCollision();
                return;
            }

            // 2. Самопересечение (проверяем до движения)
            if (CollisionChecker.IsSnakeBody(nextHead, _snake, true))
            {
                _isGameOver = _gameRules.ApplySelfCollisionEffect(_snake, ref _score);
                return;
            }

            // 3. Еда или обычное движение
            if (CollisionChecker.IsFood(nextHead, _foodManager.Position))
            {
                _isGameOver = _gameRules.ApplyFoodEffect(_snake, _foodManager, ref _score, nextHead);
            }
            else
            {
                _gameRules.ApplyNormalMove(_snake, nextHead);
            }
        }

        private void OnMineCollision()
        {
            while (_snake.Length > 2)
                _snake.CutTail();
            _score = 0;
        }

        public void Draw(ConsoleRenderer renderer)
        {
            // Стены
            foreach (var wall in _map.GetWalls())
            {
                char wallSymbol = _map.GetWallSymbol(wall);
                renderer.SetPixel(wall.X, wall.Y, wallSymbol, renderer.GetColorIndex(_palette.Walls));
            }

            // Отрисовка всех зарегистрированных объектов (змейки, еды, мин и снарядов)
            foreach (var drawable in _drawables)
            {
                drawable.Draw(renderer, _palette);
            }

            // Счёт
            renderer.DrawString($"Счёт: {_score}  Длина: {_snake.Length}", 1, _map.Height + 1, _palette.Score);
        }
    }
}
