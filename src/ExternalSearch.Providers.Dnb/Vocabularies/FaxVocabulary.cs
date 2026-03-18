using CluedIn.Core.Data.Vocabularies;

namespace CluedIn.ExternalSearch.Providers.DnB.Vocabularies;

public class FaxVocabulary : SimpleVocabulary
{
    public FaxVocabulary()
    {
        VocabularyName = "DNBFax";
        KeyPrefix = "DnB.fax";
        KeySeparator = ".";
        Grouping = "/Fax";
    }
}