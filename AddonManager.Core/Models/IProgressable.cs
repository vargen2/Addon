namespace AddonManager.Core.Models
{
    public interface IProgressable
    {
        int Progress { get; set; }
        string Message { get; set; }
        bool ShowMessage { get; }
    }
}