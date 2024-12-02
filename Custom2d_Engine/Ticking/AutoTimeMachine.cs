using System;

namespace Custom2d_Engine.Ticking
{ 
    public class AutoTimeMachine
    {
        public TimeSpan Interval { get; set; }
        public Action Action { get; set; }
        
        private readonly TimeMachine machine;
        
        public AutoTimeMachine(Action action, TimeSpan interval)
        {
            Action = action;
            Interval = interval;
            machine = new TimeMachine();
        }

        /// <summary>
        /// Forwards the time by given amount, triggers assigned action relevant amount of times
        /// </summary>
        /// <param name="time">Amount of time to forward by</param>
        public void Forward(TimeSpan time)
        {
            machine.Accumulate(time);
            int rolls = machine.RetrieveAll(Interval);
            for (int i = 0; i < rolls; i++)
            { 
                Action();
            }
        }
    }
}
