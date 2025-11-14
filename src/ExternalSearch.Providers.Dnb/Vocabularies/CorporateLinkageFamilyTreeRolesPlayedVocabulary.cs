using CluedIn.Core.Data.Vocabularies;

namespace CluedIn.ExternalSearch.Providers.DnB.Vocabularies;

public class CorporateLinkageFamilyTreeRolesPlayedVocabulary : SimpleVocabulary
{
    public CorporateLinkageFamilyTreeRolesPlayedVocabulary()
    {
        VocabularyName = "DNBCorporateLinkageFamilyTreeRolesPlayedVocabulary";
        KeyPrefix = "dnb.corporateLinkage.familyTreeRolesPlayedVocabulary";
        KeySeparator = ".";
        Grouping = "/CorporateLinkageFamilyTreeRolesPlayedVocabulary";
    }
}