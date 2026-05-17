
namespace CSharpSnakeProject.Logic.Food
{
    public class Apple : FoodBase
    {
        public override char Symbol => '●';
        public override string Name => "apple";
        public override int ScoreValue => 10;
        public override ConsoleColor Color => ConsoleColor.Green;
    }
}