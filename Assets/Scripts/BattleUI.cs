using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUI : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text levelText;
    public Slider HP_bar;

    public void SetUI(Entity entity)
    {
        nameText.text = entity.Name;
        // levelText.text = "Lvl " + entity.Level;
        HP_bar.maxValue = entity.MaxHP;
        HP_bar.value = entity.CurrentHP;
    }

    public void SetHP(int hp)
    {
        HP_bar.value = hp;
    }
}
