using HarborFlowSuite.Core.DTOs;
using Microsoft.AspNetCore.Components;
using System.Drawing;

namespace HarborFlowSuite.Client.Components
{
    public partial class VesselTooltip : ComponentBase
    {
        [Parameter] public bool IsVisible { get; set; }
        [Parameter] public VesselPositionDto? Vessel { get; set; }
        [Parameter] public Point Position { get; set; } = Point.Empty;
    }
}