using CSharpSnakeProject.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpSnakeProject.Entities
{
    public class Mine
    {
        public Cell Position { get; set; }
        public float TimeLeft { get; set; }
        public char Symbol => '☢';
        public ConsoleColor Color => ConsoleColor.Red;

        private float _moveTimer = 0f;
        private const float MoveInterval = 1.5f; // шаг каждые n секунд

        public Mine(Cell position, float lifespanSeconds = 10f)
        {
            Position = position;
            TimeLeft = lifespanSeconds;
        }

        // Обновление таймера жизни и возврат true, если пора исчезнуть
        public bool UpdateLife(float deltaTime)
        {
            TimeLeft -= deltaTime;
            return TimeLeft <= 0;
        }

        // Обновление движения: возвращает true, если мина должна быть перемещена
        public bool UpdateMovement(float deltaTime, Cell snakeHead)
        {
            _moveTimer += deltaTime;
            if (_moveTimer < MoveInterval) return false;
            _moveTimer = 0f;

            // Вычисляем направление к голове змейки
            int dx = Math.Sign(snakeHead.X - Position.X);
            int dy = Math.Sign(snakeHead.Y - Position.Y);

            if (dx != 0)
            {
                Cell newPos = new Cell(Position.X + dx, Position.Y);
                Position = newPos;
                return true;
            }
            else if (dy != 0)
            {
                Cell newPos = new Cell(Position.X, Position.Y + dy);
                Position = newPos;
                return true;
            }
            return false;
        }
    }
}
