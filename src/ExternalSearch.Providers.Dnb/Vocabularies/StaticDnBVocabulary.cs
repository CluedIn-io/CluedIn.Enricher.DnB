using CluedIn.Core.Data;
using CluedIn.Core.Data.Vocabularies;

namespace CluedIn.ExternalSearch.Providers.DnB.Vocabularies
{
    public static class StaticDnBVocabulary
    {
        static StaticDnBVocabulary()
        {
            BusinessPartner = new DnBVocabulary();
        }
        public static DnBVocabulary BusinessPartner { get; private set; }
    }
}
