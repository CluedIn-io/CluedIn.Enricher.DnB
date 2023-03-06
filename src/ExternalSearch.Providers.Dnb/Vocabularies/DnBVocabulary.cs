using CluedIn.Core.Data.Vocabularies;
using Humanizer;

namespace CluedIn.ExternalSearch.Providers.DnB.Vocabularies
{
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
                //MailingAddressCountry = group.Add(new VocabularyKey(nameof(MailingAddressCountry), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                //MailingAddressCountryCode = group.Add(new VocabularyKey(nameof(MailingAddressCountryCode), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                //MailingAddressLocality = group.Add(new VocabularyKey(nameof(MailingAddressLocality), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                //MailingAddressRegionAbbreviatedName = group.Add(new VocabularyKey(nameof(MailingAddressRegionAbbreviatedName), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                //MailingAddressRegionName = group.Add(new VocabularyKey(nameof(MailingAddressRegionName), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                //MailingAddressPostalCode = group.Add(new VocabularyKey(nameof(MailingAddressPostalCode), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                //MailingAddressAddressLine1 = group.Add(new VocabularyKey(nameof(MailingAddressAddressLine1), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                //MailingAddressAddressLine2 = group.Add(new VocabularyKey(nameof(MailingAddressAddressLine2), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                MatchConfidenceCode = group.Add(new VocabularyKey(nameof(MatchConfidenceCode), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                DomesticUltimateDuns = group.Add(new VocabularyKey(nameof(DomesticUltimateDuns), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible)).WithDisplayName("Domestic Ultimate D-U-N-S Number");
                GlobalUltimateDuns = group.Add(new VocabularyKey(nameof(GlobalUltimateDuns), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible)).WithDisplayName("Global Ultimate D-U-N-S Number");
                OperatingStatusCode = group.Add(new VocabularyKey(nameof(OperatingStatusCode), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                OperatingStatusDescription = group.Add(new VocabularyKey(nameof(OperatingStatusDescription), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                ISO2CountryCode = group.Add(new VocabularyKey(nameof(ISO2CountryCode), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                PrimaryBusinessName = group.Add(new VocabularyKey(nameof(PrimaryBusinessName), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                PrimaryAddressCountry = group.Add(new VocabularyKey(nameof(PrimaryAddressCountry), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                PrimaryAddressCountyName = group.Add(new VocabularyKey(nameof(PrimaryAddressCountyName), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                PrimaryAddressLocality = group.Add(new VocabularyKey(nameof(PrimaryAddressLocality), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                PrimaryAddressPostalCode = group.Add(new VocabularyKey(nameof(PrimaryAddressPostalCode), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                PrimaryAddressPostalCodeExtension = group.Add(new VocabularyKey(nameof(PrimaryAddressPostalCodeExtension), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                PrimaryAddressRegionAbbreviatedName = group.Add(new VocabularyKey(nameof(PrimaryAddressRegionAbbreviatedName), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                PrimaryAddressRegionName = group.Add(new VocabularyKey(nameof(PrimaryAddressRegionName), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                PrimaryAddressStreetLine1 = group.Add(new VocabularyKey(nameof(PrimaryAddressStreetLine1), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                PrimaryAddressStreetLine2 = group.Add(new VocabularyKey(nameof(PrimaryAddressStreetLine2), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                WebsiteUrl = group.Add(new VocabularyKey(nameof(WebsiteUrl), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                RegistrationNumber2 = group.Add(new VocabularyKey(nameof(RegistrationNumber2), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            });

        }
        public VocabularyKey Duns { get; protected set; }
        //public VocabularyKey MailingAddressCountry { get; protected set; }
        //public VocabularyKey MailingAddressCountryCode { get; protected set; }
        //public VocabularyKey MailingAddressLocality { get; protected set; }
        //public VocabularyKey MailingAddressRegionAbbreviatedName { get; protected set; }
        //public VocabularyKey MailingAddressRegionName { get; protected set; }
        //public VocabularyKey MailingAddressPostalCode { get; protected set; }
        //public VocabularyKey MailingAddressAddressLine1 { get; protected set; }
        //public VocabularyKey MailingAddressAddressLine2 { get; protected set; }
        public VocabularyKey MatchConfidenceCode { get; protected set; }
        public VocabularyKey PrimaryBusinessName { get; protected set; }
        public VocabularyKey ISO2CountryCode { get; protected set; }
        public VocabularyKey DomesticUltimateDuns { get; protected set; }
        public VocabularyKey GlobalUltimateDuns { get; protected set; }
        public VocabularyKey OperatingStatusCode { get; protected set; }
        public VocabularyKey OperatingStatusDescription { get; protected set; }
        public VocabularyKey PrimaryAddressCountry { get; protected set; }
        public VocabularyKey PrimaryAddressCountyName { get; protected set; }
        public VocabularyKey PrimaryAddressLocality { get; protected set; }
        public VocabularyKey PrimaryAddressRegionAbbreviatedName { get; protected set; }
        public VocabularyKey PrimaryAddressRegionName { get; protected set; }
        public VocabularyKey PrimaryAddressPostalCode { get; protected set; }
        public VocabularyKey PrimaryAddressPostalCodeExtension { get; protected set; }
        public VocabularyKey PrimaryAddressStreetLine1 { get; protected set; }
        public VocabularyKey PrimaryAddressStreetLine2 { get; protected set; }
        public VocabularyKey WebsiteUrl { get; protected set; }
        public VocabularyKey RegistrationNumber2 { get; protected set; }
    }
}
