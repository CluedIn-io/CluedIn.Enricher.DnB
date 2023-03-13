using CluedIn.Core.Data;
using CluedIn.Core.Data.Vocabularies;

namespace CluedIn.ExternalSearch.Providers.DnB.Vocabularies
{
    public static class StaticDnBVocabulary
    {
        static StaticDnBVocabulary()
        {
            BusinessPartner = new DnBVocabulary();
            Industry = new IndustryVocabulary();
        }
        public static DnBVocabulary BusinessPartner { get; private set; }

        public static IndustryVocabulary Industry { get; private set; }
    }
}
