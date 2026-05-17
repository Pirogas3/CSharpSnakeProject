using CSharpSnakeProject.Maps;
using CSharpSnakeProject.Structs;

namespace CSharpSnakeProject.Logic.Food
{
    public interface IFoodGenerator
    {
        Cell GenerateFood(IFood food, IMap map, List<Cell> snakeBody);
    }
}