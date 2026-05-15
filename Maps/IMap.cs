using CSharpSnakeProject.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpSnakeProject.Maps
{
    public interface IMap
    {
        int Width { get; }
        int Height { get; }
        IEnumerable<Cell> GetWalls();
        char GetWallSymbol(Cell position);
    }
}