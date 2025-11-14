using CluedIn.Core.Data.Vocabularies;

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
            OperatingStatusCode = group.Add(new VocabularyKey(nameof(OperatingStatusCode), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            OperatingStatusDescription = group.Add(new VocabularyKey(nameof(OperatingStatusDescription), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            ISO2CountryCode = group.Add(new VocabularyKey(nameof(ISO2CountryCode), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            GlobalUltimateISO2CountryCode = group.Add(new VocabularyKey(nameof(GlobalUltimateISO2CountryCode), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            DomesticUltimateISO2CountryCode = group.Add(new VocabularyKey(nameof(DomesticUltimateISO2CountryCode), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
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
            WebsiteUrl = group.Add(new VocabularyKey(nameof(WebsiteUrl), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
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
        });

    }
    public VocabularyKey Duns { get; protected set; }
    public VocabularyKey MatchConfidenceCode { get; protected set; }
    public VocabularyKey PrimaryBusinessName { get; protected set; }
    public VocabularyKey DomesticUltimateDuns { get; protected set; }
    public VocabularyKey GlobalUltimateDuns { get; protected set; }
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
    public VocabularyKey WebsiteUrl { get; protected set; }
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
}