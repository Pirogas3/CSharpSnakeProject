using CSharpSnakeProject.Entities;
using CSharpSnakeProject.Logic.Managers;
using CSharpSnakeProject.Maps;
using CSharpSnakeProject.Structs;

namespace CSharpSnakeProject.Logic
{
    public interface IGameRules
    {
        bool IsWallCollision(Cell head, IMap map);
        bool IsSelfCollision(Snake snake);
        bool IsFoodCollision(Cell head, FoodManager foodManager);

        bool ApplyWallCollisionEffect();  // всегда game over
        bool ApplySelfCollisionEffect(Snake snake, ref int score);
        void ApplyMineCollisionEffect(Snake snake, ref int score);
        bool ApplyFoodEffect(Snake snake, FoodManager foodManager, ref int score, Cell nextHead);
        void ApplyNormalMove(Snake snake, Cell nextHead);
    }
}
