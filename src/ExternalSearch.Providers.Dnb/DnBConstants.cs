using System;
using System.Collections.Generic;
using CluedIn.Core.Data.Relational;
using CluedIn.Core.Providers;

namespace CluedIn.ExternalSearch.Providers.DnB
{
    public static class DnBConstants
    {
        public const string ComponentName = "DnB";
        public const string ProviderName = "DnB";

        public struct KeyName
        {
            public const string ApiToken = "apiToken";
            public const string AcceptedEntityType = "acceptedEntityType";
            public const string OrgNameKey = "orgNameKey";
            public const string OrgAddressKey = "orgAddressKey";
            public const string OrgCountryCodeKey = "orgCountryCodeKey";
            public const string AuthUrl = "DNB_AUTH_URL";
            public const string AuthKey = "DNB_AUTH_KEY";
            public const string AuthSecret = "DNB_AUTH_SECRET";
            public const string AuthRequestBody = "DNB_AUTH_REQUEST_BODY";
            public const string DnBBaseUrl = "DNB_BASE_URL";
        }

        public static AuthMethods AuthMethods { get; set; } = new AuthMethods
        {
            token = new List<Control>()
            {
                new Control()
                {
                    displayName = "Auth Url",
                    type = "input",
                    isRequired = true,
                    name = KeyName.AuthUrl //https://plus.dnb.com/v2/token
                },
                new Control()
                {
                    displayName = "API Key",
                    type = "password",
                    isRequired = true,
                    name = KeyName.AuthKey
                },
                new Control()
                {
                    displayName = "API Secret",
                    type = "password",
                    isRequired = true,
                    name = KeyName.AuthSecret
                },
                new Control()
                {
                    displayName = "Auth Request Body",
                    type = "input",
                    isRequired = true,
                    name = KeyName.AuthRequestBody //{"grant_type" : "client_credentials"}
                },
                new Control()
                {
                    displayName = "DnB Base Url",
                    type = "input",
                    isRequired = true,
                    name = KeyName.DnBBaseUrl
                },
                new Control()
                {
                    displayName = "Accepted Entity Type",
                    type = "input",
                    isRequired = false,
                    name = KeyName.AcceptedEntityType
                },
                new Control()
                {
                    displayName = "Organization Name vocab key",
                    type = "input",
                    isRequired = false,
                    name = KeyName.OrgNameKey
                },
                new Control()
                {
                    displayName = "Organization Address vocab key",
                    type = "input",
                    isRequired = false,
                    name = KeyName.OrgAddressKey
                },

                new Control()
                {
                    displayName = "Organization Country Code vocab key",
                    type = "input",
                    isRequired = false,
                    name = KeyName.OrgCountryCodeKey
                }
            }
        };
    }
}
