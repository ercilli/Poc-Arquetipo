namespace BgbaArquetipoCanales
{
    /// <summary>
    /// Types of canal operations for categorizing log entries
    /// </summary>
    public enum CanalOperationType
    {
        Authentication,
        Authorization,
        Transaction,
        UserProfile,
        ChannelSwitch,
        SessionManagement,
        ErrorHandling,
        SecurityAudit
    }
}
