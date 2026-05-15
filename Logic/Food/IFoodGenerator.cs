using CSharpSnakeProject.Maps;
using CSharpSnakeProject.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpSnakeProject.Logic.Food
{
    public interface IFoodGenerator
    {
        Cell GenerateFood(IFood food, IMap map, List<Cell> snakeBody);
    }
}