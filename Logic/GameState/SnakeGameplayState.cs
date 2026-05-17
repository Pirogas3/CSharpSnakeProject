using CSharpSnakeProject.Enums;
using CSharpSnakeProject.Logic.Food;
using CSharpSnakeProject.Logic.GameWorlds;
using CSharpSnakeProject.Maps;
using CSharpSnakeProject.Renderer;

namespace CSharpSnakeProject.Logic.GameState
{
    public class SnakeGameplayState : BaseGameState
    {
        private GameWorld _world;
        private bool _isPaused = false;
        private SnakeGameLogic _gameLogic; // ссылка на логику для выхода в меню
        public override bool HandlesInput => true;

        public SnakeGameplayState(IMap map, List<IFood> availableFoods, GamePalette palette, IFoodGenerator foodGenerator, float speedSnake, SnakeGameLogic gameLogic, bool hasEnemies)
        {
            _gameLogic = gameLogic;
            _world = new GameWorld(map, availableFoods, palette, foodGenerator, speedSnake, hasEnemies);
        }

        public override void Reset()
        {
            //не понадобилось
        }

        public void Shot()
        {
            _world.Shoot();
        }

        public void TogglePause()
        {
            if (_world.IsGameOver) return;
            _isPaused = !_isPaused;
        }

        public void ExitToMenu()
        {
            _gameLogic.ChangeState(new MenuState(_gameLogic));
        }

        public override void Update(float deltaTime)
        {
            if (_world.IsGameOver)
            {
                Console.Clear();
                Console.SetCursorPosition(1, 2);
                Console.WriteLine($"Игра окончена! Ваш счёт: {_world.Score}.");
                Console.SetCursorPosition(1, 3);
                Console.WriteLine("Нажмите любую клавишу для выхода в меню.");
                Console.ReadKey();
                ExitToMenu();
            }

            if (_isPaused) return;
            _world.Update(deltaTime);
        }

        public override void Draw(ConsoleRenderer renderer)
        {
            _world.Draw(renderer);

            if (_isPaused)
            {
                string pauseText = "ПАУЗА. Нажмите Пробел";
                renderer.DrawString(pauseText, renderer.width / 2 - pauseText.Length / 2, renderer.height / 2, ConsoleColor.Red);
            }
        }

        public void SetDirection(SnakeDirection dir)
        {
            _world.SetDirection(dir);
        }
    }
}