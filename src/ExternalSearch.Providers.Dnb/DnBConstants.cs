using System;
using System.Collections.Generic;
using System.Linq;
using CluedIn.Core.Data.Relational;
using CluedIn.Core.Providers;

namespace CluedIn.ExternalSearch.Providers.DnB;

public static class DnBConstants
{
    public const string ComponentName = "DnB";
    public const string ProviderName = "DnB";

    public const string Instruction = """
                                      [
                                        {
                                          "type": "bulleted-list",
                                          "children": [
                                            {
                                              "type": "list-item",
                                              "children": [
                                                {
                                                  "text": "Add the entity type to specify the golden records you want to enrich. Only golden records belonging to that entity type will be enriched."
                                                }
                                              ]
                                            },
                                            {
                                              "type": "list-item",
                                              "children": [
                                                {
                                                  "text": "Add the vocabulary keys to provide the input for the enricher to search for additional information. For example, if you provide the website vocabulary key for the Web enricher, it will use specific websites to look for information about companies. In some cases, vocabulary keys are not required. If you don't add them, the enricher will use default vocabulary keys."
                                                }
                                              ]
                                            },
                                            {
                                              "type": "list-item",
                                              "children": [
                                                {
                                                  "text": "Add the API key and API Secret to enable the enricher to retrieve information from a specific API. For example, the DnB enricher requires an access key to authenticate with the DnB API."
                                                }
                                              ]
                                            }
                                          ]
                                        }
                                      ]
                                      """;

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
    }

    public struct ErrorMessages
    {
        public const string TooManyRequests = "Too many requests";
        public const string AccessTokenExpired = "Access Token Expired";
    }

    public static Guide Guide { get; set; } = new() { Instructions = Instruction };

    private static Version _cluedInVersion;
    public static Version CluedInVersion => _cluedInVersion ??= typeof(Core.Constants).Assembly.GetName().Version;
    public static string EntityTypeLabel => CluedInVersion < new Version(4, 5, 0) ? "Entity Type" : "Business Domain";
    public static string EntityCodeLabel => CluedInVersion < new Version(4, 5, 0) ? "Entity Code" : "Entity Identifier";


    public static IEnumerable<Control> Properties { get; set; } = new List<Control>
    {
        new()
        {
            DisplayName = "Organization Name Vocabulary Key",
            Type = "vocabularyKeySelector",
            IsRequired = false,
            Name = KeyName.OrgNameKey
        },
        new()
        {
            DisplayName = "DUNS Vocabulary Key",
            Type = "vocabularyKeySelector",
            IsRequired = false,
            Name = KeyName.DunsNumberKey
        },
        new()
        {
            DisplayName = "Organization Address Vocabulary Key",
            Type = "vocabularyKeySelector",
            IsRequired = false,
            Name = KeyName.OrgAddressKey
        },
        new()
        {
            DisplayName = "Organization Country Code Vocabulary Key",
            Type = "vocabularyKeySelector",
            IsRequired = false,
            Name = KeyName.OrgCountryCodeKey
        },
        // Match and Append 
        new()
        {
            DisplayName = $"Match and Append {KeyName.VersionId}",
            Type = "input",
            IsRequired = false,
            Name = KeyName.VersionId
        },
        new()
        {
            DisplayName = $"Match and Append {KeyName.ProductId}",
            Type = "input",
            IsRequired = false,
            Name = KeyName.ProductId
        },
        new()
        {
            DisplayName = $"Match and Append {KeyName.BlockIds}",
            Type = "input",
            IsRequired = false,
            Name = KeyName.BlockIds
        }
    };

    public static AuthMethods AuthMethods { get; set; } = new()
    {
        Token = new List<Control>
        {
            new()
            {
                DisplayName = "Auth Url",
                Type = "input",
                IsRequired = true,
                Name = KeyName.AuthUrl,
                Options = new Dictionary<string, object>
                {
                    { "defaultValue", "https://plus.dnb.com/v2/token" }
                }
            },
            new()
            {
                DisplayName = "API Key",
                Type = "password",
                IsRequired = true,
                Name = KeyName.AuthKey
            },
            new()
            {
                DisplayName = "API Secret",
                Type = "password",
                IsRequired = true,
                Name = KeyName.AuthSecret
            },
            new()
            {
                DisplayName = "Auth Request Body",
                Type = "input",
                IsRequired = true,
                Name = KeyName.AuthRequestBody,
                Options = new Dictionary<string, object>
                {
                    { "defaultValue", "{\"grant_type\" : \"client_credentials\"}" }
                }
            },
            new()
            {
                DisplayName = "DnB Base Url",
                Type = "input",
                IsRequired = true,
                Name = KeyName.DnBBaseUrl
            },
            new()
            {
                DisplayName = $"Accepted {EntityTypeLabel}",
                Type = "entityTypeSelector",
                IsRequired = false,
                Name = KeyName.AcceptedEntityType
            }
        }.Concat(Properties)
    };
}