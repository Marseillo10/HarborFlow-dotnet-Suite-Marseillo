
namespace HarborFlow.Core.Models;

public enum ApprovalAction
{
    Approve,
    Reject,
    Pending
}

public class ApprovalHistory
{
    public Guid ApprovalId { get; set; }
    public Guid RequestId { get; set; }
    public Guid ApprovedBy { get; set; }
    public ApprovalAction Action { get; set; }
    public DateTime ActionDate { get; set; }
    public string Reason { get; set; } = string.Empty;
}
