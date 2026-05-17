using CSharpSnakeProject.Structs;

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