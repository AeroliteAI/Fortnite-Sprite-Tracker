namespace FortniteSpriteTracker.Models;

public enum FilterMode
{
    All,
    Collected,
    NotCollected,
    Mastered,
    NotMastered,
    Incomplete,
    Completed
}

public enum SortMode
{
    Default,
    NameAZ,
    NameZA,
    CollectedFirst,
    NotCollectedFirst,
    MasteredFirst,
    UnmasteredFirst,
    RarityFirst
}

public enum GroupMode
{
    ByVariant,
    ByType
}
