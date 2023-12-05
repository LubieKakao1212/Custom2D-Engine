using Custom2d_Engine.Ticking.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom2d_Engine.Ticking
{
    public static class TickerExtensions
    {
        public static TickMachineBase AddSimpleRepeatingAction<T>(this T owner, Action action, float repeateInterval, float phase = 0f) where T : IManagedTicker
        {
            return owner.TickManager.AddSimpleRepeatingAction(owner, action, repeateInterval, phase);
        }

        public static TickMachineBase AddSimpleRepeatingAction<T>(this T owner, Action action, TimeSpan repeateInterval, TimeSpan phase = default) where T : IManagedTicker
        {
            return owner.TickManager.AddSimpleRepeatingAction(owner, action, repeateInterval, phase);
        }

        public static TickMachineBase AddAccurateRepeatingAction<T>(this T owner, Action action, float repeateInterval, float phase = 0f) where T : IManagedTicker
        {
            return owner.TickManager.AddAccurateRepeatingAction(owner, action, repeateInterval, phase);
        }

        public static TickMachineBase AddAccurateRepeatingAction<T>(this T owner, Action action, TimeSpan repeateInterval, TimeSpan phase = default) where T : IManagedTicker
        {
            return owner.TickManager.AddAccurateRepeatingAction(owner, action, repeateInterval, phase);
        }

        public static TickMachineBase AddActionSequence<T>(this T owner, IEnumerator<TimeSpan> sequence, TimeSpan phase = default) where T : IManagedTicker
        {
            return owner.TickManager.AddActionSequence(owner, sequence, phase);
        }

        public static TickMachineBase AddRepeetingActionSequence<T>(this T owner, IEnumerable<TimeSpan> sequence, TimeSpan phase = default) where T : IManagedTicker
        {
            return owner.TickManager.AddRepeetingActionSequence(owner, sequence, phase);
        }

    }
}
