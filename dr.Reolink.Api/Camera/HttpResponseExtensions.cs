using dr.Reolink.Api.Camera.Model;

namespace dr.Reolink.Api.Camera;

public static class HttpResponseExtensions
{
    public static async Task<T> AsReoResponse<T>(this Task<HttpResponseMessage> message)
    {
        return await AsReoResponse<T>(await message);
    }
    public static async Task<T> AsReoResponse<T>(this HttpResponseMessage message)
    {
        message.EnsureSuccessStatusCode();
        var response = await message.Content.ReadFromJsonAsync<ReoResponse<T>[]>();
        var first = response?.FirstOrDefault();
        if (first == null)
        {
            throw new ApplicationException("Reolink API did not provide a valid response (no elements in response)");
        }
        if (first.Code != 0)
        {
            throw new ApplicationException($"Non-success status code on response: {first}");
        }

        return first.Value;
    }
}