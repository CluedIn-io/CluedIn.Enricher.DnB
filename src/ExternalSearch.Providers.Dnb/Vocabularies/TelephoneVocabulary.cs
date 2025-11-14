using CluedIn.Core.Data.Vocabularies;

namespace CluedIn.ExternalSearch.Providers.DnB.Vocabularies;

public class TelephoneVocabulary : SimpleVocabulary
{
    public TelephoneVocabulary()
    {
        VocabularyName = "DNBTelephone";
        KeyPrefix = "dnb.telephone";
        KeySeparator = ".";
        Grouping = "/Telephone";
    }
}