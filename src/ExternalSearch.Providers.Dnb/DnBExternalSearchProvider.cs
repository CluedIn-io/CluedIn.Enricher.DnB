using CluedIn.Core;
using CluedIn.Core.Data;
using CluedIn.Core.Data.Parts;
using CluedIn.Core.Data.Relational;
using CluedIn.Core.Data.Vocabularies;
using CluedIn.Core.ExternalSearch;
using CluedIn.Core.Providers;
using CluedIn.Crawling.Helpers;
using CluedIn.ExternalSearch.Filters;
using CluedIn.ExternalSearch.Providers.DnB.Model;
using CluedIn.ExternalSearch.Providers.DnB.Models;
using CluedIn.ExternalSearch.Providers.DnB.Vocabularies;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using EntityType = CluedIn.Core.Data.EntityType;

namespace CluedIn.ExternalSearch.Providers.DnB
{
    /// <summary>The dnb graph external search provider.</summary>
    /// <seealso cref="CluedIn.ExternalSearch.ExternalSearchProviderBase" />
    public class DnBExternalSearchProvider : ExternalSearchProviderBase, IExtendedEnricherMetadata, IConfigurableExternalSearchProvider
    {
        public static readonly Guid ProviderId = Guid.Parse("31d78803-3a06-45a7-9ef2-4179b8242fbf");   // TODO: Replace value

        public string Icon => "Resources.dnb.svg";

        public string Domain => "https://www.dnb.com/";

        public string About => "Dun & Bradstreet is global provider of business decisioning data and analytics.";

        public AuthMethods AuthMethods { get; } = null;
        public IEnumerable<Control> Properties { get; } = null;
        public Guide Guide { get; } = null;
        public IntegrationType Type { get; } = IntegrationType.Enrichment;

        private static EntityType[] AcceptedEntityTypes = { EntityType.Organization };
        /**********************************************************************************************************
         * CONSTRUCTORS
         **********************************************************************************************************/

        //public DnBExternalSearchProvider()
        //    : base(ProviderId, "/BusinessPartner")
        //{
        //}


        public DnBExternalSearchProvider()
           : base(ProviderId, AcceptedEntityTypes)
        {
            var nameBasedTokenProvider = new NameBasedTokenProvider("DnB");

            if (nameBasedTokenProvider.ApiToken != null)
                this.TokenProvider = new RoundRobinTokenProvider(nameBasedTokenProvider.ApiToken.Split(',', ';'));
        }

        public DnBExternalSearchProvider(IList<string> tokens)
            : this(true)
        {
            this.TokenProvider = new RoundRobinTokenProvider(tokens);
        }

        public DnBExternalSearchProvider(IExternalSearchTokenProvider tokenProvider)
            : this(true)
        {
            this.TokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
        }

        private DnBExternalSearchProvider(bool tokenProviderIsRequired)
            : base(ProviderId, AcceptedEntityTypes)
        {
            this.TokenProviderIsRequired = tokenProviderIsRequired;
        }


        /**********************************************************************************************************
         * METHODS
         **********************************************************************************************************/
        public override IEnumerable<IExternalSearchQuery> BuildQueries(ExecutionContext context, IExternalSearchRequest request)
        {
            foreach (var externalSearchQuery in InternalBuildQueries(context, request))
            {
                yield return externalSearchQuery;
            }
        }

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

