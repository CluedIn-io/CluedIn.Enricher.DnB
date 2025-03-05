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
            public const string DunsNumberKey = "dunsNumberKey";
            public const string OrgNameKey = "orgNameKey";
            public const string OrgAddressKey = "orgAddressKey";
            public const string OrgCountryCodeKey = "orgCountryCodeKey";
            public const string AuthUrl = "DNB_AUTH_URL";
            public const string AuthKey = "DNB_AUTH_KEY";
            public const string AuthSecret = "DNB_AUTH_SECRET";
            public const string AuthRequestBody = "DNB_AUTH_REQUEST_BODY";
            public const string DnBBaseUrl = "DNB_BASE_URL";
            // Match and Append https://directplus.documentation.dnb.com/openAPI.html?apiID=IDRExtendedMatch
            public const string VersionId = "versionId";
            public const string ProductId = "productId";
            public const string BlockIds = "blockIDs";
            public const string SkipDunsEntityCodeCreation = "skipDunsEntityCodeCreation";
        }

        public static AuthMethods AuthMethods { get; set; } = new AuthMethods
        {
            token = new List<Control>()
            {
                new()
                {
                    displayName = "Auth Url",
                    type = "input",
                    isRequired = true,
                    name = KeyName.AuthUrl //https://plus.dnb.com/v2/token
                },
                new()
                {
                    displayName = "API Key",
                    type = "password",
                    isRequired = true,
                    name = KeyName.AuthKey
                },
                new()
                {
                    displayName = "API Secret",
                    type = "password",
                    isRequired = true,
                    name = KeyName.AuthSecret
                },
                new()
                {
                    displayName = "Auth Request Body",
                    type = "input",
                    isRequired = true,
                    name = KeyName.AuthRequestBody //{"grant_type" : "client_credentials"}
                },
                new()
                {
                    displayName = "DnB Base Url",
                    type = "input",
                    isRequired = true,
                    name = KeyName.DnBBaseUrl
                },
                new()
                {
                    displayName = "Accepted Business Domain",
                    type = "input",
                    isRequired = false,
                    name = KeyName.AcceptedEntityType
                },
                new()
                {
                    displayName = "Organization Name vocab key",
                    type = "input",
                    isRequired = false,
                    name = KeyName.OrgNameKey
                },
                new()
                {
                    displayName = "DUNS vocab key",
                    type = "input",
                    isRequired = false,
                    name = KeyName.DunsNumberKey
                },
                new()
                {
                    displayName = "Organization Address vocab key",
                    type = "input",
                    isRequired = false,
                    name = KeyName.OrgAddressKey
                },
                new()
                {
                    displayName = "Organization Country Code vocab key",
                    type = "input",
                    isRequired = false,
                    name = KeyName.OrgCountryCodeKey
                },
                // Match and Append 
                new()
                {
                    displayName = $"Match and Append {KeyName.VersionId}",
                    type = "input",
                    isRequired = false,
                    name = KeyName.VersionId
                },
                new()
                {
                    displayName = $"Match and Append {KeyName.ProductId}",
                    type = "input",
                    isRequired = false,
                    name = KeyName.ProductId
                },
                new()
                {
                    displayName = $"Match and Append {KeyName.BlockIds}",
                    type = "input",
                    isRequired = false,
                    name = KeyName.BlockIds
                },
                new()
                {
                    displayName = "Skip Identifier Creation (Duns)",
                    type = "checkbox",
                    isRequired = false,
                    name =  KeyName.SkipDunsEntityCodeCreation,
                }
            }
        };
    }
}
