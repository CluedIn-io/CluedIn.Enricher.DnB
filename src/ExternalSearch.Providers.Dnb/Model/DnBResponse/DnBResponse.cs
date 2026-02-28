using CluedIn.ExternalSearch.Providers.DnB.Custom;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CluedIn.ExternalSearch.Providers.DnB.Model.DnBResponse;

public class AddressCountry
{
    public string name { get; set; }
    public string isoAlpha2Code { get; set; }
    public string fipsCode { get; set; }
}

public class AddressCounty
{
    public string name { get; set; }
    public string administrativeDivisionCode { get; set; }
    public string fipsCode { get; set; }
}

public class AddressLocality
{
    public string name { get; set; }

}

public class AddressRegion
{
    public string name { get; set; }
    public string abbreviatedName { get; set; }
    public string isoSubDivisionName { get; set; }
    public string isoSubDivisionCode { get; set; }
    public string administrativeDivisionCode { get; set; }
    public string fipsCode { get; set; }
}

public class BusinessEntityType
{
    public string description { get; set; }
    public int dnbCode { get; set; }
}

public class ContinentalRegion
{
    public string name { get; set; }
}

public class ControlOwnershipType
{
    public string description { get; set; }
    public int dnbCode { get; set; }
}

public class CorporateLinkage
{
    public List<FamilytreeRolesPlayed> familytreeRolesPlayed { get; set; }
    public int hierarchyLevel { get; set; }
    public int globalUltimateFamilyTreeMembersCount { get; set; }
    public GlobalUltimate globalUltimate { get; set; }
    public DomesticUltimate domesticUltimate { get; set; }
    public Parent parent { get; set; }
    public HeadQuarter headQuarter { get; set; }
    public JToken branches { get; set; }
}

public class CurrentPrincipal
{
    public string givenName { get; set; }
    public string familyName { get; set; }
    public string fullName { get; set; }
    public object namePrefix { get; set; }
    public object nameSuffix { get; set; }
    public object gender { get; set; }
    public List<JobTitle> jobTitles { get; set; }
    public List<ManagementResponsibility> managementResponsibilities { get; set; }
}

public class DomesticUltimate
{
    public string duns { get; set; }
    public string primaryName { get; set; }
    public PrimaryAddress primaryAddress { get; set; }
    public List<NumberOfEmployee> numberOfEmployees { get; set; }
    public List<Financial> financials { get; set; }
}

public class DunsControlStatus
{
    public OperatingStatus operatingStatus { get; set; }
    public bool isMarketable { get; set; }
    public bool isMailUndeliverable { get; set; }
    public bool isTelephoneDisconnected { get; set; }
    public bool isDelisted { get; set; }
    public List<SubjectHandlingDetail> subjectHandlingDetails { get; set; }
    public string fullReportDate { get; set; }
    public string lastUpdateDate { get; set; }
}

public class EmployeeCategory
{
    public string employmentBasisDescription { get; set; }
    public int employmentBasisDnBCode { get; set; }
}

public class FamilytreeRolesPlayed
{
    public string description { get; set; }
    public int dnbCode { get; set; }
}

public class Financial
{
    public string financialStatementToDate { get; set; }
    public object financialStatementDuration { get; set; }
    public string informationScopeDescription { get; set; }
    public int informationScopeDnBCode { get; set; }
    public string reliabilityDescription { get; set; }
    public int reliabilityDnBCode { get; set; }
    public string unitCode { get; set; }
    public string accountantName { get; set; }
    public List<YearlyRevenue> yearlyRevenue { get; set; }
}

public class GeographicalPrecision
{
    public string description { get; set; }
    public int dnbCode { get; set; }
}

public class GlobalUltimate
{
    public string duns { get; set; }
    public string primaryName { get; set; }
    public PrimaryAddress primaryAddress { get; set; }
    public List<NumberOfEmployee> numberOfEmployees { get; set; }
    public List<Financial> financials { get; set; }
}

public class HeadQuarter
{
    public string duns { get; set; }
    public string primaryName { get; set; }
    public PrimaryAddress primaryAddress { get; set; }
}

public class IndustryCode
{
    public string code { get; set; }
    public string description { get; set; }
    public string typeDescription { get; set; }
    public int typeDnBCode { get; set; }
    public int priority { get; set; }
}

