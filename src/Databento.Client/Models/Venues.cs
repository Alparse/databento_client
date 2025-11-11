namespace Databento.Client.Models;

/// <summary>
/// Common trading venue identifiers
/// </summary>
public static class Venues
{
    // Futures and Commodities
    /// <summary>CME Globex</summary>
    public const string GLBX = "GLBX";

    /// <summary>ICE Futures US</summary>
    public const string IFUS = "IFUS";

    /// <summary>ICE Europe Commodities</summary>
    public const string IFEU = "IFEU";

    /// <summary>ICE Europe Financials</summary>
    public const string IFLL = "IFLL";

    /// <summary>ICE Endex</summary>
    public const string NDEX = "NDEX";

    /// <summary>Eurex Exchange</summary>
    public const string XEUR = "XEUR";

    /// <summary>European Energy Exchange</summary>
    public const string XEEE = "XEEE";

    // US Equities - Primary Exchanges
    /// <summary>Nasdaq - All Markets</summary>
    public const string XNAS = "XNAS";

    /// <summary>Nasdaq OMX BX</summary>
    public const string XBOS = "XBOS";

    /// <summary>Nasdaq OMX PSX</summary>
    public const string XPSX = "XPSX";

    /// <summary>New York Stock Exchange, Inc.</summary>
    public const string XNYS = "XNYS";

    /// <summary>NYSE National, Inc.</summary>
    public const string XCIS = "XCIS";

    /// <summary>NYSE American</summary>
    public const string XASE = "XASE";

    /// <summary>NYSE Arca</summary>
    public const string ARCX = "ARCX";

    /// <summary>NYSE Texas (formerly NYSE Chicago)</summary>
    public const string XCHI = "XCHI";

    /// <summary>Investors Exchange (IEX)</summary>
    public const string IEXG = "IEXG";

    /// <summary>MEMX LLC Equities</summary>
    public const string MEMX = "MEMX";

    /// <summary>MIAX Pearl Equities</summary>
    public const string EPRL = "EPRL";

    /// <summary>Long-Term Stock Exchange, Inc.</summary>
    public const string LTSE = "LTSE";

    // Cboe Exchanges
    /// <summary>Cboe BZX U.S. Equities Exchange</summary>
    public const string BATS = "BATS";

    /// <summary>Cboe BYX U.S. Equities Exchange</summary>
    public const string BATY = "BATY";

    /// <summary>Cboe EDGA U.S. Equities Exchange</summary>
    public const string EDGA = "EDGA";

    /// <summary>Cboe EDGX U.S. Equities Exchange</summary>
    public const string EDGX = "EDGX";

    // Trade Reporting Facilities
    /// <summary>FINRA/Nasdaq TRF Carteret</summary>
    public const string FINN = "FINN";

    /// <summary>FINRA/Nasdaq TRF Chicago</summary>
    public const string FINC = "FINC";

    /// <summary>FINRA/NYSE TRF</summary>
    public const string FINY = "FINY";

    // Options Exchanges
    /// <summary>NYSE American Options</summary>
    public const string AMXO = "AMXO";

    /// <summary>BOX Options</summary>
    public const string XBOX = "XBOX";

    /// <summary>Cboe Options</summary>
    public const string XCBO = "XCBO";

    /// <summary>MIAX Emerald</summary>
    public const string EMLD = "EMLD";

    /// <summary>Cboe EDGX Options</summary>
    public const string EDGO = "EDGO";

    /// <summary>Nasdaq GEMX</summary>
    public const string GMNI = "GMNI";

    /// <summary>Nasdaq ISE</summary>
    public const string XISX = "XISX";

    /// <summary>Nasdaq MRX</summary>
    public const string MCRY = "MCRY";

    /// <summary>MIAX Options</summary>
    public const string XMIO = "XMIO";

    /// <summary>NYSE Arca Options</summary>
    public const string ARCO = "ARCO";

    /// <summary>Options Price Reporting Authority</summary>
    public const string OPRA = "OPRA";

    /// <summary>MIAX Pearl Options</summary>
    public const string MPRL = "MPRL";

    /// <summary>Nasdaq Options</summary>
    public const string XNDQ = "XNDQ";

    /// <summary>Nasdaq BX Options</summary>
    public const string XBXO = "XBXO";

    /// <summary>Cboe C2 Options</summary>
    public const string C2OX = "C2OX";

    /// <summary>Nasdaq PHLX</summary>
    public const string XPHL = "XPHL";

    /// <summary>Cboe BZX Options</summary>
    public const string BATO = "BATO";

    /// <summary>MEMX Options</summary>
    public const string MXOP = "MXOP";

    /// <summary>MIAX Sapphire</summary>
    public const string SPHR = "SPHR";

    // Consolidated/Special
    /// <summary>Databento US Equities - Consolidated</summary>
    public const string DBEQ = "DBEQ";

    /// <summary>Databento US Equities - Consolidated (alias)</summary>
    public const string EQUS = "EQUS";

    /// <summary>Off-Exchange Transactions - Listed Instruments</summary>
    public const string XOFF = "XOFF";

    // IntelligentCross ASPEN
    /// <summary>IntelligentCross ASPEN Intelligent Bid/Offer</summary>
    public const string ASPN = "ASPN";

    /// <summary>IntelligentCross ASPEN Maker/Taker</summary>
    public const string ASMT = "ASMT";

    /// <summary>IntelligentCross ASPEN Inverted</summary>
    public const string ASPI = "ASPI";
}
