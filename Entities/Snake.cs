using CSharpSnakeProject.Enums;
using CSharpSnakeProject.Renderer;
using CSharpSnakeProject.Structs;

namespace CSharpSnakeProject.Entities
{
    public class Snake : IDrawable
    {
        private float _basicSpeedSnake = 4f;
        private float _speedBoost = 2.5f;
        private float _currentSpeedSnake = 4f;
        public List<Cell> Body { get; private set; }
        public SnakeDirection Direction { get; set; }
        private char _symbol = '■'; //Символ змейки меняется тут
        public char Symbol { get => _symbol; set => _symbol = value; }
        public float BasicSpeedSnake { get => _basicSpeedSnake; }
        public float SpeedBoost { get => _speedBoost; }
        public float CurrentSpeedSnake { get => _currentSpeedSnake; set => _currentSpeedSnake = value; }

        public Snake(Cell startHead, Cell startTail, SnakeDirection startDirection, float speedSnake)
        {
            Body = new List<Cell> { startHead, startTail };
            Direction = startDirection;
            _basicSpeedSnake = speedSnake;
            _currentSpeedSnake = _basicSpeedSnake;
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

        // Проверка самопересечения (голова врезалась в тело)
        public bool CollidesWithSelf()
        {
            var head = GetHead();
            return Body.Skip(1).Any(segment => segment == head);
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

        public void Draw(ConsoleRenderer renderer, GamePalette palette)
        {
            for (int i = 0; i < Body.Count; i++)
            {
                var cell = Body[i];
                byte colorIdx = i == 0
                    ? renderer.GetColorIndex(palette.SnakeHead)
                    : renderer.GetColorIndex(palette.SnakeBody);
                renderer.SetPixel(cell.X, cell.Y, Symbol, colorIdx);
            }
        }
    }
}
