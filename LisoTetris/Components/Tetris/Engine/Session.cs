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

        public bool IsLost { get; private set; } = false;

        public FieldState State { get; set; }

        public int Speed { get; set; } = 1;

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
            State = new FieldState(fieldWidth, fieldHeight);

            State.Updated += () => FieldUpdated?.Invoke();

            State.LineDeleted += delegate
            {
                Score++;
                ScoreChanged?.Invoke();
            };

            State.Lost += delegate
            {
                IsLost = true;
                Lost?.Invoke();
            };
        }

        public async Task StartAsync()
        {
            if (!IsLost)
            {
                State.IsBlocked = false;
                await Task.Run(async delegate
                {
                    while (!State.IsBlocked)
                    {
                        State.Update(Direction.Down);
                        await Task.Delay(1000 / Speed);
                    }
                });
            }
        }

        public void Stop() => State.IsBlocked = true;

        public void Control(Direction direction) => State.Update(direction);
    }
}