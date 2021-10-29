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

        public bool IsBlocked { get; set; } = false;

        public bool[,] Field { get; set; }

        public Queue<Block> Blocks { get; set; }

        public Block CurrentBlock { get; set; }

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
            switch (direction)
            {
                case Direction.Left:
                    if (CurrentBlock.Position.X != 0 && CanBePlaced(-1, 0, CurrentBlock.Figure))
                        CurrentBlock.Position.X -= 1;
                    break;
                case Direction.Right:
                    if (CurrentBlock.Position.X != Field.GetLength(0) - CurrentBlock.Figure.GetLength(0) && CanBePlaced(1, 0, CurrentBlock.Figure))
                        CurrentBlock.Position.X += 1;
                    break;
                case Direction.Down:
                    if (CurrentBlock.Position.Y != Field.GetLength(1) - CurrentBlock.Figure.GetLength(1) && CanBePlaced(0, 1, CurrentBlock.Figure))
                        CurrentBlock.Position.Y += 1;
                    else PlaceBlock();
                    break;
                case Direction.Around:
                    RotateBlock();
                    break;
            }
        }

        private void RotateBlock()
        {
            int width = CurrentBlock.Figure.GetLength(0);
            int height = CurrentBlock.Figure.GetLength(1);

            int offsetX = (width - height) / 2;
            int offsetY = (height - width) / 2;

            bool[,] newFigure = new bool[height, width];

            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    newFigure[j, i] = CurrentBlock.Figure[i, height - j - 1];
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
            try
            {
                for (int j = 0; j < figure.GetLength(1); j++)
                {
                    for (int i = 0; i < figure.GetLength(0); i++)
                    {
                        if (figure[i, j] && Field[CurrentBlock.Position.X + i + offsetX, CurrentBlock.Position.Y + j + offsetY])
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
            for (int j = 0; j < CurrentBlock.Figure.GetLength(1); j++)
            {
                for (int i = 0; i < CurrentBlock.Figure.GetLength(0); i++)
                {
                    if (CurrentBlock.Figure[i, j])
                    {
                        Field[i + CurrentBlock.Position.X, j + CurrentBlock.Position.Y] = true;
                    }
                }
            }
            CurrentBlock.IsSetted = true;

            for (int j = CurrentBlock.Position.Y; j < CurrentBlock.Position.Y + CurrentBlock.Figure.GetLength(1); j++)
              if (LineIsFull(j)) DeleteLine(j);
        }

        private bool LineIsFull(int lineIndex)
        {
            int width = Field.GetLength(0);
            int sum = 0;

            for (int i = 0; i < width; i++)
                if (Field[i, lineIndex]) sum++;
            
            if (sum == width) return true;
            else return false;
        }

        private void DeleteLine(int lineIndex)
        {
            for (int j = lineIndex; j >= 1; j--)
            {
                for (int i = 0; i < Field.GetLength(0); i++)
                {
                    Field[i, j] = Field[i, j - 1];
                }
            }
            LineDeleted?.Invoke();
        }

        public static explicit operator PixelStates[,](FieldState param)
        {
            int fieldWidth = param.Field.GetLength(0);
            int fieldHeight = param.Field.GetLength(1);
            var newState = new PixelStates[fieldWidth, fieldHeight];

            for (int j = 0; j < fieldHeight; j++)
            {
                for (int i = 0; i < fieldWidth; i++)
                {
                    if (param.Field[i, j])
                        newState[i, j] = PixelStates.Filled;
                }
            }

            Position blockPosition = param.CurrentBlock.Position;
            int blockWidth = param.CurrentBlock.Figure.GetLength(0);
            int blockHeight = param.CurrentBlock.Figure.GetLength(1);

            for (int j = 0; j < blockHeight; j++)
            {
                for (int i = 0; i < blockWidth; i++)
                {
                    if (param.CurrentBlock.Figure[i, j])
                    {
                        newState[i + blockPosition.X, j + blockPosition.Y] = PixelStates.CurrentBlock;
                    }
                }
            }

            return newState;
        }
    }
}