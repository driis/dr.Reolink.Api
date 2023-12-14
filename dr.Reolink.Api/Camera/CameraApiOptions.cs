namespace dr.Reolink.Api.Camera;

public class CameraApiOptions
{
    public string? UserName { get; set; }
    public string? Password { get; set; }
    
    public string? Endpoint { get; set; }

    internal CameraApiOptionsValidated Validate()
    {
        if (UserName != null && Password != null && Endpoint != null)
        {
            return new CameraApiOptionsValidated(UserName!, Password!, Endpoint!);
        }

        throw new ApplicationException($"Invalid configuration of CameraApiOptions. {new {UserName, Password, Endpoint}}");
    }
}
record CameraApiOptionsValidated(string UserName, string Password, string Endpoint);
