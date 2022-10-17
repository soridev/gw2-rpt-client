using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RPTClient.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime;
using System.Security;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RPTClient.Rest;

/// <summary>
/// The class used to query the rest API.
/// Implemented as a thread safe singleton since all calls are async and we wanna use the same token and so on.
/// </summary>
public sealed class PerformanceTrackerRest
{
    private bool _isLogedIn = false;
    private UserToken _accessToken = new UserToken();
    private string _username = string.Empty;
    private readonly HttpClient client = new HttpClient();

    public PerformanceTrackerRest()
    {
        client.BaseAddress = new Uri("http://localhost:8443/elite-api/");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public void Login(string username, string password)
    {
        // fetch access token
        var task = Task.Run(async () => await ConfigureToken(username, password));
        task.Wait();
        _accessToken.AccessToken(task.Result);

        _username = username;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", _accessToken.AccessToken());

        _isLogedIn = true;
    }

    private async Task<string> ConfigureToken(string username, string password)
    {
        var values = new Dictionary<string, string>()
        {
            {"username", username},
            {"password", password},
        }; 

        var content = new FormUrlEncodedContent(values);

        HttpResponseMessage response = await client.PostAsync("api-token-auth/", content);
        response.EnsureSuccessStatusCode();

        string responseString = await response.Content.ReadAsStringAsync();
        dynamic data = JObject.Parse(responseString);

        
        return data.token;
    }

    public async Task<int> GetRemoteLogCount()
    {
        CheckSelf();

        HttpResponseMessage response = await client.GetAsync("log-count/me/");
        response.EnsureSuccessStatusCode();

        string responseJson = await response.Content.ReadAsStringAsync();
        dynamic data = JObject.Parse(responseJson);

        return Convert.ToInt32(data.log_count);
    }

    private void CheckSelf()
    {
        if (_isLogedIn is false)
        {
            throw new Exception("Rest api is not authenticated. Call method Login() before using any other calls.");
        }
    }
}