        /// <summary>Builds the queries.</summary>
        /// <param name="context">The context.</param>
        /// <param name="request">The request.</param>
        /// <returns>The search queries.</returns>
        private IEnumerable<IExternalSearchQuery> InternalBuildQueries(ExecutionContext context, IExternalSearchRequest request, IDictionary<string, object> config = null)
        {
            if (config.TryGetValue(DnBConstants.KeyName.AcceptedEntityType, out var customType) && !string.IsNullOrWhiteSpace(customType?.ToString()))
            {
                if (!request.EntityMetaData.EntityType.Is(customType.ToString()))
                {
                    yield break;
                }
            }
            else if (!this.Accepts(request.EntityMetaData.EntityType))
                yield break;

            // Query Input
            //For companies use CluedInOrganization vocab, for people use CluedInPerson and so on for different types.
            var entityType = request.EntityMetaData.EntityType;
            var dunsNumber = GetValue(request, config, DnBConstants.KeyName.DunsNumberKey, Core.Data.Vocabularies.Vocabularies.CluedInOrganization.CodesDunsNumber);


            if (dunsNumber != null)
            {
                foreach (var value in dunsNumber)
                {
                    yield return new ExternalSearchQuery(this, entityType, new Dictionary<string, string>() { { "id", value } });
                }
                //yield return new ExternalSearchQuery(this, entityType, ExternalSearchQueryParameter.Name, value);
            }
        }

        /// <summary>Executes the search.</summary>
        /// <param name="context">The context.</param>
        /// <param name="query">The query.</param>
        /// <returns>The results.</returns>
        public override IEnumerable<IExternalSearchQueryResult> ExecuteSearch(ExecutionContext context, IExternalSearchQuery query)
        {
            return new List<IExternalSearchQueryResult>();
            //var apiKey = this.TokenProvider.ApiToken;

            //foreach (var externalSearchQueryResult in InternalExecuteSearch(query, apiKey)) yield return externalSearchQueryResult;
        }

