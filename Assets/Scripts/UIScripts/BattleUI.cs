using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUI : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text levelText;
    public TMP_Text hpText;
    public Slider HP_bar;
    public TMP_Text apText;
    public Slider AP_bar;

    public void SetUI(Entity entity)
    {
        nameText.text = entity.Name;
        levelText.text = "Lvl " + entity.Level;
        hpText.text = entity.CurrentHP + " / " +  entity.MaxHP;
        HP_bar.maxValue = entity.MaxHP;
        HP_bar.value = entity.CurrentHP;
        apText.text = entity.CurrentAP + " / " + entity.MaxAP;
        AP_bar.maxValue = entity.MaxAP;
        AP_bar.value = entity.CurrentAP;
    }

    public void SetHP(int hp)
    {
        hpText.text = hp + " / " + HP_bar.maxValue;
        HP_bar.value = hp;
    }

    public void SetAP(int ap)
    {
        apText.text = ap + " / " + AP_bar.maxValue;
        HP_bar.value = ap;
    }
}
