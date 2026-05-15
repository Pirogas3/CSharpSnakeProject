using CSharpSnakeProject.Renderer;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpSnakeProject.Logic.GameState
{
    public class MenuState : BaseGameState
    {
        private string[] menuItems = { "Начать новую игру", "Управление", "Титры", "Выход" };
        private int selectedIndex = 0;
        private SnakeGameLogic _gameLogic;

        public MenuState(SnakeGameLogic gameLogic)
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
                    case ConsoleKey.UpArrow:
                        selectedIndex = (selectedIndex - 1 + menuItems.Length) % menuItems.Length;
                        break;
                    case ConsoleKey.DownArrow:
                        selectedIndex = (selectedIndex + 1) % menuItems.Length;
                        break;
                    case ConsoleKey.Enter:
                        switch (selectedIndex)
                        {
                            case 0: // Новая игра
                                _gameLogic.ChangeState(new SettingsState(_gameLogic));
                                break;
                            case 1: // Управление
                                _gameLogic.ChangeState(new ControlsState(_gameLogic));
                                break;
                            case 2: // Титры
                                _gameLogic.ChangeState(new CreditsState(_gameLogic));
                                break;
                            case 3: // Выход
                                Environment.Exit(0);
                                break;
                        }
                        break;
                }
            }
        }

        public override void Draw(ConsoleRenderer renderer)
        {
            renderer.Clear();
            int centerX = renderer.width / 2;
            int centerY = renderer.height / 2;

            for (int i = 0; i < menuItems.Length; i++)
            {
                string prefix = (i == selectedIndex) ? "> " : "  ";
                string line = prefix + menuItems[i];
                int x = centerX - line.Length / 2;
                renderer.DrawString(line, x, centerY - menuItems.Length / 2 + i, ConsoleColor.White);
            }
            renderer.Render();
        }

        public override void Reset() { }
    }
}
