namespace Custom2d_Engine.Ticking {
    public interface IManagedTicker {
        /// <summary>
        /// <see cref="Ticking.TickManager"/> handling this <see cref="IManagedTicker" />
        /// </summary>
        public TickManager TickManager { get; }
    }
}