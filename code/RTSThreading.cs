using ICities;

namespace RealTimeStatistics
{
    /// <summary>
    /// handle threading
    /// </summary>
    public class RTSThreading : ThreadingExtensionBase
    {
        private bool _gameDateInitialized;

        /// <summary>
        /// called once when a game is loaded
        /// </summary>
        public override void OnCreated(IThreading threading)
        {
            // do base processing
            base.OnCreated(threading);

            // not initialized
            _gameDateInitialized = false;
        }

        /// <summary>
        /// called after every simulation tick, even when simulation is paused
        /// </summary>
        public override void OnAfterSimulationTick()
        {
            // do base processing
            base.OnAfterSimulationTick();

            // The following analysis was performed to determine the likelihood that a snapshot could be missed on day one of a game month

            // the table below shows approximate ticks per game day on a minimal city (no roads, no buildings, no population)
            // ticks per game day do not change with bigger more populated cities (pop 500000+)

            // sim speed x1   = 585 ticks/day:    base game, More Simulation Speed Options, V10Speed
            // sim speed x2   = 293 ticks/day:    base game, More Simulation Speed Options, V10Speed
            // sim speed x4   = 145 ticks/day:    base game, More Simulation Speed Options, V10Speed
            // sim speed x6   =  98 ticks/day:               More Simulation Speed Options
            // sim speed x8   =  72 ticks/day:                                              V10Speed
            // sim speed x9   =  65 ticks/day:               More Simulation Speed Options
            // sim speed x16  =  37 ticks/day:                                              V10Speed
            // sim speed x32  =  18 ticks/day:                                              V10Speed
            // sim speed x64  =   9 ticks/day:                                              V10Speed
            // sim speed x128 =   5 ticks/day:                                              V10Speed
            // sim speed x256 = 2-3 ticks/day:                                              V10Speed
            // sim speed x512 = 1-2 ticks/day:                                              V10Speed

            // Speed Slider V2 mod does not cause ticks per game day to change
            // even when Speed Slider V2 is used with the other speed mods

            // Game Speed mod and Real Time mod can only make the game run slower (i.e. more ticks per game day)
            // so there is no concern with those two mods on missing day one snapshot

            // on my PC, going past x16 on V10Speed did not make the minimal city run any faster
            // perhaps a faster PC could make use of the higher speeds on V10Speed

            // in the worst case (V10speed at x512), there is still at least one tick per game day
            // so that the SimulationTick routine will be called on day one of a game month to take a snapshot

            // when game date is initialized, process snapshots
            if (_gameDateInitialized && ModUtil.IsWorkshopModEnabled(ModUtil.ModIDRealTime))
            {
                Snapshots.instance.SimulationTick();
            }
        }

        /// <summary>
        /// the game date is not initialized until the first call to OnUpdate
        /// </summary>
        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            // do base processing
            base.OnUpdate(realTimeDelta, simulationTimeDelta);

            // game date is initialized
            _gameDateInitialized = true;
        }
    }
}
