namespace Databento.Client.Models;

/// <summary>
/// Bit flags for record metadata
/// </summary>
public struct FlagSet
{
    private byte _flags;

    public FlagSet(byte flags)
    {
        _flags = flags;
    }

    /// <summary>
    /// Last message in the packet from the venue for this instrument
    /// </summary>
    public const byte Last = 1 << 7;

    /// <summary>
    /// Top of book message (not an individual order)
    /// </summary>
    public const byte Tob = 1 << 6;

    /// <summary>
    /// Snapshot, not an incremental update
    /// </summary>
    public const byte Snapshot = 1 << 5;

    /// <summary>
    /// MBP message (Market by Price)
    /// </summary>
    public const byte Mbp = 1 << 4;

    /// <summary>
    /// Possibly inaccurate receive timestamp (clock issues)
    /// </summary>
    public const byte BadTsRecv = 1 << 3;

    /// <summary>
    /// Book may be inaccurate (recovery in progress)
    /// </summary>
    public const byte MaybeBadBook = 1 << 2;

    /// <summary>
    /// Publisher-specific flag
    /// </summary>
    public const byte PublisherSpecific = 1 << 1;

    public bool IsEmpty() => _flags == 0;
    public bool IsLast() => (_flags & Last) != 0;
    public void SetLast(bool value) { if (value) _flags |= Last; else _flags &= unchecked((byte)~Last); }

    public bool IsTob() => (_flags & Tob) != 0;
    public void SetTob(bool value) { if (value) _flags |= Tob; else _flags &= unchecked((byte)~Tob); }

    public bool IsSnapshot() => (_flags & Snapshot) != 0;
    public void SetSnapshot(bool value) { if (value) _flags |= Snapshot; else _flags &= unchecked((byte)~Snapshot); }

    public bool IsMbp() => (_flags & Mbp) != 0;
    public void SetMbp(bool value) { if (value) _flags |= Mbp; else _flags &= unchecked((byte)~Mbp); }

    public bool IsBadTsRecv() => (_flags & BadTsRecv) != 0;
    public void SetBadTsRecv(bool value) { if (value) _flags |= BadTsRecv; else _flags &= unchecked((byte)~BadTsRecv); }

    public bool IsMaybeBadBook() => (_flags & MaybeBadBook) != 0;
    public void SetMaybeBadBook(bool value) { if (value) _flags |= MaybeBadBook; else _flags &= unchecked((byte)~MaybeBadBook); }

    public bool IsPublisherSpecific() => (_flags & PublisherSpecific) != 0;
    public void SetPublisherSpecific(bool value) { if (value) _flags |= PublisherSpecific; else _flags &= unchecked((byte)~PublisherSpecific); }

    public void Clear() => _flags = 0;
    public byte Raw() => _flags;
    public void SetRaw(byte value) => _flags = value;
    public bool Any() => _flags != 0;

    public override string ToString()
    {
        var flags = new List<string>();
        if (IsLast()) flags.Add("Last");
        if (IsTob()) flags.Add("Tob");
        if (IsSnapshot()) flags.Add("Snapshot");
        if (IsMbp()) flags.Add("Mbp");
        if (IsBadTsRecv()) flags.Add("BadTsRecv");
        if (IsMaybeBadBook()) flags.Add("MaybeBadBook");
        if (IsPublisherSpecific()) flags.Add("PublisherSpecific");
        return flags.Count > 0 ? string.Join("|", flags) : "None";
    }
}
