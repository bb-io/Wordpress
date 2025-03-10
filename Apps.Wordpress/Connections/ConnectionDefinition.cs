﻿using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;

namespace Apps.Wordpress.Connections;

public class ConnectionDefinition : IConnectionDefinition
{
    public IEnumerable<ConnectionPropertyGroup> ConnectionPropertyGroups => new List<ConnectionPropertyGroup>()
    {
        new()
        {
            Name = "Wordpress login",
            AuthenticationType = ConnectionAuthenticationType.Undefined,
            ConnectionProperties = new List<ConnectionProperty>()
            {
                new("url") { DisplayName = "URL" },
                new("login") { DisplayName = "Login" },
                new("applicationPassword") { DisplayName = "Application password", Sensitive = true }
            }
        }
    };

    public IEnumerable<AuthenticationCredentialsProvider> CreateAuthorizationCredentialsProviders(
        Dictionary<string, string> values)
    {
        var url = values.First(v => v.Key == "url");
        yield return new AuthenticationCredentialsProvider(
            url.Key,
            url.Value
        );

        var login = values.First(v => v.Key == "login");
        yield return new AuthenticationCredentialsProvider(
            login.Key,
            login.Value
        );

        var applicationPassword = values.First(v => v.Key == "applicationPassword");
        yield return new AuthenticationCredentialsProvider(
            applicationPassword.Key,
            applicationPassword.Value
        );
    }
}