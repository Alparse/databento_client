namespace Databento.Client.Models;

/// <summary>
/// Trade with BBO (Best Bid/Offer) message - 80 bytes
/// Contains trade information along with the best bid and offer immediately before the trade
/// This is structurally identical to MBP-1 but represents the Tbbo schema
/// </summary>
public class TbboMessage : Mbp1Message
{
    /// <summary>
    /// String representation showing this is a trade with BBO snapshot
    /// </summary>
    public override string ToString()
    {
        return $"TBBO: {PriceDecimal} x {Size} ({Side}/{Action}) | BBO: {Level} [{Timestamp:O}]";
    }
}
