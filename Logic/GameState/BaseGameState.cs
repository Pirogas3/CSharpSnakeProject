using CSharpSnakeProject.Renderer;

namespace CSharpSnakeProject.Logic.GameState
{
    public abstract class BaseGameState
    {
        public abstract void Update(float deltaTime);
        public abstract void Reset();
        public abstract void Draw(ConsoleRenderer renderer);
        public virtual bool HandlesInput => false;
    }
}