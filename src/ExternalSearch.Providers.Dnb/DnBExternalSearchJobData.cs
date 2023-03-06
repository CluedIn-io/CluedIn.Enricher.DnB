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
            OrgAddressKey = GetValue<string>(configuration, DnBConstants.KeyName.OrgAddressKey);
            OrgCountryCodeKey = GetValue<string>(configuration, DnBConstants.KeyName.OrgCountryCodeKey);
            AuthUrl = GetValue<string>(configuration, DnBConstants.KeyName.AuthUrl);
            AuthKey = GetValue<string>(configuration, DnBConstants.KeyName.AuthKey);
            AuthSecret = GetValue<string>(configuration, DnBConstants.KeyName.AuthSecret);
            AuthRequestBody = GetValue<string>(configuration, DnBConstants.KeyName.AuthRequestBody);
            DnBBaseUrl = GetValue<string>(configuration, DnBConstants.KeyName.DnBBaseUrl);
        }

        public IDictionary<string, object> ToDictionary()
        {
            //return new Dictionary<string, object>();
            return new Dictionary<string, object> {
                { DnBConstants.KeyName.AcceptedEntityType, AcceptedEntityType },
                { DnBConstants.KeyName.OrgNameKey, OrgNameKey },
                { DnBConstants.KeyName.OrgAddressKey, OrgAddressKey },
                { DnBConstants.KeyName.OrgCountryCodeKey, OrgCountryCodeKey },
                { DnBConstants.KeyName.AuthUrl, AuthUrl },
                { DnBConstants.KeyName.AuthKey, AuthKey },
                { DnBConstants.KeyName.AuthSecret, AuthSecret },
                { DnBConstants.KeyName.AuthRequestBody, AuthRequestBody },
                { DnBConstants.KeyName.DnBBaseUrl, DnBBaseUrl }
            };
        }

        public string AcceptedEntityType { get; set; }
        public string OrgNameKey { get; set; }
        public string OrgAddressKey { get; set; }
        public string OrgCountryCodeKey { get; set; }
        public string AuthUrl { get; set; }
        public string AuthKey { get; set; }
        public string AuthSecret { get; set; }
        public string AuthRequestBody { get; set; }
        public string DnBBaseUrl { get; set; }

    }
}
