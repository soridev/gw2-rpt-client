using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPTClient.Models;

public class UserToken
{
    private bool _firstUse = true;

    private TimeSpan _useableTimespan = new TimeSpan(hours: 24, minutes:0, seconds:0);
    public DateTime CreationTimestamp { get; set; }

    private string _accessToken;
    public string AccessToken()
    {
        if ((_firstUse is false) && (DateTime.Now < (CreationTimestamp + _useableTimespan)))
        {
            throw new Exception(String.Format("The currently used user token expired after {0}. Retry your action and a new token will be used.",
                _useableTimespan.ToString()));
        }
        else
        {
            return _accessToken;
        }
    }
    public void AccessToken(string accessToken)
    {
        _accessToken = accessToken;
    }
}