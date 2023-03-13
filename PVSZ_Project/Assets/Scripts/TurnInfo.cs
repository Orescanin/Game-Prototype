public enum TurnType
{
    Tech,
    Alien,
}

public struct TurnInfo
{
    public int      Num;
    public TurnType Type;
    
    public TurnInfo(int num, TurnType type) { Num = num; Type = type; }
    
    public override bool Equals(object oth)
    {
        TurnInfo? othTI = oth as TurnInfo?;
        if (othTI == null) return false;                                    // NOTE(sftl): never equal to null
        
        return (othTI.Value.Num == Num) && (othTI.Value.Type == Type);
    }
    
    public override int GetHashCode() => (Num, Type).GetHashCode();
    
    public static bool operator ==(TurnInfo a, TurnInfo b) { return  a.Equals(b); }
    public static bool operator !=(TurnInfo a, TurnInfo b) { return !a.Equals(b); }
}
