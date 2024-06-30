using System.Drawing;

namespace Blaztrix.Core.Elements
{
    public class Block
    {
        public bool[,] Figure { get; set; }

        public Point Position { get; set; }

        public bool IsSetted { get; set; }

        public Block(int fieldWidth) => Position = new Point(fieldWidth / 2 - 2, 0);

        public Block Generate()
        {
            Figure = Random.Shared.Next(1, 8) switch
            {
                1 => Presets.FigureO,
                2 => Presets.FigureI,
                3 => Presets.FigureS,
                4 => Presets.FigureZ,
                5 => Presets.FigureL,
                6 => Presets.FigureJ,
                7 => Presets.FigureT,
                _ => throw new NotImplementedException(),
            };
            return this;
        }
    }
}