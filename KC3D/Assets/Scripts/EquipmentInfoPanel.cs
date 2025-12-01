using UnityEngine;
using TMPro;

public class EquipmentInfoPanel : MonoBehaviour
{
    [Header("UI Elements")]
    public Animator animator;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI functionText;
    public TextMeshProUGUI roleText;
    public TextMeshProUGUI safetyText;

    public void ShowInfo(string name, string function, string role, string safety)
    {
        if (nameText != null) nameText.text = name;
        if (functionText != null) functionText.text = "Функция: " + function;
        if (roleText != null) roleText.text = "Роль: " + role;
        if (safetyText != null) safetyText.text = "Безопасность: " + safety;

        if (animator != null)
            animator.SetTrigger("Show");
        else
            gameObject.SetActive(true);
    }

    public void HideInfo()
    {
        if (animator != null)
            animator.SetTrigger("Hide");
        else
            gameObject.SetActive(false);
    }
}