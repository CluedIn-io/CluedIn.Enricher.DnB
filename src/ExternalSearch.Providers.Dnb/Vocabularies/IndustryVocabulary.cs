using CluedIn.Core.Data.Vocabularies;
using Humanizer;

namespace CluedIn.ExternalSearch.Providers.DnB.Vocabularies
{
    public class IndustryVocabulary : SimpleVocabulary
    {
        public IndustryVocabulary()
        {
            VocabularyName = "DNBIndustry";
            KeyPrefix = "DnBIndustry";
            KeySeparator = ".";
            Grouping = "/Industry";

            AddGroup("D&B Industry Information", group =>
            {
                Description = group.Add(new VocabularyKey(nameof(Description), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible).WithDisplayName("Description"));
                TypeDescription = group.Add(new VocabularyKey(nameof(TypeDescription), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible).WithDisplayName("TypeDescription"));
                TypeDnBCode = group.Add(new VocabularyKey(nameof(TypeDnBCode), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible).WithDisplayName("TypeDnBCode"));
                Priority = group.Add(new VocabularyKey(nameof(Priority), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible).WithDisplayName("Priority"));
                Code = group.Add(new VocabularyKey(nameof(Code), VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible).WithDisplayName("Code"));

            });

        }
    

        public VocabularyKey Description { get; internal set; }
        public VocabularyKey TypeDescription { get; internal set; }
        public VocabularyKey TypeDnBCode { get; internal set; }
        public VocabularyKey Priority { get; internal set; }
        public VocabularyKey Code { get; internal set; }

    }
}
