using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpSnakeProject.Input
{
    public interface IGameInputListener
    {
        void OnArrowUp();
        void OnArrowDown();
        void OnArrowLeft();
        void OnArrowRight();
        void OnPause();
        void OnExit();
    }
}