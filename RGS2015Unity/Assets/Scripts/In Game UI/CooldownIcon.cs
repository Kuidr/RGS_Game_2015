using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CooldownIcon : MonoBehaviour
{
    public Image icon;

    
	public void Enable(Spell spell)
    {
        icon.enabled = true;
        StopAllCoroutines();
        StartCoroutine(UpdateCoolingDown(spell));
    }
    public void SetVisible(bool visible)
    {
        icon.enabled = visible;
    }
    public bool IsVisible()
    {
        return icon.enabled;
    }

    private void Awake()
    {
        icon.enabled = false;
    }
    private IEnumerator UpdateCoolingDown(Spell spell)
    {
        float cd = 0;
        float rev = 360f * spell.cooldown_time; // one revolution for each second of cooldown

        while (true)
        {
            cd = Mathf.Pow(1 - spell.GetCooldownPercent(), 2);
            transform.rotation = Quaternion.Euler(0, 0, cd*rev);

            if (!spell.IsOnCooldown()) break;
            yield return null;
        }
        icon.enabled = false;
    }
}
