using System;
using System.Threading.Tasks;

namespace LisoTetris.Components.Tetris.Engine
{
    public class Session
    {
        private int score;

        public event Action FieldUpdated;

        public event Action ScoreChanged;

        public event Action Lost;

        public bool IsLost { get; private set; }

        public FieldState FieldState { get; private set; }

        public int Speed { get; private set; } = 1;

        public int Score
        {
            get => score;
            private set
            {
                score = value;
                if (score % 5 == 0) Speed++;
            }
        }

        public Session(int fieldWidth, int fieldHeight, int speed)
        {
            Speed = speed;
            FieldState = new FieldState(fieldWidth, fieldHeight);

            FieldState.Updated += () => FieldUpdated?.Invoke();

            FieldState.LineDeleted += delegate
            {
                Score++;
                ScoreChanged?.Invoke();
            };

            FieldState.Lost += delegate
            {
                IsLost = true;
                Lost?.Invoke();
            };
        }

        public async Task StartAsync()
        {
            if (!IsLost)
            {
                FieldState.IsBlocked = false;
                await Task.Run(async delegate
                {
                    while (!FieldState.IsBlocked)
                    {
                        FieldState.Update(Direction.Down);
                        await Task.Delay(1000 / Speed);
                    }
                });
            }
        }

        public void Stop() => FieldState.IsBlocked = true;

        public void Control(Direction direction) => FieldState.Update(direction);
    }
}