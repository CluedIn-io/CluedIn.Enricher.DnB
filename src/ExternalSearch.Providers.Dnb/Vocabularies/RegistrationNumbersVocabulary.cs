using CluedIn.Core.Data.Vocabularies;

namespace CluedIn.ExternalSearch.Providers.DnB.Vocabularies;

public class RegistrationNumbersVocabulary : SimpleVocabulary
{
    public RegistrationNumbersVocabulary()
    {
        VocabularyName = "DNBRegistrationNumbers";
        KeyPrefix = "dnb.registrationNumbers";
        KeySeparator = ".";
        Grouping = "/RegistrationNumbers";
    }
}