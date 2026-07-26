using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Audio;

public enum LEVEL 
{ 
    SQUID,
    PLATFORMER,
    ICE_CURSOR,
    GOBLINS,
    MATH,
    HOT_COLD,
    WANTED,
    CRANE,
    SNIPER,
    MAGNIFY,
    CARROT,
    MATCH,
    TURRET,
    RUN_AWAY,
    MAZE,
    GOLF,
}


[CreateAssetMenu(menuName = "Scriptable Object/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Level Info")]
    public LEVEL Index;
    public string SceneString;
    public Sprite Icon;
    public Sprite WindowBorderSprite;
    public Sprite WindowTextSprite;
    public Sprite ButtonSprite;
    public Sprite ButtonSpriteComplete;
}
