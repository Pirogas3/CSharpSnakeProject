
namespace CSharpSnakeProject.Input
{
    public class ConsoleInput
    {
        private readonly HashSet<IGameInputListener> listeners = new();

        public void Subscribe(IGameInputListener l) => listeners.Add(l);


        public void Update()
        {
            while (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow or ConsoleKey.W:
                        foreach (var l in listeners) l.OnArrowUp();
                        break;
                    case ConsoleKey.DownArrow or ConsoleKey.S:
                        foreach (var l in listeners) l.OnArrowDown();
                        break;
                    case ConsoleKey.LeftArrow or ConsoleKey.A:
                        foreach (var l in listeners) l.OnArrowLeft();
                        break;
                    case ConsoleKey.RightArrow or ConsoleKey.D:
                        foreach (var l in listeners) l.OnArrowRight();
                        break;
                    case ConsoleKey.Spacebar:
                        foreach (var l in listeners) l.OnPause();
                        break;
                    case ConsoleKey.Z or ConsoleKey.C:
                        foreach (var l in listeners) l.OnShot();
                        break;
                    case ConsoleKey.Escape:
                        foreach (var l in listeners) l.OnExit();
                        break;
                }
            }
        }
    }
}