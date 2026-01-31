using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private Slider HP_bar;
    [SerializeField] private TMP_Text apText;
    [SerializeField] private Slider AP_bar;

    public void SetUI(Entity entity)
    {
        nameText.text = entity.Name;
        hpText.text = "HP " + entity.CurrentHP + " / " +  entity.MaxHP;
        HP_bar.maxValue = entity.MaxHP;
        HP_bar.value = entity.CurrentHP;
        apText.text = "AP " + entity.CurrentAP + " / " + entity.MaxAP;
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
        AP_bar.value = ap;
    }
}
