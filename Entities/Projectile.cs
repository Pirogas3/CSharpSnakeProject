
using CSharpSnakeProject.Enums;
using CSharpSnakeProject.Maps;
using CSharpSnakeProject.Structs;

namespace CSharpSnakeProject.Entities
{
    public class Projectile
    {
        public Cell Position { get; set; }
        public SnakeDirection Direction { get; set; }
        public int Range { get; set; }
        public char Symbol => '*';
        public ConsoleColor Color => ConsoleColor.Green;

        public Projectile(Cell startPos, SnakeDirection direction, int maxRange = 8)
        {
            Position = startPos;
            Direction = direction;
            Range = maxRange;
        }
    }
}
