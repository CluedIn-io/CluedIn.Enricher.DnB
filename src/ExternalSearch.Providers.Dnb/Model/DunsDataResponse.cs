using System;
using System.Collections.Generic;
using System.Text;

namespace CluedIn.ExternalSearch.Providers.DnB.Model
{
    public class TransactionDetail
    {
        public string transactionID { get; set; }
        public DateTime transactionTimestamp { get; set; }
        public string inLanguage { get; set; }
    }

    public class InquiryDetail
    {
        public string duns { get; set; }
        public IList<string> blockIDs { get; set; }
    }

    public class IndustryCode
    {
        public string code { get; set; }
        public string description { get; set; }
        public string typeDescription { get; set; }
        public int typeDnBCode { get; set; }
        public int priority { get; set; }
    }

    public class PrimaryIndustryCode
    {
        public string usSicV4 { get; set; }
        public string usSicV4Description { get; set; }
    }

    public class FamilytreeRolesPlayed
    {
        public string description { get; set; }
        public int dnbCode { get; set; }
    }

    public class EmployeeReliability
    {
        public string description { get; set; }
        public int dnbCode { get; set; }
    }

    public class SalesReliability
    {
        public string description { get; set; }
        public int dnbCode { get; set; }
    }

    public class PhysicalLocation
    {
        public int employeeCount { get; set; }
        public double salesAmount { get; set; }
    }

    public class LinkedCompanies
    {
        public int employeeCount { get; set; }
        public double salesAmount { get; set; }
    }

    public class CountryGroup
    {
        public int employeeCount { get; set; }
        public double salesAmount { get; set; }
    }

    public class PrimarySector
    {
        public string industryCode { get; set; }
        public string industryDescription { get; set; }
        public int typeDnBCode { get; set; }
        public string typeDescription { get; set; }
        public double sectorPercentage { get; set; }
    }

    public class SecondarySector
    {
        public string industryCode { get; set; }
        public string industryDescription { get; set; }
        public int typeDnBCode { get; set; }
        public string typeDescription { get; set; }
        public double sectorPercentage { get; set; }
    }

    public class UnclassifiedSector
    {
    }

    public class GlobalUltimate
    {
        public string duns { get; set; }
        public int employeeCount { get; set; }
        public double salesAmount { get; set; }
        public int familyTreeMembersCount { get; set; }
        public int industrySectorsCount { get; set; }
        public PrimarySector primarySector { get; set; }
        public SecondarySector secondarySector { get; set; }
        public UnclassifiedSector unclassifiedSector { get; set; }
    }

    public class DomesticUltimate
    {
        public string duns { get; set; }
        public int employeeCount { get; set; }
        public double salesAmount { get; set; }
        public int familyTreeMembersCount { get; set; }
        public int industrySectorsCount { get; set; }
        public PrimarySector primarySector { get; set; }
        public SecondarySector secondarySector { get; set; }
        public UnclassifiedSector unclassifiedSector { get; set; }
    }

    public class AssignmentModel
    {
        public bool isStandalone { get; set; }
        public IList<FamilytreeRolesPlayed> familytreeRolesPlayed { get; set; }
        public EmployeeReliability employeeReliability { get; set; }
        public SalesReliability salesReliability { get; set; }
        public bool hasChangeInGlobalUltimate { get; set; }
        public PhysicalLocation physicalLocation { get; set; }
        public LinkedCompanies linkedCompanies { get; set; }
        public CountryGroup countryGroup { get; set; }
        public GlobalUltimate globalUltimate { get; set; }
        public DomesticUltimate domesticUltimate { get; set; }
    }

    public class Telephone
    {
        public string telephoneNumber { get; set; }
        public string isdCode { get; set; }
    }

    public class OrganizationSizeCategory
    {
    }

    public class RegisteredAddress
    {
    }

    public class BusinessEntityType
    {
        public string description { get; set; }
        public int dnbCode { get; set; }
    }

    public class OperatingStatus
    {
        public string description { get; set; }
        public int dnbCode { get; set; }
        public string startDate { get; set; }
    }

    public class OperatingSubStatus
    {
        public string description { get; set; }
        public int dnbCode { get; set; }
        public string startDate { get; set; }
    }

    public class DetailedOperatingStatus
    {
        public string description { get; set; }
        public int dnbCode { get; set; }
    }

