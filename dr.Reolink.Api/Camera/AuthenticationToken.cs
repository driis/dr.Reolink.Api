namespace dr.Reolink.Api.Camera;

public record AuthenticationToken(DateTimeOffset Expires, string Value)
{
    public bool Expired => DateTimeOffset.Now > Expires;
}