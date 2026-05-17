using CSharpSnakeProject.Renderer;

namespace CSharpSnakeProject.Logic.GameState
{
    public class CreditsState : BaseGameState
    {
        private SnakeGameLogic _gameLogic;
        private string[] credits = {
            "Титры:",
            "Разработчик: Victor Levachin",
            "Дипломная работа: Змейка",
            "",
            "Нажмите любую клавишу для возврата в меню"
        };

        public CreditsState(SnakeGameLogic gameLogic)
        {
            _gameLogic = gameLogic;
        }

        public override void Update(float deltaTime)
        {
            if (Console.KeyAvailable)
            {
                Console.ReadKey(true);
                _gameLogic.ChangeState(new MenuState(_gameLogic));
            }
        }

        public override void Draw(ConsoleRenderer renderer)
        {
            renderer.Clear();
            int centerX = renderer.width / 2;
            int centerY = renderer.height / 2;
            for (int i = 0; i < credits.Length; i++)
            {
                int x = centerX - credits[i].Length / 2;
                renderer.DrawString(credits[i], x, centerY - credits.Length / 2 + i, ConsoleColor.White);
            }
            renderer.Render();
        }

        public override void Reset() { }
    }
}
