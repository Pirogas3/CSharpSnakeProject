using CSharpSnakeProject.Enums;
using CSharpSnakeProject.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpSnakeProject.Entities
{
    public class Snake
    {
        public List<Cell> Body { get; private set; }
        public SnakeDirection Direction { get; set; }
        private char _symbol = '■'; //Символ змейки меняется тут
        public char Symbol { get => _symbol; set => _symbol = value; }

        public Snake(Cell startHead, Cell startTail, SnakeDirection startDirection)
        {
            Body = new List<Cell> { startHead, startTail };
            Direction = startDirection;
        }

        // Голова змейки
        public Cell GetHead() => Body[0];

        // Хвост (последний сегмент)
        public Cell GetTail() => Body[^1];

        // Длина змейки
        public int Length => Body.Count;

        // Движение змейки с опцией "съела ли еду"
        public void Move(Cell newHead, bool ateFood = false)
        {
            Body.Insert(0, newHead);
            if (!ateFood)
                Body.RemoveAt(Body.Count - 1);
        }

        // Добавление сегмента без удаления хвоста (используется когда съели еду, но можно и отдельный метод Grow)
        public void Grow(Cell newHead)
        {
            Body.Insert(0, newHead);
        }

        // Проверка, находится ли заданная клетка в теле (включая голову)
        public bool Occupies(Cell cell)
        {
            return Body.Contains(cell);
        }

        // Проверка самопересечения (голова врезалась в тело)
        public bool CollidesWithSelf()
        {
            var head = GetHead();
            return Body.Skip(1).Any(segment => segment == head);
        }

        // Проверка столкновения с произвольным набором препятствий (стены, враги, мины)
        public bool CollidesWith(IEnumerable<Cell> obstacles)
        {
            return obstacles.Contains(GetHead());
        }

        // Вспомогательный метод: вычислить новую позицию головы при движении в текущем направлении
        public Cell GetNextHead()
        {
            return Direction switch
            {
                SnakeDirection.Up => new Cell(GetHead().X, GetHead().Y - 1),
                SnakeDirection.Down => new Cell(GetHead().X, GetHead().Y + 1),
                SnakeDirection.Left => new Cell(GetHead().X - 1, GetHead().Y),
                SnakeDirection.Right => new Cell(GetHead().X + 1, GetHead().Y),
                _ => GetHead()
            };
        }

        // Откусить хвост (штраф, когда змейка врезалась в себя)
        public void CutTail()
        {
            if (Body.Count > 0)
                Body.RemoveAt(Body.Count - 1);
        }

        // Сброс змейки в начальное состояние (при перезапуске уровня)
        public void Reset(Cell startHead, Cell startTail, SnakeDirection startDirection)
        {
            Body.Clear();
            Body.Add(startHead);
            Body.Add(startTail);
            Direction = startDirection;
        }
    }
}
