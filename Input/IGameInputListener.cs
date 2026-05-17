
namespace CSharpSnakeProject.Input
{
    public interface IGameInputListener
    {
        void OnArrowUp();
        void OnArrowDown();
        void OnArrowLeft();
        void OnArrowRight();
        void OnPause();
        void OnShot();
        void OnExit();
    }
}