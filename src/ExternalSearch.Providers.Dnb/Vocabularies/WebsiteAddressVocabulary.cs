using CluedIn.Core.Data.Vocabularies;

namespace CluedIn.ExternalSearch.Providers.DnB.Vocabularies;

public class WebsiteAddressVocabulary : SimpleVocabulary
{
    public WebsiteAddressVocabulary()
    {
        VocabularyName = "DNBWebsiteAddress";
        KeyPrefix = "dnb.websiteAddress";
        KeySeparator = ".";
        Grouping = "/WebsiteAddress";
    }
}