        private static IEnumerable<IExternalSearchQueryResult> InternalExecuteSearch(IExternalSearchQuery query, DnBExternalSearchJobData jobData)
        {

            //TODO: replace hardcoded value
            var token = GetAuthToken(jobData);
            var dunsNumber = query.QueryParameters["id"].FirstOrDefault();

            if (string.IsNullOrEmpty(dunsNumber))
                yield break;

            var client = new RestClient(jobData.DnBBaseUrl);

            //TODO: Request
            var requestResource = $"data/duns/{dunsNumber}?productId=cmpelk&versionId=v1";
            var request = new RestRequest(requestResource, Method.GET);
            request.AddHeader("Authorization", $"Bearer {token}");

            var cleanseResponse = client.ExecuteAsync<DNBResponse>(request).Result;

            if (cleanseResponse.StatusCode == HttpStatusCode.OK)
            {
                if (cleanseResponse.Data != null)
                {

                    yield return new ExternalSearchQueryResult<DNBResponse>(query, cleanseResponse.Data);

                }
            }
            else if (cleanseResponse.StatusCode == HttpStatusCode.NoContent || cleanseResponse.StatusCode == HttpStatusCode.NotFound)
            {
                yield break;
            }
            else if (cleanseResponse.ErrorException != null)
            {
                throw new AggregateException(cleanseResponse.ErrorException.Message, cleanseResponse.ErrorException);
            }
            else
            {
                throw new ApplicationException("Could not execute external search query - StatusCode:" + cleanseResponse.StatusCode + "; Content: " + cleanseResponse.Content);
            }
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

        /// <summary>Builds the clues.</summary>post
        /// <param name="context">The context.</param>
        /// <param name="query">The query.</param>
        /// <param name="result">The result.</param>
        /// <param name="request">The request.</param>
        /// <returns>The clues.</returns>
        public override IEnumerable<Clue> BuildClues(ExecutionContext context, IExternalSearchQuery query, IExternalSearchQueryResult result, IExternalSearchRequest request)
        {
            var resultItem = result.As<DNBResponse>();

            var code = this.GetOriginEntityCode(resultItem, request);

            var clue = new Clue(code, context.Organization);

            this.PopulateMetadata(clue.Data.EntityData, resultItem, request);

            //Create all Companies from Ultimate and Global Parents
            if (resultItem.Data.organization.corporateLinkage.domesticUltimate != null)
            {
                if (resultItem.Data.organization.corporateLinkage.domesticUltimate.primaryAddress != null)
                {
                    var domesticUltimateEntityCode = new EntityCode(request.EntityMetaData.EntityType, "DnB", resultItem.Data.organization.corporateLinkage.domesticUltimate.duns);
                    var domesticUltimateClue = new Clue(domesticUltimateEntityCode, context.Organization);

                    //metadata.OutgoingEdges.Add();
                    domesticUltimateClue.Data.EntityData.EntityType = request.EntityMetaData.EntityType;
                    //TODO: add Name
                    domesticUltimateClue.Data.EntityData.Name = resultItem.Data.organization.corporateLinkage.domesticUltimate.primaryName;
                    domesticUltimateClue.Data.EntityData.OriginEntityCode = domesticUltimateEntityCode;

                    domesticUltimateClue.Data.EntityData.Codes.Add(domesticUltimateEntityCode);

                    domesticUltimateClue.Data.EntityData.Properties[StaticDnBVocabulary.BusinessPartner.PrimaryAddressCountry] = resultItem.Data.organization.corporateLinkage.domesticUltimate.primaryAddress.addressCountry.name;
                    domesticUltimateClue.Data.EntityData.Properties[StaticDnBVocabulary.BusinessPartner.PrimaryAddressCountyName] = resultItem.Data.organization.corporateLinkage.domesticUltimate.primaryAddress.addressCounty.name;
                    domesticUltimateClue.Data.EntityData.Properties[StaticDnBVocabulary.BusinessPartner.PrimaryAddressLocality] = resultItem.Data.organization.corporateLinkage.domesticUltimate.primaryAddress.addressLocality.name;
                    domesticUltimateClue.Data.EntityData.Properties[StaticDnBVocabulary.BusinessPartner.PrimaryAddressPostalCode] = resultItem.Data.organization.corporateLinkage.domesticUltimate.primaryAddress.postalCode;
                    domesticUltimateClue.Data.EntityData.Properties[StaticDnBVocabulary.BusinessPartner.PrimaryAddressRegionName] = resultItem.Data.organization.corporateLinkage.domesticUltimate.primaryAddress.addressRegion.name;
                    //domesticUltimateClue.Data.EntityData.Properties[StaticDnBVocabulary.BusinessPartner.PrimaryAddressStreetLine1] = resultItem.Data.organization.corporateLinkage.domesticUltimate.primaryAddress.streetNumber.;
                    //domesticUltimateClue.Data.EntityData.Properties[StaticDnBVocabulary.BusinessPartner.PrimaryAddressStreetLine2] = resultItem.Data.organization.corporateLinkage.domesticUltimate.primaryAddress.streetNumber.name;


                    yield return domesticUltimateClue;
                }
            }

            if (resultItem.Data.organization.corporateLinkage.globalUltimate != null)
            {
                if (resultItem.Data.organization.corporateLinkage.globalUltimate.primaryAddress != null)
                {
                    var domesticUltimateEntityCode = new EntityCode(request.EntityMetaData.EntityType, "DnB", resultItem.Data.organization.corporateLinkage.globalUltimate.duns);
                    var domesticUltimateClue = new Clue(domesticUltimateEntityCode, context.Organization);

                    //metadata.OutgoingEdges.Add();
                    domesticUltimateClue.Data.EntityData.EntityType = request.EntityMetaData.EntityType;
                    //TODO: add Name
                    domesticUltimateClue.Data.EntityData.Name = resultItem.Data.organization.corporateLinkage.globalUltimate.primaryName;
                    domesticUltimateClue.Data.EntityData.OriginEntityCode = domesticUltimateEntityCode;

                    domesticUltimateClue.Data.EntityData.Codes.Add(domesticUltimateEntityCode);

                    domesticUltimateClue.Data.EntityData.Properties[StaticDnBVocabulary.BusinessPartner.PrimaryAddressCountry] = resultItem.Data.organization.corporateLinkage.globalUltimate.primaryAddress.addressCountry.name;
                    domesticUltimateClue.Data.EntityData.Properties[StaticDnBVocabulary.BusinessPartner.PrimaryAddressCountyName] = resultItem.Data.organization.corporateLinkage.globalUltimate.primaryAddress.addressCounty.name;
                    domesticUltimateClue.Data.EntityData.Properties[StaticDnBVocabulary.BusinessPartner.PrimaryAddressLocality] = resultItem.Data.organization.corporateLinkage.globalUltimate.primaryAddress.addressLocality.name;
                    domesticUltimateClue.Data.EntityData.Properties[StaticDnBVocabulary.BusinessPartner.PrimaryAddressPostalCode] = resultItem.Data.organization.corporateLinkage.globalUltimate.primaryAddress.postalCode;
                    domesticUltimateClue.Data.EntityData.Properties[StaticDnBVocabulary.BusinessPartner.PrimaryAddressRegionName] = resultItem.Data.organization.corporateLinkage.globalUltimate.primaryAddress.addressRegion.name;
                    //domesticUltimateClue.Data.EntityData.Properties[StaticDnBVocabulary.BusinessPartner.PrimaryAddressStreetLine1] = resultItem.Data.organization.corporateLinkage.domesticUltimate.primaryAddress.streetNumber.;
                    //domesticUltimateClue.Data.EntityData.Properties[StaticDnBVocabulary.BusinessPartner.PrimaryAddressStreetLine2] = resultItem.Data.organization.corporateLinkage.domesticUltimate.primaryAddress.streetNumber.name;


                    yield return domesticUltimateClue;
                }
            }



            //Create all Industry Codes
            if (resultItem.Data.organization.industryCodes != null)
                foreach (var industryCode in resultItem.Data.organization.industryCodes)
                {
                    var industryEntityCode = new EntityCode("/IndustrySIC", "DnB", industryCode.code);
                    var industryClue = new Clue(industryEntityCode, context.Organization);

                    //metadata.OutgoingEdges.Add();
                    industryClue.Data.EntityData.EntityType = "/IndustrySIC";
                    //TODO: add Name
                    industryClue.Data.EntityData.Name = industryCode.description;
                    industryClue.Data.EntityData.OriginEntityCode = industryEntityCode;

                    industryClue.Data.EntityData.Codes.Add(industryEntityCode);


                    industryClue.Data.EntityData.Properties[StaticDnBVocabulary.Industry.Description] = industryCode.description;
                    industryClue.Data.EntityData.Properties[StaticDnBVocabulary.Industry.TypeDescription] = industryCode.typeDescription;
                    industryClue.Data.EntityData.Properties[StaticDnBVocabulary.Industry.TypeDnBCode] = industryCode.typeDnBCode.PrintIfAvailable();
                    industryClue.Data.EntityData.Properties[StaticDnBVocabulary.Industry.Priority] = industryCode.priority.PrintIfAvailable();
                    industryClue.Data.EntityData.Properties[StaticDnBVocabulary.Industry.Code] = industryCode.code;

                    yield return industryClue;
                }


            //Create all Senior Principals

            //Create all Curren Principals

            // TODO: If necessary, you can create multiple clues and return them.

            yield return clue;
        }

        /// <summary>Gets the primary entity metadata.</summary>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        /// <param name="request">The request.</param>
        /// <returns>The primary entity metadata.</returns>
        public override IEntityMetadata GetPrimaryEntityMetadata(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request)
        {
            var resultItem = result.As<DNBResponse>();
            return this.CreateMetadata(resultItem, request);
        }

        /// <summary>Gets the preview image.</summary>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        /// <param name="request">The request.</param>
        /// <returns>The preview image.</returns>
        public override IPreviewImage GetPrimaryEntityPreviewImage(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request)
        {
            return null;
        }

        /// <summary>Creates the metadata.</summary>
        /// <param name="resultItem">The result item.</param>
        /// <returns>The metadata.</returns>
        private IEntityMetadata CreateMetadata(IExternalSearchQueryResult<DNBResponse> resultItem, IExternalSearchRequest request)
        {
            var metadata = new EntityMetadataPart();

            this.PopulateMetadata(metadata, resultItem, request);

            return metadata;
        }

        /// <summary>Gets the origin entity code.</summary>
        /// <param name="resultItem">The result item.</param>
        /// <returns>The origin entity code.</returns>
        private EntityCode GetOriginEntityCode(IExternalSearchQueryResult<DNBResponse> resultItem, IExternalSearchRequest request)
        {
            return new EntityCode(request.EntityMetaData.EntityType, this.GetCodeOrigin(), resultItem.Data.organization.duns);
        }

        /// <summary>Gets the code origin.</summary>
        /// <returns>The code origin</returns>
        private CodeOrigin GetCodeOrigin()
        {
            return CodeOrigin.CluedIn.CreateSpecific("DnB");
        }

        /// <summary>Populates the metadata.</summary>
        /// <param name="metadata">The metadata.</param>
        /// <param name="resultItem">The result item.</param>
        private void PopulateMetadata(IEntityMetadata metadata, IExternalSearchQueryResult<DNBResponse> resultItem, IExternalSearchRequest request)
        {
            //var firstMatch = resultItem.Data.matchCandidates[0];

            var code = this.GetOriginEntityCode(resultItem, request);
            //metadata.OutgoingEdges.Add();
            metadata.EntityType = request.EntityMetaData.EntityType;
            //TODO: add Name
            metadata.Name = request.EntityMetaData.Name;
            metadata.OriginEntityCode = code;

            metadata.Codes.Add(code);
            metadata.Codes.Add(request.EntityMetaData.OriginEntityCode);

            var domesticUltimateDuns = resultItem.Data.organization.corporateLinkage.domesticUltimate.duns;
            var globalUltimateDuns = resultItem.Data.organization.corporateLinkage.globalUltimate.duns;

            var domesticCode = new EntityCode(request.EntityMetaData.EntityType, "DnB", domesticUltimateDuns);
            var globalCode = new EntityCode(request.EntityMetaData.EntityType, "DnB", globalUltimateDuns);

            metadata.Codes.Add(code);
            metadata.Codes.Add(request.EntityMetaData.OriginEntityCode);

            metadata.OutgoingEdges.Add(new EntityEdge(new EntityReference(code), new EntityReference(domesticCode), "/DomesticUltimateParent"));
            metadata.OutgoingEdges.Add(new EntityEdge(new EntityReference(code), new EntityReference(globalCode), "/GlobalUltimateParent"));
            if (resultItem.Data.organization.dunsControlStatus != null)
            {
                metadata.Properties[StaticDnBVocabulary.BusinessPartner.DunsControlStatusFullReportDate] = resultItem.Data.organization.dunsControlStatus.fullReportDate;
                metadata.Properties[StaticDnBVocabulary.BusinessPartner.DunsControlStatusLastUpdateDate] = resultItem.Data.organization.dunsControlStatus.lastUpdateDate;
                metadata.Properties[StaticDnBVocabulary.BusinessPartner.DunsControlStatusOperatingStatusDescription] = resultItem.Data.organization.dunsControlStatus.operatingStatus.description;
                metadata.Properties[StaticDnBVocabulary.BusinessPartner.DunsControlStatusOperatingStatusDnbCode] = resultItem.Data.organization.dunsControlStatus.operatingStatus.dnbCode.PrintIfAvailable();

                metadata.Properties[StaticDnBVocabulary.BusinessPartner.DunsControlStatusIsMarketable] = resultItem.Data.organization.dunsControlStatus.isMarketable.PrintIfAvailable();
                metadata.Properties[StaticDnBVocabulary.BusinessPartner.DunsControlStatusIsMailUndeliverable] = resultItem.Data.organization.dunsControlStatus.isMailUndeliverable.PrintIfAvailable();
                metadata.Properties[StaticDnBVocabulary.BusinessPartner.DunsControlStatusIsTelephoneDisconnected] = resultItem.Data.organization.dunsControlStatus.isTelephoneDisconnected.PrintIfAvailable();
                metadata.Properties[StaticDnBVocabulary.BusinessPartner.DunsControlStatusIsDelisted] = resultItem.Data.organization.dunsControlStatus.isDelisted.PrintIfAvailable();
                metadata.Properties[StaticDnBVocabulary.BusinessPartner.DunsControlStatusSubjectHandlingDetails] = resultItem.Data.organization.dunsControlStatus.subjectHandlingDetails.PrintIfAvailable();
                if (resultItem.Data.organization.dunsControlStatus.operatingStatus != null)
                {
                    // Operating Status
                    metadata.Properties[StaticDnBVocabulary.BusinessPartner.OperatingStatusCode] = resultItem.Data.organization.dunsControlStatus.operatingStatus.dnbCode.PrintIfAvailable();
                    metadata.Properties[StaticDnBVocabulary.BusinessPartner.OperatingStatusDescription] = resultItem.Data.organization.dunsControlStatus.operatingStatus.description.PrintIfAvailable();
                }
            }
            // DUNS Numbers
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.Duns] = resultItem.Data.organization.duns;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.DomesticUltimateDuns] = domesticUltimateDuns;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.GlobalUltimateDuns] = globalUltimateDuns;


