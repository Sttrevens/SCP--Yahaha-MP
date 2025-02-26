using System.Collections;
using UnityEngine;

public class CactusSpell : MonoBehaviour
{
    [SerializeField] private GameObject[] vFXs;
    [SerializeField] private EnemySpell spell;
    public void CastSpell()
    {
        foreach (var vFX in vFXs)
            vFX.SetActive(true);
        
        StartCoroutine(ResetSpell());
    }

    private IEnumerator ResetSpell()
    {
        yield return new WaitForSeconds(spell.spellDuration);
        
        foreach (var vFX in vFXs)
            vFX.SetActive(false);
    }
}