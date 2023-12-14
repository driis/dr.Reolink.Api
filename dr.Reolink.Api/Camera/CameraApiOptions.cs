namespace dr.Reolink.Api.Camera;

public class CameraApiOptions
{
    public string? UserName { get; set; }
    public string? Password { get; set; }

    internal CameraApiOptionsValidated Validate()
    {
        if (UserName != null && Password != null)
        {
            return new CameraApiOptionsValidated(UserName!, Password!);
        }

        throw new ApplicationException($"Invalid configuration of CameraApiOptions. {new {UserName, Password}}");
    }
}
record CameraApiOptionsValidated(string UserName, string Password);