            //resultItem.Data.organization.telephone

            // Business Information
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.PrimaryBusinessName] = resultItem.Data.organization.primaryName;
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.PrimaryAddressCountry] = resultItem.Data.organization.primaryAddress.addressCountry.name.PrintIfAvailable();
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.ISO2CountryCode] = resultItem.Data.organization.primaryAddress.addressCountry.isoAlpha2Code.PrintIfAvailable();
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.PrimaryAddressLocality] = resultItem.Data.organization.primaryAddress.addressLocality.name.PrintIfAvailable();
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.PrimaryAddressRegionAbbreviatedName] = resultItem.Data.organization.primaryAddress.addressRegion.abbreviatedName.PrintIfAvailable();
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.PrimaryAddressRegionName] = resultItem.Data.organization.primaryAddress.addressRegion.name.PrintIfAvailable();
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.PrimaryAddressPostalCode] = resultItem.Data.organization.primaryAddress.postalCode.PrintIfAvailable();
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.PrimaryAddressStreetLine1] = resultItem.Data.organization.primaryAddress.streetAddress.line1.PrintIfAvailable();
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.PrimaryAddressStreetLine2] = resultItem.Data.organization.primaryAddress.streetAddress.line2.PrintIfAvailable();
            //metadata.Properties[StaticDnBVocabulary.BusinessPartner.WebsiteUrl] = resultItem.Data.organization.websiteAddress.First()..PrintIfAvailable();

            if (resultItem.Data.organization.industryCodes != null)
                foreach (var industryCode in resultItem.Data.organization.industryCodes)
                {
                    var industryEntityCode = new EntityCode("/IndustrySIC", "DnB", industryCode.code);
                    metadata.OutgoingEdges.Add(new EntityEdge(new EntityReference(code), new EntityReference(industryEntityCode), "/IndustrySic"));
                }

            metadata.Properties[StaticDnBVocabulary.BusinessPartner.BusinessEntityTypeDnbCode] = resultItem.Data.organization.businessEntityType.dnbCode.PrintIfAvailable();
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.BusinessEntityTypeDescription] = resultItem.Data.organization.businessEntityType.description;
        }

        public IEnumerable<EntityType> Accepts(IDictionary<string, object> config, IProvider provider)
        {
            //var customTypes = config[Constants.KeyName.AcceptedEntityType].ToString();
            //if (string.IsNullOrWhiteSpace(customTypes))
            //{
            //    AcceptedEntityTypes = new EntityType[] { config[Constants.KeyName.AcceptedEntityType].ToString() };
            //};

            return AcceptedEntityTypes;
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
            return BuildClues(context, query, result, request);
        }

        public IEntityMetadata GetPrimaryEntityMetadata(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request, IDictionary<string, object> config, IProvider provider)
        {
            return GetPrimaryEntityMetadata(context, result, request);
        }

        public IPreviewImage GetPrimaryEntityPreviewImage(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request, IDictionary<string, object> config, IProvider provider)
        {
            return GetPrimaryEntityPreviewImage(context, result, request);
        }
    }
}