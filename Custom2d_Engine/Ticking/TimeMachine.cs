using System;
using static System.Math;

namespace Custom2d_Engine.Ticking {
    public class TimeMachine {
        private TimeSpan _time = TimeSpan.Zero;

        /// <summary>
        /// Adds time into the TimeMachine
        /// </summary>
        /// <param name="time">Amount of time to be added</param>
        public void Accumulate(TimeSpan time) {
            this._time += time;
        }

        /// <summary>
        /// Retrieves given amount of time from the TimeMachine
        /// </summary>
        /// <param name="maxTime"></param>
        /// <returns>Amount of time retrievend</returns>
        public TimeSpan Retrieve(TimeSpan maxTime) {
            AssertPositive(_time);
            TimeSpan timeLeft = _time - maxTime;
            _time = timeLeft > TimeSpan.Zero ? timeLeft : TimeSpan.Zero;
            return maxTime + (timeLeft < TimeSpan.Zero ? timeLeft : TimeSpan.Zero);
        }

        /// <summary>
        /// Attempts to retrieves given amount of time from the TimeMachine <br/>
        /// If there is enough <paramref name="time"/> accumulated in this machine subtructs that amount and returns true, otherwise returns false
        /// </summary>
        /// <param name="time"></param>
        public bool TryRetrieve(TimeSpan time) {
            AssertPositive(time);
            if (this._time >= time) {
                this._time -= time;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Result is equivalent to calling <see cref="TryRetrieve(TimeSpan)"/> as many times as possible, but is faster for larger <paramref name="limit"/> values
        /// </summary>
        /// <param name="interval">Single unit of warp time, must be positive</param>
        /// <param name="limit">Maximum amount of warps, must be positive</param>
        /// <returns>Amount of warps</returns>
        public int RetrieveAll(TimeSpan interval, int limit = int.MaxValue) {
            AssertPositive(interval);
            int result = (int)Floor(_time / interval);
            result = Clamp(result, 0, limit);
            _time -= result * interval;
            return result;
        }

        public void AssertPositive(TimeSpan time) {
            if (time < TimeSpan.Zero) {
                throw new ApplicationException();
            }
        }
    }
}