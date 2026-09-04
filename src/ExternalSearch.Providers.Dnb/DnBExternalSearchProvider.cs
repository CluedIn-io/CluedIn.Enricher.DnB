using CluedIn.Core;
using CluedIn.Core.Caching.MicrosoftExtensions;
using CluedIn.Core.Connectors;
using CluedIn.Core.Data;
using CluedIn.Core.Data.Parts;
using CluedIn.Core.Data.Relational;
using CluedIn.Core.Data.Vocabularies;
using CluedIn.Core.ExternalSearch;
using CluedIn.Core.Providers;
using CluedIn.Crawling.Helpers;
using CluedIn.ExternalSearch.Providers.DnB.Custom;
using CluedIn.ExternalSearch.Providers.DnB.Model.AuthResponse;
using CluedIn.ExternalSearch.Providers.DnB.Model.DnBResponse;
using CluedIn.ExternalSearch.Providers.DnB.Vocabularies;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EntityType = CluedIn.Core.Data.EntityType;
using ExecutionContext = CluedIn.Core.ExecutionContext;

namespace CluedIn.ExternalSearch.Providers.DnB;

/// <summary>The dnb graph external search provider.</summary>
/// <seealso cref="CluedIn.ExternalSearch.ExternalSearchProviderBase" />
public class DnBExternalSearchProvider : ExternalSearchProviderBase, IExtendedEnricherMetadata, IConfigurableExternalSearchProvider, IExternalSearchProviderWithVerifyConnection
{
    public static readonly Guid ProviderId = Guid.Parse("31d78803-3a06-45a7-9ef2-4179b8242fbf");

    public string Icon => "Resources.dnb.svg";

    public string Domain => "https://www.dnb.com/";

    public string About => "Dun & Bradstreet is global provider of business decisioning data and analytics.";

    public AuthMethods AuthMethods { get; } = DnBConstants.AuthMethods;
    public IEnumerable<Control> Properties { get; } = new List<Control>();
    public Guide Guide => null;
    public IntegrationType Type => IntegrationType.Enrichment;
    public override Version Version => new Version("4.6.2.0"); // Update version to expire cached results when we make changes like changing the result type

    private static readonly EntityType[] DefaultAcceptedEntityTypes = { EntityType.Organization };
    private static readonly SemaphoreSlim semaphore = new(1, 1);
    /**********************************************************************************************************
     * CONSTRUCTORS
     **********************************************************************************************************/

    public DnBExternalSearchProvider()
        : base(ProviderId, DefaultAcceptedEntityTypes)
    {
    }

    /**********************************************************************************************************
     * METHODS
     **********************************************************************************************************/
    public override bool Accepts(EntityType entityType) => true;

    public override IEnumerable<IExternalSearchQuery> BuildQueries(ExecutionContext context, IExternalSearchRequest request) => throw new NotSupportedException();

    public override IEnumerable<IExternalSearchQueryResult> ExecuteSearch(ExecutionContext context, IExternalSearchQuery query) => throw new NotSupportedException();

    public override IEnumerable<Clue> BuildClues(ExecutionContext context, IExternalSearchQuery query, IExternalSearchQueryResult result, IExternalSearchRequest request) => BuildClues(context, query, result, request, null, null).AsEnumerable();

    public override IEntityMetadata GetPrimaryEntityMetadata(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request) => GetPrimaryEntityMetadata(context, result, request, null, null);

    public override IPreviewImage GetPrimaryEntityPreviewImage(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request) => throw new NotSupportedException();

    private static HashSet<string> GetValue(IExternalSearchRequest request, IDictionary<string, object> config, string keyName, VocabularyKey defaultKey)
    {
        HashSet<string> value;
        if (config.TryGetValue(keyName, out var customVocabKey) && !string.IsNullOrWhiteSpace(customVocabKey?.ToString()))
        {
            value = request.QueryParameters.GetValue<string, HashSet<string>>(customVocabKey.ToString(), new HashSet<string>());
        }
        else
        {
            value = request.QueryParameters.GetValue(defaultKey, new HashSet<string>());
        }

        return value;
    }

    // ReSharper disable once UnusedParameter.Local
    private IEnumerable<IExternalSearchQuery> InternalBuildQueries(ExecutionContext context, IExternalSearchRequest request, IDictionary<string, object> config)
    {
        if (!this.Accepts(config, request.EntityMetaData.EntityType))
            yield break;

        var jobData = new DnBExternalSearchJobData(config);

        // Query Input
        //For companies use CluedInOrganization vocab, for people use CluedInPerson and so on for different types.
        var entityType = request.EntityMetaData.EntityType;
        var dunsNumber = GetValue(request, config, DnBConstants.KeyName.DunsNumberKey, Core.Data.Vocabularies.Vocabularies.CluedInOrganization.CodesDunsNumber);


        if (dunsNumber != null)
        {
            foreach (var value in dunsNumber)
            {
                yield return new ExternalSearchQuery(this, entityType, new Dictionary<string, string> { { "id", value } });
            }
        }

        var orgName = GetValue(request, config, DnBConstants.KeyName.OrgNameKey, Core.Data.Vocabularies.Vocabularies.CluedInOrganization.OrganizationName)?.FirstOrDefault();
        var registrationNumber = GetValue(request, config, DnBConstants.KeyName.RegistrationNumberKey, Core.Data.Vocabularies.Vocabularies.CluedInOrganization.TaxId)?.FirstOrDefault();
        var orgCountryCode = GetValue(request, config, DnBConstants.KeyName.OrgCountryCodeKey, Core.Data.Vocabularies.Vocabularies.CluedInOrganization.AddressCountryCode)?.FirstOrDefault();
        var orgStreetAddressLine1 = GetValue(request, config, DnBConstants.KeyName.OrgStreetAddressLine1Key, Core.Data.Vocabularies.Vocabularies.CluedInOrganization.AddressStreetName)?.FirstOrDefault();
        var orgStreetAddressLine2 = GetValue(request, config, DnBConstants.KeyName.OrgStreetAddressLine2Key, Core.Data.Vocabularies.Vocabularies.CluedInOrganization.AddressStreetName)?.FirstOrDefault();
        var orgPostalCode = GetValue(request, config, DnBConstants.KeyName.OrgPostalCodeKey, Core.Data.Vocabularies.Vocabularies.CluedInOrganization.AddressZipCode)?.FirstOrDefault();
        var orgAddressLocality = GetValue(request, config, DnBConstants.KeyName.OrgAddressLocalityKey, Core.Data.Vocabularies.Vocabularies.CluedInOrganization.AddressCity)?.FirstOrDefault();
        var orgCounty = GetValue(request, config, DnBConstants.KeyName.OrgAddressCountyKey, Core.Data.Vocabularies.Vocabularies.CluedInOrganization.AddressState)?.FirstOrDefault();
        var orgRegion = GetValue(request, config, DnBConstants.KeyName.OrgAddressRegionKey, Core.Data.Vocabularies.Vocabularies.CluedInOrganization.AddressState)?.FirstOrDefault();
        var orgTelephoneNumber = GetValue(request, config, DnBConstants.KeyName.OrgTelephoneNumberKey, Core.Data.Vocabularies.Vocabularies.CluedInOrganization.PhoneNumber)?.FirstOrDefault();
        var orgUrl = GetValue(request, config, DnBConstants.KeyName.OrgUrlKey, Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Website)?.FirstOrDefault();
        var orgEmail = GetValue(request, config, DnBConstants.KeyName.OrgEmailKey, Core.Data.Vocabularies.Vocabularies.CluedInOrganization.ContactEmail)?.FirstOrDefault();

        var orgNameAndCountryHasValue = !string.IsNullOrWhiteSpace(orgName) && !string.IsNullOrWhiteSpace(orgCountryCode);
        var versionIdAndProductIdHasValue = !string.IsNullOrWhiteSpace(jobData.VersionId) && !string.IsNullOrWhiteSpace(jobData.ProductId);
        var blockIds = !string.IsNullOrWhiteSpace(jobData.BlockIds);

        if (orgNameAndCountryHasValue && (versionIdAndProductIdHasValue || blockIds))
        {
            var parameters = new Dictionary<string, string>
            {
                { DnBConstants.KeyName.OrgNameKey, orgName },
                { DnBConstants.KeyName.OrgCountryCodeKey, orgCountryCode }
            };

            void AddIfNotNullOrWhiteSpace(string key, string value)
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    parameters[key] = value;
                }
            }