public class InquiryDetail
{
    public string productVersion { get; set; }
    public string productID { get; set; }
    public string duns { get; set; }
}

public class JobTitle
{
    public string title { get; set; }
}

public class Language
{
    public string description { get; set; }
    public string dnbCode { get; set; }
}

public class LocationOwnership
{
    public string description { get; set; }
    public int dnbCode { get; set; }
}

public class MailingAddress
{
}

public class ManagementResponsibility
{
    public string description { get; set; }
    public string mrcCode { get; set; }
}

public class MostSeniorPrincipal
{
    public string givenName { get; set; }
    public string familyName { get; set; }
    public string fullName { get; set; }
    public object namePrefix { get; set; }
    public object nameSuffix { get; set; }
    public object gender { get; set; }
    public List<JobTitle> jobTitles { get; set; }
    public List<ManagementResponsibility> managementResponsibilities { get; set; }
}

public class NumberOfEmployee
{
    public int value { get; set; }
    public int minimumValue { get; set; }
    public int maximumValue { get; set; }
    public string employeeFiguresDate { get; set; }
    public string informationScopeDescription { get; set; }
    public int informationScopeDnBCode { get; set; }
    public string reliabilityDescription { get; set; }
    public int reliabilityDnBCode { get; set; }
    public List<EmployeeCategory> employeeCategories { get; set; }
    public JToken trend { get; set; }
}

public class OperatingStatus
{
    public string description { get; set; }
    public int dnbCode { get; set; }
}

