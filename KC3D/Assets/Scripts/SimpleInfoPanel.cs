using UnityEngine;
using TMPro;

public class SimpleInfoPanel : MonoBehaviour
{
    [Header("Текстовые элементы")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI functionText;
    public TextMeshProUGUI roleText;
    public TextMeshProUGUI safetyText;

    void Start()
    {
        // Скрываем панель при старте
        gameObject.SetActive(false);
    }

    public void ShowInfo(string equipmentName, string function, string role, string safety)
    {
        if (nameText != null) nameText.text = equipmentName;
        if (functionText != null) functionText.text = "Функция: " + function;
        if (roleText != null) roleText.text = "Роль: " + role;
        if (safetyText != null) safetyText.text = "Безопасность: " + safety;

        gameObject.SetActive(true);
    }

    public void HideInfo()
    {
        gameObject.SetActive(false);
    }
}