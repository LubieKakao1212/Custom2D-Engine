using Custom2d_Engine.Ticking.Actions;
using Custom2d_Engine.Util;
using System;
using System.Collections.Generic;

namespace Custom2d_Engine.Ticking
{
    //Consider adding update order
    public class TickManager
    {
        private readonly Dictionary<object, List<ITickMachine>> _actions = new();
        private readonly List<(object Owner, ITickMachine Ticker)> _additionList = new();
        private bool _isUpdating;

        public TickManager(bool global = false) 
        {
            _isUpdating = false;
        }

        public void Forward(TimeSpan deltaTime)
        {
            _isUpdating = true;
            Dictionary<object, List<ITickMachine>> toRemove = new();
            foreach (var entry in _actions.EnumerateNestedEntries())
            {
                var ticker = entry.Value;
                if (ticker.Disposed)
                {
                    toRemove.AddNested(entry.Key, ticker);
                }
                ticker.Forward(deltaTime);
            }
            _isUpdating = false;

            //May be optimised (if needed) by caching lists from each object
            foreach (var entry in toRemove.EnumerateNestedEntries())
            {
                _actions.RemoveNested(entry.Key, entry.Value);
            }

            foreach (var entry in _additionList)
            {
                _actions.AddNested(entry.Owner, entry.Ticker);
            }
        }

        public TickMachineBase AddSimpleRepeatingAction(object owner, Action action, float repeateInterval, float phase = 0f)
        {
            return AddSimpleRepeatingAction(owner, action, TimeSpan.FromSeconds(repeateInterval), TimeSpan.FromSeconds(phase));
        }

        public TickMachineBase AddSimpleRepeatingAction(object owner, Action action, TimeSpan repeateInterval, TimeSpan phase = default)
        {
            return AddTicker(owner, new SimpleRepeatingTickMachine(action, repeateInterval, phase));
        }

        public TickMachineBase AddAccurateRepeatingAction(object owner, Action action, float repeateInterval, float phase = 0f)
        {
            return AddAccurateRepeatingAction(owner, action, TimeSpan.FromSeconds(repeateInterval), TimeSpan.FromSeconds(phase));
        }
        
        public TickMachineBase AddAccurateRepeatingAction(object owner, Action action, TimeSpan repeateInterval, TimeSpan phase = default)
        {
            return AddTicker(owner, new AccurateRepeatingTickMachine(action, repeateInterval, phase));
        }
        
        public TickMachineBase AddActionSequence(object owner, IEnumerator<TimeSpan> sequence, TimeSpan phase = default)
        {
            return AddTicker(owner, new SequenceTickMachine(sequence, TimeSpan.Zero, phase));
        }

        public TickMachineBase AddRepeatingActionSequence(object owner, IEnumerable<TimeSpan> sequence, TimeSpan phase = default)
        {
            throw new NotImplementedException("This is on TODO list");
        }

        public void RemoveTicker(object owner, ITickMachine ticker)
        {
            _actions.GetValueOrDefault(owner, null)?.Remove(ticker);
        }

        public List<ITickMachine> RemoveAllTickers(object owner)
        {
            _actions.Remove(owner, out var list);
            return list ?? new List<ITickMachine>();
        }

        public TickMachineBase AddTicker(object owner, TickMachineBase ticker)
        {
            if (_isUpdating)
            {
                _additionList.Add((owner, ticker));
            }
            else
            {
                _actions.AddNested(owner, ticker);
            }
            return ticker;
        }
    }
}
