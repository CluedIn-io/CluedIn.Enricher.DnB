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
        public const string OrgCountryCodeKey = "orgCountryCodeKey";
        public const string OrgStreetAddressLine1Key = "orgStreetAddressLine1Key";
        public const string OrgStreetAddressLine2Key = "orgStreetAddressLine2Key";
        public const string OrgPostalCodeKey = "orgPostalCodeKey";
        public const string OrgAddressLocalityKey = "orgAddressLocalityKey";
        public const string OrgAddressCountyKey = "orgAddressCountyKey";
        public const string OrgAddressRegionKey = "orgAddressRegionKey";
        public const string OrgTelephoneNumberKey = "orgTelephoneNumberKey";
        public const string OrgUrlKey = "orgUrlKey";
        public const string OrgEmailKey = "orgEmailKey";
        public const string RegistrationNumbersKey = "registrationNumbersKey";
        public const string IndustryCodesKey = "industryCodesKey";
        public const string AuthUrl = "DNB_AUTH_URL";
        public const string AuthKey = "DNB_AUTH_KEY";
        public const string AuthSecret = "DNB_AUTH_SECRET";
        public const string AuthRequestBody = "DNB_AUTH_REQUEST_BODY";
        public const string DnBBaseUrl = "DNB_BASE_URL";
        // Match and Append https://directplus.documentation.dnb.com/openAPI.html?apiID=IDRExtendedMatch
        public const string VersionId = "versionId";
        public const string ProductId = "productId";
        public const string BlockIds = "blockIDs";
        public const string CustomerBillingEndorsementKey = "customerBillingEndorsementKey";
        public const string CandidateMaximumQuantityKey = "candidateMaximumQuantityKey";
        public const string ConfidenceLowerLevelThresholdValueKey = "confidenceLowerLevelThresholdValueKey";
        public const string ExclusionCriteriaKey = "exclusionCriteriaKey";
        public const string IsCleanseAndStandardizeInformationRequiredKey = "isCleanseAndStandardizeInformationRequiredKey";
        public const string TradeUpKey = "tradeUpKey";
        public const string OrderReasonKey = "orderReasonKey";
        public const string CustomerReference1Key = "customerReference1Key";
        public const string CustomerReference2Key = "customerReference2Key";
        public const string CustomerReference3Key = "customerReference3Key";
        public const string CustomerReference4Key = "customerReference4Key";
        public const string CustomerReference5Key = "customerReference5Key";
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
            DisplayName = "DUNS Vocabulary Key",
            Type = "vocabularyKeySelector",
            IsRequired = false,
            Name = KeyName.DunsNumberKey
        },
        new()
        {
            DisplayName = "Organization Name Vocabulary Key",
            Type = "vocabularyKeySelector",
            IsRequired = false,
            Name = KeyName.OrgNameKey
        },
        new()
        {
            DisplayName = "Organization Street Address Line 1 Vocabulary Key",
            Type = "vocabularyKeySelector",
            IsRequired = false,
            Name = KeyName.OrgStreetAddressLine1Key
        },
        new()
        {
            DisplayName = "Organization Street Address Line 2 Vocabulary Key",
            Type = "vocabularyKeySelector",
            IsRequired = false,
            Name = KeyName.OrgStreetAddressLine2Key
        },
        new()
        {
            DisplayName = "Organization Country Code Vocabulary Key",
            Type = "vocabularyKeySelector",
            IsRequired = false,
            Name = KeyName.OrgCountryCodeKey
        },
        new()
        {
            DisplayName = "Organization Postal Code Vocabulary Key",
            Type = "vocabularyKeySelector",
            IsRequired = false,
            Name = KeyName.OrgPostalCodeKey
        },
        new()
        {
            DisplayName = "Organization Address Locality Vocabulary Key",
            Type = "vocabularyKeySelector",
            IsRequired = false,
            Name = KeyName.OrgAddressLocalityKey
        },
        new()
        {
            DisplayName = "Organization Address County Vocabulary Key",
            Type = "vocabularyKeySelector",
            IsRequired = false,
            Name = KeyName.OrgAddressCountyKey
        },
        new()
        {
            DisplayName = "Organization Address Region Vocabulary Key",
            Type = "vocabularyKeySelector",
            IsRequired = false,
            Name = KeyName.OrgAddressRegionKey
        },
        new()
        {
            DisplayName = "Organization Telephone Number Vocabulary Key",
            Type = "vocabularyKeySelector",
            IsRequired = false,
            Name = KeyName.OrgTelephoneNumberKey
        },
        new()
        {
            DisplayName = "Organization Url Vocabulary Key",
            Type = "vocabularyKeySelector",
            IsRequired = false,
            Name = KeyName.OrgUrlKey
        },
        new()
        {
            DisplayName = "Organization Email Vocabulary Key",
            Type = "vocabularyKeySelector",
            IsRequired = false,
            Name = KeyName.OrgEmailKey
        },
        new()
        {
            DisplayName = "Customer Billing Endorsement",
            Type = "input",
            IsRequired = false,
            Help = "A reference used during the billing process.",
            Name = KeyName.CustomerBillingEndorsementKey
        },
        new()
        {
            DisplayName = "Candidate Maximum Quantity",
            Type = "input",
            IsRequired = false,
            Name = KeyName.CandidateMaximumQuantityKey,
            Help = "The maximum number of results to be returned. Default is 10",
            ValidationRules = new List<Dictionary<string, string>>
            {
                new() { { "regex", "[^0-9]" }, { "message", "Non-numeric values are not allowed." } },
                new() { { "regex", "^(?:0|10[1-9]|1[1-9]\\d|[2-9]\\d{2,})$" }, { "message", "Valid values: 1 to 100" } }
            }
        },
        new()
        {
            DisplayName = "Confidence Lower Level Threshold Value",
            Type = "input",
            IsRequired = false,
            Name = KeyName.ConfidenceLowerLevelThresholdValueKey,
            Help = "The lowest confidence level for entities returned in the response. Default is 4",
            ValidationRules = new List<Dictionary<string, string>>
            {
                new() { { "regex", "[^0-9]" }, { "message", "Non-numeric values are not allowed." } },
                new() { { "regex", "^(?:0|1[1-9]|\\d{2,})$" }, { "message", "Valid values: 1 to 10" } }
            }
        },
        new()
        {
            DisplayName = "Exclusion Criteria",
            Type = "input",
            IsRequired = false,
            Help = "Exclude entities based on several properties. (e.g., ExcludeNonHeadQuarters,ExcludeNonMarketable,ExcludeOutofBusiness,ExcludeUndeliverable,ExcludeUnreachable)",
            Name = KeyName.ExclusionCriteriaKey
        },
        new()
        {
            DisplayName = "Is Cleanse and Standardize Information Required",
            Type = "checkbox",
            IsRequired = false,
            Help = "Indicates if the cleanse and standardize information should be returned with the response.",
            Name = KeyName.IsCleanseAndStandardizeInformationRequiredKey
        },
        new()
        {
            DisplayName = "Trade Up",
            Type = "input",
            IsRequired = false,
            Help = "Indicates if the Headquarters D-U-N-S Number should be returned if a Branch is requested. (e.g., hq)",
            Name = KeyName.TradeUpKey
        },
        new()
        {
            DisplayName = "Order Reason",
            Type = "input",
            IsRequired = false,
            Help = "A code value that defines the grounds for the customer requesting the product.",
            Name = KeyName.OrderReasonKey
        },
        new()
        {
            DisplayName = "Customer Reference 1",
            Type = "input",
            IsRequired = false,
            Help = "A free form reference string to be linked to the request in order to support subsequent order reconciliation.",
            Name = KeyName.CustomerReference1Key
        },
        new()
        {
            DisplayName = "Customer Reference 2",
            Type = "input",
            IsRequired = false,
            Help = "A free form reference string to be linked to the request in order to support subsequent order reconciliation.",
            Name = KeyName.CustomerReference2Key
        },
        new()
        {
            DisplayName = "Customer Reference 3",
            Type = "input",
            IsRequired = false,
            Help = "A free form reference string to be linked to the request in order to support subsequent order reconciliation.",
            Name = KeyName.CustomerReference3Key
        },
        new()
        {
            DisplayName = "Customer Reference 4",
            Type = "input",
            IsRequired = false,
            Help = "A free form reference string to be linked to the request in order to support subsequent order reconciliation.",
            Name = KeyName.CustomerReference4Key
        },
        new()
        {
            DisplayName = "Customer Reference 5",
            Type = "input",
            IsRequired = false,
            Help = "A free form reference string to be linked to the request in order to support subsequent order reconciliation.",
            Name = KeyName.CustomerReference5Key
        },
        new()
        {
            DisplayName = "Industry Code Types",
            Type = "input",
            IsRequired = false,
            Name = KeyName.IndustryCodesKey,
            Help = "The TypeDnBCode values that will determine which industry codes are returned in the result. (e.g., 19295,37788)",
            ValidationRules = new List<Dictionary<string, string>>
            {
                new() { { "regex", "[^0-9,]" }, { "message", "Non-numeric values are not allowed." } }
            }
        },
        new()
        {
            DisplayName = "Registration Number Types",
            Type = "input",
            IsRequired = false,
            Name = KeyName.RegistrationNumbersKey,
            Help = "The TypeDnBCode values that will determine which registration numbers are returned in the result (e.g., 12897,12444)",
            ValidationRules = new List<Dictionary<string, string>>
            {
                new() { { "regex", "[^0-9,]" }, { "message", "Non-numeric values are not allowed." } }
            }
        },
        // Match and Append 
        new()
        {
            DisplayName = $"Match and Append {KeyName.VersionId}",
            Type = "input",
            IsRequired = false,
            Help = "The version of the product to be returned.",
            Name = KeyName.VersionId
        },
        new()
        {
            DisplayName = $"Match and Append {KeyName.ProductId}",
            Type = "input",
            IsRequired = false,
            Help = "The product ID provided by Dun & Bradstreet that identifies the product to be returned.",
            Name = KeyName.ProductId
        },
        new()
        {
            DisplayName = $"Match and Append {KeyName.BlockIds}",
            Type = "input",
            IsRequired = false,
            Help = "The block ID provided by Dun & Bradstreet that identifies the data block to be returned.",
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