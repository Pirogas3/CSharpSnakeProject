using CSharpSnakeProject.Renderer;

namespace CSharpSnakeProject.Logic.GameState
{
    public class ControlsState : BaseGameState
    {
        private SnakeGameLogic _gameLogic;
        private string[] controls = {
            "Управление:",
            "Стрелки / WASD - движение змейки",
            "Пробел - пауза во время игры",
            "Shift - ускорение змейки",
            "Z / C - плевок ядом (растворяет мины)", 
            "Esc - выход в главное меню",
            "",
            "Нажмите любую клавишу для возврата в меню"
        };

        public ControlsState(SnakeGameLogic gameLogic)
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
            for (int i = 0; i < controls.Length; i++)
            {
                int x = centerX - controls[i].Length / 2;
                renderer.DrawString(controls[i], x, centerY - controls.Length / 2 + i, ConsoleColor.White);
            }
            renderer.Render();
        }

        public override void Reset() { }
    }
}