public class Organization
{
    [ManualMap]
    public string countryISOAlpha2Code { get; set; } // already mapped to ISO2CountryCode country code
    [ManualMap]
    public string duns { get; set; }
    [JsonIgnore, ManualMap]
    public DunsControlStatus dunsControlStatus =>
        dunsControlStatusRaw?.ToObject<DunsControlStatus>();
    [JsonProperty("dunsControlStatus")]
    public JToken dunsControlStatusRaw { get; set; }
    [ManualMap]
    public string primaryName { get; set; }
    [ManualMap]
    public List<TradeStyleName> tradeStyleNames { get; set; }
    [ManualMap]
    public List<WebsiteAddress> websiteAddress { get; set; }
    [ManualMap]
    public List<Telephone> telephone { get; set; }
    [ManualMap]
    public List<Fax> fax { get; set; }
    [ManualMap]
    public PrimaryAddress primaryAddress { get; set; }
    public PrimaryAddress registeredAddress { get; set; }
    public List<PrimaryAddress> multilingualRegisteredAddress { get; set; }
    public PrimaryAddress mailingAddress { get; set; }
    public List<PrimaryAddress> formerPrimaryAddresses { get; set; }
    public List<PrimaryAddress> formerRegisteredAddresses { get; set; }
    [ManualMap]
    public List<StockExchange> stockExchanges { get; set; }
    [ManualMap]
    public List<RegistrationNumber> registrationNumbers { get; set; }
    [ManualMap]
    public List<IndustryCode> industryCodes { get; set; }
    [ManualMap]
    public BusinessEntityType businessEntityType { get; set; }
    public string controlOwnershipDate { get; set; }
    public ControlOwnershipType controlOwnershipType { get; set; }
    public bool isAgent { get; set; }
    public bool isImporter { get; set; }
    public bool isExporter { get; set; }
    [JsonIgnore, ManualMap]
    public List<NumberOfEmployee> numberOfEmployees =>
        numberOfEmployeesRaw?.Select(x => x.ToObject<NumberOfEmployee>()).ToList();
    [JsonProperty("numberOfEmployees")]
    public JToken numberOfEmployeesRaw { get; set; }
    public List<Financial> financials { get; set; }
    public List<MostSeniorPrincipal> mostSeniorPrincipals { get; set; }
    public bool isStandalone { get; set; }
    public CorporateLinkage corporateLinkage { get; set; }
    public GlobalUltimate globalUltimate { get; set; }
    public DomesticUltimate domesticUltimate { get; set; }
    public JToken businessActivityInsight { get; set; }
    public JToken climateExposureIndices { get; set; }
    public JToken latestFiscalFinancials { get; set; }
    public JToken otherFinancials { get; set; }
    public JToken abridgedLatestFiscalFinancials { get; set; }
    public JToken abridgedOtherFinancials { get; set; }
    public JToken thirdPartyValuationRatios { get; set; }
    public JToken thirdPartyFinancialsComparison { get; set; }
    public string thirdPartyFinancialsAccountantName { get; set; }
    public string thirdPartyIndustryTemplateCode { get; set; }
    public string thirdPartyReportStatus { get; set; }
    public JToken thirdPartyFinancials { get; set; }
    public JToken localOperatingStatus { get; set; }
    public string registeredName { get; set; }
    public JToken multilingualPrimaryName { get; set; }
    public JToken multilingualRegisteredNames { get; set; }
    public JToken summary { get; set; }
    public JToken multilingualTradestyleNames { get; set; }
    public JToken formerPrimaryNames { get; set; }
    public JToken formerRegisteredNames { get; set; }
    public string defaultCurrency { get; set; }
    public List<Email> email { get; set; }
    public string certifiedEmail { get; set; }
    public PrimaryAddress iso20022StructuredPrimaryAddress { get; set; }
    public List<PrimaryAddress> multilingualPrimaryAddress { get; set; }
    public JToken standardizedStockExchanges { get; set; }
    public bool isForbesLargestPrivateCompaniesListed { get; set; }
    public bool isFortune1000Listed { get; set; }
    public JToken thirdPartyAssessment { get; set; }
    public string legalEntityIdentifier { get; set; }
    public JToken primaryIndustryCode { get; set; }
    public JToken unspscCodes { get; set; }
    public bool isNonClassifiedEstablishment { get; set; }
    public JToken activities { get; set; }
    public string startDate { get; set; }
    public string incorporatedDate { get; set; }
    public JToken legalForm { get; set; }
    public JToken operations { get; set; }
    public JToken charterType { get; set; }
    public JToken subjectComments { get; set; }
    public JToken registeredDetails { get; set; }
    public JToken individualStatementYearlyRevenue { get; set; }
    public string fiscalYearEnd { get; set; }
    public JToken banks { get; set; }
    public bool isSmallBusiness { get; set; }
    public JToken competitors { get; set; }
    public JToken otherCompetitors { get; set; }
    public JToken regulations { get; set; }
    public JToken franchiseOperationType { get; set; }
    public JToken assignmentModel { get; set; }
    public JToken organizationSizeCategory { get; set; }
    public JToken employerDesignation { get; set; }
    public double individualNetWorthToTotalAssets { get; set; }
    public double netWorthToTotalAssets { get; set; }
    public JToken preferredLanguage { get; set; }
    public JToken suppliers { get; set; }
    public JToken customers { get; set; }
    public JToken lineOfBusinessSummary { get; set; }
    public JToken multiLingualSearchNames { get; set; }
    public string imperialCalendarStartYear { get; set; }
    public JToken businessTrustIndex { get; set; }
    public string securitiesReportID { get; set; }
    public JToken tsrCommodityCodes { get; set; }
    public string investigationDate { get; set; }
    public string tsrReportDate { get; set; }
    public JToken legalEntityIdentifierDetails { get; set; }
    public JToken socioEconomicInformation { get; set; }
    public JToken dtri { get; set; }
    public JToken educationalData { get; set; }
    public JToken environmentalInsight { get; set; }
    public JToken esgIndustryCategories { get; set; }
    public JToken esgRanking { get; set; }
    public JToken socialInsight { get; set; }
    public bool hasCompanyMoved { get; set; }
    public JToken documentFilings { get; set; }
    public JToken legalEvents { get; set; }
    public JToken commercialCollectionClaims { get; set; }
    public JToken financingEvents { get; set; }
    public JToken defaultEvents { get; set; }
    public JToken significantEvents { get; set; }
    public JToken awards { get; set; }
    public JToken exclusions { get; set; }
    public JToken violations { get; set; }
    public JToken delinquencyScoreNorms { get; set; }
    public JToken failureScoreNorms { get; set; }
    public bool isHighRiskBusiness { get; set; }
    public bool isDeterioratingBusiness { get; set; }
    public JToken dnbAssessment { get; set; }
    public JToken layOffScore { get; set; }
    public JToken tsrRating { get; set; }
    public JToken tsrRatingHistory { get; set; }
    public JToken globalBusinessRanking { get; set; }
    public JToken standardizedFinancials { get; set; }
    public int industrialPlantsCount { get; set; }
    public JToken affiliates { get; set; }
    public JToken extendedLinkageInsight { get; set; }
    public JToken fraudRiskSignals { get; set; }
    public JToken inquiryInsight { get; set; }
    public JToken shareOwnership { get; set; }
    public JToken capitalDetails { get; set; }
    public string normsCalculationTimestamp { get; set; }
    public JToken businessTradingNorms { get; set; }
    public JToken businessTrading { get; set; }
    public JToken businessTradingNormsHistory { get; set; }
    public JToken physicalClimateRiskInsights { get; set; }
    public JToken principalsSummary { get; set; }
    public JToken signingAuthorities { get; set; }
    public JToken currentPrincipals { get; set; }
    public JToken formerPrincipals { get; set; }
    public JToken mostSeniorPrincipal { get; set; }
    public JToken registryAuthorityAsFiled { get; set; }
    public JToken financialServicesProspectorModel { get; set; }
    public JToken salesMarketingAssessment { get; set; }
    public JToken shipmentInformation { get; set; }
    public JToken supplyChainRiskIndex { get; set; }
    public JToken thirdPartyRiskAssessment { get; set; }
}

