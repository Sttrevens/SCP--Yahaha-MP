using System.Collections;
using UnityEngine;
/// <summary>
/// 作用：利用Monobehavior转换状态
/// </summary>
public class EnemySpell : MonoBehaviour
{
    protected bool isSpelled = false;
    public int magicPoints = 100;
    public float interval = 2f; // 间隔时间
    public int spellCost = 20; // 施法消耗
    public float spellDuration = 5f; // 法术持续时间
    public int magicRegenRate = 1; // MP恢复速度

    protected Enemy enemy;

    // Start is called before第一个帧更新之前调用
    protected virtual void Start()
    {
        enemy = GetComponent<Enemy>();
        InvokeRepeating(nameof(TryCastSpell), interval, interval);
    }

    protected virtual void Update()
    {
    }

    protected virtual void FixedUpdate()
    {
        magicPoints += (int)(magicRegenRate * Time.fixedDeltaTime);
    }

    protected virtual void TryCastSpell()
    {
        if (!isSpelled && magicPoints >= spellCost &&
            (enemy.GetComponent<ChasingEnemy>().targetPlayer != null))
        {
            CastSpell();
        }
    }

    protected virtual void CastSpell()
    {
        isSpelled = true;
        magicPoints -= spellCost;
        enemy.SwitchState(new CastingSpellState());
        StartCoroutine(ResetSpell());
    }

    protected virtual IEnumerator ResetSpell()
    {
        yield return new WaitForSeconds(spellDuration);
        isSpelled = false;
    }
}