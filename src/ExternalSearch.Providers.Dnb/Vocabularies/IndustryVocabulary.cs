using CluedIn.Core.Data.Vocabularies;

namespace CluedIn.ExternalSearch.Providers.DnB.Vocabularies;

public class IndustryVocabulary : SimpleVocabulary
{
    public IndustryVocabulary()
    {
        VocabularyName = "DNBIndustry";
        KeyPrefix = "DnB.industry";
        KeySeparator = ".";
        Grouping = "/Industry";
    }
}