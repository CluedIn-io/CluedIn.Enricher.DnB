using System;
using System.Collections.Generic;

namespace CluedIn.ExternalSearch.Providers.DnB.Models
{

    public class TransactionDetail
    {
        public string transactionID { get; set; }
        public DateTime transactionTimestamp { get; set; }
        public string inLanguage { get; set; }
        public string serviceVersion { get; set; }
    }

    public class StreetAddressLine
    {
        public string line1 { get; set; }
    }

    public class Address
    {
        public string countryISOAlpha2Code { get; set; }
        public string addressLocality { get; set; }
        public string addressRegion { get; set; }
        public string postalCode { get; set; }
        public StreetAddressLine streetAddressLine { get; set; }
    }

    public class InquiryDetail
    {
        public string inLanguage { get; set; }
        public string name { get; set; }
        public Address address { get; set; }
        public bool isCleanseAndStandardizeInformationRequired { get; set; }
    }

    public class WebsiteAddress
    {
        public string url { get; set; }
        public string domainName { get; set; }
    }

    public class OperatingStatus
    {
        public string description { get; set; }
        public int dnbCode { get; set; }
    }

    public class DunsControlStatus
    {
        public OperatingStatus operatingStatus { get; set; }
        public bool isMailUndeliverable { get; set; }
    }

    public class TradeStyleName
    {
        public string name { get; set; }
        public string priority { get; set; }
    }

    public class Telephone
    {
        public string telephoneNumber { get; set; }
        public bool isUnreachable { get; set; }
    }

    public class AddressCountry
    {
        public string isoAlpha2Code { get; set; }
        public string name { get; set; }
    }

    public class AddressLocality
    {
        public string name { get; set; }
    }

    public class AddressRegion
    {
        public object name { get; set; }
        public string abbreviatedName { get; set; }
    }

    public class StreetAddress
    {
        public string line1 { get; set; }
        public object line2 { get; set; }
    }

    public class PrimaryAddress
    {
        public AddressCountry addressCountry { get; set; }
        public AddressLocality addressLocality { get; set; }
        public AddressRegion addressRegion { get; set; }
        public string postalCode { get; set; }
        public string postalCodeExtension { get; set; }
        public StreetAddress streetAddress { get; set; }
    }

    public class MailingAddress
    {
        public AddressCountry addressCountry { get; set; }
        public AddressLocality addressLocality { get; set; }
        public AddressRegion addressRegion { get; set; }
        public string postalCode { get; set; }
        public string postalCodeExtension { get; set; }
        public StreetAddress streetAddress { get; set; }
    }

    public class RegistrationNumber
    {
        public string registrationNumber { get; set; }
        public string typeDescription { get; set; }
        public int dnbTypeCode { get; set; }
    }

    public class MostSeniorPrincipal
    {
        public string fullName { get; set; }
    }

    public class FamilytreeRolesPlayed
    {
        public string description { get; set; }
        public int dnbCode { get; set; }
    }

    public class CorporateLinkage
    {
        public IList<FamilytreeRolesPlayed> familytreeRolesPlayed { get; set; }
    }

    public class Organization
    {
        public string duns { get; set; }
        public IList<WebsiteAddress> websiteAddress { get; set; }
        public DunsControlStatus dunsControlStatus { get; set; }
        public string primaryName { get; set; }
        public IList<TradeStyleName> tradeStyleNames { get; set; }
        public IList<Telephone> telephone { get; set; }
        public PrimaryAddress primaryAddress { get; set; }
        public MailingAddress mailingAddress { get; set; }
        public IList<RegistrationNumber> registrationNumbers { get; set; }
        public IList<MostSeniorPrincipal> mostSeniorPrincipals { get; set; }
        public bool isStandalone { get; set; }
        public CorporateLinkage corporateLinkage { get; set; }
    }

    public class MatchGradeComponent
    {
        public string componentType { get; set; }
        public string componentRating { get; set; }
    }

    public class MatchDataProfileComponent
    {
        public string componentType { get; set; }
        public string componentValue { get; set; }
    }

    public class MatchQualityInformation
    {
        public int confidenceCode { get; set; }
        public string matchGrade { get; set; }
        public int matchGradeComponentsCount { get; set; }
        public IList<MatchGradeComponent> matchGradeComponents { get; set; }
        public string matchDataProfile { get; set; }
        public int matchDataProfileComponentsCount { get; set; }
        public IList<MatchDataProfileComponent> matchDataProfileComponents { get; set; }
        public double nameMatchScore { get; set; }
    }

    public class MatchCandidate
    {
        public int displaySequence { get; set; }
        public Organization organization { get; set; }
        public MatchQualityInformation matchQualityInformation { get; set; }
    }

    public class AddressCounty
    {
        public string name { get; set; }
    }

    public class StandardizedAddress
    {
        public AddressCountry addressCountry { get; set; }
        public AddressLocality addressLocality { get; set; }
        public AddressRegion addressRegion { get; set; }
        public AddressCounty addressCounty { get; set; }
        public string postalCode { get; set; }
        public object postalCodeExtension { get; set; }
        public StreetAddress streetAddress { get; set; }
        public object deliveryPointValidationStatus { get; set; }
        public object deliveryPointValidationCMRAValue { get; set; }
        public bool isInexactAddress { get; set; }
        public string addressType { get; set; }
    }

    public class CleanseAndStandardizeInformation
    {
        public string standardizedName { get; set; }
        public StandardizedAddress standardizedAddress { get; set; }
    }

    public class DnBResponse
    {
        public TransactionDetail transactionDetail { get; set; }
        public InquiryDetail inquiryDetail { get; set; }
        public int candidatesMatchedQuantity { get; set; }
        public string matchDataCriteria { get; set; }
        public IList<MatchCandidate> matchCandidates { get; set; }
        public CleanseAndStandardizeInformation cleanseAndStandardizeInformation { get; set; }
    }
}