using CluedIn.Core;
using CluedIn.Core.Connectors;
using CluedIn.Core.Data;
using CluedIn.Core.Data.Parts;
using CluedIn.Core.Data.Relational;
using CluedIn.Core.Data.Vocabularies;
using CluedIn.Core.ExternalSearch;
using CluedIn.Core.Providers;
using CluedIn.Crawling.Helpers;
using CluedIn.ExternalSearch.Providers.DnB.Model.AuthResponse;
using CluedIn.ExternalSearch.Providers.DnB.Model.DnBResponse;
using CluedIn.ExternalSearch.Providers.DnB.Vocabularies;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using EntityType = CluedIn.Core.Data.EntityType;

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

    private static readonly EntityType[] DefaultAcceptedEntityTypes = { EntityType.Organization };
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

    public override IEntityMetadata GetPrimaryEntityMetadata(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request) =>  GetPrimaryEntityMetadata(context, result, request, null, null);

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

        var orgName        = GetValue(request, config, DnBConstants.KeyName.OrgNameKey,        Core.Data.Vocabularies.Vocabularies.CluedInOrganization.OrganizationName)  ?.FirstOrDefault();
        var orgCountryCode = GetValue(request, config, DnBConstants.KeyName.OrgCountryCodeKey, Core.Data.Vocabularies.Vocabularies.CluedInOrganization.AddressCountryCode)?.FirstOrDefault();

        var orgNameAndCountry     = !string.IsNullOrWhiteSpace(orgName) && !string.IsNullOrWhiteSpace(orgCountryCode);
        var versionIdAndProductId = !string.IsNullOrWhiteSpace(jobData.VersionId) && !string.IsNullOrWhiteSpace(jobData.ProductId);
        var blockIds              = !string.IsNullOrWhiteSpace(jobData.BlockIds);

        if (orgNameAndCountry && (versionIdAndProductId || blockIds))
        {
            yield return new ExternalSearchQuery(this, entityType, new Dictionary<string, string>() { { DnBConstants.KeyName.OrgNameKey, orgName }, { DnBConstants.KeyName.OrgCountryCodeKey, orgCountryCode } } );
        }

    }

    private static IEnumerable<IExternalSearchQueryResult> InternalExecuteSearch(IExternalSearchQuery query, DnBExternalSearchJobData jobData)
    {
        //TODO: replace hardcoded value
        var token = GetAuthToken(jobData);
        var dunsNumber      = query.QueryParameters.GetValue("id")?.FirstOrDefault();
        var orgName         = query.QueryParameters.GetValue(DnBConstants.KeyName.OrgNameKey)?.FirstOrDefault();
        var orgCountryCode  = query.QueryParameters.GetValue(DnBConstants.KeyName.OrgCountryCodeKey)?.FirstOrDefault();

        var client = new RestClient(jobData.DnBBaseUrl);

        RestRequest request;
        if (!string.IsNullOrEmpty(dunsNumber))
        {
            var requestResource = $"data/duns/{dunsNumber}";
            request = new RestRequest(requestResource, Method.GET);
        }
        else if (!string.IsNullOrWhiteSpace(orgName) && !string.IsNullOrWhiteSpace(orgCountryCode))
        {
            const string requestResource = "match/extendedMatch";
            request = new RestRequest(requestResource, Method.GET);
            request.AddQueryParameter("name", orgName);
            request.AddQueryParameter("countryISOAlpha2Code", orgCountryCode);
        }
        else
        {
            throw new Exception("Could not execute external search query - name and countryISOAlpha2Code must be specified.");
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

        var cleanseResponse = client.ExecuteAsync(request).Result;

        if (cleanseResponse.StatusCode == HttpStatusCode.OK)
        {
            var data = JsonUtility.Deserialize<DNBResponse>(cleanseResponse.Content, new JsonSerializer() { NullValueHandling = NullValueHandling.Ignore });

            if (data == null)
            {
                throw new ApplicationException("Could not execute external search query - DnB returned empty data");
            }

            var organization = data.organization ?? data.embeddedProduct.organization;

            if (organization == null)
            {
                throw new ApplicationException("Could not execute external search query - DnB returned empty organization");
            }

            // Not using industry codes from extended match API
            if (data.embeddedProduct.organization != null)
            {
                if (organization.industryCodes != null && organization.industryCodes.Any())
                {
                    organization.industryCodes.Clear();
                }
            }

            data.organization = organization;
            yield return new ExternalSearchQueryResult<DNBResponse>(query, data);
            yield break;
        }

        if (cleanseResponse.ErrorException != null)
        {
            throw new AggregateException(cleanseResponse.ErrorException.Message, cleanseResponse.ErrorException);
        }

        throw new ApplicationException("Could not execute external search query - StatusCode:" + cleanseResponse.StatusCode + "; Content: " + cleanseResponse.Content);
    }

    private static string GetAuthToken(DnBExternalSearchJobData jobData)
    {
        var key = jobData.AuthKey;
        var secret = jobData.AuthSecret;
        byte[] bytes = Encoding.ASCII.GetBytes($"{key}:{secret}");

        var restClient = new RestClient(jobData.AuthUrl)
        {
            Timeout = -1
        };
        var request = new RestRequest(Method.POST);
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Authorization", $"Basic {Convert.ToBase64String(bytes)}");
        var body = jobData.AuthRequestBody;
        request.AddParameter("application/json", body, ParameterType.RequestBody);
        IRestResponse response = restClient.Execute(request);
        var responseContent = JsonUtility.Deserialize<AuthResponse>(response.Content);
        return responseContent.AccessToken;
    }

    private IEntityMetadata CreateMetadata(IExternalSearchQueryResult<DNBResponse> resultItem, IExternalSearchRequest request)
    {
        var metadata = new EntityMetadataPart();

        this.PopulateMetadata(metadata, resultItem, request);

        return metadata;
    }

    private EntityCode GetOriginEntityCode(IExternalSearchQueryResult<DNBResponse> resultItem, IExternalSearchRequest request, IExternalSearchQuery query)
    {
        return new EntityCode(request.EntityMetaData.EntityType, this.GetCodeOrigin(), resultItem.Data.organization?.duns ?? $"{query.QueryKey}{request.EntityMetaData.OriginEntityCode}".ToDeterministicGuid().ToString());
    }

    /// <summary>Gets the code origin.</summary>
    /// <returns>The code origin</returns>
    private CodeOrigin GetCodeOrigin()
    {
        return CodeOrigin.CluedIn.CreateSpecific("DnB");
    }

    private void PopulateMetadata(IEntityMetadata metadata, IExternalSearchQueryResult<DNBResponse> resultItem, IExternalSearchRequest request)
    {
        var code = this.GetOriginEntityCode(resultItem, request, request.Queries.FirstOrDefault());
        //var firstMatch = resultItem.Data.matchCandidates[0];
        //metadata.OutgoingEdges.Add();
        metadata.EntityType = request.EntityMetaData.EntityType;
        //TODO: add Name
        metadata.Name = request.EntityMetaData.Name;
        metadata.OriginEntityCode = code;
        metadata.Codes.Add(request.EntityMetaData.OriginEntityCode);

        PopulatePrimaryAddresses(metadata, resultItem);

        PopulateOrganizationInfo(metadata, resultItem);

        PopulateIndustryCodes(metadata, resultItem);

        PopulateConfidenceScore(metadata, resultItem);
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

        foreach (var externalSearchQueryResult in InternalExecuteSearch(query, jobData)) yield return externalSearchQueryResult;
    }

    public IEnumerable<Clue> BuildClues(ExecutionContext context, IExternalSearchQuery query, IExternalSearchQueryResult result, IExternalSearchRequest request, IDictionary<string, object> config, IProvider provider)
    {
        var resultItem = result.As<DNBResponse>();
        var code = this.GetOriginEntityCode(resultItem, request, query);
        var clue = new Clue(code, context.Organization);

        this.PopulateMetadata(clue.Data.EntityData, resultItem, request);

        yield return clue;
    }

    private void PopulatePrimaryAddresses(IEntityMetadata metadata, IExternalSearchQueryResult<DNBResponse> resultItem)
    {
        var domesticUltimateDuns = resultItem.Data.organization?.corporateLinkage?.domesticUltimate?.duns;
        var globalUltimateDuns = resultItem.Data.organization?.corporateLinkage?.globalUltimate?.duns;
        var parentDuns = resultItem.Data.organization?.corporateLinkage?.parent?.duns;
        var headQuarterDuns = resultItem.Data.organization?.corporateLinkage?.headQuarter?.duns;
        var duns = resultItem.Data.organization?.duns;

        if (resultItem.Data.organization?.corporateLinkage?.domesticUltimate?.primaryAddress != null && !string.IsNullOrWhiteSpace(domesticUltimateDuns) && domesticUltimateDuns != duns)
        {
            var domesticUltimatePrimaryAddress = resultItem.Data.organization.corporateLinkage.domesticUltimate.primaryAddress;

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

        if (resultItem.Data.organization?.corporateLinkage?.globalUltimate?.primaryAddress != null && !string.IsNullOrWhiteSpace(globalUltimateDuns) && globalUltimateDuns != duns)
        {
            var globalUltimatePrimaryAddress = resultItem.Data.organization.corporateLinkage.globalUltimate.primaryAddress;

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

        if (resultItem.Data.organization?.corporateLinkage?.parent?.primaryAddress != null && !string.IsNullOrWhiteSpace(parentDuns) && parentDuns != duns)
        {
            var parentPrimaryAddress = resultItem.Data.organization.corporateLinkage.parent.primaryAddress;

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

        if (resultItem.Data.organization?.corporateLinkage?.headQuarter?.primaryAddress != null && !string.IsNullOrWhiteSpace(headQuarterDuns) && headQuarterDuns != duns)
        {
            var headQuarterPrimaryAddress = resultItem.Data.organization.corporateLinkage.headQuarter.primaryAddress;

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

        if (resultItem.Data.organization?.primaryAddress == null) return;

        var primaryAddress = resultItem.Data.organization.primaryAddress;
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

    private static void PopulateOrganizationInfo(IEntityMetadata metadata, IExternalSearchQueryResult<DNBResponse> resultItem)
    {
        if (resultItem.Data.organization?.dunsControlStatus != null)
        {
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.DunsControlStatusFullReportDate] = resultItem.Data.organization.dunsControlStatus?.fullReportDate;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.DunsControlStatusLastUpdateDate] = resultItem.Data.organization.dunsControlStatus?.lastUpdateDate;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.DunsControlStatusOperatingStatusDescription] = resultItem.Data.organization.dunsControlStatus?.operatingStatus?.description;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.DunsControlStatusOperatingStatusDnbCode] = resultItem.Data.organization.dunsControlStatus?.operatingStatus?.dnbCode.PrintIfAvailable();

            metadata.Properties[StaticDnBVocabulary.BusinessPartner.DunsControlStatusIsMarketable] = resultItem.Data.organization.dunsControlStatus?.isMarketable.PrintIfAvailable();
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.DunsControlStatusIsMailUndeliverable] = resultItem.Data.organization.dunsControlStatus?.isMailUndeliverable.PrintIfAvailable();
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.DunsControlStatusIsTelephoneDisconnected] = resultItem.Data.organization.dunsControlStatus?.isTelephoneDisconnected.PrintIfAvailable();
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.DunsControlStatusIsDelisted] = resultItem.Data.organization.dunsControlStatus?.isDelisted.PrintIfAvailable();
            //metadata.Properties[StaticDnBVocabulary.BusinessPartner.DunsControlStatusSubjectHandlingDetails] = resultItem.Data.organization.dunsControlStatus.subjectHandlingDetails.PrintIfAvailable();
            if (resultItem.Data.organization?.dunsControlStatus?.operatingStatus != null)
            {
                // Operating Status
                metadata.Properties[StaticDnBVocabulary.BusinessPartner.OperatingStatusCode] = resultItem.Data.organization.dunsControlStatus.operatingStatus?.dnbCode.PrintIfAvailable();
                metadata.Properties[StaticDnBVocabulary.BusinessPartner.OperatingStatusDescription] = resultItem.Data.organization.dunsControlStatus.operatingStatus?.description.PrintIfAvailable();
            }
        }

        // DUNS Numbers
        metadata.Properties[StaticDnBVocabulary.BusinessPartner.Duns] = resultItem.Data.organization?.duns;
        metadata.Properties[StaticDnBVocabulary.BusinessPartner.DomesticUltimateDuns] = resultItem.Data.organization?.corporateLinkage?.domesticUltimate?.duns;
        metadata.Properties[StaticDnBVocabulary.BusinessPartner.GlobalUltimateDuns] = resultItem.Data.organization?.corporateLinkage?.globalUltimate?.duns;
        metadata.Properties[StaticDnBVocabulary.BusinessPartner.ParentDuns] = resultItem.Data.organization?.corporateLinkage?.parent?.duns;
        metadata.Properties[StaticDnBVocabulary.BusinessPartner.HeadQuarterDuns] = resultItem.Data.organization?.corporateLinkage?.headQuarter?.duns;

        //resultItem.Data.organization.telephone

        // Business Information
        metadata.Properties[StaticDnBVocabulary.BusinessPartner.PrimaryBusinessName] = resultItem.Data.organization?.primaryName;
        metadata.Properties[StaticDnBVocabulary.BusinessPartner.BusinessEntityTypeDnbCode] = resultItem.Data.organization?.businessEntityType?.dnbCode.PrintIfAvailable();
        metadata.Properties[StaticDnBVocabulary.BusinessPartner.BusinessEntityTypeDescription] = resultItem.Data.organization?.businessEntityType?.description;

        // Trade Style Names
        var tradeStyleNameIndex = 0;
        foreach (var tradeStyleName in resultItem.Data.organization?.tradeStyleNames ?? Enumerable.Empty<TradeStyleName>())
        {
            metadata.Properties[$"{StaticDnBVocabulary.TradeStyleNames.KeyPrefix}{StaticDnBVocabulary.TradeStyleNames.KeySeparator}{tradeStyleNameIndex}.name"] = tradeStyleName.name;
            metadata.Properties[$"{StaticDnBVocabulary.TradeStyleNames.KeyPrefix}{StaticDnBVocabulary.TradeStyleNames.KeySeparator}{tradeStyleNameIndex}.priority"] = tradeStyleName.priority.PrintIfAvailable();

            tradeStyleNameIndex++;
        }

        // WebsiteAddress
        var websiteAddressIndex = 0;
        foreach (var websiteAddress in resultItem.Data.organization?.websiteAddress ?? Enumerable.Empty<WebsiteAddress>())
        {
            metadata.Properties[$"{StaticDnBVocabulary.WebsiteAddress.KeyPrefix}{StaticDnBVocabulary.WebsiteAddress.KeySeparator}{websiteAddressIndex}.url"] = websiteAddress.url;
            metadata.Properties[$"{StaticDnBVocabulary.WebsiteAddress.KeyPrefix}{StaticDnBVocabulary.WebsiteAddress.KeySeparator}{websiteAddressIndex}.domainName"] = websiteAddress.domainName;

            websiteAddressIndex++;
        }

        // Telephone
        var telephoneIndex = 0;
        foreach (var telephone in resultItem.Data.organization?.telephone ?? Enumerable.Empty<Telephone>())
        {
            metadata.Properties[$"{StaticDnBVocabulary.Telephone.KeyPrefix}{StaticDnBVocabulary.Telephone.KeySeparator}{telephoneIndex}.telephoneNumber"] = telephone.telephoneNumber;
            metadata.Properties[$"{StaticDnBVocabulary.Telephone.KeyPrefix}{StaticDnBVocabulary.Telephone.KeySeparator}{telephoneIndex}.isdCode"] = telephone.isdCode;

            telephoneIndex++;
        }

        // Fax
        var faxIndex = 0;
        foreach (var fax in resultItem.Data.organization?.fax ?? Enumerable.Empty<Fax>())
        {
            metadata.Properties[$"{StaticDnBVocabulary.Fax.KeyPrefix}{StaticDnBVocabulary.Fax.KeySeparator}{faxIndex}.faxNumber"] = fax.faxNumber;
            metadata.Properties[$"{StaticDnBVocabulary.Fax.KeyPrefix}{StaticDnBVocabulary.Fax.KeySeparator}{faxIndex}.isdCode"] = fax.isdCode;

            faxIndex++;
        }

        // Stock Exchanges
        var stockExchangesIndex = 0;
        foreach (var stockExchange in resultItem.Data.organization?.stockExchanges ?? Enumerable.Empty<StockExchange>())
        {
            metadata.Properties[$"{StaticDnBVocabulary.StockExchanges.KeyPrefix}{StaticDnBVocabulary.StockExchanges.KeySeparator}{stockExchangesIndex}.tickerName"] = stockExchange.tickerName;
            metadata.Properties[$"{StaticDnBVocabulary.StockExchanges.KeyPrefix}{StaticDnBVocabulary.StockExchanges.KeySeparator}{stockExchangesIndex}.exchangeName.description"] = stockExchange.exchangeName?.description;
            metadata.Properties[$"{StaticDnBVocabulary.StockExchanges.KeyPrefix}{StaticDnBVocabulary.StockExchanges.KeySeparator}{stockExchangesIndex}.exchangeCountry.exchangeCountry"] = stockExchange.exchangeCountry?.isoAlpha2Code;

            stockExchangesIndex++;
        }

        // Registration Numbers
        var registrationNumbersIndex = 0;
        foreach (var registrationNumbers in resultItem.Data.organization?.registrationNumbers ?? Enumerable.Empty<RegistrationNumber>())
        {
            metadata.Properties[$"{StaticDnBVocabulary.RegistrationNumbers.KeyPrefix}{StaticDnBVocabulary.RegistrationNumbers.KeySeparator}{registrationNumbersIndex}.registrationNumber"] = registrationNumbers.registrationNumber;
            metadata.Properties[$"{StaticDnBVocabulary.RegistrationNumbers.KeyPrefix}{StaticDnBVocabulary.RegistrationNumbers.KeySeparator}{registrationNumbersIndex}.typeDescription"] = registrationNumbers.typeDescription;
            metadata.Properties[$"{StaticDnBVocabulary.RegistrationNumbers.KeyPrefix}{StaticDnBVocabulary.RegistrationNumbers.KeySeparator}{registrationNumbersIndex}.typeDnBCode"] = registrationNumbers.typeDnBCode.PrintIfAvailable();
            metadata.Properties[$"{StaticDnBVocabulary.RegistrationNumbers.KeyPrefix}{StaticDnBVocabulary.RegistrationNumbers.KeySeparator}{registrationNumbersIndex}.registrationNumberClass.description"] = registrationNumbers.registrationNumberClass?.description;
            metadata.Properties[$"{StaticDnBVocabulary.RegistrationNumbers.KeyPrefix}{StaticDnBVocabulary.RegistrationNumbers.KeySeparator}{registrationNumbersIndex}.registrationNumberClass.dnbCode"] = registrationNumbers.registrationNumberClass?.dnbCode.PrintIfAvailable();

            registrationNumbersIndex++;
        }

        // Corporate Linkage
        // Family Tree Roles Played
        var familyTreeRolesPlayedIndex = 0;
        foreach (var role in resultItem.Data.organization?.corporateLinkage.familytreeRolesPlayed ?? Enumerable.Empty<FamilytreeRolesPlayed>())
        {
            metadata.Properties[$"{StaticDnBVocabulary.CorporateLinkageFamilyTreeRolesPlayedVocabulary.KeyPrefix}{StaticDnBVocabulary.CorporateLinkageFamilyTreeRolesPlayedVocabulary.KeySeparator}{familyTreeRolesPlayedIndex}.description"] = role.description;
            metadata.Properties[$"{StaticDnBVocabulary.CorporateLinkageFamilyTreeRolesPlayedVocabulary.KeyPrefix}{StaticDnBVocabulary.CorporateLinkageFamilyTreeRolesPlayedVocabulary.KeySeparator}{familyTreeRolesPlayedIndex}.dnbCode"] = role.dnbCode.PrintIfAvailable();

            familyTreeRolesPlayedIndex++;
        }

        metadata.Properties[StaticDnBVocabulary.BusinessPartner.HierarchyLevel] = resultItem.Data.organization?.corporateLinkage?.hierarchyLevel.PrintIfAvailable();
        metadata.Properties[StaticDnBVocabulary.BusinessPartner.GlobalUltimateFamilyTreeMembersCount] = resultItem.Data.organization?.corporateLinkage?.globalUltimateFamilyTreeMembersCount.PrintIfAvailable();

        //metadata.Properties[StaticDnBVocabulary.BusinessPartner.WebsiteUrl] = resultItem.Data.organization.websiteAddress.First()..PrintIfAvailable();
    }

    private static void PopulateIndustryCodes(IEntityMetadata metadata, IExternalSearchQueryResult<DNBResponse> resultItem)
    {
        var industryCodesIndex = 0;
        foreach (var industryCode in resultItem.Data.organization?.industryCodes ?? Enumerable.Empty<IndustryCode>())
        {
            metadata.Properties[$"{StaticDnBVocabulary.Industry.KeyPrefix}{StaticDnBVocabulary.Industry.KeySeparator}{industryCodesIndex}.description"] = industryCode.description;
            metadata.Properties[$"{StaticDnBVocabulary.Industry.KeyPrefix}{StaticDnBVocabulary.Industry.KeySeparator}{industryCodesIndex}.typeDescription"] = industryCode.typeDescription;
            metadata.Properties[$"{StaticDnBVocabulary.Industry.KeyPrefix}{StaticDnBVocabulary.Industry.KeySeparator}{industryCodesIndex}.typeDnBCode"] = industryCode.typeDnBCode.PrintIfAvailable();
            metadata.Properties[$"{StaticDnBVocabulary.Industry.KeyPrefix}{StaticDnBVocabulary.Industry.KeySeparator}{industryCodesIndex}.priority"] = industryCode.priority.PrintIfAvailable();
            metadata.Properties[$"{StaticDnBVocabulary.Industry.KeyPrefix}{StaticDnBVocabulary.Industry.KeySeparator}{industryCodesIndex}.code"] = industryCode.code;

            industryCodesIndex++;
        }
    }

    private static void PopulateConfidenceScore(IEntityMetadata metadata, IExternalSearchQueryResult<DNBResponse> resultItem)
    {
        var matchCandidateConfidenceCode = resultItem.Data.matchCandidates.FirstOrDefault()?.matchQualityInformation?.confidenceCode;
        var confidenceScore = 100;

        // Match candidate confidence code ranges from 1 (low) to 10 (high).
        // If the value is null or 0, the data is considered to come from the SearchByDUNS API,
        // and the confidence score is set to 100.
        if (resultItem.Data.matchCandidates.FirstOrDefault() != null && matchCandidateConfidenceCode != null && matchCandidateConfidenceCode != 0)
        {
            confidenceScore = (int)matchCandidateConfidenceCode * 10;
        }

        metadata.Properties[StaticDnBVocabulary.BusinessPartner.ConfidenceScore] = confidenceScore.PrintIfAvailable();
    }

    public IEntityMetadata GetPrimaryEntityMetadata(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request, IDictionary<string, object> config, IProvider provider)
    {
        var resultItem = result.As<DNBResponse>();
        return this.CreateMetadata(resultItem, request);
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
            var token = GetAuthToken(jobData);

            var client = new RestClient(jobData.DnBBaseUrl);

            const string dunsRequestResource = $"data/duns/{dummyDunsNumber}";
            var dunsRequest = new RestRequest(dunsRequestResource, Method.GET);
            dunsRequest.AddHeader("Authorization", $"Bearer {token}");

            if (!string.IsNullOrWhiteSpace(jobData.BlockIds))
            {
                dunsRequest.AddQueryParameter("blockIDs", jobData.BlockIds);
            }

            var cleanseDunsResponse = client.ExecuteAsync(dunsRequest).Result;

            if (cleanseDunsResponse.StatusCode != HttpStatusCode.OK)
            {
                var data = JsonUtility.Deserialize<DNBResponse>(cleanseDunsResponse.Content, new JsonSerializer { NullValueHandling = NullValueHandling.Ignore });

                return ConstructFailedConnectionResponse(cleanseDunsResponse, data);
            }

            const string requestResource = "match/extendedMatch";
            var extendedMatchRequest = new RestRequest(requestResource, Method.GET);
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

            var cleanseExtendedMatchResponse = client.ExecuteAsync(extendedMatchRequest).Result;

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

    private static ConnectionVerificationResult ConstructFailedConnectionResponse(IRestResponse response, DNBResponse data)
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