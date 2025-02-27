using System.Collections;
using UnityEngine;
/// <summary>
/// 作用：利用Monobehavior转换状态
/// </summary>
public class EnemySpell : MonoBehaviour
{
    // Constants for default values
    private const int DEFAULT_MAGIC_POINTS = 100;
    private const int MAX_MAGIC_POINTS = DEFAULT_MAGIC_POINTS; // 设置上限
    private float manaRegenTimer = 0f;
    
    public bool isSpellActive = false;
    public int magicPoints = DEFAULT_MAGIC_POINTS;
    public float spellInterval = 2f; // Interval between spells
    public int spellCost = 20; // Mana cost per spell
    public float spellDuration = 5f; // Spell duration 
    public int magicRegenRate = 1; // Mana regeneration rate

    protected Enemy enemy;
    private ChasingEnemy chasingEnemy;

    private float spellTimer = 0f;

    protected virtual void Start()
    {
        Debug.Log("[EnemySpell] Starting EnemySpell component");
        enemy = GetComponent<Enemy>();
        chasingEnemy = enemy.GetComponent<ChasingEnemy>();
        Debug.Log("[EnemySpell] Enemy and ChasingEnemy components initialized");
    }

    protected virtual void FixedUpdate()
    {
        manaRegenTimer += Time.fixedDeltaTime;

        if (manaRegenTimer >= 1f) // 每秒一次的回复
        {
            RegenerateMana(); // 调用法力恢复
            manaRegenTimer = 0f;
        }

        if (CanCastSpell())
        {
            spellTimer += Time.fixedDeltaTime;

            if (spellTimer >= spellInterval)
            {
                CastingSpell();
                spellTimer = 0f;
            }
        }
    }

    // Check if casting a spell is possible
    public bool CanCastSpell()
    {
        if (isSpellActive) return false;  // 如果法术已经激活，直接返回 false
        if (chasingEnemy.targetPlayer == null) return false;  // 如果目标玩家为空，直接返回 false
        if (magicPoints < spellCost) return false;  // 如果法力点不足，直接返回 false

        // 到这里，说明所有前置条件均满足
        Debug.Log("[EnemySpell] CanCastSpell evaluated to true");
        return true;
    }

    // Regenerate magic points over time
    private void RegenerateMana()
    {
        int oldMagicPoints = magicPoints;
        magicPoints = Mathf.Min(MAX_MAGIC_POINTS, magicPoints + Mathf.CeilToInt(magicRegenRate));
        Debug.Log($"[EnemySpell] RegenerateMana: Magic points increased from {oldMagicPoints} to {magicPoints}");
    }


    protected virtual void CastingSpell()
    {
        Debug.Log("[EnemySpell] Casting spell");
        isSpellActive = true;
        magicPoints -= spellCost;
        Debug.Log($"[EnemySpell] Magic points decreased by {spellCost}. Current points: {magicPoints}");
        enemy.SwitchState(new CastingSpellState());
        Debug.Log("[EnemySpell] State switched to CastingSpellState");
    }
}