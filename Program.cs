using CSharpSnakeProject.Input;
using CSharpSnakeProject.Logic;
using CSharpSnakeProject.Logic.GameState;
using CSharpSnakeProject.Renderer;
using System.Diagnostics;
using System.Runtime.InteropServices;


internal class Program
{
    static void Main()
    {
        try
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "Змейка";
        }
        catch { /* игнорируем, если не поддерживается */ }

        const float targetFrameTime = 1f / 60f; // fps

        var gameLogic = new SnakeGameLogic();
        var palette = gameLogic.CreatePalette();
        var input = new ConsoleInput();
        gameLogic.InitializeInput(input);

        var renderer = new ConsoleRenderer(palette.ToColorArray());
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        float deltaTime = targetFrameTime;

        while (true)
        {
            // Измеряем точное время, прошедшее с предыдущего кадра
            long elapsedTicks = stopwatch.ElapsedTicks;
            stopwatch.Restart();
            deltaTime = (float)elapsedTicks / System.Diagnostics.Stopwatch.Frequency;

            // Ограничиваем максимальный дельта-тайм (если игра зависла, не делаем больших скачков)
            if (deltaTime > 0.1f) deltaTime = 0.1f;

            // Обновляем ввод только если текущее состояние требует этого
            if (gameLogic.currentState is BaseGameState state && state.HandlesInput)
                input.Update();
            else
            {
                
            }

            gameLogic.DrawNewState(deltaTime, renderer);
            renderer.Render();
            renderer.Clear();

            // Грубое ожидание до следующего кадра, чтобы не нагружать CPU
            int sleepMs = (int)((targetFrameTime - deltaTime) * 1000);
            if (sleepMs > 0) Thread.Sleep(sleepMs);
        }
    }
}