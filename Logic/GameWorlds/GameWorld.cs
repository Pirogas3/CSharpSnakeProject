using CSharpSnakeProject.Entities;
using CSharpSnakeProject.Enums;
using CSharpSnakeProject.Logic.Food;
using CSharpSnakeProject.Maps;
using CSharpSnakeProject.Renderer;
using CSharpSnakeProject.Structs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CSharpSnakeProject.Logic.GameWorlds
{
    public class GameWorld
    {
        private readonly IMap _map;
        private readonly List<IFood> _availableFoods;
        private readonly IFoodGenerator _foodGenerator;
        private readonly Random _rand = new Random();
        private readonly GamePalette _palette;

        private Snake _snake;
        private IFood _currentFood;
        private Cell _foodPosition;
        private int _score;
        private bool _isGameOver;
        private float _timeToMove;
        private float _foodTimeLeft;
        private float _speedSnake = 12f; //можно задать скорость движения змейки, чем больше число, тем быстрее.

        // Свойства для доступа извне
        public IMap Map => _map;
        public Snake Snake => _snake;
        public int Score => _score;
        public bool IsGameOver => _isGameOver;

        public GameWorld(IMap map, List<IFood> availableFoods, GamePalette palette, IFoodGenerator foodGenerator, float speedSnake)
        {
            _map = map;
            _availableFoods = availableFoods;
            _palette = palette;
            _foodGenerator = foodGenerator;
            _speedSnake = speedSnake;

            Reset(); // начальная инициализация
        }

        public void Reset()
        {
            int middleY = _map.Height / 2;
            int middleX = _map.Width / 2;
            Cell startHead = new(middleX, middleY);
            Cell startTail = new(middleX + 1, middleY);

            _snake = new Snake(startHead, startTail, SnakeDirection.Left);
            _score = 0;
            _isGameOver = false;
            _timeToMove = 0f;

            GenerateFood();
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
            if (_isGameOver) return;

            //время жизни еды
            _foodTimeLeft -= deltaTime;
            if (_foodTimeLeft <= 0f)
            {
                GenerateFood();
            }

            _timeToMove -= deltaTime;
            if (_timeToMove > 0f) return;
            _timeToMove = 1f / _speedSnake; //скорость движения змейки

            Cell nextHead = _snake.GetNextHead();

            // Стена
            if (_map.GetWalls().Contains(nextHead))
            {
                GameOver();
                return;
            }

            // Самопересечение (включая хвост)
            if (_snake.CollidesWithSelf())   // используем старый метод
            {
                _snake.CutTail();                   // откусили хвост
                _score = Math.Max(0, _score - 10); // штраф
                if (_snake.Length == 0)
                    GameOver();
                return;   // ⚠️ не двигаем змейку в этом кадре
            }

            // Еда
            bool ateFood = (nextHead == _foodPosition);
            if (ateFood)
            {
                _snake.Grow(nextHead);
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

            // Счёт
            renderer.DrawString($"Счёт: {_score}  Длина: {_snake.Length}", 1, 0, _palette.Score);
        }
    }
}
