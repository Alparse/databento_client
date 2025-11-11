namespace Databento.Client.Models;

/// <summary>
/// Trade with Consolidated BBO (Best Bid/Offer) message - 80 bytes
/// Contains trade information along with the consolidated best bid and offer immediately before the trade
/// This is structurally identical to CMBP-1 but represents the Tcbbo schema
/// The consolidated BBO aggregates the best prices across multiple venues
/// </summary>
public class TcbboMessage : Cmbp1Message
{
    /// <summary>
    /// String representation showing this is a trade with consolidated BBO snapshot
    /// </summary>
    public override string ToString()
    {
        return $"TCBBO: {PriceDecimal} x {Size} ({Side}/{Action}) | CBBO: {Level} [{Timestamp:O}]";
    }
}
