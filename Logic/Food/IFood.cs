using CSharpSnakeProject.Maps;
using CSharpSnakeProject.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpSnakeProject.Logic.Food
{
    public interface IFood
    {
        char Symbol { get; }
        string Name { get; }
        int ScoreValue { get; }
        float LifespanSeconds { get; }
        ConsoleColor Color { get; }
        bool IsValidPosition(Cell position, IMap map, List<Cell> snakeBody);
    }
}