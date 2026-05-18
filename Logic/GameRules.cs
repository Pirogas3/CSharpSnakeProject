using CSharpSnakeProject.Entities;
using CSharpSnakeProject.Logic.Managers;
using CSharpSnakeProject.Maps;
using CSharpSnakeProject.Structs;

namespace CSharpSnakeProject.Logic
{
    public class GameRules : IGameRules
    {
        public bool IsWallCollision(Cell head, IMap map) => map.GetWalls().Contains(head);
        public bool IsSelfCollision(Snake snake) => snake.CollidesWithSelf();
        public bool IsFoodCollision(Cell head, FoodManager foodManager) => head == foodManager.Position;

        public bool ApplyWallCollisionEffect() => true; // игра окончена

        public bool ApplySelfCollisionEffect(Snake snake, ref int score)
        {
            snake.CutTail();
            score = Math.Max(0, score - 10);
            return snake.Length == 0;
        }

        public void ApplyMineCollisionEffect(Snake snake, ref int score)
        {
            while (snake.Length > 2) //штраф за столкновение с миной
                snake.CutTail();
            score = 0;
        }

        public bool ApplyFoodEffect(Snake snake, FoodManager foodManager, ref int score, Cell nextHead)
        {
            snake.Move(nextHead, true);
            score += foodManager.CurrentFood.ScoreValue;
            foodManager.GenerateFood(snake.Body);
            return false;
        }

        public void ApplyNormalMove(Snake snake, Cell nextHead)
        {
            snake.Move(nextHead, false);
        }
    }
}
