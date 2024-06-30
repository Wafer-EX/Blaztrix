namespace Blaztrix.Components.Tetris.Engine.Elements
{
    public static class Presets
    {
        public static readonly bool[,] FigureO = new bool[2, 2] { { true, true }, { true, true } };

        public static readonly bool[,] FigureI = new bool[1, 4] { { true, true, true, true } };

        public static readonly bool[,] FigureS = new bool[3, 2] { { false, true }, { true, true }, { true, false} };

        public static readonly bool[,] FigureZ = new bool[3, 2] { { true, false }, { true, true }, { false, true } };

        public static readonly bool[,] FigureL = new bool[2, 3] { { true, true, true }, { false, false, true } };

        public static readonly bool[,] FigureJ = new bool[2, 3] { { false, false, true }, { true, true, true } };

        public static readonly bool[,] FigureT = new bool[3, 2] { { true, false }, { true, true }, { true, false } };
    }
}