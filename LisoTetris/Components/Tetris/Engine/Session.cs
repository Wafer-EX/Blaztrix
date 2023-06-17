using System;
using System.Threading;
using System.Threading.Tasks;

namespace LisoTetris.Components.Tetris.Engine
{
    public class Session
    {
        private CancellationTokenSource cancelAutoMoveSource = new();

        private int score;

        public event Action FieldUpdated;

        public event Action AutoMoved;

        public event Action ScoreChanged;

        public event Action Lost;

        public int Score
        {
            get => score;
            private set
            {
                score = value;
                if (score % 5 == 0) Speed++;
            }
        }

        public int Speed { get; private set; }

        public FieldState FieldState { get; private set; }

        public bool IsLost { get; private set; }

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

        public void StartInBackground()
        {
            if (!IsLost)
            {
                FieldState.IsBlocked = false;
                if (cancelAutoMoveSource.IsCancellationRequested)
                {
                    cancelAutoMoveSource.Dispose();
                    cancelAutoMoveSource = new CancellationTokenSource();
                }

                Task.Run(async delegate
                {
                    while (!FieldState.IsBlocked)
                    {
                        FieldState.Update(Direction.Down);
                        AutoMoved?.Invoke();
                        await Task.Delay(1000 / Speed, cancelAutoMoveSource.Token);
                    }
                });
            }
        }

        public void Stop()
        {
            FieldState.IsBlocked = true;
            cancelAutoMoveSource.Cancel();
        }

        public void Control(Direction direction) => FieldState.Update(direction);
    }
}