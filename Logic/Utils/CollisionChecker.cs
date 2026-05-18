using CSharpSnakeProject.Entities;
using CSharpSnakeProject.Logic.Managers;
using CSharpSnakeProject.Maps;
using CSharpSnakeProject.Structs;

namespace CSharpSnakeProject.Logic.Utils
{
    public static class CollisionChecker
    {
        // Проверка стены
        public static bool IsWall(Cell cell, IMap map) => map.GetWalls().Contains(cell);

        // Проверка тела змеи (можно исключить голову)
        public static bool IsSnakeBody(Cell cell, Snake snake, bool includeHead = true)
        {
            if (includeHead)
                return snake.Body.Contains(cell);
            else
                return snake.Body.Skip(1).Any(segment => segment == cell);
        }

        // Проверка еды (по позиции)
        public static bool IsFood(Cell cell, Cell foodPosition) => cell == foodPosition;

        // Проверка мины (если менеджер мин существует)
        public static bool IsMine(Cell cell, MineManager mineManager) =>
            mineManager != null && mineManager.Mines.Any(m => m.Position == cell);
    }
}
