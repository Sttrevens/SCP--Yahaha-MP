using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MessageType
{
    Email,
    Codex
}

public abstract class Messages : ScriptableObject
{
    public string title;
    [TextArea] public string content;
    public Sprite icon;
    public MessageType messageType;
    public bool isRead;
}
