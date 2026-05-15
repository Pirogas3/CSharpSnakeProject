using CSharpSnakeProject.Maps;
using CSharpSnakeProject.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpSnakeProject.Logic.Food
{
    public class FoodGenerator : IFoodGenerator
    {
        private readonly Random _random = new Random();

        public Cell GenerateFood(IFood food, IMap map, List<Cell> snakeBody)
        {
            const int maxAttempts = 100;
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                Cell candidate = new Cell(_random.Next(1, map.Width - 1), _random.Next(1, map.Height - 1));
                if (food.IsValidPosition(candidate, map, snakeBody))
                    return candidate;
            }
            // Если не повезло, ищем полным перебором
            for (int x = 1; x < map.Width - 1; x++)
                for (int y = 1; y < map.Height - 1; y++)
                {
                    Cell candidate = new Cell(x, y);
                    if (food.IsValidPosition(candidate, map, snakeBody))
                        return candidate;
                }
            throw new InvalidOperationException("Нет свободных клеток для еды");
        }
    }
}