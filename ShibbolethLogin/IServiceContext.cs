namespace ShibbolethLogin
{
    public interface IServiceContext
    {
        string UserId { get; }
        string UserIdWithDomain { get; }
        string IpAddress { get; }
    }
}
