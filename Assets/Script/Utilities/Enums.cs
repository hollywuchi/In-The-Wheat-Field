public enum ItemType
{
    Seed, Commondity, Funiture,
    HoeTool, ChopTool, BreakTool, ReapTool, WaterTool, CollectTool,
    // 挖坑的，砍树的，凿石的，收割的，浇水的，吸附的
    ReapableScenery
}

public enum SoltType
{
    Bag, Box, Shop, Funiture,Reward
}

public enum InventoryLocation
{
    Player, Box
}

public enum PartType
{
    None, Carry, Hoe, Break, Water, Collect, Chop, Reap
}

public enum PartName
{
    Body, Hair, Arm, Tool
}

public enum Season
{
    春天, 夏天, 秋天, 冬天
}

public enum GirdType
{
    Diggable, DropItem, PlaceFurinture, NPCObstacle
}

public enum ParticalEffectType
{
    None, LeavesFalling01, LeavesFalling02, Rock, ReapableScenery
}

public enum GameState
{
    GamePlay, Pause
}

public enum LightShift
{
    Morning, Night
}
public enum SoundName
{
    None, FootStepSoft, FootStepHard,
    Axe, Pickaxe, Hoe, Reap, Water, Basket, Chop,
    Pickup, Plant, TreeFalling, Rustle,
    AmbientCountryside1, AmbientCountryside2, MusicCalm1, MusicCalm2, MusicCalm3, AmbientIndoor1
}
public enum QuestStates
{
    Waitting, Accept, Complete
}