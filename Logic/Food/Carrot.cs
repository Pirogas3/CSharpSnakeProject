using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpSnakeProject.Logic.Food
{
    public class Carrot : FoodBase
    {
        public override char Symbol => '◎';
        public override string Name => "carrot";
        public override int ScoreValue => 15;
        public override ConsoleColor Color => ConsoleColor.Red;
    }
}