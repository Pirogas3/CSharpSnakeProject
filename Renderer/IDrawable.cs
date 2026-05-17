using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpSnakeProject.Renderer
{
    public interface IDrawable
    {
        void Draw(ConsoleRenderer renderer, GamePalette palette);
    }
}
