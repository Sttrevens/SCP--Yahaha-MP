using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MessagesType
{
    Email,
    Codex
}

public abstract class Messages : ScriptableObject
{
    public string title;
    [TextArea] public string content;
    public Sprite icon;
    public MessagesType messageType;
    public bool isRead;
}
