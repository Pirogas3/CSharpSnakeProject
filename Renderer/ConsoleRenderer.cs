using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpSnakeProject.Renderer
{
    public class ConsoleRenderer
    {
        public int width { get; private set; }
        public int height { get; private set; }

        private const int MaxColors = 8;
        private readonly ConsoleColor[] _colors;
        private readonly char[,] _pixels;
        private readonly byte[,] _pixelColors;
        private readonly int _maxWidth;
        private readonly int _maxHeight;

        public ConsoleColor bgColor { get; set; }

        public char this[int w, int h]
        {
            get { return _pixels[w, h]; }
            set { _pixels[w, h] = value; }
        }

        public ConsoleRenderer(ConsoleColor[] colors)
        {
            if (colors.Length > MaxColors)
            {
                var tmp = new ConsoleColor[MaxColors];
                Array.Copy(colors, tmp, tmp.Length);
                colors = tmp;
            }

            _colors = colors;

            _maxWidth = Console.LargestWindowWidth;
            _maxHeight = Console.LargestWindowHeight;
            width = Console.WindowWidth;
            height = Console.WindowHeight;

            _pixels = new char[_maxWidth, _maxHeight];
            _pixelColors = new byte[_maxWidth, _maxHeight];
        }

        public void SetPixel(int w, int h, char val, byte colorIdx)
        {
            _pixels[w, h] = val;
            _pixelColors[w, h] = colorIdx;
        }

        public byte GetColorIndex(ConsoleColor color)
        {
            var index = Array.IndexOf(_colors, color);
            return index == -1 ? (byte)0 : (byte)index;
        }

        public void Render()
        {
            Console.BackgroundColor = bgColor;
            for (int w = 0; w < Math.Min(width, _maxWidth); w++)
            {
                for (int h = 0; h < Math.Min(height, _maxHeight); h++)
                {
                    byte colorIdx = _pixelColors[w, h];
                    char symbol = _pixels[w, h];
                    if (symbol == 0 || _colors[colorIdx] == bgColor)
                    {
                        symbol = ' ';
                        colorIdx = GetColorIndex(bgColor);
                    }
                    Console.ForegroundColor = _colors[colorIdx];
                    Console.SetCursorPosition(w, h);
                    Console.Write(symbol);
                }
            }
            Console.ResetColor();
            Console.CursorVisible = false;
        }

        public void DrawString(string text, int atWidth, int atHeight, ConsoleColor color)
        {
            var colorIdx = Array.IndexOf(_colors, color);
            if (colorIdx < 0) return;

            for (int i = 0; i < text.Length; i++)
            {
                int x = atWidth + i;
                if (x >= 0 && x < width && atHeight >= 0 && atHeight < height)
                {
                    _pixels[x, atHeight] = text[i];
                    _pixelColors[x, atHeight] = (byte)colorIdx;
                }
            }
        }

        public void Clear()
        {
            for (int w = 0; w < width; w++)
                for (int h = 0; h < height; h++)
                {
                    _pixelColors[w, h] = 0;
                    _pixels[w, h] = (char)0;
                }
        }

        //public override bool Equals(object? obj)
        //{
        //    if (obj is not ConsoleRenderer casted)
        //        return false;

        //    if (_maxWidth != casted._maxWidth || _maxHeight != casted._maxHeight ||
        //        width != casted.width || height != casted.height ||
        //        _colors.Length != casted._colors.Length)
        //    {
        //        return false;
        //    }


        //    for (int i = 0; i < _colors.Length; i++)
        //    {
        //        if (_colors[i] != casted._colors[i])
        //            return false;
        //    }

        //    for (int w = 0; w < width; w++)
        //        for (var h = 0; h < height; h++)
        //        {
        //            if (_pixels[w, h] != casted._pixels[w, h] ||
        //                            _pixelColors[w, h] != casted._pixelColors[w, h])
        //            {
        //                return false;
        //            }
        //        }

        //    return true;
        //}

        public override int GetHashCode()
        {
            var hash = HashCode.Combine(_maxWidth, _maxHeight, width, height);

            for (int i = 0; i < _colors.Length; i++)
            {
                hash = HashCode.Combine(hash, _colors[i]);
            }

            for (int w = 0; w < width; w++)
                for (var h = 0; h < height; h++)
                {
                    hash = HashCode.Combine(hash, _pixelColors[w, h], _pixels[w, h]);
                }

            return hash;
        }
    }
}