using System;

namespace Blaztrix.Components.Tetris.Engine.Elements
{
    public class Block
    {
        private static readonly Random random = new();

        public bool[,] Figure { get; set; }

        public Position Position { get; set; }

        public bool IsSetted { get; set; }

        public Block(int fieldWidth) => Position = new Position(fieldWidth / 2 - 2, 0);

        public Block Generate()
        {
            switch (random.Next(1, 8))
            {
                case 1: Figure = Presets.FigureO; break;
                case 2: Figure = Presets.FigureI; break;
                case 3: Figure = Presets.FigureS; break;
                case 4: Figure = Presets.FigureZ; break;
                case 5: Figure = Presets.FigureL; break;
                case 6: Figure = Presets.FigureJ; break;
                case 7: Figure = Presets.FigureT; break;
            }
            return this;
        }
    }
}