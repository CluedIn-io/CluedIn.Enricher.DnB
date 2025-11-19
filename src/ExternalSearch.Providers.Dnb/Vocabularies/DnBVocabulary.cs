using CluedIn.Core.Data.Vocabularies;
using CluedIn.ExternalSearch.Providers.DnB.Model.DnBResponse;

namespace CluedIn.ExternalSearch.Providers.DnB.Vocabularies;

public class DnBVocabulary : SimpleVocabulary
{
    public DnBVocabulary()
    {
        VocabularyName = "DnB";
        KeyPrefix = "DnB";
        KeySeparator = ".";
        Grouping = "/BusinessPartner";

        AddGroup("D&B Information", group =>
        {
            Duns = group.Add(new VocabularyKey(nameof(Duns), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible).WithDisplayName("D-U-N-S Number"));
            DomesticUltimateDuns = group.Add(new VocabularyKey(nameof(DomesticUltimateDuns), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible)).WithDisplayName("Domestic Ultimate D-U-N-S Number");
            GlobalUltimateDuns = group.Add(new VocabularyKey(nameof(GlobalUltimateDuns), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible)).WithDisplayName("Global Ultimate D-U-N-S Number");
            ParentDuns = group.Add(new VocabularyKey(nameof(ParentDuns), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible)).WithDisplayName("Parent D-U-N-S Number");
            HeadQuarterDuns = group.Add(new VocabularyKey(nameof(HeadQuarterDuns), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible)).WithDisplayName("Head Quarter D-U-N-S Number");
            OperatingStatusCode = group.Add(new VocabularyKey(nameof(OperatingStatusCode), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            OperatingStatusDescription = group.Add(new VocabularyKey(nameof(OperatingStatusDescription), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            ISO2CountryCode = group.Add(new VocabularyKey(nameof(ISO2CountryCode), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            GlobalUltimateISO2CountryCode = group.Add(new VocabularyKey(nameof(GlobalUltimateISO2CountryCode), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            DomesticUltimateISO2CountryCode = group.Add(new VocabularyKey(nameof(DomesticUltimateISO2CountryCode), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            ParentISO2CountryCode = group.Add(new VocabularyKey(nameof(ParentISO2CountryCode), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            HeadQuarterISO2CountryCode = group.Add(new VocabularyKey(nameof(HeadQuarterISO2CountryCode), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            PrimaryBusinessName = group.Add(new VocabularyKey(nameof(PrimaryBusinessName), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            PrimaryAddressCountry = group.Add(new VocabularyKey(nameof(PrimaryAddressCountry), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            PrimaryAddressCountyName = group.Add(new VocabularyKey(nameof(PrimaryAddressCountyName), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            PrimaryAddressLocality = group.Add(new VocabularyKey(nameof(PrimaryAddressLocality), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            PrimaryAddressPostalCode = group.Add(new VocabularyKey(nameof(PrimaryAddressPostalCode), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            PrimaryAddressRegionAbbreviatedName = group.Add(new VocabularyKey(nameof(PrimaryAddressRegionAbbreviatedName), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            PrimaryAddressRegionName = group.Add(new VocabularyKey(nameof(PrimaryAddressRegionName), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            PrimaryAddressStreetLine1 = group.Add(new VocabularyKey(nameof(PrimaryAddressStreetLine1), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            PrimaryAddressStreetLine2 = group.Add(new VocabularyKey(nameof(PrimaryAddressStreetLine2), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            GlobalUltimatePrimaryAddressCountry = group.Add(new VocabularyKey(nameof(GlobalUltimatePrimaryAddressCountry), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            GlobalUltimatePrimaryAddressCountyName = group.Add(new VocabularyKey(nameof(GlobalUltimatePrimaryAddressCountyName), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            GlobalUltimatePrimaryAddressLocality = group.Add(new VocabularyKey(nameof(GlobalUltimatePrimaryAddressLocality), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            GlobalUltimatePrimaryAddressPostalCode = group.Add(new VocabularyKey(nameof(GlobalUltimatePrimaryAddressPostalCode), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            PrimaryAddressPostalCodeExtension = group.Add(new VocabularyKey(nameof(PrimaryAddressPostalCodeExtension), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            GlobalUltimatePrimaryAddressRegionAbbreviatedName = group.Add(new VocabularyKey(nameof(GlobalUltimatePrimaryAddressRegionAbbreviatedName), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            GlobalUltimatePrimaryAddressRegionName = group.Add(new VocabularyKey(nameof(GlobalUltimatePrimaryAddressRegionName), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            GlobalUltimatePrimaryAddressStreetLine1 = group.Add(new VocabularyKey(nameof(GlobalUltimatePrimaryAddressStreetLine1), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            GlobalUltimatePrimaryAddressStreetLine2 = group.Add(new VocabularyKey(nameof(GlobalUltimatePrimaryAddressStreetLine2), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            DomesticUltimatePrimaryAddressCountry = group.Add(new VocabularyKey(nameof(DomesticUltimatePrimaryAddressCountry), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            DomesticUltimatePrimaryAddressCountyName = group.Add(new VocabularyKey(nameof(DomesticUltimatePrimaryAddressCountyName), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            DomesticUltimatePrimaryAddressLocality = group.Add(new VocabularyKey(nameof(DomesticUltimatePrimaryAddressLocality), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            DomesticUltimatePrimaryAddressPostalCode = group.Add(new VocabularyKey(nameof(DomesticUltimatePrimaryAddressPostalCode), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            DomesticUltimatePrimaryAddressRegionAbbreviatedName = group.Add(new VocabularyKey(nameof(DomesticUltimatePrimaryAddressRegionAbbreviatedName), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            DomesticUltimatePrimaryAddressRegionName = group.Add(new VocabularyKey(nameof(DomesticUltimatePrimaryAddressRegionName), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            DomesticUltimatePrimaryAddressStreetLine1 = group.Add(new VocabularyKey(nameof(DomesticUltimatePrimaryAddressStreetLine1), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            DomesticUltimatePrimaryAddressStreetLine2 = group.Add(new VocabularyKey(nameof(DomesticUltimatePrimaryAddressStreetLine2), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            ParentPrimaryAddressCountry = group.Add(new VocabularyKey(nameof(ParentPrimaryAddressCountry), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            ParentPrimaryAddressCountyName = group.Add(new VocabularyKey(nameof(ParentPrimaryAddressCountyName), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            ParentPrimaryAddressLocality = group.Add(new VocabularyKey(nameof(ParentPrimaryAddressLocality), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            ParentPrimaryAddressPostalCode = group.Add(new VocabularyKey(nameof(ParentPrimaryAddressPostalCode), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            ParentPrimaryAddressRegionAbbreviatedName = group.Add(new VocabularyKey(nameof(ParentPrimaryAddressRegionAbbreviatedName), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            ParentPrimaryAddressRegionName = group.Add(new VocabularyKey(nameof(ParentPrimaryAddressRegionName), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            ParentPrimaryAddressStreetLine1 = group.Add(new VocabularyKey(nameof(ParentPrimaryAddressStreetLine1), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            ParentPrimaryAddressStreetLine2 = group.Add(new VocabularyKey(nameof(ParentPrimaryAddressStreetLine2), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            HeadQuarterPrimaryAddressCountry = group.Add(new VocabularyKey(nameof(HeadQuarterPrimaryAddressCountry), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            HeadQuarterPrimaryAddressCountyName = group.Add(new VocabularyKey(nameof(HeadQuarterPrimaryAddressCountyName), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            HeadQuarterPrimaryAddressLocality = group.Add(new VocabularyKey(nameof(HeadQuarterPrimaryAddressLocality), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            HeadQuarterPrimaryAddressPostalCode = group.Add(new VocabularyKey(nameof(HeadQuarterPrimaryAddressPostalCode), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            HeadQuarterPrimaryAddressRegionAbbreviatedName = group.Add(new VocabularyKey(nameof(HeadQuarterPrimaryAddressRegionAbbreviatedName), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            HeadQuarterPrimaryAddressRegionName = group.Add(new VocabularyKey(nameof(HeadQuarterPrimaryAddressRegionName), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            HeadQuarterPrimaryAddressStreetLine1 = group.Add(new VocabularyKey(nameof(HeadQuarterPrimaryAddressStreetLine1), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            HeadQuarterPrimaryAddressStreetLine2 = group.Add(new VocabularyKey(nameof(HeadQuarterPrimaryAddressStreetLine2), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            WebsiteUrl = group.Add(new VocabularyKey(nameof(WebsiteUrl), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            Telephone = group.Add(new VocabularyKey(nameof(Telephone), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            Fax = group.Add(new VocabularyKey(nameof(Fax), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            RegistrationNumber2 = group.Add(new VocabularyKey(nameof(RegistrationNumber2), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            DunsControlStatusFullReportDate = group.Add(new VocabularyKey(nameof(DunsControlStatusFullReportDate), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            DunsControlStatusLastUpdateDate = group.Add(new VocabularyKey(nameof(DunsControlStatusLastUpdateDate), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            DunsControlStatusOperatingStatusDescription = group.Add(new VocabularyKey(nameof(DunsControlStatusOperatingStatusDescription), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            DunsControlStatusOperatingStatusDnbCode = group.Add(new VocabularyKey(nameof(DunsControlStatusOperatingStatusDnbCode), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            DunsControlStatusIsMarketable = group.Add(new VocabularyKey(nameof(DunsControlStatusIsMarketable), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            DunsControlStatusIsMailUndeliverable = group.Add(new VocabularyKey(nameof(DunsControlStatusIsMailUndeliverable), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            DunsControlStatusIsTelephoneDisconnected = group.Add(new VocabularyKey(nameof(DunsControlStatusIsTelephoneDisconnected), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            DunsControlStatusIsDelisted = group.Add(new VocabularyKey(nameof(DunsControlStatusIsDelisted), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            DunsControlStatusSubjectHandlingDetails = group.Add(new VocabularyKey(nameof(DunsControlStatusSubjectHandlingDetails), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            BusinessEntityTypeDnbCode = group.Add(new VocabularyKey(nameof(BusinessEntityTypeDnbCode), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            BusinessEntityTypeDescription = group.Add(new VocabularyKey(nameof(BusinessEntityTypeDescription), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            MatchConfidenceCode = group.Add(new VocabularyKey(nameof(MatchConfidenceCode), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            ConfidenceScore = group.Add(new VocabularyKey("_cluedin_confidenceScore"));
            HierarchyLevel = group.Add(new VocabularyKey(nameof(HierarchyLevel), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            GlobalUltimateFamilyTreeMembersCount = group.Add(new VocabularyKey(nameof(GlobalUltimateFamilyTreeMembersCount), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            TradeStyleNames = group.Add(new VocabularyKey(nameof(TradeStyleNames), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            StockExchangeTickerName = group.Add(new VocabularyKey(nameof(StockExchangeTickerName), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            StockExchangeName = group.Add(new VocabularyKey(nameof(StockExchangeName), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            StockExchangeCountryCode = group.Add(new VocabularyKey(nameof(StockExchangeCountryCode), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            NumberOfEmployees = group.Add(new VocabularyKey(nameof(NumberOfEmployees), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            GlobalUltimateNumberOfEmployees = group.Add(new VocabularyKey(nameof(GlobalUltimateNumberOfEmployees), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            DomesticUltimateNumberOfEmployees = group.Add(new VocabularyKey(nameof(DomesticUltimateNumberOfEmployees), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            YearlyRevenue = group.Add(new VocabularyKey(nameof(YearlyRevenue), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            GlobalUltimateYearlyRevenue = group.Add(new VocabularyKey(nameof(GlobalUltimateYearlyRevenue), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            DomesticUltimateYearlyRevenue = group.Add(new VocabularyKey(nameof(DomesticUltimateYearlyRevenue), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
        });
    }

    public VocabularyKey Duns { get; protected set; }
    public VocabularyKey MatchConfidenceCode { get; protected set; }
    public VocabularyKey PrimaryBusinessName { get; protected set; }
    public VocabularyKey DomesticUltimateDuns { get; protected set; }
    public VocabularyKey GlobalUltimateDuns { get; protected set; }
    public VocabularyKey ParentDuns { get; protected set; }
    public VocabularyKey HeadQuarterDuns { get; protected set; }
    public VocabularyKey OperatingStatusCode { get; protected set; }
    public VocabularyKey OperatingStatusDescription { get; protected set; }
    public VocabularyKey ISO2CountryCode { get; protected set; }
    public VocabularyKey PrimaryAddressCountry { get; protected set; }
    public VocabularyKey PrimaryAddressCountyName { get; protected set; }
    public VocabularyKey PrimaryAddressLocality { get; protected set; }
    public VocabularyKey PrimaryAddressRegionAbbreviatedName { get; protected set; }
    public VocabularyKey PrimaryAddressRegionName { get; protected set; }
    public VocabularyKey PrimaryAddressPostalCode { get; protected set; }
    public VocabularyKey PrimaryAddressStreetLine1 { get; protected set; }
    public VocabularyKey PrimaryAddressStreetLine2 { get; protected set; }
    public VocabularyKey GlobalUltimateISO2CountryCode { get; protected set; }
    public VocabularyKey GlobalUltimatePrimaryAddressCountry { get; protected set; }
    public VocabularyKey GlobalUltimatePrimaryAddressCountyName { get; protected set; }
    public VocabularyKey GlobalUltimatePrimaryAddressLocality { get; protected set; }
    public VocabularyKey GlobalUltimatePrimaryAddressRegionAbbreviatedName { get; protected set; }
    public VocabularyKey GlobalUltimatePrimaryAddressRegionName { get; protected set; }
    public VocabularyKey GlobalUltimatePrimaryAddressPostalCode { get; protected set; }
    public VocabularyKey PrimaryAddressPostalCodeExtension { get; protected set; }
    public VocabularyKey GlobalUltimatePrimaryAddressStreetLine1 { get; protected set; }
    public VocabularyKey GlobalUltimatePrimaryAddressStreetLine2 { get; protected set; }
    public VocabularyKey DomesticUltimateISO2CountryCode { get; protected set; }
    public VocabularyKey DomesticUltimatePrimaryAddressCountry { get; protected set; }
    public VocabularyKey DomesticUltimatePrimaryAddressCountyName { get; protected set; }
    public VocabularyKey DomesticUltimatePrimaryAddressLocality { get; protected set; }
    public VocabularyKey DomesticUltimatePrimaryAddressRegionAbbreviatedName { get; protected set; }
    public VocabularyKey DomesticUltimatePrimaryAddressRegionName { get; protected set; }
    public VocabularyKey DomesticUltimatePrimaryAddressPostalCode { get; protected set; }
    public VocabularyKey DomesticUltimatePrimaryAddressStreetLine1 { get; protected set; }
    public VocabularyKey DomesticUltimatePrimaryAddressStreetLine2 { get; protected set; }
    public VocabularyKey ParentISO2CountryCode { get; protected set; }
    public VocabularyKey ParentPrimaryAddressCountry { get; protected set; }
    public VocabularyKey ParentPrimaryAddressCountyName { get; protected set; }
    public VocabularyKey ParentPrimaryAddressLocality { get; protected set; }
    public VocabularyKey ParentPrimaryAddressRegionAbbreviatedName { get; protected set; }
    public VocabularyKey ParentPrimaryAddressRegionName { get; protected set; }
    public VocabularyKey ParentPrimaryAddressPostalCode { get; protected set; }
    public VocabularyKey ParentPrimaryAddressStreetLine1 { get; protected set; }
    public VocabularyKey ParentPrimaryAddressStreetLine2 { get; protected set; }
    public VocabularyKey HeadQuarterISO2CountryCode { get; protected set; }
    public VocabularyKey HeadQuarterPrimaryAddressCountry { get; protected set; }
    public VocabularyKey HeadQuarterPrimaryAddressCountyName { get; protected set; }
    public VocabularyKey HeadQuarterPrimaryAddressLocality { get; protected set; }
    public VocabularyKey HeadQuarterPrimaryAddressRegionAbbreviatedName { get; protected set; }
    public VocabularyKey HeadQuarterPrimaryAddressRegionName { get; protected set; }
    public VocabularyKey HeadQuarterPrimaryAddressPostalCode { get; protected set; }
    public VocabularyKey HeadQuarterPrimaryAddressStreetLine1 { get; protected set; }
    public VocabularyKey HeadQuarterPrimaryAddressStreetLine2 { get; protected set; }
    public VocabularyKey WebsiteUrl { get; protected set; }
    public VocabularyKey Telephone { get; protected set; }
    public VocabularyKey Fax { get; protected set; }
    public VocabularyKey RegistrationNumber2 { get; protected set; }
    public VocabularyKey DunsControlStatusFullReportDate { get; protected set; }
    public VocabularyKey DunsControlStatusLastUpdateDate { get; protected set; }
    public VocabularyKey DunsControlStatusOperatingStatusDescription { get; protected set; }
    public VocabularyKey DunsControlStatusOperatingStatusDnbCode { get; protected set; }
    public VocabularyKey DunsControlStatusIsMarketable { get; protected set; }
    public VocabularyKey DunsControlStatusIsMailUndeliverable { get; protected set; }
    public VocabularyKey DunsControlStatusIsTelephoneDisconnected { get; protected set; }
    public VocabularyKey DunsControlStatusIsDelisted { get; protected set; }
    public VocabularyKey DunsControlStatusSubjectHandlingDetails { get; protected set; }
    public VocabularyKey BusinessEntityTypeDnbCode { get; protected set; }
    public VocabularyKey BusinessEntityTypeDescription { get; protected set; }
    public VocabularyKey ConfidenceScore { get; protected set; }
    public VocabularyKey HierarchyLevel { get; protected set; }
    public VocabularyKey GlobalUltimateFamilyTreeMembersCount { get; protected set; }
    public VocabularyKey TradeStyleNames { get; protected set; }
    public VocabularyKey StockExchangeTickerName { get; protected set; }
    public VocabularyKey StockExchangeName { get; protected set; }
    public VocabularyKey StockExchangeCountryCode { get; protected set; }
    public VocabularyKey NumberOfEmployees { get; protected set; }
    public VocabularyKey GlobalUltimateNumberOfEmployees { get; protected set; }
    public VocabularyKey DomesticUltimateNumberOfEmployees { get; protected set; }
    public VocabularyKey YearlyRevenue { get; protected set; }
    public VocabularyKey GlobalUltimateYearlyRevenue { get; protected set; }
    public VocabularyKey DomesticUltimateYearlyRevenue { get; protected set; }
}