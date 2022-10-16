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
    private static readonly Lazy<PerformanceTrackerRest> lazy = new Lazy<PerformanceTrackerRest>(() => new PerformanceTrackerRest());
    private UserToken _accessToken = new UserToken();
    private readonly HttpClient client = new HttpClient();

    public static PerformanceTrackerRest Instance { get { return lazy.Value; } }

    private PerformanceTrackerRest()
    {
        Setup();
    }

    public async void Setup()
    {
        client.BaseAddress = new Uri("http://localhost:8443/elite-api/");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _accessToken.AccessToken(await GetToken("daniel", "elite-insider"));
    }

    private async Task<string> GetToken(string username, string password)
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
}