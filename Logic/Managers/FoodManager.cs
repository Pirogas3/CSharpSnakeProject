using CSharpSnakeProject.Logic.Food;
using CSharpSnakeProject.Maps;
using CSharpSnakeProject.Renderer;
using CSharpSnakeProject.Structs;

namespace CSharpSnakeProject.Logic.Managers
{
    public class FoodManager : IDrawable
    {
        private readonly IMap _map;
        private readonly List<IFood> _availableFoods;
        private readonly IFoodGenerator _foodGenerator;
        private readonly Random _rand = new Random();

        public IFood CurrentFood { get; private set; }
        public Cell Position { get; private set; }
        private float _timeLeft;

        public FoodManager(IMap map, List<IFood> availableFoods, IFoodGenerator foodGenerator)
        {
            _map = map;
            _availableFoods = availableFoods;
            _foodGenerator = foodGenerator;
        }

        public void GenerateFood(IList<Cell> snakeBody)
        {
            if (_availableFoods.Count == 0)
                throw new InvalidOperationException("Нет доступной еды");

            CurrentFood = _availableFoods[_rand.Next(_availableFoods.Count)];
            Position = _foodGenerator.GenerateFood(CurrentFood, _map, snakeBody);
            _timeLeft = CurrentFood.LifespanSeconds;
        }

        public void UpdateTimer(float deltaTime, IList<Cell> snakeBody)
        {
            _timeLeft -= deltaTime;
            if (_timeLeft <= 0f)
            {
                GenerateFood(snakeBody);
            }
        }

        public void Draw(ConsoleRenderer renderer, GamePalette palette)
        {
            renderer.SetPixel(Position.X, Position.Y, CurrentFood.Symbol,
                renderer.GetColorIndex(CurrentFood.Color));
        }
    }
}
