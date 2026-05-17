
namespace CSharpSnakeProject.Logic.Utils
{
    public class MovementTimer
    {
        private float _timeToMove; // оставшееся время до следующего шага
        private float _lastSpeed; // предыдущая скорость для корректировки

        public MovementTimer(float initialDelay = 0f)
        {
            _timeToMove = initialDelay;
            _lastSpeed = 0f;
        }

        public bool Update(float deltaTime, float currentSpeed)
        {
            // Корректируем оставшееся время при изменении скорости (пропорционально)
            if (_lastSpeed != 0 && _lastSpeed != currentSpeed)
            {
                float ratio = _lastSpeed / currentSpeed;
                _timeToMove *= ratio;
                if (_timeToMove < 0) _timeToMove = 0;
            }
            _lastSpeed = currentSpeed;

            _timeToMove -= deltaTime;
            if (_timeToMove > 0f) return false;

            // Сброс таймера на следующий интервал
            _timeToMove = 1f / currentSpeed;
            return true;
        }

        public void Reset()
        {
            _timeToMove = 0f;
            _lastSpeed = 0f;
        }
    }
}
