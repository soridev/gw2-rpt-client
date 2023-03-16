using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RPTClient.Models;

namespace RPTClient.Rest;

/// <summary>
///     The class used to query the rest API.
///     Implemented as a thread safe singleton since all calls are async and we wanna use the same token and so on.
/// </summary>
public sealed class PerformanceTrackerRest
{
    private readonly HttpClient _client = new();
    private readonly UserToken _accessToken = new();
    private bool _isLogedIn;
    private string _username = string.Empty;

    public PerformanceTrackerRest()
    {
        _client.BaseAddress = new Uri("http://localhost:8443/elite-api/");
        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public void Login(string username, string password)
    {
        // fetch access token
        var task = Task.Run(async () => await ConfigureToken(username, password));
        task.Wait();
        _accessToken.AccessToken(task.Result);

        _username = username;
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", _accessToken.AccessToken());

        _isLogedIn = true;
    }

    private async Task<string> ConfigureToken(string username, string password)
    {
        var values = new Dictionary<string, string>
        {
            { "username", username },
            { "password", password }
        };

        var content = new FormUrlEncodedContent(values);

        var response = await _client.PostAsync("api-token-auth/", content);
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        dynamic data = JObject.Parse(responseString);


        return data.token;
    }

    public async Task<int> GetRemoteLogCount()
    {
        CheckSelf();

        var response = await _client.GetAsync("log-count/me/");
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        dynamic data = JObject.Parse(responseJson);

        return Convert.ToInt32(data.log_count);
    }

    private void CheckSelf()
    {
        if (_isLogedIn is false)
            throw new Exception("Rest api is not authenticated. Call method Login() before using any other calls.");
    }

    public async Task Upload(string pathToFile)
    {
        var filename = Path.GetFileName(pathToFile);

        await using var stream = File.OpenRead(pathToFile);
        using var request = new HttpRequestMessage(HttpMethod.Post, "upload/");
        using var content = new MultipartFormDataContent
        {
            { new StreamContent(stream), "file", filename }
        };

        request.Content = content;

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
}