using CluedIn.Core.Data.Vocabularies;

namespace CluedIn.ExternalSearch.Providers.DnB.Vocabularies;

public class TelephoneVocabulary : SimpleVocabulary
{
    public TelephoneVocabulary()
    {
        VocabularyName = "DNBTelephone";
        KeyPrefix = "DnB.telephone";
        KeySeparator = ".";
        Grouping = "/Telephone";
    }
}