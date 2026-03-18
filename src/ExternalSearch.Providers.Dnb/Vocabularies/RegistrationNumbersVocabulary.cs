using CluedIn.Core.Data.Vocabularies;

namespace CluedIn.ExternalSearch.Providers.DnB.Vocabularies;

public class RegistrationNumbersVocabulary : SimpleVocabulary
{
    public RegistrationNumbersVocabulary()
    {
        VocabularyName = "DNBRegistrationNumbers";
        KeyPrefix = "DnB.registrationNumbers";
        KeySeparator = ".";
        Grouping = "/RegistrationNumbers";
    }
}