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

        public  AuthMethods AuthMethods { get; } = null;
        public  IEnumerable<Control> Properties { get; } = null;
        public  Guide Guide { get; } = null;
        public  IntegrationType Type { get; } = IntegrationType.Enrichment;

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
            var organizationName = GetValue(request, config, DnBConstants.KeyName.OrgNameKey, Core.Data.Vocabularies.Vocabularies.CluedInOrganization.OrganizationName);
            var organizationCountry = GetValue(request, config, DnBConstants.KeyName.OrgCountryCodeKey, Core.Data.Vocabularies.Vocabularies.CluedInOrganization.AddressCountryCode);
            var organizationAddress = GetValue(request, config, DnBConstants.KeyName.OrgAddressKey, Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Address);

            if (!string.IsNullOrEmpty(request.EntityMetaData.Name))
                organizationName.Add(request.EntityMetaData.Name);
            if (!string.IsNullOrEmpty(request.EntityMetaData.DisplayName))
                organizationName.Add(request.EntityMetaData.DisplayName);

            if (organizationName != null && organizationCountry != null)
            {
                var values = organizationName.Select(NameNormalization.Normalize);
                if (organizationCountry.Count > 0)
                {
                    var countryValue = organizationCountry.Select(NameNormalization.Normalize).First();
                    foreach (var value in values)
                    {
                        if (organizationAddress.Count > 0)
                        {
                            var addressValue = organizationAddress.Select(NameNormalization.Normalize).First();
                            yield return new ExternalSearchQuery(this, entityType, new Dictionary<string, string>() { { "name", value }, { "country", countryValue }, { "address", addressValue } });
                        }
                        else
                        {
                            yield return new ExternalSearchQuery(this, entityType, new Dictionary<string, string>() { { "name", value }, { "country", countryValue } });

                        }
                    }
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
            var name = query.QueryParameters["name"].FirstOrDefault();
            var country = query.QueryParameters["country"].FirstOrDefault();

            if (string.IsNullOrEmpty(name))
                yield break;

            var client = new RestClient(jobData.DnBBaseUrl);

            //TODO: Request
            var requestResource = $"match/cleanseMatch?name={name}&countryISOAlpha2Code={country}";
            if (query.QueryParameters.ContainsKey("address"))
            {
                requestResource += $"&streetAddressLine1={query.QueryParameters["address"].FirstOrDefault()}";
            }
            var request = new RestRequest(requestResource, Method.GET);
            request.AddHeader("Authorization", $"Bearer {token}");

            var cleanseResponse = client.ExecuteAsync<DnBResponse>(request).Result;

            if (cleanseResponse.StatusCode == HttpStatusCode.OK)
            {
                if (cleanseResponse.Data != null)
                {
                    if (cleanseResponse.Data.matchCandidates.Count > 0)
                    {
                        yield return new ExternalSearchQueryResult<DnBResponse>(query, cleanseResponse.Data);
                    }
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

        /// <summary>Builds the clues.</summary>
        /// <param name="context">The context.</param>
        /// <param name="query">The query.</param>
        /// <param name="result">The result.</param>
        /// <param name="request">The request.</param>
        /// <returns>The clues.</returns>
        public override IEnumerable<Clue> BuildClues(ExecutionContext context, IExternalSearchQuery query, IExternalSearchQueryResult result, IExternalSearchRequest request)
        {
            var resultItem = result.As<DnBResponse>();

            var code = this.GetOriginEntityCode(resultItem, request);

            var clue = new Clue(code, context.Organization);

            this.PopulateMetadata(clue.Data.EntityData, resultItem, request);

            // TODO: If necessary, you can create multiple clues and return them.

            return new[] { clue };
        }

        /// <summary>Gets the primary entity metadata.</summary>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        /// <param name="request">The request.</param>
        /// <returns>The primary entity metadata.</returns>
        public override IEntityMetadata GetPrimaryEntityMetadata(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request)
        {
            var resultItem = result.As<DnBResponse>();
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
        private IEntityMetadata CreateMetadata(IExternalSearchQueryResult<DnBResponse> resultItem, IExternalSearchRequest request)
        {
            var metadata = new EntityMetadataPart();

            this.PopulateMetadata(metadata, resultItem, request);

            return metadata;
        }

        /// <summary>Gets the origin entity code.</summary>
        /// <param name="resultItem">The result item.</param>
        /// <returns>The origin entity code.</returns>
        private EntityCode GetOriginEntityCode(IExternalSearchQueryResult<DnBResponse> resultItem, IExternalSearchRequest request)
        {
            return new EntityCode(request.EntityMetaData.EntityType, this.GetCodeOrigin(), resultItem.Data.matchCandidates[0].organization.duns);
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
        private void PopulateMetadata(IEntityMetadata metadata, IExternalSearchQueryResult<DnBResponse> resultItem, IExternalSearchRequest request)
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

            // DUNS Numbers
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.Duns] = resultItem.Data.matchCandidates[0].organization.duns;

            // Operating Status
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.OperatingStatusCode] = resultItem.Data.matchCandidates[0].organization.dunsControlStatus.operatingStatus.dnbCode.ToString();
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.OperatingStatusDescription] = resultItem.Data.matchCandidates[0].organization.dunsControlStatus.operatingStatus.description.ToString();

            // Match Confidence Cod
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.MatchConfidenceCode] = resultItem.Data.matchCandidates[0].matchQualityInformation.confidenceCode.ToString();

            // Business Information
            metadata.Properties[StaticDnBVocabulary.BusinessPartner.PrimaryBusinessName] = resultItem.Data.matchCandidates[0].organization.primaryName;
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