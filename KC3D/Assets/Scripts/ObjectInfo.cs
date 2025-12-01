using UnityEngine;
using TMPro;

public class ObjectInfo : MonoBehaviour
{
    [Header("Цвета")]
    public Color normalColor = Color.gray;
    public Color highlightColor = Color.yellow;

    [Header("Информация об оборудовании")]
    public string equipmentName = "Оборудование";
    public string function = "Описание функции";
    public string role = "Роль в системе";
    public string safety = "Меры безопасности";

    [Header("UI Elements")]
    public GameObject infoPanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI functionText;
    public TextMeshProUGUI roleText;
    public TextMeshProUGUI safetyText;

    private Renderer[] childRenderers;
    private Color[] originalColors;
    private bool isHighlighted = false;

    void Start()
    {
        childRenderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[childRenderers.Length];

        for (int i = 0; i < childRenderers.Length; i++)
        {
            if (childRenderers[i] != null)
            {
                originalColors[i] = childRenderers[i].material.color;
            }
        }
        if (infoPanel != null)
            infoPanel.SetActive(false);
    }

    void OnMouseEnter()
    {
        if (isHighlighted) return;

        isHighlighted = true;

        // Подсветка объекта
        foreach (Renderer renderer in childRenderers)
        {
            if (renderer != null && renderer.material != null)
            {
                renderer.material.color = highlightColor;
            }
        }

        // Показ информации в UI
        ShowInfo();
    }

    void OnMouseExit()
    {
        if (!isHighlighted) return;

        isHighlighted = false;

        // Возвращаем оригинальные цвета
        for (int i = 0; i < childRenderers.Length; i++)
        {
            if (childRenderers[i] != null && childRenderers[i].material != null)
            {
                childRenderers[i].material.color = originalColors[i];
            }
        }

        // Скрываем информацию
        HideInfo();
    }

    void ShowInfo()
    {
        if (infoPanel != null && nameText != null)
        {
            nameText.text = equipmentName;
            functionText.text = function;
            roleText.text = role;
            safetyText.text = safety;
            infoPanel.SetActive(true);
        }
    }

    void HideInfo()
    {
        if (infoPanel != null)
            infoPanel.SetActive(false);
    }
}