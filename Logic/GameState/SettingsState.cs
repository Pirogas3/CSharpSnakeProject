using CSharpSnakeProject.Logic.Food;
using CSharpSnakeProject.Maps;
using CSharpSnakeProject.Renderer;

namespace CSharpSnakeProject.Logic.GameState
{
    public class SettingsState : BaseGameState
    {
        private SnakeGameLogic _gameLogic;
        private string[] options = { "Карта: Без препятствий", "Сложность: Средняя", "Враги: Нет" };
        private int selectedIndex = 0;
        private int mapType = 0;     // 0 - Basic, 1 - Obstacle
        private int difficulty = 1;   // 0 - Easy, 1 - Medium, 2 - Hard
        private int enemies = 0;      // 0 - Нет, 1 - Да (пока только 0)

        public SettingsState(SnakeGameLogic gameLogic)
        {
            _gameLogic = gameLogic;
        }

        public override void Update(float deltaTime)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.UpArrow or ConsoleKey.W:
                        selectedIndex = (selectedIndex - 1 + options.Length) % options.Length;
                        break;
                    case ConsoleKey.DownArrow or ConsoleKey.S:
                        selectedIndex = (selectedIndex + 1) % options.Length;
                        break;
                    case ConsoleKey.LeftArrow or ConsoleKey.A:
                        ChangeOption(-1);
                        break;
                    case ConsoleKey.RightArrow or ConsoleKey.D:
                        ChangeOption(1);
                        break;
                    case ConsoleKey.Enter:
                        StartGame();
                        break;
                    case ConsoleKey.Escape:
                        _gameLogic.ChangeState(new MenuState(_gameLogic));
                        break;
                }
            }
        }

        private void ChangeOption(int delta)
        {
            switch (selectedIndex)
            {
                case 0: // Карта
                    mapType = (mapType + delta) % 2;
                    options[0] = mapType == 0 ? "Карта: Без препятствий" : "Карта: С препятствиями";
                    break;
                case 1: // Сложность
                    difficulty = (difficulty + delta) % 3;
                    string[] diffNames = { "Лёгкая", "Средняя", "Сложная" };
                    options[1] = $"Сложность: {diffNames[difficulty]} (скорость змейки)";
                    break;
                case 2: // Враги
                    enemies = (enemies + delta) % 2;
                    options[2] = enemies == 0 ? "Враги: Нет" : "Враги: Да (✵  - мины)";
                    break;
            }
        }

        private void StartGame()
        {
            // Создаём карту
            IMap map;
            int width = 30, height = 20;
            if (mapType == 0)
                map = new BasicMap(width, height);
            else
                map = new ObstacleMap(width, height);

            // Скорость змейки (шагов в секунду)
            float snakeSpeed;
            switch (difficulty)
            {
                case 0: snakeSpeed = 4f; break;
                case 1: snakeSpeed = 8f; break;
                default: snakeSpeed = 12f; break;
            }

            // Еда
            List<IFood> availableFoods = new List<IFood> { new Apple(), new Carrot() };
            GamePalette palette = _gameLogic.CreatePalette();
            IFoodGenerator foodGenerator = new FoodGenerator();

            bool hasEnemies = enemies == 1;

            _gameLogic.StartGame(map, availableFoods, palette, foodGenerator, snakeSpeed, hasEnemies);
        }

        public override void Draw(ConsoleRenderer renderer)
        {
            renderer.Clear();
            int centerX = renderer.width / 2;
            int centerY = renderer.height / 2;

            renderer.DrawString("НАСТРОЙКИ УРОВНЯ", centerX - 8, centerY - 3, ConsoleColor.White);

            for (int i = 0; i < options.Length; i++)
            {
                string prefix = (i == selectedIndex) ? "> " : "  ";
                string line = prefix + options[i];
                int x = centerX - line.Length / 2;
                renderer.DrawString(line, x, centerY - 1 + i, ConsoleColor.White);
            }

            renderer.DrawString("Enter - начать игру, Esc - назад", centerX - 15, centerY + 3, ConsoleColor.White);
            renderer.Render();
        }

        public override void Reset() { }
    }
}