            AddIfNotNullOrWhiteSpace(DnBConstants.KeyName.RegistrationNumberKey, registrationNumber);
            AddIfNotNullOrWhiteSpace(DnBConstants.KeyName.OrgStreetAddressLine1Key, orgStreetAddressLine1);
            AddIfNotNullOrWhiteSpace(DnBConstants.KeyName.OrgStreetAddressLine2Key, orgStreetAddressLine2);
            AddIfNotNullOrWhiteSpace(DnBConstants.KeyName.OrgPostalCodeKey, orgPostalCode);
            AddIfNotNullOrWhiteSpace(DnBConstants.KeyName.OrgAddressLocalityKey, orgAddressLocality);
            AddIfNotNullOrWhiteSpace(DnBConstants.KeyName.OrgAddressCountyKey, orgCounty);
            AddIfNotNullOrWhiteSpace(DnBConstants.KeyName.OrgAddressRegionKey, orgRegion);
            AddIfNotNullOrWhiteSpace(DnBConstants.KeyName.OrgTelephoneNumberKey, orgTelephoneNumber);
            AddIfNotNullOrWhiteSpace(DnBConstants.KeyName.OrgUrlKey, orgUrl);
            AddIfNotNullOrWhiteSpace(DnBConstants.KeyName.OrgEmailKey, orgEmail);
            AddIfNotNullOrWhiteSpace(DnBConstants.KeyName.CustomerBillingEndorsementKey, jobData.CustomerBillingEndorsement);
            AddIfNotNullOrWhiteSpace(DnBConstants.KeyName.CandidateMaximumQuantityKey, jobData.CandidateMaximumQuantity);
            AddIfNotNullOrWhiteSpace(DnBConstants.KeyName.ConfidenceLowerLevelThresholdValueKey, jobData.ConfidenceLowerLevelThresholdValue);
            AddIfNotNullOrWhiteSpace(DnBConstants.KeyName.ExclusionCriteriaKey, jobData.ExclusionCriteria);
            parameters[DnBConstants.KeyName.IsCleanseAndStandardizeInformationRequiredKey] = jobData.IsCleanseAndStandardizeInformationRequired.ToString();
            AddIfNotNullOrWhiteSpace(DnBConstants.KeyName.TradeUpKey, jobData.TradeUp);
            AddIfNotNullOrWhiteSpace(DnBConstants.KeyName.OrderReasonKey, jobData.OrderReason);
            AddIfNotNullOrWhiteSpace(DnBConstants.KeyName.CustomerReference1Key, jobData.CustomerReference1);
            AddIfNotNullOrWhiteSpace(DnBConstants.KeyName.CustomerReference2Key, jobData.CustomerReference2);
            AddIfNotNullOrWhiteSpace(DnBConstants.KeyName.CustomerReference3Key, jobData.CustomerReference3);
            AddIfNotNullOrWhiteSpace(DnBConstants.KeyName.CustomerReference4Key, jobData.CustomerReference4);
            AddIfNotNullOrWhiteSpace(DnBConstants.KeyName.CustomerReference5Key, jobData.CustomerReference5);

