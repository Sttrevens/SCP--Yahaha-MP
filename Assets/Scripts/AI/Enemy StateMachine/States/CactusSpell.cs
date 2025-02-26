using System.Collections;
using UnityEngine;

public class CactusSpell : MonoBehaviour
{
    [SerializeField] private GameObject[] vFXs;
    [SerializeField] private EnemySpell spell;
    [SerializeField] private EnemyAttack _enemyAttack;

    [SerializeField] private AudioClip cactusSpellClip;

    private int originalEnemyDamage;

    private void Start()
    {
        originalEnemyDamage = _enemyAttack.attackDamage;
    }
    public void CastSpell()
    {
        foreach (var vFX in vFXs)
            vFX.SetActive(true);
        _enemyAttack.attackDamage *= 2;
        
        if (cactusSpellClip != null)
            AudioManager.instance.PlaySFX(this.gameObject, cactusSpellClip);
        StartCoroutine(ResetSpell());
    }

    private IEnumerator ResetSpell()
    {
        yield return new WaitForSeconds(spell.spellDuration);
        
        foreach (var vFX in vFXs)
            vFX.SetActive(false);

        _enemyAttack.attackDamage = originalEnemyDamage;
        spell.isSpellActive = false;
        
        yield return null;
    }
}