using System.Collections.Generic;
using CluedIn.Core.Crawling;

namespace CluedIn.ExternalSearch.Providers.DnB;

public class DnBExternalSearchJobData : CrawlJobData
{
    public DnBExternalSearchJobData(IDictionary<string, object> configuration)
    {
        AcceptedEntityType = GetValue<string>(configuration, DnBConstants.KeyName.AcceptedEntityType);
        OrgNameKey = GetValue<string>(configuration, DnBConstants.KeyName.OrgNameKey);
        DunsNumberKey = GetValue<string>(configuration, DnBConstants.KeyName.DunsNumberKey);
        OrgCountryCodeKey = GetValue<string>(configuration, DnBConstants.KeyName.OrgCountryCodeKey);
        OrgStreetAddressLine1 = GetValue<string>(configuration, DnBConstants.KeyName.OrgStreetAddressLine1Key);
        OrgStreetAddressLine2 = GetValue<string>(configuration, DnBConstants.KeyName.OrgStreetAddressLine2Key);
        OrgPostalCode = GetValue<string>(configuration, DnBConstants.KeyName.OrgPostalCodeKey);
        OrgAddressLocality = GetValue<string>(configuration, DnBConstants.KeyName.OrgAddressLocalityKey);
        OrgAddressCounty = GetValue<string>(configuration, DnBConstants.KeyName.OrgAddressCountyKey);
        OrgAddressRegion = GetValue<string>(configuration, DnBConstants.KeyName.OrgAddressRegionKey);
        OrgTelephoneNumber = GetValue<string>(configuration, DnBConstants.KeyName.OrgTelephoneNumberKey);
        OrgUrl = GetValue<string>(configuration, DnBConstants.KeyName.OrgUrlKey);
        OrgEmail = GetValue<string>(configuration, DnBConstants.KeyName.OrgEmailKey);
        IndustryCodesKey = GetValue<string>(configuration, DnBConstants.KeyName.IndustryCodesKey);
        RegistrationNumbersKey = GetValue<string>(configuration, DnBConstants.KeyName.RegistrationNumbersKey);
        AuthUrl = GetValue<string>(configuration, DnBConstants.KeyName.AuthUrl);
        AuthKey = GetValue<string>(configuration, DnBConstants.KeyName.AuthKey);
        AuthSecret = GetValue<string>(configuration, DnBConstants.KeyName.AuthSecret);
        AuthRequestBody = GetValue<string>(configuration, DnBConstants.KeyName.AuthRequestBody);
        DnBBaseUrl = GetValue<string>(configuration, DnBConstants.KeyName.DnBBaseUrl);
        VersionId = GetValue<string>(configuration, DnBConstants.KeyName.VersionId);
        ProductId = GetValue<string>(configuration, DnBConstants.KeyName.ProductId);
        BlockIds = GetValue<string>(configuration, DnBConstants.KeyName.BlockIds);
        CustomerBillingEndorsement =
            GetValue<string>(configuration, DnBConstants.KeyName.CustomerBillingEndorsementKey);
        CandidateMaximumQuantity = GetValue<string>(configuration, DnBConstants.KeyName.CandidateMaximumQuantityKey);
        ConfidenceLowerLevelThresholdValue =
            GetValue<string>(configuration, DnBConstants.KeyName.ConfidenceLowerLevelThresholdValueKey);
        ExclusionCriteria = GetValue<string>(configuration, DnBConstants.KeyName.ExclusionCriteriaKey);
        IsCleanseAndStandardizeInformationRequired = GetValue<bool>(configuration,
            DnBConstants.KeyName.IsCleanseAndStandardizeInformationRequiredKey);
        TradeUp = GetValue<string>(configuration, DnBConstants.KeyName.TradeUpKey);
        OrderReason = GetValue<string>(configuration, DnBConstants.KeyName.OrderReasonKey);
        CustomerReference1 = GetValue<string>(configuration, DnBConstants.KeyName.CustomerReference1Key);
        CustomerReference2 = GetValue<string>(configuration, DnBConstants.KeyName.CustomerReference2Key);
        CustomerReference3 = GetValue<string>(configuration, DnBConstants.KeyName.CustomerReference3Key);
        CustomerReference4 = GetValue<string>(configuration, DnBConstants.KeyName.CustomerReference4Key);
        CustomerReference5 = GetValue<string>(configuration, DnBConstants.KeyName.CustomerReference5Key);
    }