            yield return new ExternalSearchQuery(this, entityType, parameters);
        }
    }

    private static IEnumerable<IExternalSearchQueryResult> InternalExecuteSearch(ExecutionContext context, IExternalSearchQuery query, DnBExternalSearchJobData jobData)
    {
        try
        {
            return HandleSearch(context, query, jobData, false);
        }
        catch (BadTokenException)
        {
            return HandleSearch(context, query, jobData, true);
        }
    }

    private static IEnumerable<IExternalSearchQueryResult> HandleSearch(ExecutionContext context, IExternalSearchQuery query, DnBExternalSearchJobData jobData, bool bypassCache)
    {
        var token = GetAuthToken(context, jobData, query.ProviderDefinitionId, bypassCache).GetAwaiter().GetResult();
        var dunsNumber = query.QueryParameters.GetValue("id")?.FirstOrDefault();
        var orgName = query.QueryParameters.GetValue(DnBConstants.KeyName.OrgNameKey)?.FirstOrDefault();
        var orgCountryCode = query.QueryParameters.GetValue(DnBConstants.KeyName.OrgCountryCodeKey)?.FirstOrDefault();
        var includeLastApiCallDetails = jobData.IncludeLastApiCallDetails;
        var identityResolutionApi = jobData.IdentityResolutionApi;
        var getDataUsingMatchesDuns = jobData.GetDataUsingMatchesDuns;
        var isCleanseApi = string.Equals(identityResolutionApi, "CleanseMatch", StringComparison.OrdinalIgnoreCase);

        var client = new RestClient(jobData.DnBBaseUrl);

        RestRequest request;

        // If DUNS is provided, then we can directly retrieve the organization data using the DUNS number.
        if (!string.IsNullOrEmpty(dunsNumber))
        {
            var (dunsData, statusCode, errorMessage, timestamp) = ExecuteDunsRequest(client, dunsNumber, jobData, token);

            if (dunsData == null)
            {
                if (!includeLastApiCallDetails)
                {
                    throw new ApplicationException(
                        $"Could not execute external search query - DnB returned error: {errorMessage}");
                }

                yield return new ExternalSearchQueryResult<JObject>(query, CreateLastApiCallErrorData(statusCode, errorMessage, timestamp));
                yield break;

            }

            AddLastApiCallDetails(dunsData, statusCode, errorMessage, timestamp);

            var organization = dunsData.SelectToken("organization");

            if (organization == null)
            {
                if (!includeLastApiCallDetails)
                {
                    throw new ApplicationException(
                        "Could not execute external search query - DnB returned empty organization");
                }
            }

            yield return new ExternalSearchQueryResult<JObject>(query, dunsData);
            yield break;
        }

        // If DUNS is not provided, then Name and Country Code (ISO Alpha-2 code) must be specified for identity resolution.
        if (!string.IsNullOrWhiteSpace(orgName) && !string.IsNullOrWhiteSpace(orgCountryCode))
        {
            var requestResource = isCleanseApi
                ? "match/cleanseMatch"
                : "match/extendedMatch";
            request = new RestRequest(requestResource, Method.Get);
            request.AddQueryParameter("name", orgName);
            request.AddQueryParameter("countryISOAlpha2Code", orgCountryCode);

            AddExtendedMatchParameters(query, request);
        }
        else
        {
            throw new Exception("Could not execute external search query - Name and Country Code (ISO Alpha-2 code) must be specified if DUNS is not provided.");
        }

        if (!string.IsNullOrWhiteSpace(jobData.VersionId) && !string.IsNullOrWhiteSpace(jobData.ProductId))
        {
            request.AddQueryParameter("versionId", jobData.VersionId);
            request.AddQueryParameter("productId", jobData.ProductId);
        }
        else if (!string.IsNullOrWhiteSpace(jobData.BlockIds))
        {
            request.AddQueryParameter("blockIDs", jobData.BlockIds);
        }
        else
        {
            throw new Exception("Could not execute external search query - Either productId & versionId or blockIDs must be specified.");
        }

        request.AddHeader("Authorization", $"Bearer {token}");

        var cleanseResponse = ExecuteWithRateLimitHandling(client, request);
        var cleanseTimestamp = DateTimeOffset.UtcNow;
        var data = JsonUtility.Deserialize<JObject>(cleanseResponse.Content, new JsonSerializer() { NullValueHandling = NullValueHandling.Ignore });

        if (data == null)
        {
            throw new ApplicationException("Could not execute external search query - DnB returned empty response content");
        }

        var cleanseResponseError = data.SelectToken("error.errorMessage")?.ToString();

        if (cleanseResponse.StatusCode == HttpStatusCode.OK)
        {
            var candidatesMatchedQuantity = data.SelectToken("candidatesMatchedQuantity")?.ToObject<long?>();

            // If DUNS is not provided and the user has requested to get data using the matched DUNS
            // Then we need to find the DUNS number from the match candidates and retrieve the organization data using that DUNS number.
            if (getDataUsingMatchesDuns)
            {
                var lastApiCallStatusCode = string.Empty;
                var lastApiCallErrorMessage = string.Empty;
                var lastApiCallTimeStamp = DateTimeOffset.MinValue;

                // CleanseMatch API returns a list of match candidates,
                // So we need to iterate through the candidates and find the first one with a valid DUNS number (with response).
                if (isCleanseApi)
                {
                    var matchCandidates = data.SelectTokens("matchCandidates[*]").ToList();

                    foreach (var candidate in matchCandidates)
                    {
                        var duns = candidate.SelectToken("organization.duns")?.ToString();
                        if (string.IsNullOrEmpty(duns))
                        {
                            continue;
                        }

                        var dunsResult = TryExecuteDunsRequest(client, duns, jobData, token);
                        if (dunsResult == null) continue;

                        // Track error details from failed candidates
                        if (dunsResult.Value.Data == null)
                        {
                            lastApiCallStatusCode = dunsResult.Value.StatusCode;
                            lastApiCallErrorMessage = dunsResult.Value.ErrorMessage;
                            lastApiCallTimeStamp = dunsResult.Value.Timestamp;
                            continue;
                        }

                        var matchConfidenceCode = candidate.SelectToken("matchQualityInformation.confidenceCode")?.ToString();
                        data = dunsResult.Value.Data;
                        data.TryAdd("candidatesMatchedQuantity", candidatesMatchedQuantity);
                        data.TryAdd("matchConfidenceCode", matchConfidenceCode);
                        AddLastApiCallDetails(data, dunsResult.Value.StatusCode, dunsResult.Value.ErrorMessage, dunsResult.Value.Timestamp);

                        break;
                    }

                    // If all candidates failed, add the last error details to data
                    // TryAdd will not overwrite existing keys, so the last successful candidate's details will be preserved if any candidate succeeded
                    if (!string.IsNullOrEmpty(lastApiCallStatusCode))
                    {
                        AddLastApiCallDetails(data, lastApiCallStatusCode, lastApiCallErrorMessage, lastApiCallTimeStamp);
                    }
                    else
                    {
                        // No DUNS calls were made or all returned null; fall back to match API call details
                        AddLastApiCallDetails(data, cleanseResponse.StatusCode.ToString(), cleanseResponseError ?? cleanseResponse.StatusDescription, cleanseTimestamp);
                    }
                }
                else
                {
                    // ExtendedMatch API returns a single match candidate, so we can directly retrieve the DUNS number from the response.
                    var matchDuns = data.SelectToken("embeddedProduct.organization.duns")?.ToString();

                    if (!string.IsNullOrEmpty(matchDuns))
                    {
                        var dunsResult = ExecuteDunsRequest(client, matchDuns, jobData, token);
                        data = dunsResult.Data;
                        AddLastApiCallDetails(data, dunsResult.StatusCode, dunsResult.ErrorMessage, dunsResult.Timestamp);
                    }
                    else
                    {
                        AddLastApiCallDetails(data, cleanseResponse.StatusCode.ToString(), cleanseResponseError ?? cleanseResponse.StatusDescription, cleanseTimestamp);
                    }
                }
            }
            else
            {
                // If user has not requested to get data using the matched DUNS, then we just return the response from the match API.
                AddLastApiCallDetails(data, cleanseResponse.StatusCode.ToString(), cleanseResponseError ?? cleanseResponse.StatusDescription, cleanseTimestamp);
            }

            var organization = data.SelectToken("organization") ?? data.SelectToken("embeddedProduct.organization");

            if (isCleanseApi && organization == null)
            {
                var organizations = data.SelectTokens("matchCandidates[*].organization")?.ToList();

                foreach (var cleanseOrg in organizations.TakeWhile(cleanseOrg => cleanseOrg != null))
                {
                    var cleanseApiData = (JObject)data.DeepClone();
                    cleanseApiData.Remove("organization");
                    cleanseApiData["organization"] = cleanseOrg;

                    yield return new ExternalSearchQueryResult<JObject>(query, cleanseApiData);
                }

                yield break;
            }

            if (isCleanseApi && organization == null)
            {
                var organizations = data.SelectTokens("matchCandidates[*].organization")?.ToList();

                foreach (var cleanseOrg in organizations.TakeWhile(cleanseOrg => cleanseOrg != null))
                {
                    var cleanseApiData = (JObject)data.DeepClone();
                    cleanseApiData.Remove("organization");
                    cleanseApiData["organization"] = cleanseOrg;

                    yield return new ExternalSearchQueryResult<JObject>(query, cleanseApiData);
                }

                yield break;
            }

            if (organization == null)
            {
                if (!includeLastApiCallDetails)
                {
                    throw new ApplicationException(
                        "Could not execute external search query - DnB returned empty organization");
                }

                yield return new ExternalSearchQueryResult<JObject>(query, data);
                yield break;
            }

            data.Remove("organization");
            data.TryAdd("organization", organization);

            yield return new ExternalSearchQueryResult<JObject>(query, data);
            yield break;
        }

        if (cleanseResponse.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new BadTokenException("Access token expired");
        }

        // If includeLastApiCallDetails is true, we return the error details in the result instead of throwing an exception
        if (includeLastApiCallDetails)
        {
            yield return new ExternalSearchQueryResult<JObject>(query, CreateLastApiCallErrorData(
                cleanseResponse.StatusCode.ToString(),
                cleanseResponseError ?? cleanseResponse.ErrorException?.Message ?? cleanseResponse.StatusDescription,
                cleanseTimestamp));
            yield break;
        }

        // If includeLastApiCallDetails is true, we return the error details in the result instead of throwing an exception
        if (includeLastApiCallDetails)
        {
            yield return new ExternalSearchQueryResult<JObject>(query, CreateLastApiCallErrorData(
                cleanseResponse.StatusCode.ToString(),
                cleanseResponseError ?? cleanseResponse.ErrorException?.Message ?? cleanseResponse.StatusDescription,
                cleanseTimestamp));
            yield break;
        }

        if (cleanseResponse.ErrorException != null)
        {
            throw new AggregateException(cleanseResponse.ErrorException.Message, cleanseResponse.ErrorException);
        }

        throw new ApplicationException("Could not execute external search query - StatusCode:" + cleanseResponse.StatusCode + "; Content: " + cleanseResponse.Content);
    }

    private static void AddLastApiCallDetails(JObject data, string statusCode, string errorMessage, DateTimeOffset timestamp)
    {
        data.TryAdd("lastApiCallStatusCode", statusCode);
        data.TryAdd("lastApiCallErrorMessage", errorMessage);
        data.TryAdd("lastApiCallTimestamp", timestamp.ToString("yyyy-MM-dd HH:mm:ss"));
    }

    private static JObject CreateLastApiCallErrorData(string statusCode, string errorMessage, DateTimeOffset timestamp)
    {
        return new JObject
        {
            ["lastApiCallStatusCode"] = statusCode,
            ["lastApiCallErrorMessage"] = errorMessage,
            ["lastApiCallTimestamp"] = timestamp.ToString("yyyy-MM-dd HH:mm:ss")
        };
    }

    private static (JObject Data, string StatusCode, string ErrorMessage, DateTimeOffset Timestamp) ExecuteDunsRequest(RestClient client, string dunsNumber, DnBExternalSearchJobData jobData, string token)
    {
        var requestResource = $"data/duns/{dunsNumber}";
        var request = new RestRequest(requestResource, Method.Get);

        if (!string.IsNullOrWhiteSpace(jobData.VersionId) && !string.IsNullOrWhiteSpace(jobData.ProductId))
        {
            request.AddQueryParameter("versionId", jobData.VersionId);
            request.AddQueryParameter("productId", jobData.ProductId);
        }
        else if (!string.IsNullOrWhiteSpace(jobData.BlockIds))
        {
            request.AddQueryParameter("blockIDs", jobData.BlockIds);
        }

        request.AddHeader("Authorization", $"Bearer {token}");

        var response = ExecuteWithRateLimitHandling(client, request);
        var timestamp = DateTimeOffset.UtcNow;

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new BadTokenException("Access token expired");
        }

        if (response.StatusCode != HttpStatusCode.OK)
        {
            var errorMessage = response.StatusDescription;

            try
            {
                var errorBody = JObject.Parse(response.Content);
                errorMessage = errorBody.SelectToken("error.errorMessage")?.ToString() ?? errorMessage;
            }
            catch
            {
                return (null, response.StatusCode.ToString(), errorMessage, timestamp);
            }

            return (null, response.StatusCode.ToString(), errorMessage, timestamp);
        }

        var data = JsonUtility.Deserialize<JObject>(response.Content, new JsonSerializer() { NullValueHandling = NullValueHandling.Ignore });

        return data == null ? throw new ApplicationException("Could not execute external search query - DnB returned empty data") : (data, response.StatusCode.ToString(), response.StatusDescription, timestamp);
    }

    private static (JObject Data, string StatusCode, string ErrorMessage, DateTimeOffset Timestamp)? TryExecuteDunsRequest(RestClient client, string dunsNumber, DnBExternalSearchJobData jobData, string token)
    {
        try
        {
            return ExecuteDunsRequest(client, dunsNumber, jobData, token);
        }
        catch (BadTokenException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return (null, null, ex.Message, DateTimeOffset.UtcNow);
        }
    }

    private static RestResponse ExecuteWithRateLimitHandling(RestClient client, RestRequest request, int maxRetries = 3)
    {
        for (var attempt = 0; attempt <= maxRetries; attempt++)
        {
            var result = client.ExecuteAsync(request).GetAwaiter().GetResult();

            if (result.StatusCode != HttpStatusCode.TooManyRequests)
            {
                return result;
            }

            if (attempt == maxRetries)
            {
                throw new WebException("TooManyRequests");
            }

            Thread.Sleep(TimeSpan.FromSeconds(60));
        }

        throw new WebException("TooManyRequests");
    }

    private static void AddExtendedMatchParameters(IExternalSearchQuery query, RestRequest request)
    {
        var registrationNumber = query.QueryParameters.GetValue(DnBConstants.KeyName.RegistrationNumberKey)?.FirstOrDefault();
        var orgStreetAddress1 = query.QueryParameters.GetValue(DnBConstants.KeyName.OrgStreetAddressLine1Key)?.FirstOrDefault();
        var orgStreetAddress2 = query.QueryParameters.GetValue(DnBConstants.KeyName.OrgStreetAddressLine2Key)?.FirstOrDefault();
        var orgPostalCode = query.QueryParameters.GetValue(DnBConstants.KeyName.OrgPostalCodeKey)?.FirstOrDefault();
        var orgAddressLocality = query.QueryParameters.GetValue(DnBConstants.KeyName.OrgAddressLocalityKey)?.FirstOrDefault();
        var orgAddressCounty = query.QueryParameters.GetValue(DnBConstants.KeyName.OrgAddressCountyKey)?.FirstOrDefault();
        var orgAddressRegion = query.QueryParameters.GetValue(DnBConstants.KeyName.OrgAddressRegionKey)?.FirstOrDefault();
        var orgTelephoneNumber = query.QueryParameters.GetValue(DnBConstants.KeyName.OrgTelephoneNumberKey)?.FirstOrDefault();
        var orgUrl = query.QueryParameters.GetValue(DnBConstants.KeyName.OrgUrlKey)?.FirstOrDefault();
        var orgEmail = query.QueryParameters.GetValue(DnBConstants.KeyName.OrgEmailKey)?.FirstOrDefault();
        var customerBillingEndorsement = query.QueryParameters.GetValue(DnBConstants.KeyName.CustomerBillingEndorsementKey)?.FirstOrDefault();
        var candidateMaximumQuantity = query.QueryParameters.GetValue(DnBConstants.KeyName.CandidateMaximumQuantityKey)?.FirstOrDefault();
        var confidenceLowerLevelThresholdValue = query.QueryParameters.GetValue(DnBConstants.KeyName.ConfidenceLowerLevelThresholdValueKey)?.FirstOrDefault();
        var exclusionCriteria = query.QueryParameters.GetValue(DnBConstants.KeyName.ExclusionCriteriaKey)?.FirstOrDefault();
        var isCleanseAndStandardizeInformationRequired = query.QueryParameters.GetValue(DnBConstants.KeyName.IsCleanseAndStandardizeInformationRequiredKey)?.FirstOrDefault();
        var tradeUp = query.QueryParameters.GetValue(DnBConstants.KeyName.TradeUpKey)?.FirstOrDefault();
        var orderReason = query.QueryParameters.GetValue(DnBConstants.KeyName.OrderReasonKey)?.FirstOrDefault();
        var customerReference1 = query.QueryParameters.GetValue(DnBConstants.KeyName.CustomerReference1Key)?.FirstOrDefault();
        var customerReference2 = query.QueryParameters.GetValue(DnBConstants.KeyName.CustomerReference2Key)?.FirstOrDefault();
        var customerReference3 = query.QueryParameters.GetValue(DnBConstants.KeyName.CustomerReference3Key)?.FirstOrDefault();
        var customerReference4 = query.QueryParameters.GetValue(DnBConstants.KeyName.CustomerReference4Key)?.FirstOrDefault();
        var customerReference5 = query.QueryParameters.GetValue(DnBConstants.KeyName.CustomerReference5Key)?.FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(registrationNumber))
        {
            request.AddQueryParameter("registrationNumber", registrationNumber);
        }

        if (!string.IsNullOrWhiteSpace(orgStreetAddress1))
        {
            request.AddQueryParameter("streetAddressLine1", orgStreetAddress1);
        }

        if (!string.IsNullOrWhiteSpace(orgStreetAddress2))
        {
            request.AddQueryParameter("streetAddressLine2", orgStreetAddress2);
        }

        if (!string.IsNullOrWhiteSpace(orgPostalCode))
        {
            request.AddQueryParameter("postalCode", orgPostalCode);
        }

        if (!string.IsNullOrWhiteSpace(orgAddressLocality))
        {
            request.AddQueryParameter("addressLocality", orgAddressLocality);
        }

        if (!string.IsNullOrWhiteSpace(orgAddressCounty))
        {
            request.AddQueryParameter("addressCounty", orgAddressCounty);
        }

        if (!string.IsNullOrWhiteSpace(orgAddressRegion))
        {
            request.AddQueryParameter("addressRegion", orgAddressRegion);
        }

        if (!string.IsNullOrWhiteSpace(orgTelephoneNumber))
        {
            request.AddQueryParameter("telephoneNumber", orgTelephoneNumber);
        }

        if (!string.IsNullOrWhiteSpace(orgUrl))
        {
            request.AddQueryParameter("url", orgUrl);
        }

        if (!string.IsNullOrWhiteSpace(orgEmail))
        {
            request.AddQueryParameter("email", orgEmail);
        }

        if (!string.IsNullOrWhiteSpace(customerBillingEndorsement))
        {
            request.AddQueryParameter("customerBillingEndorsement", customerBillingEndorsement);
        }

        if (!string.IsNullOrWhiteSpace(candidateMaximumQuantity))
        {
            request.AddQueryParameter("candidateMaximumQuantity", candidateMaximumQuantity);
        }

        if (!string.IsNullOrWhiteSpace(confidenceLowerLevelThresholdValue))
        {
            request.AddQueryParameter("confidenceLowerLevelThresholdValue", confidenceLowerLevelThresholdValue);
        }

        if (!string.IsNullOrWhiteSpace(exclusionCriteria))
        {
            request.AddQueryParameter("exclusionCriteria", exclusionCriteria);
        }

        if (!string.IsNullOrWhiteSpace(isCleanseAndStandardizeInformationRequired))
        {
            request.AddQueryParameter("isCleanseAndStandardizeInformationRequired", isCleanseAndStandardizeInformationRequired);
        }

        if (!string.IsNullOrWhiteSpace(tradeUp))
        {
            request.AddQueryParameter("tradeUp", tradeUp);
        }

        if (!string.IsNullOrWhiteSpace(orderReason))
        {
            request.AddQueryParameter("orderReason", orderReason);
        }

        if (!string.IsNullOrWhiteSpace(customerReference1))
        {
            request.AddQueryParameter("customerReference1", customerReference1);
        }

        if (!string.IsNullOrWhiteSpace(customerReference2))
        {
            request.AddQueryParameter("customerReference2", customerReference2);
        }

        if (!string.IsNullOrWhiteSpace(customerReference3))
        {
            request.AddQueryParameter("customerReference3", customerReference3);
        }

        if (!string.IsNullOrWhiteSpace(customerReference4))
        {
            request.AddQueryParameter("customerReference4", customerReference4);
        }

        if (!string.IsNullOrWhiteSpace(customerReference5))
        {
            request.AddQueryParameter("customerReference5", customerReference5);
        }
    }

    private static async Task<string> GetAuthToken(ExecutionContext context, DnBExternalSearchJobData jobData, Guid providerDefinitionId, bool bypassCache)
    {
        var memoryCache = context.ApplicationContext.Container.Resolve<IMemoryCache>();
        var cacheKey = $"DnBDirectPlusService.AuthToken({providerDefinitionId})";

        if (!bypassCache && memoryCache.TryGetValue(cacheKey, out string cached))
        {
            return cached;
        }

        await semaphore.WaitAsync();
        try
        {
            if (!bypassCache && memoryCache.TryGetValue(cacheKey, out cached))
            {
                return cached;
            }

            var key = jobData.AuthKey;
            var secret = jobData.AuthSecret;
            var bytes = Encoding.ASCII.GetBytes($"{key}:{secret}");

            var restClient = new RestClient(new RestClientOptions(jobData.AuthUrl) { Timeout = Timeout.InfiniteTimeSpan } );
            var request = new RestRequest { Method = Method.Post };
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Basic {Convert.ToBase64String(bytes)}");
            var body = jobData.AuthRequestBody;
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            var response = await restClient.ExecuteAsync(request);
            var responseContent = JsonUtility.Deserialize<AuthResponse>(response.Content);

            if (!response.IsSuccessful)
            {
                throw new Exception($"Could not get access token. StatusCode: {response.StatusCode}, Status Description: {response.StatusDescription}, Error Message:{response.ErrorMessage}", response.ErrorException);
            }

            if (string.IsNullOrEmpty(responseContent?.AccessToken))
            {
                throw new Exception($"Access token returned is empty. StatusCode: {response.StatusCode}, Status Description: {response.StatusDescription}, Error Message:{response.ErrorMessage}");
            }

            using (var entry = memoryCache.CreateEntry(cacheKey))
            {
                entry.Value = responseContent.AccessToken;
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24); // DnB Token lives for 24 hours
            }

            return responseContent.AccessToken;
        }
        finally
        {
            semaphore.Release();
        }
    }

    private IEntityMetadata CreateMetadata(ExecutionContext context, IExternalSearchQueryResult<JObject> resultItem, IExternalSearchRequest request, DnBExternalSearchJobData jobData)
    {
        var metadata = new EntityMetadataPart();

        this.PopulateMetadata(context, metadata, resultItem, request, jobData);

        return metadata;
    }

    private EntityCode GetOriginEntityCode(IExternalSearchQueryResult<JObject> resultItem, IExternalSearchRequest request, IExternalSearchQuery query)
    {
        string duns = resultItem.Data?
            .SelectToken("organization.duns")
            ?.Value<string>();
        
        return new EntityCode(request.EntityMetaData.EntityType, this.GetCodeOrigin(), duns ?? $"{query.QueryKey}{request.EntityMetaData.OriginEntityCode}".ToDeterministicGuid().ToString());
    }

    /// <summary>Gets the code origin.</summary>
    /// <returns>The code origin</returns>
    private CodeOrigin GetCodeOrigin()
    {
        return CodeOrigin.CluedIn.CreateSpecific("DnB");
    }

    private void PopulateMetadata(ExecutionContext context, IEntityMetadata metadata, IExternalSearchQueryResult<JObject> resultItem, IExternalSearchRequest request, DnBExternalSearchJobData jobData)
    {
        var query = request.Queries.FirstOrDefault(x => x.Id == resultItem.QueryId) ?? request.Queries.FirstOrDefault();
        var code = this.GetOriginEntityCode(resultItem, request, query);
        metadata.EntityType = request.EntityMetaData.EntityType;
        //TODO: add Name
        metadata.Name = request.EntityMetaData.Name;
        metadata.OriginEntityCode = code;
        metadata.Codes.Add(request.EntityMetaData.OriginEntityCode);

        var serializer = JsonSerializer.Create(new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        });

        var dnbResult = resultItem.Data?.ToObject<DNBResponse>(serializer);
        var matchCandidateConfidenceCode = resultItem.Data?.SelectToken("matchConfidenceCode")?.Value<long?>();

        if (jobData.IncludeLastApiCallDetails)
        {
            var lastApiCallStatusCode = resultItem.Data?.SelectToken("lastApiCallStatusCode")?.ToString();
            var lastApiCallErrorMessage = resultItem.Data?.SelectToken("lastApiCallErrorMessage")?.ToString();
            var lastApiCallTimestamp = resultItem.Data?.SelectToken("lastApiCallTimestamp")?.ToString();

            metadata.Properties[StaticDnBVocabulary.BusinessPartner.LastApiCallStatusCode] = lastApiCallStatusCode;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.LastApiCallErrorMessage] = lastApiCallErrorMessage;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.LastApiCallTimestamp] = lastApiCallTimestamp;
        }

        if (dnbResult?.organization == null)
        {
            return;
        }

        PopulatePrimaryAddresses(metadata, dnbResult);

        PopulateOrganizationInfo(metadata, dnbResult, jobData);

        PopulateIndustryCodes(metadata, dnbResult, jobData);

        PopulateManualMappedProperties(context, metadata, resultItem.Data, jobData);

        PopulateConfidenceScore(metadata, dnbResult, matchCandidateConfidenceCode);
    }

    public IEnumerable<EntityType> Accepts(IDictionary<string, object> config, IProvider provider) => Accepts(config);

    private IEnumerable<EntityType> Accepts(IDictionary<string, object> config)
    {
        if (config.TryGetValue(DnBConstants.KeyName.AcceptedEntityType, out var acceptedEntityTypeObj) && acceptedEntityTypeObj is string acceptedEntityType && !string.IsNullOrWhiteSpace(acceptedEntityType))
        {
            // If configured, only accept the configured entity types
            return new EntityType[] { acceptedEntityType };
        }

        // Fallback to default accepted entity types
        return DefaultAcceptedEntityTypes;
    }

    private bool Accepts(IDictionary<string, object> config, EntityType entityTypeToEvaluate)
    {
        var configurableAcceptedEntityTypes = this.Accepts(config).ToArray();

        return configurableAcceptedEntityTypes.Any(entityTypeToEvaluate.Is);
    }

    public IEnumerable<IExternalSearchQuery> BuildQueries(ExecutionContext context, IExternalSearchRequest request, IDictionary<string, object> config,
        IProvider provider)
    {
        return InternalBuildQueries(context, request, config);
    }

    public IEnumerable<IExternalSearchQueryResult> ExecuteSearch(ExecutionContext context, IExternalSearchQuery query, IDictionary<string, object> config, IProvider provider)
    {
        var jobData = new DnBExternalSearchJobData(config);

        return ActionExtensions.ExecuteWithRetry(
            () => InternalExecuteSearch(context, query, jobData).ToArray(),
            retryCount: 1000,
            isTransient: ex => ex.IsTransient() || ex.ToString().Contains("TooManyRequests")
        );
    }

    public IEnumerable<Clue> BuildClues(ExecutionContext context, IExternalSearchQuery query, IExternalSearchQueryResult result, IExternalSearchRequest request, IDictionary<string, object> config, IProvider provider)
    {
        var resultItem = result.As<JObject>();
        var code = this.GetOriginEntityCode(resultItem, request, query);
        var clue = new Clue(code, context.Organization);
        var jobData = new DnBExternalSearchJobData(config);

        this.PopulateMetadata(context, clue.Data.EntityData, resultItem, request, jobData);

        yield return clue;
    }

    private void PopulatePrimaryAddresses(IEntityMetadata metadata, DNBResponse dnbResult)
    {
        var domesticUltimateDuns = dnbResult.organization?.corporateLinkage?.domesticUltimate?.duns;
        var globalUltimateDuns = dnbResult.organization?.corporateLinkage?.globalUltimate?.duns;
        var parentDuns = dnbResult.organization?.corporateLinkage?.parent?.duns;
        var headQuarterDuns = dnbResult.organization?.corporateLinkage?.headQuarter?.duns;
        var duns = dnbResult.organization?.duns;

        if (dnbResult.organization?.corporateLinkage?.domesticUltimate?.primaryAddress != null && !string.IsNullOrWhiteSpace(domesticUltimateDuns) && domesticUltimateDuns != duns)
        {
            var domesticUltimatePrimaryAddress = dnbResult.organization.corporateLinkage.domesticUltimate.primaryAddress;

            metadata.Properties[StaticDnBVocabulary.BusinessPartner.DomesticUltimatePrimaryAddressCountry] = domesticUltimatePrimaryAddress.addressCountry?.name;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.DomesticUltimateISO2CountryCode] = domesticUltimatePrimaryAddress.addressCountry?.isoAlpha2Code;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.DomesticUltimatePrimaryAddressCountyName] = domesticUltimatePrimaryAddress.addressCounty?.name;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.DomesticUltimatePrimaryAddressLocality] = domesticUltimatePrimaryAddress.addressLocality?.name;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.DomesticUltimatePrimaryAddressPostalCode] = domesticUltimatePrimaryAddress.postalCode;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.DomesticUltimatePrimaryAddressRegionName] = domesticUltimatePrimaryAddress.addressRegion?.name;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.DomesticUltimatePrimaryAddressRegionAbbreviatedName] = domesticUltimatePrimaryAddress.addressRegion?.abbreviatedName;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.DomesticUltimatePrimaryAddressStreetLine1] = domesticUltimatePrimaryAddress.streetAddress?.line1;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.DomesticUltimatePrimaryAddressStreetLine2] = domesticUltimatePrimaryAddress.streetAddress?.line2.PrintIfAvailable();
        }

        if (dnbResult.organization?.corporateLinkage?.globalUltimate?.primaryAddress != null && !string.IsNullOrWhiteSpace(globalUltimateDuns) && globalUltimateDuns != duns)
        {
            var globalUltimatePrimaryAddress = dnbResult.organization.corporateLinkage.globalUltimate.primaryAddress;

            metadata.Properties[StaticDnBVocabulary.BusinessPartner.GlobalUltimatePrimaryAddressCountry] = globalUltimatePrimaryAddress.addressCountry?.name;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.GlobalUltimateISO2CountryCode] = globalUltimatePrimaryAddress.addressCountry?.isoAlpha2Code;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.GlobalUltimatePrimaryAddressCountyName] = globalUltimatePrimaryAddress.addressCounty?.name;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.GlobalUltimatePrimaryAddressLocality] = globalUltimatePrimaryAddress.addressLocality?.name;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.GlobalUltimatePrimaryAddressPostalCode] = globalUltimatePrimaryAddress.postalCode;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.GlobalUltimatePrimaryAddressRegionName] = globalUltimatePrimaryAddress.addressRegion?.name;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.GlobalUltimatePrimaryAddressRegionAbbreviatedName] = globalUltimatePrimaryAddress.addressRegion?.abbreviatedName;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.GlobalUltimatePrimaryAddressStreetLine1] = globalUltimatePrimaryAddress.streetAddress?.line1;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.GlobalUltimatePrimaryAddressStreetLine2] = globalUltimatePrimaryAddress.streetAddress?.line2.PrintIfAvailable();
        }

        if (dnbResult.organization?.corporateLinkage?.parent?.primaryAddress != null && !string.IsNullOrWhiteSpace(parentDuns) && parentDuns != duns)
        {
            var parentPrimaryAddress = dnbResult.organization.corporateLinkage.parent.primaryAddress;

            metadata.Properties[StaticDnBVocabulary.BusinessPartner.ParentPrimaryAddressCountry] = parentPrimaryAddress.addressCountry?.name;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.ParentISO2CountryCode] = parentPrimaryAddress.addressCountry?.isoAlpha2Code;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.ParentPrimaryAddressCountyName] = parentPrimaryAddress.addressCounty?.name;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.ParentPrimaryAddressLocality] = parentPrimaryAddress.addressLocality?.name;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.ParentPrimaryAddressPostalCode] = parentPrimaryAddress.postalCode;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.ParentPrimaryAddressRegionName] = parentPrimaryAddress.addressRegion?.name;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.ParentPrimaryAddressRegionAbbreviatedName] = parentPrimaryAddress.addressRegion?.abbreviatedName;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.ParentPrimaryAddressStreetLine1] = parentPrimaryAddress.streetAddress?.line1;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.ParentPrimaryAddressStreetLine2] = parentPrimaryAddress.streetAddress?.line2.PrintIfAvailable();
        }

        if (dnbResult.organization?.corporateLinkage?.headQuarter?.primaryAddress != null && !string.IsNullOrWhiteSpace(headQuarterDuns) && headQuarterDuns != duns)
        {
            var headQuarterPrimaryAddress = dnbResult.organization.corporateLinkage.headQuarter.primaryAddress;

            metadata.Properties[StaticDnBVocabulary.BusinessPartner.HeadQuarterPrimaryAddressCountry] = headQuarterPrimaryAddress.addressCountry?.name;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.HeadQuarterISO2CountryCode] = headQuarterPrimaryAddress.addressCountry?.isoAlpha2Code;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.HeadQuarterPrimaryAddressCountyName] = headQuarterPrimaryAddress.addressCounty?.name;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.HeadQuarterPrimaryAddressLocality] = headQuarterPrimaryAddress.addressLocality?.name;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.HeadQuarterPrimaryAddressPostalCode] = headQuarterPrimaryAddress.postalCode;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.HeadQuarterPrimaryAddressRegionName] = headQuarterPrimaryAddress.addressRegion?.name;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.HeadQuarterPrimaryAddressRegionAbbreviatedName] = headQuarterPrimaryAddress.addressRegion?.abbreviatedName;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.HeadQuarterPrimaryAddressStreetLine1] = headQuarterPrimaryAddress.streetAddress?.line1;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.HeadQuarterPrimaryAddressStreetLine2] = headQuarterPrimaryAddress.streetAddress?.line2.PrintIfAvailable();
        }

        if (dnbResult.organization?.primaryAddress == null) return;

        var primaryAddress = dnbResult.organization.primaryAddress;
        metadata.Properties[StaticDnBVocabulary.BusinessPartner.PrimaryAddressCountry] = primaryAddress.addressCountry?.name;
        metadata.Properties[StaticDnBVocabulary.BusinessPartner.ISO2CountryCode] = primaryAddress.addressCountry?.isoAlpha2Code;
        metadata.Properties[StaticDnBVocabulary.BusinessPartner.PrimaryAddressCountyName] = primaryAddress.addressCounty?.name;
        metadata.Properties[StaticDnBVocabulary.BusinessPartner.PrimaryAddressLocality] = primaryAddress.addressLocality?.name;
        metadata.Properties[StaticDnBVocabulary.BusinessPartner.PrimaryAddressPostalCode] = primaryAddress.postalCode;
        metadata.Properties[StaticDnBVocabulary.BusinessPartner.PrimaryAddressRegionName] = primaryAddress.addressRegion?.name;
        metadata.Properties[StaticDnBVocabulary.BusinessPartner.PrimaryAddressRegionAbbreviatedName] = primaryAddress.addressRegion?.abbreviatedName;
        metadata.Properties[StaticDnBVocabulary.BusinessPartner.PrimaryAddressStreetLine1] = primaryAddress.streetAddress?.line1;
        metadata.Properties[StaticDnBVocabulary.BusinessPartner.PrimaryAddressStreetLine2] = primaryAddress.streetAddress?.line2.PrintIfAvailable();
    }

    private static void PopulateOrganizationInfo(IEntityMetadata metadata, DNBResponse dnbResult, DnBExternalSearchJobData jobData)
    {
        if (dnbResult.organization?.dunsControlStatus != null)
        {
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.DunsControlStatusFullReportDate] = dnbResult.organization.dunsControlStatus?.fullReportDate;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.DunsControlStatusLastUpdateDate] = dnbResult.organization.dunsControlStatus?.lastUpdateDate;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.DunsControlStatusOperatingStatusDescription] = dnbResult.organization.dunsControlStatus?.operatingStatus?.description;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.DunsControlStatusOperatingStatusDnbCode] = dnbResult.organization.dunsControlStatus?.operatingStatus?.dnbCode.PrintIfAvailable();

            metadata.Properties[StaticDnBVocabulary.BusinessPartner.DunsControlStatusIsMarketable] = dnbResult.organization.dunsControlStatus?.isMarketable.PrintIfAvailable();
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.DunsControlStatusIsMailUndeliverable] = dnbResult.organization.dunsControlStatus?.isMailUndeliverable.PrintIfAvailable();
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.DunsControlStatusIsTelephoneDisconnected] = dnbResult.organization.dunsControlStatus?.isTelephoneDisconnected.PrintIfAvailable();
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.DunsControlStatusIsDelisted] = dnbResult.organization.dunsControlStatus?.isDelisted.PrintIfAvailable();
            //metadata.Properties[StaticDnBVocabulary.BusinessPartner.DunsControlStatusSubjectHandlingDetails] = resultItem.Data.organization.dunsControlStatus.subjectHandlingDetails.PrintIfAvailable();
            if (dnbResult.organization?.dunsControlStatus?.operatingStatus != null)
            {
                // Operating Status
                metadata.Properties[StaticDnBVocabulary.BusinessPartner.OperatingStatusCode] = dnbResult.organization.dunsControlStatus.operatingStatus?.dnbCode.PrintIfAvailable();
                metadata.Properties[StaticDnBVocabulary.BusinessPartner.OperatingStatusDescription] = dnbResult.organization.dunsControlStatus.operatingStatus?.description.PrintIfAvailable();
            }
        }

        // DUNS Numbers
        metadata.Properties[StaticDnBVocabulary.BusinessPartner.Duns] = dnbResult.organization?.duns;
        metadata.Properties[StaticDnBVocabulary.BusinessPartner.DomesticUltimateDuns] = dnbResult.organization?.corporateLinkage?.domesticUltimate?.duns;
        metadata.Properties[StaticDnBVocabulary.BusinessPartner.GlobalUltimateDuns] = dnbResult.organization?.corporateLinkage?.globalUltimate?.duns;
        metadata.Properties[StaticDnBVocabulary.BusinessPartner.ParentDuns] = dnbResult.organization?.corporateLinkage?.parent?.duns;
        metadata.Properties[StaticDnBVocabulary.BusinessPartner.HeadQuarterDuns] = dnbResult.organization?.corporateLinkage?.headQuarter?.duns;

        //dnbResult.organization.telephone

        // Business Information
        metadata.Properties[StaticDnBVocabulary.BusinessPartner.PrimaryBusinessName] = dnbResult.organization?.primaryName;
        metadata.Properties[StaticDnBVocabulary.BusinessPartner.BusinessEntityTypeDnbCode] = dnbResult.organization?.businessEntityType?.dnbCode.PrintIfAvailable();
        metadata.Properties[StaticDnBVocabulary.BusinessPartner.BusinessEntityTypeDescription] = dnbResult.organization?.businessEntityType?.description;

        // Trade Style Names
        var tradeStyleNameValues = dnbResult.organization?.tradeStyleNames?.Select(t => t.name).Where(n => !string.IsNullOrEmpty(n));
        var tradeStyleNames = tradeStyleNameValues?.ToList();
        if (tradeStyleNameValues != null && tradeStyleNames.Any())
        {
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.TradeStyleNames] = string.Join(" | ", tradeStyleNames);
        }

        // WebsiteAddress
        var website = dnbResult.organization?.websiteAddress?.FirstOrDefault();
        if (website != null)
        {
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.WebsiteUrl] = website.url;
        }

        // Telephone
        var telephone = dnbResult.organization?.telephone?.FirstOrDefault();
        if (telephone != null && !string.IsNullOrEmpty(telephone.isdCode) && !string.IsNullOrEmpty(telephone.telephoneNumber))
        {
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.Telephone] = $"+{telephone.isdCode} {telephone.telephoneNumber}";
        }

        // Fax
        var fax = dnbResult.organization?.fax?.FirstOrDefault();
        if (fax != null && !string.IsNullOrEmpty(fax.isdCode) && !string.IsNullOrEmpty(fax.faxNumber))
        {
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.Fax] = $"+{fax.isdCode} {fax.faxNumber}";
        }

        // Stock Exchanges
        var primaryStockExchange = dnbResult.organization?.stockExchanges?.FirstOrDefault(x => x.isPrimary == true);
        if (primaryStockExchange != null)
        {
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.StockExchangeTickerName] = primaryStockExchange.tickerName;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.StockExchangeName] = primaryStockExchange.exchangeName?.description;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.StockExchangeCountryCode] = primaryStockExchange.exchangeCountry?.isoAlpha2Code;
        }

        // Registration Numbers
        var selectedRegistrationNumberTypes = !string.IsNullOrWhiteSpace(jobData.RegistrationNumbersKey) ? jobData.RegistrationNumbersKey.Split(",") : Array.Empty<string>();

        if (selectedRegistrationNumberTypes.Any())
        {
            var registrationNumbers = dnbResult.organization?.registrationNumbers?.Where(x => x.typeDnBCode > 0 && selectedRegistrationNumberTypes.Contains(x.typeDnBCode.ToString()));
            var registrationNumbersIndex = 0;
            foreach (var registrationNumber in registrationNumbers ?? Enumerable.Empty<RegistrationNumber>())
            {
                metadata.Properties[$"{StaticDnBVocabulary.RegistrationNumbers.KeyPrefix}{StaticDnBVocabulary.RegistrationNumbers.KeySeparator}{registrationNumbersIndex}.registrationNumber"] = registrationNumber.registrationNumber;
                metadata.Properties[$"{StaticDnBVocabulary.RegistrationNumbers.KeyPrefix}{StaticDnBVocabulary.RegistrationNumbers.KeySeparator}{registrationNumbersIndex}.typeDescription"] = registrationNumber.typeDescription;
                metadata.Properties[$"{StaticDnBVocabulary.RegistrationNumbers.KeyPrefix}{StaticDnBVocabulary.RegistrationNumbers.KeySeparator}{registrationNumbersIndex}.typeDnBCode"] = registrationNumber.typeDnBCode.PrintIfAvailable();
                metadata.Properties[$"{StaticDnBVocabulary.RegistrationNumbers.KeyPrefix}{StaticDnBVocabulary.RegistrationNumbers.KeySeparator}{registrationNumbersIndex}.registrationNumberClass.description"] = registrationNumber.registrationNumberClass?.description;
                metadata.Properties[$"{StaticDnBVocabulary.RegistrationNumbers.KeyPrefix}{StaticDnBVocabulary.RegistrationNumbers.KeySeparator}{registrationNumbersIndex}.registrationNumberClass.dnbCode"] = registrationNumber.registrationNumberClass?.dnbCode.PrintIfAvailable();

                registrationNumbersIndex++;
            }
        }

        // Corporate Linkage
        // Family Tree Roles Played
        var familyTreeRolesPlayedIndex = 0;
        foreach (var role in dnbResult.organization?.corporateLinkage?.familytreeRolesPlayed ?? Enumerable.Empty<FamilytreeRolesPlayed>())
        {
            metadata.Properties[$"{StaticDnBVocabulary.CorporateLinkageFamilyTreeRolesPlayedVocabulary.KeyPrefix}{StaticDnBVocabulary.CorporateLinkageFamilyTreeRolesPlayedVocabulary.KeySeparator}{familyTreeRolesPlayedIndex}.description"] = role.description;
            metadata.Properties[$"{StaticDnBVocabulary.CorporateLinkageFamilyTreeRolesPlayedVocabulary.KeyPrefix}{StaticDnBVocabulary.CorporateLinkageFamilyTreeRolesPlayedVocabulary.KeySeparator}{familyTreeRolesPlayedIndex}.dnbCode"] = role.dnbCode.PrintIfAvailable();

            familyTreeRolesPlayedIndex++;
        }

        metadata.Properties[StaticDnBVocabulary.BusinessPartner.HierarchyLevel] = dnbResult.organization?.corporateLinkage?.hierarchyLevel.PrintIfAvailable();
        metadata.Properties[StaticDnBVocabulary.BusinessPartner.GlobalUltimateFamilyTreeMembersCount] = dnbResult.organization?.corporateLinkage?.globalUltimateFamilyTreeMembersCount.PrintIfAvailable();

        // Number of Employees
        metadata.Properties[StaticDnBVocabulary.BusinessPartner.NumberOfEmployees] = dnbResult.organization?.numberOfEmployees?.FirstOrDefault()?.value.PrintIfAvailable();
        metadata.Properties[StaticDnBVocabulary.BusinessPartner.GlobalUltimateNumberOfEmployees] = dnbResult.organization?.corporateLinkage?.globalUltimate?.numberOfEmployees?.FirstOrDefault()?.value.PrintIfAvailable();
        metadata.Properties[StaticDnBVocabulary.BusinessPartner.DomesticUltimateNumberOfEmployees] = dnbResult.organization?.corporateLinkage?.domesticUltimate?.numberOfEmployees?.FirstOrDefault()?.value.PrintIfAvailable();

        // Yearly Revenue
        var orgFinancial = dnbResult.organization?.financials?.FirstOrDefault();
        var orgYearlyRevenue = orgFinancial?.yearlyRevenue?.FirstOrDefault();
        metadata.Properties[StaticDnBVocabulary.BusinessPartner.YearlyRevenue] = orgYearlyRevenue != null && !string.IsNullOrEmpty(orgYearlyRevenue.currency) ? $"{orgYearlyRevenue.value} {orgYearlyRevenue.currency}" : null;

        var globalUltimateFinancial = dnbResult.organization?.globalUltimate?.financials?.FirstOrDefault();
        var globalUltimateYearlyRevenue = globalUltimateFinancial?.yearlyRevenue?.FirstOrDefault();
        metadata.Properties[StaticDnBVocabulary.BusinessPartner.GlobalUltimateYearlyRevenue] = globalUltimateYearlyRevenue != null && !string.IsNullOrEmpty(globalUltimateYearlyRevenue.currency) ? $"{globalUltimateYearlyRevenue.value} {globalUltimateYearlyRevenue.currency}" : null;

        var domesticUltimateFinancial = dnbResult.organization?.domesticUltimate?.financials?.FirstOrDefault();
        var domesticUltimateYearlyRevenue = domesticUltimateFinancial?.yearlyRevenue?.FirstOrDefault();
        metadata.Properties[StaticDnBVocabulary.BusinessPartner.DomesticUltimateYearlyRevenue] = domesticUltimateYearlyRevenue != null && !string.IsNullOrEmpty(domesticUltimateYearlyRevenue.currency) ? $"{domesticUltimateYearlyRevenue.value} {domesticUltimateYearlyRevenue.currency}" : null;
    }

    private static void PopulateIndustryCodes(IEntityMetadata metadata, DNBResponse dnbResult, DnBExternalSearchJobData jobData)
    {
        var selectedTypes = !string.IsNullOrWhiteSpace(jobData.IndustryCodesKey) ? jobData.IndustryCodesKey.Split(",") : Array.Empty<string>();

        if (!selectedTypes.Any()) return;

        var industryCodeValues = dnbResult.organization?.industryCodes?.Where(x => x.typeDnBCode > 0 && selectedTypes.Contains(x.typeDnBCode.ToString()));

        var industryCodeList = industryCodeValues?.ToList();
        if (industryCodeValues == null || !industryCodeList.Any()) return;

        var industryCodesIndex = 0;
        foreach (var industryCode in industryCodeList)
        {
            metadata.Properties[$"{StaticDnBVocabulary.Industry.KeyPrefix}{StaticDnBVocabulary.Industry.KeySeparator}{industryCodesIndex}.description"] = industryCode.description;
            metadata.Properties[$"{StaticDnBVocabulary.Industry.KeyPrefix}{StaticDnBVocabulary.Industry.KeySeparator}{industryCodesIndex}.typeDescription"] = industryCode.typeDescription;
            metadata.Properties[$"{StaticDnBVocabulary.Industry.KeyPrefix}{StaticDnBVocabulary.Industry.KeySeparator}{industryCodesIndex}.typeDnBCode"] = industryCode.typeDnBCode.PrintIfAvailable();
            metadata.Properties[$"{StaticDnBVocabulary.Industry.KeyPrefix}{StaticDnBVocabulary.Industry.KeySeparator}{industryCodesIndex}.priority"] = industryCode.priority.PrintIfAvailable();
            metadata.Properties[$"{StaticDnBVocabulary.Industry.KeyPrefix}{StaticDnBVocabulary.Industry.KeySeparator}{industryCodesIndex}.code"] = industryCode.code;

            industryCodesIndex++;
        }
    }

    private static void PopulateManualMappedProperties(ExecutionContext context, IEntityMetadata metadata, JObject dnbResult, DnBExternalSearchJobData jobData)
    {
        var propertyMappings = !string.IsNullOrWhiteSpace(jobData.PropertyMappings) ? jobData.PropertyMappings.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries) : Array.Empty<string>();

        foreach(var mapping in propertyMappings)
        {
            try
            {
                var mappingParts = mapping.Split(new[] { '=' }, 2); // maximum 2 parts, in case the token path contains '=' character
                if (mappingParts.Length != 2) continue;
                var propertyName = mappingParts[0].Trim();
                var dnbTokenPath = mappingParts[1].Trim();
                var tokenValue = dnbTokenPath.Contains("[*]")
                    ? string.Join(", ", dnbResult.SelectTokens(dnbTokenPath).Select(x => x.ToString()))
                    : dnbResult.SelectTokens(dnbTokenPath).FirstOrDefault()?.ToString();

                if (!string.IsNullOrEmpty(tokenValue))
                {
                    metadata.Properties[propertyName] = tokenValue;
                }
            } catch (Exception)
            {
                context.Log.LogWarning("Invalid mapping: {Mapping}", mapping);
            }
        }
    }

    private static void PopulateConfidenceScore(IEntityMetadata metadata, DNBResponse dnbResult, long? matchCandidateConfidenceCode)
    {
        matchCandidateConfidenceCode ??= dnbResult?.matchCandidates?.FirstOrDefault()?.matchQualityInformation?.confidenceCode;

        var confidenceScore = 100;

        // Match candidate confidence code ranges from 1 (low) to 10 (high).
        // If the value is null or 0, the data is considered to come from the SearchByDUNS API,
        // and the confidence score is set to 100.
        if (matchCandidateConfidenceCode != null && matchCandidateConfidenceCode != 0)
        {
            confidenceScore = (int)matchCandidateConfidenceCode * 10;
        }

        metadata.Properties[StaticDnBVocabulary.BusinessPartner.ConfidenceScore] = confidenceScore.PrintIfAvailable();
    }

    public IEntityMetadata GetPrimaryEntityMetadata(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request, IDictionary<string, object> config, IProvider provider)
    {
        var resultItem = result.As<JObject>();
        var jobData = new DnBExternalSearchJobData(config);
        return this.CreateMetadata(context, resultItem, request, jobData);
    }

    public IPreviewImage GetPrimaryEntityPreviewImage(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request, IDictionary<string, object> config, IProvider provider)
    {
        return null;
    }

    public ConnectionVerificationResult VerifyConnection(ExecutionContext context, IReadOnlyDictionary<string, object> config)
    {
        var configDict = new Dictionary<string, object>(config);
        var jobData = new DnBExternalSearchJobData(configDict);

        try
        {
            const string dummyDunsNumber = "515042588"; // Pfizer Duns number
            const string dummyOrgName = "Pfizer";
            const string dummyOrgCountryCode = "US";
            var providerDefinitionGuid = new Guid("dc866ac5-89fa-49c9-9eb8-398c3872b8e6");

            var token = GetAuthToken(context, jobData, providerDefinitionGuid, false).GetAwaiter().GetResult();

            var client = new RestClient(jobData.DnBBaseUrl);

            const string dunsRequestResource = $"data/duns/{dummyDunsNumber}";
            var dunsRequest = new RestRequest(dunsRequestResource, Method.Get);
            dunsRequest.AddHeader("Authorization", $"Bearer {token}");

            if (!string.IsNullOrWhiteSpace(jobData.BlockIds))
            {
                dunsRequest.AddQueryParameter("blockIDs", jobData.BlockIds);
            }

            var cleanseDunsResponse = client.ExecuteAsync(dunsRequest).GetAwaiter().GetResult();

            if (cleanseDunsResponse.StatusCode != HttpStatusCode.OK)
            {
                var data = JsonUtility.Deserialize<DNBResponse>(cleanseDunsResponse.Content, new JsonSerializer { NullValueHandling = NullValueHandling.Ignore });

                return ConstructFailedConnectionResponse(cleanseDunsResponse, data);
            }

            const string requestResource = "match/extendedMatch";
            var extendedMatchRequest = new RestRequest(requestResource, Method.Get);
            if (!string.IsNullOrWhiteSpace(jobData.VersionId) && !string.IsNullOrWhiteSpace(jobData.ProductId))
            {
                extendedMatchRequest.AddQueryParameter("versionId", jobData.VersionId);
                extendedMatchRequest.AddQueryParameter("productId", jobData.ProductId);
            }
            else if (!string.IsNullOrWhiteSpace(jobData.BlockIds))
            {
                extendedMatchRequest.AddQueryParameter("blockIDs", jobData.BlockIds);
            }
            else
            {
                return new ConnectionVerificationResult(false,
                    "Could not execute external search query - Either productId & versionId or blockIDs must be specified.");
            }

            extendedMatchRequest.AddQueryParameter("name", dummyOrgName);
            extendedMatchRequest.AddQueryParameter("countryISOAlpha2Code", dummyOrgCountryCode);
            extendedMatchRequest.AddHeader("Authorization", $"Bearer {token}");

            var cleanseExtendedMatchResponse = client.ExecuteAsync(extendedMatchRequest).GetAwaiter().GetResult();

            if (cleanseExtendedMatchResponse.StatusCode != HttpStatusCode.OK)
            {
                var data = JsonUtility.Deserialize<DNBResponse>(cleanseExtendedMatchResponse.Content, new JsonSerializer { NullValueHandling = NullValueHandling.Ignore });

                return ConstructFailedConnectionResponse(cleanseExtendedMatchResponse, data);
            }
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains(DnBConstants.ErrorMessages.TooManyRequests))
            {
                return new ConnectionVerificationResult(false, $"{DnBConstants.ProviderName} returned {HttpStatusCode.TooManyRequests} {ex.Message}.");
            }

            return ex.Message.Contains(DnBConstants.ErrorMessages.AccessTokenExpired) ? new ConnectionVerificationResult(false, $"{DnBConstants.ProviderName} returned {HttpStatusCode.Unauthorized} {ex.Message}.") : new ConnectionVerificationResult(false, ex.Message);
        }

        return new ConnectionVerificationResult(true);
    }

    private static ConnectionVerificationResult ConstructFailedConnectionResponse(RestResponse response, DNBResponse data)
    {
        var errorMessageBase = $"{DnBConstants.ProviderName} returned \"{(int)response.StatusCode} {response.StatusDescription}\".";

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return new ConnectionVerificationResult(
                false,
                $"{errorMessageBase} This could be due to an invalid API key or API Secret."
            );
        }

        if (!string.IsNullOrWhiteSpace(data?.error?.errorCode) && !string.IsNullOrWhiteSpace(data.error?.errorMessage))
        {
            return new ConnectionVerificationResult(
                false,
                $"{errorMessageBase} {data.error.errorCode} {data.error.errorMessage}"
            );
        }

        if (response.ErrorException != null)
        {
            return new ConnectionVerificationResult(
                false,
                $"{errorMessageBase} {(!string.IsNullOrWhiteSpace(response.ErrorException.Message) ? response.ErrorException.Message : "This could be due to breaking changes in the external system")}."
            );
        }

        return new ConnectionVerificationResult(false, "This could be due to breaking changes in the external system");
    }
}