public class Parent
{
    public string duns { get; set; }
    public string primaryName { get; set; }
    public PrimaryAddress primaryAddress { get; set; }
}

public class PopulationRank
{
    public string rankNumber { get; set; }
    public int rankDnBCode { get; set; }
    public string rankDescription { get; set; }
}

public class PostalCodePosition
{
    public string description { get; set; }
    public string dnbCode { get; set; }
}

public class PostOfficeBox
{
    public string postOfficeBoxNumber { get; set; }
    public string typeDescription { get; set; }
    public string typeDnBCode { get; set; }
}

public class PremisesArea
{
    public int measurement { get; set; }
    public string unitDescription { get; set; }
    public int unitDnBCode { get; set; }
    public string reliabilityDescription { get; set; }
    public int reliabilityDnBCode { get; set; }
}

public class StandardAddressCode
{
    public string addressCode { get; set; }
    public CodeType codeType { get; set; }
}

public class CodeType
{
    public string description { get; set; }
    public int dnbCode { get; set; }
}

public class CongressionalDistrict
{
    public string district { get; set; }
}

public class WritingScript
{
    public string description { get; set; }
    public int dnbCode { get; set; }
}

// Can be used for different address type like PrimaryAddress/RegisteredAddress/...
public class PrimaryAddress
{
    public Language language { get; set; }
    public WritingScript writingScript { get; set; }
    public AddressCountry addressCountry { get; set; }
    public ContinentalRegion continentalRegion { get; set; }
    public AddressLocality addressLocality { get; set; }
    public string minorTownName { get; set; }
    public AddressRegion addressRegion { get; set; }
    public AddressCounty addressCounty { get; set; }
    public string postalCode { get; set; }
    public PostalCodePosition postalCodePosition { get; set; }
    public string postalRoute { get; set; }
    public string streetNumber { get; set; }
    public string streetName { get; set; }
    public StreetAddress streetAddress { get; set; }
    public PostOfficeBox postOfficeBox { get; set; }
    public double latitude { get; set; }
    public double longitude { get; set; }
    public GeographicalPrecision geographicalPrecision { get; set; }
    public StatisticalArea statisticalArea { get; set; }
    public LocationOwnership locationOwnership { get; set; }
    public PremisesArea premisesArea { get; set; }
    public List<StandardAddressCode> standardAddressCodes { get; set; }
    public bool isRegisteredAddress { get; set; }
    public bool isManufacturingLocation { get; set; }
    public bool isResidentialAddress { get; set; }
    public List<CongressionalDistrict> congressionalDistricts { get; set; }
    public string startDate { get; set; }
    public string endDate { get; set; }
}

public class RegisteredAddress
{
}

