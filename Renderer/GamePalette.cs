
namespace CSharpSnakeProject.Renderer
{
    public class GamePalette
    {
        public ConsoleColor Background { get; }
        public ConsoleColor Food { get; }
        public ConsoleColor Walls { get; }
        public ConsoleColor SnakeBody { get; }
        public ConsoleColor SnakeHead { get; }
        public ConsoleColor Score { get; }
        public ConsoleColor Enemis { get; }

        public GamePalette(ConsoleColor background, ConsoleColor food, ConsoleColor walls,
            ConsoleColor snakeBody, ConsoleColor snakeHead, ConsoleColor score, ConsoleColor enemis)
        {
            Background = background;
            Food = food;
            Walls = walls;
            SnakeBody = snakeBody;
            SnakeHead = snakeHead;
            Score = score;
            Enemis = enemis;
        }

        public ConsoleColor[] ToColorArray()
        {
            return new ConsoleColor[]
            {
                Background,
                Food,
                Walls,
                SnakeBody,
                SnakeHead,
                Score,
                Enemis
            };
        }
    }
}