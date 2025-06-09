using UnityEngine;


[CreateAssetMenu(fileName = "New Codex Entry", menuName = "Message System/Codex Entry")]
public class CodexData : Messages
{
    public string category;

    private void OnEnable()
    {
        messageType = MessagesType.Codex;
    }
}
