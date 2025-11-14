using CluedIn.Core.Data.Vocabularies;

namespace CluedIn.ExternalSearch.Providers.DnB.Vocabularies;

public class StockExchangesVocabulary : SimpleVocabulary
{
    public StockExchangesVocabulary()
    {
        VocabularyName = "DNBStockExchanges";
        KeyPrefix = "dnb.stockExchanges";
        KeySeparator = ".";
        Grouping = "/StockExchanges";
    }
}