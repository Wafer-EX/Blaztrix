using Blaztrix.Core.Enums;

namespace Blaztrix.Core
{
    public class Session
    {
        private CancellationTokenSource _cancelAutoMoveSource = new();
        private int _score;

        public event Action? FieldUpdated;
        public event Action? AutoMoved;
        public event Action? ScoreChanged;
        public event Action? Lost;

        public int Score
        {
            get => _score;
            private set
            {
                _score = value;
                if (_score % 5 == 0) Speed++;
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
                if (_cancelAutoMoveSource.IsCancellationRequested)
                {
                    _cancelAutoMoveSource.Dispose();
                    _cancelAutoMoveSource = new CancellationTokenSource();
                }

                Task.Run(async delegate
                {
                    while (!FieldState.IsBlocked)
                    {
                        FieldState.Update(Directions.Down);
                        AutoMoved?.Invoke();
                        await Task.Delay(1000 / Speed, _cancelAutoMoveSource.Token);
                    }
                });
            }
        }

        public void Stop()
        {
            FieldState.IsBlocked = true;
            _cancelAutoMoveSource.Cancel();
        }

        public void Control(Directions direction) => FieldState.Update(direction);
    }
}