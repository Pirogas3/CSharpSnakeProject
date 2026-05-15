using CSharpSnakeProject.Input;
using CSharpSnakeProject.Logic.GameState;
using CSharpSnakeProject.Renderer;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpSnakeProject.Logic
{
    public abstract class BaseGameLogic : IGameInputListener
    {
        public BaseGameState? currentState { get; private set; }
        protected float time { get; private set; }
        protected int screenWidth { get; private set; }
        protected int screenHeight { get; private set; }

        public abstract void OnArrowUp();
        public abstract void OnArrowDown();
        public abstract void OnArrowLeft();
        public abstract void OnArrowRight();
        public abstract void OnPause();
        public abstract void OnExit();

        public abstract void Update(float deltaTime);

        public abstract GamePalette CreatePalette();

        public void DrawNewState(float deltaTime, ConsoleRenderer renderer)
        {
            time += deltaTime;
            screenWidth = renderer.width;
            screenHeight = renderer.height;

            currentState?.Update(deltaTime);
            currentState?.Draw(renderer);

            Update(deltaTime);
        }

        public void ChangeState(BaseGameState? state)
        {
            currentState?.Reset();
            currentState = state;
        }

        public void InitializeInput(ConsoleInput input)
        {
            input.Subscribe(this);
        }
    }
}