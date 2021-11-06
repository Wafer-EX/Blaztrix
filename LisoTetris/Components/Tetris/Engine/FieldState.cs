using System;
using System.Collections.Generic;
using LisoTetris.Components.Tetris.Engine.Elements;

namespace LisoTetris.Components.Tetris.Engine
{
    public class FieldState
    {
        public event Action Lost;

        public event Action LineDeleted;

        public event Action Updated;

        public bool IsBlocked { get; set; }

        public bool[,] Field { get; private set; }

        public Queue<Block> Blocks { get; private set; }

        public Block CurrentBlock { get; private set; }

        public FieldState(int width, int height)
        {
            if (width < 8 || height < 8)
                throw new ArgumentOutOfRangeException();

            Field = new bool[width, height];
            Blocks = new Queue<Block>();
            GenerateNextBlock();
        }

        public void Update(Direction direction)
        {
            if (!IsBlocked)
            {
                MoveBlock(direction);
                if (CurrentBlock.IsSetted)
                {
                    CurrentBlock = Blocks.Dequeue();
                    GenerateNextBlock();
                }
                Updated?.Invoke();
            }
        }

        private void GenerateNextBlock()
        {
            var block = new Block(Field.GetLength(0)).Generate();
            Blocks.Enqueue(block);

            CurrentBlock ??= new Block(Field.GetLength(0)).Generate();
            if (!CanBePlaced(0, 0, block.Figure))
            {
                Lost?.Invoke();
                IsBlocked = true;
            }
        }

        private void MoveBlock(Direction direction)
        {
            int figureWidth = CurrentBlock.Figure.GetLength(0);
            int figureHeight = CurrentBlock.Figure.GetLength(1);
            int fieldWidth = Field.GetLength(0);
            int fieldHeight = Field.GetLength(1);
            Position position = CurrentBlock.Position;

            switch (direction)
            {
                case Direction.Left:
                    if (position.X != 0 && CanBePlaced(-1, 0, CurrentBlock.Figure))
                        position.X -= 1;
                    break;
                case Direction.Right:
                    if (position.X != fieldWidth - figureWidth && CanBePlaced(1, 0, CurrentBlock.Figure))
                        position.X += 1;
                    break;
                case Direction.Down:
                    if (position.Y != fieldHeight - figureHeight && CanBePlaced(0, 1, CurrentBlock.Figure))
                        position.Y += 1;
                    else PlaceBlock();
                    break;
                case Direction.Around:
                    RotateBlock();
                    break;
            }
        }

        private void RotateBlock()
        {
            int figureWidth = CurrentBlock.Figure.GetLength(0);
            int figureHeight = CurrentBlock.Figure.GetLength(1);
            int offsetX = (figureWidth - figureHeight) / 2;
            int offsetY = (figureHeight - figureWidth) / 2;

            bool[,] newFigure = new bool[figureHeight, figureWidth];
            for (int heightPoint = 0; heightPoint < figureHeight; heightPoint++)
            {
                for (int widthPoint = 0; widthPoint < figureWidth; widthPoint++)
                {
                    newFigure[heightPoint, widthPoint] = CurrentBlock.Figure[widthPoint, figureHeight - heightPoint - 1];
                }
            }

            if (CanBePlaced(offsetX, offsetY, newFigure))
            {
                CurrentBlock.Figure = newFigure;
                CurrentBlock.Position.X += offsetX;
                CurrentBlock.Position.Y += offsetY;
            }
        }

        private bool CanBePlaced(int offsetX, int offsetY, bool[,] figure)
        {
            int figureWidth = figure.GetLength(0);
            int figureHeight = figure.GetLength(1);
            Position position = CurrentBlock.Position;

            try
            {
                for (int heightPoint = 0; heightPoint < figureHeight; heightPoint++)
                {
                    for (int widthPoint = 0; widthPoint < figureWidth; widthPoint++)
                    {
                        if (figure[widthPoint, heightPoint] && Field[position.X + widthPoint + offsetX, position.Y + heightPoint + offsetY])
                        {
                            return false;
                        }
                    }
                }
            }
            catch { return false; }
            return true;
        }

        private void PlaceBlock()
        {
            Position position = CurrentBlock.Position;
            int figureWidth = CurrentBlock.Figure.GetLength(0);
            int figureHeight = CurrentBlock.Figure.GetLength(1);

            for (int heightPoint = 0; heightPoint < figureHeight; heightPoint++)
            {
                for (int widthPoint = 0; widthPoint < figureWidth; widthPoint++)
                {
                    if (CurrentBlock.Figure[widthPoint, heightPoint])
                    {
                        Field[widthPoint + position.X, heightPoint + position.Y] = true;
                    }
                }
            }
            CurrentBlock.IsSetted = true;

            for (int heightPoint = position.Y; heightPoint < position.Y + figureHeight; heightPoint++)
              if (LineIsFull(heightPoint)) DeleteLine(heightPoint);
        }

        private bool LineIsFull(int lineIndex)
        {
            int fieldWidth = Field.GetLength(0);
            int sum = 0;

            for (int widthPoint = 0; widthPoint < fieldWidth; widthPoint++)
                if (Field[widthPoint, lineIndex]) sum++;
            
            if (sum == fieldWidth) return true;
            else return false;
        }

        private void DeleteLine(int lineIndex)
        {
            int fieldWidth = Field.GetLength(0);

            for (int heightPoint = lineIndex; heightPoint >= 1; heightPoint--)
            {
                for (int widthPoint = 0; widthPoint < fieldWidth; widthPoint++)
                {
                    Field[widthPoint, heightPoint] = Field[widthPoint, heightPoint - 1];
                }
            }
            LineDeleted?.Invoke();
        }

        public static explicit operator PixelStates[,](FieldState fieldState)
        {
            int fieldWidth = fieldState.Field.GetLength(0);
            int fieldHeight = fieldState.Field.GetLength(1);
            var pixelStates = new PixelStates[fieldWidth, fieldHeight];

            for (int heightPoint = 0; heightPoint < fieldHeight; heightPoint++)
            {
                for (int widthPoint = 0; widthPoint < fieldWidth; widthPoint++)
                {
                    if (fieldState.Field[widthPoint, heightPoint])
                        pixelStates[widthPoint, heightPoint] = PixelStates.Filled;
                }
            }

            Position blockPosition = fieldState.CurrentBlock.Position;
            int figureWidth = fieldState.CurrentBlock.Figure.GetLength(0);
            int figureHeight = fieldState.CurrentBlock.Figure.GetLength(1);

            for (int heightPoint = 0; heightPoint < figureHeight; heightPoint++)
            {
                for (int widthPoint = 0; widthPoint < figureWidth; widthPoint++)
                {
                    if (fieldState.CurrentBlock.Figure[widthPoint, heightPoint])
                    {
                        pixelStates[blockPosition.X + widthPoint, blockPosition.Y + heightPoint] = PixelStates.CurrentBlock;
                    }
                }
            }

            return pixelStates;
        }
    }
}