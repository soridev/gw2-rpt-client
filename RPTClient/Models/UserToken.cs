using System;

namespace RPTClient.Models;

public class UserToken
{
    private string _accessToken;
    private readonly bool _firstUse = true;

    private readonly TimeSpan _useableTimespan = new(24, 0, 0);
    public DateTime CreationTimestamp { get; set; }

    public string AccessToken()
    {
        if (_firstUse is false && DateTime.Now < CreationTimestamp + _useableTimespan)
            throw new Exception(string.Format(
                "The currently used user token expired after {0}. Retry your action and a new token will be used.",
                _useableTimespan.ToString()));
        return _accessToken;
    }

    public void AccessToken(string accessToken)
    {
        _accessToken = accessToken;
    }
}