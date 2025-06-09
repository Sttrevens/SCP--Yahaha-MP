using UnityEngine;

[CreateAssetMenu(fileName = "New Email", menuName = "Message System/Email")]
public class EmailsData : Messages
{
    public string sender;
    public string recipient;
    public string timestamp;

    private void OnEnable()
    {
        messageType = MessageType.Email;
    }
}