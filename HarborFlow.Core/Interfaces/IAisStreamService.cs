using System;
using HarborFlow.Core.Models;

namespace HarborFlow.Core.Interfaces
{
    public interface IAisStreamService
    {
        event Action<VesselPosition> PositionReceived;
        void Start();
        void Stop();
    }
}