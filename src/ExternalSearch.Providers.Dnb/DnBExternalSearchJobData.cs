using System.Collections.Generic;
using CluedIn.Core.Crawling;

namespace CluedIn.ExternalSearch.Providers.DnB
{
    public class DnBExternalSearchJobData : CrawlJobData
    {
        public DnBExternalSearchJobData(IDictionary<string, object> configuration)
        {
            AcceptedEntityType = GetValue<string>(configuration, DnBConstants.KeyName.AcceptedEntityType);
            OrgNameKey = GetValue<string>(configuration, DnBConstants.KeyName.OrgNameKey);
            DunsNumberKey = GetValue<string>(configuration, DnBConstants.KeyName.DunsNumberKey);
            OrgAddressKey = GetValue<string>(configuration, DnBConstants.KeyName.OrgAddressKey);
            OrgCountryCodeKey = GetValue<string>(configuration, DnBConstants.KeyName.OrgCountryCodeKey);
            AuthUrl = GetValue<string>(configuration, DnBConstants.KeyName.AuthUrl);
            AuthKey = GetValue<string>(configuration, DnBConstants.KeyName.AuthKey);
            AuthSecret = GetValue<string>(configuration, DnBConstants.KeyName.AuthSecret);
            AuthRequestBody = GetValue<string>(configuration, DnBConstants.KeyName.AuthRequestBody);
            DnBBaseUrl = GetValue<string>(configuration, DnBConstants.KeyName.DnBBaseUrl);
            VersionId = GetValue<string>(configuration, DnBConstants.KeyName.VersionId);
            ProductId = GetValue<string>(configuration, DnBConstants.KeyName.ProductId);
            BlockIds = GetValue<string>(configuration, DnBConstants.KeyName.BlockIds);
        }

        public IDictionary<string, object> ToDictionary()
        {
            //return new Dictionary<string, object>();
            return new Dictionary<string, object> {
                { DnBConstants.KeyName.AcceptedEntityType, AcceptedEntityType },
                { DnBConstants.KeyName.OrgNameKey, OrgNameKey },
                { DnBConstants.KeyName.DunsNumberKey, DunsNumberKey },
                { DnBConstants.KeyName.OrgAddressKey, OrgAddressKey },
                { DnBConstants.KeyName.OrgCountryCodeKey, OrgCountryCodeKey },
                { DnBConstants.KeyName.AuthUrl, AuthUrl },
                { DnBConstants.KeyName.AuthKey, AuthKey },
                { DnBConstants.KeyName.AuthSecret, AuthSecret },
                { DnBConstants.KeyName.AuthRequestBody, AuthRequestBody },
                { DnBConstants.KeyName.DnBBaseUrl, DnBBaseUrl },
                { DnBConstants.KeyName.VersionId, VersionId },
                { DnBConstants.KeyName.ProductId, ProductId },
                { DnBConstants.KeyName.BlockIds, BlockIds },

            };
        }

        public string AcceptedEntityType { get; set; }
        public string OrgNameKey { get; set; }
        public string DunsNumberKey { get; set; }
        public string OrgAddressKey { get; set; }
        public string OrgCountryCodeKey { get; set; }
        public string AuthUrl { get; set; }
        public string AuthKey { get; set; }
        public string AuthSecret { get; set; }
        public string AuthRequestBody { get; set; }
        public string DnBBaseUrl { get; set; }
        public string VersionId { get; set; }
        public string ProductId { get; set; }
        public string BlockIds { get; set; }

    }
}
