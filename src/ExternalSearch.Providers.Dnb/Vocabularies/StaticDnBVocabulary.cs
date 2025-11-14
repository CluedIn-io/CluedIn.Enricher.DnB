namespace CluedIn.ExternalSearch.Providers.DnB.Vocabularies;

public static class StaticDnBVocabulary
{
    static StaticDnBVocabulary()
    {
        BusinessPartner = new DnBVocabulary();
        Industry = new IndustryVocabulary();
        TradeStyleNames = new TradeStyleNamesVocabulary();
        Fax = new FaxVocabulary();
        RegistrationNumbers = new RegistrationNumbersVocabulary();
        StockExchanges = new StockExchangesVocabulary();
        Telephone = new TelephoneVocabulary();
        WebsiteAddress = new WebsiteAddressVocabulary();
        CorporateLinkageFamilyTreeRolesPlayedVocabulary = new CorporateLinkageFamilyTreeRolesPlayedVocabulary();
    }

    public static DnBVocabulary BusinessPartner { get; private set; }
    public static IndustryVocabulary Industry { get; private set; }
    public static TradeStyleNamesVocabulary TradeStyleNames { get; private set; }
    public static FaxVocabulary Fax { get; private set; }
    public static RegistrationNumbersVocabulary RegistrationNumbers { get; private set; }
    public static StockExchangesVocabulary StockExchanges { get; private set; }
    public static TelephoneVocabulary Telephone { get; private set; }
    public static WebsiteAddressVocabulary WebsiteAddress { get; private set; }
    public static CorporateLinkageFamilyTreeRolesPlayedVocabulary CorporateLinkageFamilyTreeRolesPlayedVocabulary { get; private set; }
}