    public class SubjectHandlingDetail
    {
        public string description { get; set; }
        public int dnbCode { get; set; }
    }

    public class RecordClass
    {
        public string description { get; set; }
        public int dnbCode { get; set; }
    }

    public class DunsControlStatus
    {
        public OperatingStatus operatingStatus { get; set; }
        public OperatingSubStatus operatingSubStatus { get; set; }
        public DetailedOperatingStatus detailedOperatingStatus { get; set; }
        public bool isMarketable { get; set; }
        public bool isMailUndeliverable { get; set; }
        public bool isTelephoneDisconnected { get; set; }
        public bool isDelisted { get; set; }
        public IList<SubjectHandlingDetail> subjectHandlingDetails { get; set; }
        public string firstReportDate { get; set; }
        public RecordClass recordClass { get; set; }
        public object isSelfRequestedDUNS { get; set; }
        public object selfRequestDate { get; set; }
    }

    public class ControlOwnershipType
    {
    }

    public class YearlyRevenue
    {
        public double value { get; set; }
        public string currency { get; set; }
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
        public IList<YearlyRevenue> yearlyRevenue { get; set; }
    }

    public class RegistrationLocation
    {
        public string addressRegion { get; set; }
    }

    public class LegalForm
    {
        public string description { get; set; }
        public int dnbCode { get; set; }
        public string startDate { get; set; }
        public RegistrationLocation registrationLocation { get; set; }
    }

    public class PreferredLanguage
    {
    }

    public class Operation
    {
        public string description { get; set; }
    }

    public class MailingAddress
    {
    }

    public class RegisteredDetails
    {
        public LegalForm legalForm { get; set; }
    }

    public class EmployerDesignation
    {
    }

    public class CharterType
    {
        public string description { get; set; }
        public int dnbCode { get; set; }
    }

    public class Language
    {
        public string description { get; set; }
        public int dnbCode { get; set; }
    }

    public class Activity
    {
        public string description { get; set; }
        public Language language { get; set; }
    }

    public class BusinessTrustIndex
    {
    }

    public class UnspscCode
    {
        public string code { get; set; }
        public string description { get; set; }
        public int priority { get; set; }
    }

    public class EmployeeCategory
    {
        public string employmentBasisDescription { get; set; }
        public int employmentBasisDnBCode { get; set; }
    }

    public class NumberOfEmployee
    {
        public int value { get; set; }
        public object employeeFiguresDate { get; set; }
        public string informationScopeDescription { get; set; }
        public int informationScopeDnBCode { get; set; }
        public string reliabilityDescription { get; set; }
        public int reliabilityDnBCode { get; set; }
        public IList<EmployeeCategory> employeeCategories { get; set; }
        public IList<object> trend { get; set; }
    }

    public class AddressCountry
    {
        public string name { get; set; }
        public string isoAlpha2Code { get; set; }
    }

    public class ContinentalRegion
    {
        public string name { get; set; }
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
    }

    public class AddressCounty
    {
        public string name { get; set; }
    }

    public class PostalCodePosition
    {
    }

    public class StreetAddress
    {
        public string line1 { get; set; }
        public object line2 { get; set; }
    }

    public class PostOfficeBox
    {
    }

    public class GeographicalPrecision
    {
        public string description { get; set; }
        public int dnbCode { get; set; }
    }

    public class PopulationRank
    {
        public string rankNumber { get; set; }
        public int rankDnBCode { get; set; }
        public string rankDescription { get; set; }
    }

    public class StatisticalArea
    {
        public string cbsaName { get; set; }
        public string cbsaCode { get; set; }
        public string economicAreaOfInfluenceCode { get; set; }
        public PopulationRank populationRank { get; set; }
    }

    public class LocationOwnership
    {
        public string description { get; set; }
        public int dnbCode { get; set; }
    }

    public class PremisesArea
    {
    }

    public class CongressionalDistrict
    {
        public string district { get; set; }
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
        public StreetAddress streetAddress { get; set; }
        public object streetNumber { get; set; }
        public object streetName { get; set; }
        public PostOfficeBox postOfficeBox { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public GeographicalPrecision geographicalPrecision { get; set; }
        public StatisticalArea statisticalArea { get; set; }
        public LocationOwnership locationOwnership { get; set; }
        public PremisesArea premisesArea { get; set; }
        public IList<object> standardAddressCodes { get; set; }
        public bool isManufacturingLocation { get; set; }
        public bool isRegisteredAddress { get; set; }
        public object isResidentialAddress { get; set; }
        public IList<CongressionalDistrict> congressionalDistricts { get; set; }
    }