    public IDictionary<string, object> ToDictionary()
    {
        //return new Dictionary<string, object>();
        return new Dictionary<string, object>
        {
            { DnBConstants.KeyName.AcceptedEntityType, AcceptedEntityType },
            { DnBConstants.KeyName.OrgNameKey, OrgNameKey },
            { DnBConstants.KeyName.DunsNumberKey, DunsNumberKey },
            { DnBConstants.KeyName.OrgCountryCodeKey, OrgCountryCodeKey },
            { DnBConstants.KeyName.OrgStreetAddressLine1Key, OrgStreetAddressLine1 },
            { DnBConstants.KeyName.OrgStreetAddressLine2Key, OrgStreetAddressLine2 },
            { DnBConstants.KeyName.OrgPostalCodeKey, OrgPostalCode },
            { DnBConstants.KeyName.OrgAddressLocalityKey, OrgAddressLocality },
            { DnBConstants.KeyName.OrgAddressCountyKey, OrgAddressCounty },
            { DnBConstants.KeyName.OrgAddressRegionKey, OrgAddressRegion },
            { DnBConstants.KeyName.OrgTelephoneNumberKey, OrgTelephoneNumber },
            { DnBConstants.KeyName.OrgUrlKey, OrgUrl },
            { DnBConstants.KeyName.OrgEmailKey, OrgEmail },
            { DnBConstants.KeyName.IndustryCodesKey, IndustryCodesKey },
            { DnBConstants.KeyName.RegistrationNumbersKey, RegistrationNumbersKey },
            { DnBConstants.KeyName.AuthUrl, AuthUrl },
            { DnBConstants.KeyName.AuthKey, AuthKey },
            { DnBConstants.KeyName.AuthSecret, AuthSecret },
            { DnBConstants.KeyName.AuthRequestBody, AuthRequestBody },
            { DnBConstants.KeyName.DnBBaseUrl, DnBBaseUrl },
            { DnBConstants.KeyName.VersionId, VersionId },
            { DnBConstants.KeyName.ProductId, ProductId },
            { DnBConstants.KeyName.BlockIds, BlockIds },
            { DnBConstants.KeyName.CustomerBillingEndorsementKey, CustomerBillingEndorsement },
            { DnBConstants.KeyName.CandidateMaximumQuantityKey, CandidateMaximumQuantity },
            { DnBConstants.KeyName.ConfidenceLowerLevelThresholdValueKey, ConfidenceLowerLevelThresholdValue },
            { DnBConstants.KeyName.ExclusionCriteriaKey, ExclusionCriteria },
            {
                DnBConstants.KeyName.IsCleanseAndStandardizeInformationRequiredKey,
                IsCleanseAndStandardizeInformationRequired
            },
            { DnBConstants.KeyName.TradeUpKey, TradeUp },
            { DnBConstants.KeyName.OrderReasonKey, OrderReason },
            { DnBConstants.KeyName.CustomerReference1Key, CustomerReference1 },
            { DnBConstants.KeyName.CustomerReference2Key, CustomerReference2 },
            { DnBConstants.KeyName.CustomerReference3Key, CustomerReference3 },
            { DnBConstants.KeyName.CustomerReference4Key, CustomerReference4 },
            { DnBConstants.KeyName.CustomerReference5Key, CustomerReference5 },
        };
    }

    public string AcceptedEntityType { get; set; }
    public string OrgNameKey { get; set; }
    public string DunsNumberKey { get; set; }
    public string OrgCountryCodeKey { get; set; }
    public string OrgStreetAddressLine1 { get; set; }
    public string OrgStreetAddressLine2 { get; set; }
    public string OrgPostalCode { get; set; }
    public string OrgAddressLocality { get; set; }
    public string OrgAddressCounty { get; set; }
    public string OrgAddressRegion { get; set; }
    public string OrgTelephoneNumber { get; set; }
    public string OrgUrl { get; set; }
    public string OrgEmail { get; set; }
    public string IndustryCodesKey { get; set; }
    public string RegistrationNumbersKey { get; set; }
    public string AuthUrl { get; set; }
    public string AuthKey { get; set; }
    public string AuthSecret { get; set; }
    public string AuthRequestBody { get; set; }
    public string DnBBaseUrl { get; set; }
    public string VersionId { get; set; }
    public string ProductId { get; set; }
    public string BlockIds { get; set; }
    public string CustomerBillingEndorsement { get; set; }
    public string CandidateMaximumQuantity { get; set; }
    public string ConfidenceLowerLevelThresholdValue { get; set; }
    public string ExclusionCriteria { get; set; }
    public bool IsCleanseAndStandardizeInformationRequired { get; set; }
    public string TradeUp { get; set; }
    public string OrderReason { get; set; }
    public string CustomerReference1 { get; set; }
    public string CustomerReference2 { get; set; }
    public string CustomerReference3 { get; set; }
    public string CustomerReference4 { get; set; }
    public string CustomerReference5 { get; set; }
}