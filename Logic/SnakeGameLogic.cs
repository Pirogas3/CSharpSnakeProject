using CSharpSnakeProject.Enums;
using CSharpSnakeProject.Input;
using CSharpSnakeProject.Logic.Food;
using CSharpSnakeProject.Logic.GameState;
using CSharpSnakeProject.Maps;
using CSharpSnakeProject.Renderer;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpSnakeProject.Logic
{
    public class SnakeGameLogic : BaseGameLogic
    {
        public SnakeGameLogic()
        {
            ChangeState(new MenuState(this));
        }

        public void StartGame(IMap map, List<IFood> availableFoods, GamePalette palette, IFoodGenerator foodGenerator, float speedSnake)
        {
            var gameplayState = new SnakeGameplayState(map, availableFoods, palette, foodGenerator, speedSnake, this);
            ChangeState(gameplayState);
            gameplayState.Reset();
        }

        public override void OnArrowUp()
        {
            if (currentState is SnakeGameplayState gameplay)
                gameplay.SetDirection(SnakeDirection.Up);
        }

        public override void OnArrowDown()
        {
            if (currentState is SnakeGameplayState gameplay)
                gameplay.SetDirection(SnakeDirection.Down);
        }

        public override void OnArrowLeft()
        {
            if (currentState is SnakeGameplayState gameplay)
                gameplay.SetDirection(SnakeDirection.Left);
        }

        public override void OnArrowRight()
        {
            if (currentState is SnakeGameplayState gameplay)
                gameplay.SetDirection(SnakeDirection.Right);
        }

        public override void OnPause()
        {
            if (currentState is SnakeGameplayState gameplay)
                gameplay.TogglePause();
        }

        public override void OnExit()
        {
            if (currentState is SnakeGameplayState gameplay)
                gameplay.ExitToMenu();
        }

        public override void Update(float deltaTime)
        {
            
        }

        public override GamePalette CreatePalette()
        {
            return new GamePalette(
                background: ConsoleColor.Black,
                food: ConsoleColor.Red,
                walls: ConsoleColor.White,
                snakeBody: ConsoleColor.Green,
                snakeHead: ConsoleColor.Blue,
                score: ConsoleColor.White
            );
        }
    }
}