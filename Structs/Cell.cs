using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpSnakeProject.Structs
{
    public readonly struct Cell
    {
        public int X { get; }
        public int Y { get; }

        public Cell(int x, int y)
        {
            X = x;
            Y = y;
        }

        // Переопределение Equals для сравнения ячеек
        public bool Equals(Cell other) => X == other.X && Y == other.Y;

        public override bool Equals(object? obj) => obj is Cell cell && Equals(cell);

        public override int GetHashCode() => HashCode.Combine(X, Y);

        // Операторы сравнения для удобства
        public static bool operator ==(Cell left, Cell right) => left.Equals(right);
        public static bool operator !=(Cell left, Cell right) => !left.Equals(right);

        // Строковое представление для отладки
        public override string ToString() => $"({X}, {Y})";
    }
}