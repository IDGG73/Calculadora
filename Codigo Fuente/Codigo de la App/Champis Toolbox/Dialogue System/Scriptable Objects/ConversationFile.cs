using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Conversation File", menuName = "Champis/Dialogue/Conversation File")]
public class ConversationFile : ScriptableObject
{
    public Dialogue[] dialogues;
}