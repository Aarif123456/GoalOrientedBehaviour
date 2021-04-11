namespace GameBrains.AI
{
    using System.ComponentModel;
    
    public enum GoalTypes
    {
        Think,
        Explore,
        
        [Description("Arrive at position")]
        ArriveAtPosition,
        
        [Description("Seek to position")]
        SeekToPosition,
        
        [Description("Follow path")]
        FollowPath,
        
        [Description("Traverse edge")]
        TraverseEdge,
        
        [Description("Move to position")]
        MoveToPosition,
        
        [Description("Get health")]
        GetHealth,
        
        [Description("Get shotgun")]
        GetShotgun,
        
        [Description("Get railgun")]
        GetRailgun,
        
        [Description("Get rocket launcher")]
        GetRocketLauncher,
        
        [Description("Wander about")]
        WanderAbout,
        
        [Description("Negotiate door")]
        NegotiateDoor,
        
        [Description("Attack target")]
        AttackTarget,
        
        [Description("Hunt target")]
        HuntTarget,
        
        Strafe,
        
        [Description("Adjust range")]
        AdjustRange,
        
        [Description("Evade bot")]
        EvadeBot,
        
        [Description("Pursue bot")]
        PursueBot,
    }
}
