using System;
using System.Collections.Generic;

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
    public string informationScopeDescription { get; set; }
    public int informationScopeDnBCode { get; set; }
    public string reliabilityDescription { get; set; }
    public int reliabilityDnBCode { get; set; }
    public List<EmployeeCategory> employeeCategories { get; set; }
    public List<object> trend { get; set; }
}

public class OperatingStatus
{
    public string description { get; set; }
    public int dnbCode { get; set; }
}

public class Organization
{
    public string duns { get; set; }
    public DunsControlStatus dunsControlStatus { get; set; }
    public string primaryName { get; set; }
    public List<TradeStyleName> tradeStyleNames { get; set; }
    public List<WebsiteAddress> websiteAddress { get; set; }
    public List<Telephone> telephone { get; set; }
    public List<Fax> fax { get; set; }
    public PrimaryAddress primaryAddress { get; set; }
    public RegisteredAddress registeredAddress { get; set; }
    public MailingAddress mailingAddress { get; set; }
    public List<StockExchange> stockExchanges { get; set; }
    public List<object> thirdPartyAssessment { get; set; }
    public List<RegistrationNumber> registrationNumbers { get; set; }
    public List<IndustryCode> industryCodes { get; set; }
    public BusinessEntityType businessEntityType { get; set; }
    public string controlOwnershipDate { get; set; }
    public ControlOwnershipType controlOwnershipType { get; set; }
    public object isAgent { get; set; }
    public object isImporter { get; set; }
    public object isExporter { get; set; }
    public List<NumberOfEmployee> numberOfEmployees { get; set; }
    public List<Financial> financials { get; set; }
    public List<MostSeniorPrincipal> mostSeniorPrincipals { get; set; }
    public List<CurrentPrincipal> currentPrincipals { get; set; }
    public SocioEconomicInformation socioEconomicInformation { get; set; }
    public bool isStandalone { get; set; }
    public CorporateLinkage corporateLinkage { get; set; }
    public GlobalUltimate globalUltimate { get; set; }
    public DomesticUltimate domesticUltimate { get; set; }
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
}

public class PostOfficeBox
{
}

public class PremisesArea
{
}

public class PrimaryAddress
{
    public Language language { get; set; }
    public AddressCountry addressCountry { get; set; }
    public ContinentalRegion continentalRegion { get; set; }
    public AddressLocality addressLocality { get; set; }
    public object minorTownName { get; set; }
    public AddressRegion addressRegion { get; set; }
    public AddressCounty addressCounty { get; set; }
    public string postalCode { get; set; }
    public PostalCodePosition postalCodePosition { get; set; }
    public object streetNumber { get; set; }
    public object streetName { get; set; }
    public StreetAddress streetAddress { get; set; }
    public PostOfficeBox postOfficeBox { get; set; }
    public double latitude { get; set; }
    public double longitude { get; set; }
    public GeographicalPrecision geographicalPrecision { get; set; }
    public bool isRegisteredAddress { get; set; }
    public StatisticalArea statisticalArea { get; set; }
    public LocationOwnership locationOwnership { get; set; }
    public PremisesArea premisesArea { get; set; }
    public bool isManufacturingLocation { get; set; }
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
    public object line2 { get; set; }
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
