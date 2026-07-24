using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Scriptable Object/Number Block")]
public class NumberBlockData : ScriptableObject
{
    [Header("Block Info")]
    public int Value;
    public AudioResource ToneSound;

    [Header("Button Up References")]
    public Sprite ButtonUpSpriteBottom;
    public Sprite ButtonUpSpriteTop;

    [Header("Button Down References")]
    public Sprite ButtonDownSpriteBottom;
    public Sprite ButtonDownSpriteTop;
}