    public class FranchiseOperationType
    {
    }

    public class Organization
    {
        public string duns { get; set; }
        public object isFortune1000Listed { get; set; }
        public object isForbesLargestPrivateCompaniesListed { get; set; }
        public IList<IndustryCode> industryCodes { get; set; }
        public IList<object> registrationNumbers { get; set; }
        public object isNonClassifiedEstablishment { get; set; }
        public PrimaryIndustryCode primaryIndustryCode { get; set; }
        public string primaryName { get; set; }
        public IList<object> formerPrimaryNames { get; set; }
        public IList<object> formerRegisteredNames { get; set; }
        public AssignmentModel assignmentModel { get; set; }
        public string controlOwnershipDate { get; set; }
        public bool isStandalone { get; set; }
        public IList<Telephone> telephone { get; set; }
        public object isAgent { get; set; }
        public object isImporter { get; set; }
        public object isExporter { get; set; }
        public bool isSmallBusiness { get; set; }
        public OrganizationSizeCategory organizationSizeCategory { get; set; }
        public IList<object> tradeStyleNames { get; set; }
        public RegisteredAddress registeredAddress { get; set; }
        public BusinessEntityType businessEntityType { get; set; }
        public string countryISOAlpha2Code { get; set; }
        public DunsControlStatus dunsControlStatus { get; set; }
        public IList<object> formerRegisteredAddresses { get; set; }
        public ControlOwnershipType controlOwnershipType { get; set; }
        public IList<Financial> financials { get; set; }
        public IList<object> regulations { get; set; }
        public IList<object> summary { get; set; }
        public IList<object> multilingualPrimaryName { get; set; }
        public IList<object> multilingualRegisteredNames { get; set; }
        public IList<object> formerPrimaryAddresses { get; set; }
        public LegalForm legalForm { get; set; }
        public IList<object> multilingualTradestyleNames { get; set; }
        public IList<object> thirdPartyAssessment { get; set; }
        public PreferredLanguage preferredLanguage { get; set; }
        public IList<object> stockExchanges { get; set; }
        public IList<Operation> operations { get; set; }
        public MailingAddress mailingAddress { get; set; }
        public IList<object> standardizedStockExchanges { get; set; }
        public IList<object> websiteAddress { get; set; }
        public string startDate { get; set; }
        public GlobalUltimate globalUltimate { get; set; }
        public DomesticUltimate domesticUltimate { get; set; }
        public IList<object> otherCompetitors { get; set; }
        public object certifiedEmail { get; set; }
        public RegisteredDetails registeredDetails { get; set; }
        public EmployerDesignation employerDesignation { get; set; }
        public IList<object> multilingualRegisteredAddress { get; set; }
        public object investigationDate { get; set; }
        public string registeredName { get; set; }
        public CharterType charterType { get; set; }
        public object tsrReportDate { get; set; }
        public IList<object> multiLingualSearchNames { get; set; }
        public object imperialCalendarStartYear { get; set; }
        public IList<object> banks { get; set; }
        public IList<object> multilingualPrimaryAddress { get; set; }
        public string incorporatedDate { get; set; }
        public object securitiesReportID { get; set; }
        public IList<object> tsrCommodityCodes { get; set; }
        public IList<Activity> activities { get; set; }
        public string defaultCurrency { get; set; }
        public IList<object> email { get; set; }
        public BusinessTrustIndex businessTrustIndex { get; set; }
        public IList<UnspscCode> unspscCodes { get; set; }
        public IList<NumberOfEmployee> numberOfEmployees { get; set; }
        public PrimaryAddress primaryAddress { get; set; }
        public FranchiseOperationType franchiseOperationType { get; set; }
        public IList<object> competitors { get; set; }
    }

    public class BlockStatu
    {
        public string blockID { get; set; }
        public string status { get; set; }
        public object reason { get; set; }
    }

    public class DunsDataResponse
    {
        public TransactionDetail transactionDetail { get; set; }
        public InquiryDetail inquiryDetail { get; set; }
        public Organization organization { get; set; }
        public IList<BlockStatu> blockStatus { get; set; }
    }


}
