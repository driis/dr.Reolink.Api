namespace dr.Reolink.Api.Camera;

public sealed class AuthenticationHelper
{
    private readonly Func<CancellationToken, Task<AuthenticationToken>> _authenticator;
    private static readonly AutoResetEvent AuthLock = new(true);
    
    private static AuthenticationToken? _token;
    
    public AuthenticationHelper(Func<CancellationToken, Task<AuthenticationToken>> authenticator)
    {
        _authenticator = authenticator;
    }

    public async Task<AuthenticationToken> GetToken(CancellationToken ct)
    {
        if (_token is {Expired: false})
        {
            return _token!;
        }

        try
        {
            AuthLock.WaitOne();
            if (_token is {Expired: false})    
            {                                  
                return _token!;                
            }
            return _token = await _authenticator(ct);
        }
        finally
        {
            AuthLock.Reset();
        }
    }
}