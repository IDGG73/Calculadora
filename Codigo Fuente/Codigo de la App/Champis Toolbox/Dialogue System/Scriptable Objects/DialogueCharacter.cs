using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Champis/Dialogue/Character")]
public class DialogueCharacter : ScriptableObject
{
    [Header("CHARACTER INFO")]
    public string speakerName;
    public Sprite portrait;
    public bool friendly;

    [Header("UI COLORS")]
    public Color nameColor = Color.white;
    public Color dialogueColor = Color.white;
    public Color boxColor = Color.white;
    public Color portraitBackgroundColor = Color.white;

    [Header("OTHER")]
    public bool useVoiceClips = true;
    [Tooltip("When a letter of the dialogue appears, one of these clips will be picked randomly.")]
    public AudioClip[] voiceClips;
    [Tooltip("Time to wait before revealing the next character when using Typewriting.")]
    public float timeBetweenCharacters = 0.01f;
}
