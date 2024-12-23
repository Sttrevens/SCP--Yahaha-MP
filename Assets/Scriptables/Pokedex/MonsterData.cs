using UnityEngine;

[CreateAssetMenu(
    fileName = "NewMonsterData",
    menuName = "Monster/Create New Monster Data"  
)]
public class MonsterData : ScriptableObject
{
    public string monsterName;
    public Sprite monsterSprite;
    [TextArea]
    public string description;
    public int hp;
    public int attack;
}
