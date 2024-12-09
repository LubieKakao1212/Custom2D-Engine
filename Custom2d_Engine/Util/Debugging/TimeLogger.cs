using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Custom2d_Engine.Util.Debugging {
    using DebugLog = System.Diagnostics.Debug;

    public class TimeLogger {
        public static readonly TimeLogger Instance = new();

        public bool Enabled { get; set; } = true;

        public double UnitScale { get; set; } = 1f;

        private readonly Stack<Scope> _scopes = new();

        public void Push(string name) {
            if (!Enabled) {
                return;
            }

            var lastName = _scopes.TryPeek(out var s) ? s.name : "";
            _scopes.Push(new Scope { stopwatch = new Stopwatch(), id = name, name = lastName + "/" + name });
            _scopes.Peek().stopwatch.Start();
        }

        public void Pop(string name) {
            if (!Enabled) {
                return;
            }

            var scope = _scopes.Pop();
            if (scope.id != name) {
                throw new ArgumentException($"Unbalanced profiling scopes, expected: {scope.name}, got: {name}");
            }

            var time = scope.stopwatch.Elapsed;
            DebugLog.WriteLine(scope.name + ": " + $"{time} >> {time.TotalSeconds / UnitScale}");
            scope.stopwatch.Stop();
        }

        private struct Scope {
            public Stopwatch stopwatch;
            public string name;
            public string id;
        }
    }
}