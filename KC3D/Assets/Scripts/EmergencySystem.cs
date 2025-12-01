using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;

public class EmergencySystem : MonoBehaviour
{
    [Header(" Датчики")]
    public List<GameObject> pressureSensors = new List<GameObject>();
    public List<GameObject> temperatureSensors = new List<GameObject>();

    [Header(" Аварийная сигнализация")]
    public GameObject emergencyLights;
    public AudioSource alarmSound;

    [Header(" Образовательный UI")]
    public GameObject emergencyPanel;
    public TextMeshProUGUI emergencyTitle;
    public TextMeshProUGUI emergencyDescription;
    public TextMeshProUGUI safetyInstructions;
    public TextMeshProUGUI operatorActions;

    [Header(" Настройки аварий")]
    public float blinkInterval = 0.7f;
    public float emergencyDuration = 10f;
    public Material sensorNormalMaterial;
    public Material sensorAlarmMaterial;

    // Типы аварийных ситуаций
    public enum EmergencyType
    {
        None,
        PressureDrop,     // Падение давления
        Overheat         // Перегрев газа
    }

    private EmergencyType currentEmergency = EmergencyType.None;
    private bool isEmergencyActive = false;
    private Coroutine currentEmergencyRoutine;
    private Dictionary<EmergencyType, EmergencyScenario> emergencyScenarios;

    [System.Serializable]
    public class EmergencyScenario
    {
        public string title;
        public string description;
        public string safetyInstructions;
        public string operatorActions;
    }

    void Start()
    {
        InitializeEmergencyScenarios();
        HideEmergencyUI();
    }

    void InitializeEmergencyScenarios()
    {
        emergencyScenarios = new Dictionary<EmergencyType, EmergencyScenario>()
        {
            {
                EmergencyType.PressureDrop,
                new EmergencyScenario()
                {
                    title = " АВАРИЯ: ПАДЕНИЕ ДАВЛЕНИЯ",
                    description = "Резкое снижение давления в магистральном газопроводе.",
                    safetyInstructions = " Не приближаться к трубопроводу\n Эвакуироваться из опасной зоны",
                    operatorActions = "1. Проверить герметичность\n2. Увеличить мощность ГПА\n3. Вызвать аварийную бригаду"
                }
            },
            {
                EmergencyType.Overheat,
                new EmergencyScenario()
                {
                    title = " АВАРИЯ: ПЕРЕГРЕВ ГАЗА",
                    description = "Температура газа превышает допустимые нормы.",
                    safetyInstructions = " Не касаться горячих поверхностей\n Включить дополнительное охлаждение",
                    operatorActions = "1. Увеличить мощность охлаждения\n2. Снизить нагрузку ГПА\n3. Контролировать температуру"
                }
            }
        };
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1) && !isEmergencyActive)
            StartEmergency(EmergencyType.PressureDrop);

        if (Input.GetKeyDown(KeyCode.F2) && !isEmergencyActive)
            StartEmergency(EmergencyType.Overheat);

        if (Input.GetKeyDown(KeyCode.F3))
            StopEmergency();
    }

    public void StartEmergency(EmergencyType emergencyType)
    {
        if (isEmergencyActive) return;

        currentEmergency = emergencyType;
        isEmergencyActive = true;

        if (currentEmergencyRoutine != null)
            StopCoroutine(currentEmergencyRoutine);

        currentEmergencyRoutine = StartCoroutine(EmergencyRoutine());
    }

    IEnumerator EmergencyRoutine()
    {
        var scenario = emergencyScenarios[currentEmergency];

        StartCoroutine(BlinkSensors());
        StartCoroutine(BlinkEmergencyLights());

        ShowEmergencyUI(scenario);

        if (alarmSound != null)
            alarmSound.Play();

        yield return new WaitForSeconds(emergencyDuration);

        StopEmergency();
    }

    IEnumerator BlinkSensors()
    {
        while (isEmergencyActive)
        {
            // Мигание соответствующих датчиков КРАСНЫМ цветом
            switch (currentEmergency)
            {
                case EmergencyType.PressureDrop:
                    // Мигают только датчики давления
                    SetSensorsColor(pressureSensors, sensorAlarmMaterial);
                    SetSensorsColor(temperatureSensors, sensorNormalMaterial);
                    break;

                case EmergencyType.Overheat:
                    // Мигают только датчики температуры
                    SetSensorsColor(temperatureSensors, sensorAlarmMaterial);
                    SetSensorsColor(pressureSensors, sensorNormalMaterial);
                    break;
            }

            yield return new WaitForSeconds(blinkInterval);

            // Возврат к нормальному цвету
            SetAllSensorsNormal();

            yield return new WaitForSeconds(blinkInterval);
        }

        // Гарантированный возврат к нормальному состоянию
        SetAllSensorsNormal();
    }

    IEnumerator BlinkEmergencyLights()
    {
        while (isEmergencyActive)
        {
            if (emergencyLights != null)
                emergencyLights.SetActive(!emergencyLights.activeSelf);

            yield return new WaitForSeconds(blinkInterval);
        }

        if (emergencyLights != null)
            emergencyLights.SetActive(false);
    }

    void SetSensorsColor(List<GameObject> sensors, Material material)
    {
        foreach (var sensor in sensors)
        {
            if (sensor != null)
            {
                var renderer = sensor.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = material;
                }
            }
        }
    }

    void SetAllSensorsNormal()
    {
        SetSensorsColor(pressureSensors, sensorNormalMaterial);
        SetSensorsColor(temperatureSensors, sensorNormalMaterial);
    }

    void ShowEmergencyUI(EmergencyScenario scenario)
    {
        if (emergencyPanel == null) return;

        emergencyTitle.text = scenario.title;
        emergencyDescription.text = scenario.description;
        safetyInstructions.text = scenario.safetyInstructions;
        operatorActions.text = scenario.operatorActions;

        emergencyPanel.SetActive(true);
    }

    void HideEmergencyUI()
    {
        if (emergencyPanel != null)
            emergencyPanel.SetActive(false);

        if (alarmSound != null)
            alarmSound.Stop();
    }

    public void StopEmergency()
    {
        if (!isEmergencyActive) return;

        isEmergencyActive = false;
        currentEmergency = EmergencyType.None;

        StopAllCoroutines();
        HideEmergencyUI();
        SetAllSensorsNormal();

        if (emergencyLights != null)
            emergencyLights.SetActive(false);
    }

    // Методы для вызова из UI (кнопок)
    public void UI_StartPressureDrop() => StartEmergency(EmergencyType.PressureDrop);
    public void UI_StartOverheat() => StartEmergency(EmergencyType.Overheat);
    public void UI_StopEmergency() => StopEmergency();
}