using CSharpSnakeProject.Maps;
using CSharpSnakeProject.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpSnakeProject.Logic.Food
{
    public abstract class FoodBase : IFood
    {
        public abstract char Symbol { get; }
        public abstract string Name { get; }
        public abstract int ScoreValue { get; }
        public virtual float LifespanSeconds => 30f; //стандартное время жизни еды
        public abstract ConsoleColor Color { get; }

        public virtual bool IsValidPosition(Cell position, IMap map, List<Cell> snakeBody)
        {
            return !snakeBody.Contains(position) && !map.GetWalls().Contains(position);
        }
    }
}