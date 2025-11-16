using CluedIn.Core.Data.Vocabularies;

namespace CluedIn.ExternalSearch.Providers.DnB.Vocabularies;

public class WebsiteAddressVocabulary : SimpleVocabulary
{
    public WebsiteAddressVocabulary()
    {
        VocabularyName = "DNBWebsiteAddress";
        KeyPrefix = "DnB.websiteAddress";
        KeySeparator = ".";
        Grouping = "/WebsiteAddress";
    }
}