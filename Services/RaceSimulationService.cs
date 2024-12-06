namespace SprintSimulationBackend.Services
{
    using Microsoft.AspNetCore.SignalR;
    using SprintSimulationBackend.Hubs;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class RaceSimulationService
    {
        private const double TimeStep = 0.05; // 50ms
        private const double TrackLength = 100.0; // 100 meters
        private readonly List<Runner> _runners;
        private readonly IHubContext<RaceHub> _hubContext;

        public RaceSimulationService(List<Runner> runners, IHubContext<RaceHub> hubContext)
        {
            _runners = runners;
            _hubContext = hubContext;
        }
        public async Task StartRace(CancellationToken cancellationToken)
        {
            bool raceComplete = false;
            double timeElapsed = 0;

            while (!raceComplete && !cancellationToken.IsCancellationRequested)
            {
                raceComplete = true;

                foreach (var runner in _runners)
                {
                    if (timeElapsed >= runner.ReactionTime)
                    {
                        runner.Speed += runner.Acceleration * TimeStep;
                        runner.Position += runner.Speed * TimeStep;
                    }

                    if (runner.Position < TrackLength)
                    {

                        raceComplete = false;

                    }
                }

                await _hubContext.Clients.All.SendAsync("ReceiveRaceUpdate", _runners);

                await Task.Delay((int)(TimeStep * 1000), cancellationToken);
                timeElapsed += TimeStep;
            }
        }
    }

}
