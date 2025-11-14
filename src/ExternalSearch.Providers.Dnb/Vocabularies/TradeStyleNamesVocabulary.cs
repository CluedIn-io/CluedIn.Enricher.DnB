using CluedIn.Core.Data.Vocabularies;

namespace CluedIn.ExternalSearch.Providers.DnB.Vocabularies;

public class TradeStyleNamesVocabulary : SimpleVocabulary
{
    public TradeStyleNamesVocabulary()
    {
        VocabularyName = "DNBTradeStyleNames";
        KeyPrefix = "dnb.tradeStyleNames";
        KeySeparator = ".";
        Grouping = "/TradeStyleNames";
    }
}