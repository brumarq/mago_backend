namespace Domain.Tests;

public class MockHttpMessageHandler : HttpMessageHandler
{
    private HttpResponseMessage _fakeResponse;

    public void SetFakeResponse(HttpResponseMessage response)
    {
        _fakeResponse = response;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        return await Task.FromResult(_fakeResponse);
    }
}