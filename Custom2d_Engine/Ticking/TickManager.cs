﻿using Custom2d_Engine.Ticking.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom2d_Engine.Ticking
{
    public class TickManager
    {
        public Dictionary<object, List<ITickMachine>> actions;

        public TickManager() { }
    }
}
