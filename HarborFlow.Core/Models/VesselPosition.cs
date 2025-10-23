
namespace HarborFlow.Core.Models;

public enum PositionSource
{
    Ais,
    Gps,
    Manual
}

public class VesselPosition
{
    public Guid Id { get; set; }
    public string VesselImo { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public DateTime PositionTimestamp { get; set; }
    public decimal SpeedOverGround { get; set; }
    public decimal CourseOverGround { get; set; }
    public PositionSource Source { get; set; }
    public decimal Accuracy { get; set; }

    public Vessel Vessel { get; set; } = null!;
}