public class DNBResponse
{
    public TransactionDetail transactionDetail { get; set; }
    public InquiryDetail inquiryDetail { get; set; }
    public Organization organization { get; set; }
    public long? candidatesMatchedQuantity { get; set; }
    public string matchDataCriteria { get; set; }
    public List<MatchCandidate> matchCandidates { get; set; }
    public CleanseAndStandardizeInformation cleanseAndStandardizeInformation { get; set; }
    public EmbeddedProduct embeddedProduct { get; set; }
    public Error error { get; set; }
}

public class Error
{
    public string errorCode { get; set; }
    public string errorMessage { get; set; }
    public Errordetail[] errorDetails { get; set; }
}

public class Errordetail
{
    public string parameter { get; set; }
    public string description { get; set; }
}

public class CleanseAndStandardizeInformation
{

}
public class EmbeddedProduct
{
    public Organization organization { get; set; }
    public List<string> blockIDs { get; set; }
    public string inLanguage { get; set; }
    public List<BlockStatus> blockStatus { get; set; }
}

public class BlockStatus
{
    public string blockID { get; set; }
    public string status { get; set; }
    public object reason { get; set; }
}

public class MatchCandidate
{
    public long? displaySequence { get; set; }
    public Organization organization { get; set; }
    public MatchQualityInformation matchQualityInformation { get; set; }
}

public class MatchQualityInformation
{
    public long? confidenceCode { get; set; }
    public string matchGrade { get; set; }
    public long? matchGradeComponentsCount { get; set; }
    public List<MatchGradeComponent> MatchGradeComponents { get; set; }
    public string matchDataProfile { get; set; }
    public long? matchDataProfileComponentsCount { get; set; }
    public List<MatchDataProfileComponent> matchDataProfileComponents { get; set; }
    public long? nameMatchScore { get; set; }
}

public partial class MatchDataProfileComponent
{
    public string componentType { get; set; }
    public string componentValue { get; set; }
}

public partial class MatchGradeComponent
{
    public string componentType { get; set; }
    public string componentRating { get; set; }
}

public class SocioEconomicInformation
{
    public object isMinorityOwned { get; set; }
    public bool isSmallBusiness { get; set; }
}

public class StatisticalArea
{
    public string cbsaName { get; set; }
    public string cbsaCode { get; set; }
    public string economicAreaOfInfluenceCode { get; set; }
    public PopulationRank populationRank { get; set; }
}

public class StreetAddress
{
    public string line1 { get; set; }
    public string line2 { get; set; }
    public string line3 { get; set; }
    public string line4 { get; set; }
}

public class SubjectHandlingDetail
{
    public string description { get; set; }
    public int dnbCode { get; set; }
}

public class Telephone
{
    public string telephoneNumber { get; set; }
    public string isdCode { get; set; }
    public bool isUnreachable { get; set; }
}

public class TransactionDetail
{
    public string transactionID { get; set; }
    public DateTime transactionTimestamp { get; set; }
    public string inLanguage { get; set; }
    public string productID { get; set; }
    public string productVersion { get; set; }
}

public class YearlyRevenue
{
    public double value { get; set; }
    public double minimumValue { get; set; }
    public double maximumValue { get; set; }
    public string currency { get; set; }
}

public class TradeStyleName
{
    public string name { get; set; }
    public int priority { get; set; }
}

public class WebsiteAddress
{
    public string url { get; set; }
    public string domainName { get; set; }
}

public class Email
{
    public string address { get; set; }
}

public class Fax
{
    public string faxNumber { get; set; }
    public string isdCode { get; set; }
}

public class StockExchange
{
    public string tickerName { get; set; }
    public ExchangeName exchangeName { get; set; }
    public ExchangeCountry exchangeCountry { get; set; }
    public bool? isPrimary { get; set; }
}

public class ExchangeName
{
    public string description { get; set; }
}

public class ExchangeCountry
{
    public string isoAlpha2Code { get; set; }
}

public class RegistrationNumber
{
    public string registrationNumber { get; set; }
    public string typeDescription { get; set; }
    public int typeDnBCode { get; set; }
    public RegistrationNumberClass registrationNumberClass { get; set; }
    public bool? isPreferredRegistrationNumber { get; set; }
    public string registrationLocation { get; set; }
}

public class RegistrationNumberClass
{
    public string description { get; set; }
    public int dnbCode { get; set; }
}
