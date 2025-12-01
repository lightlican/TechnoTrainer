using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AdvancedCameraController : MonoBehaviour
{
    [Header("Основные настройки")]
    public float moveSpeed = 10f;
    public float rotateSpeed = 100f;
    public float zoomSpeed = 5f;
    public float shiftMultiplier = 2f;

    [Header("Ограничения камеры")]
    public bool enableBounds = true;
    public Vector3 minBounds = new Vector3(-15, 2, -10);
    public Vector3 maxBounds = new Vector3(15, 10, 10);

    [Header("Пресеты позиций")]
    public Transform[] presetPositions;
    public string[] presetNames = { "Общий вид", "ГПА", "Охладитель", "Пульт управления" };

    [Header("UI элементы")]
    public GameObject cameraControlsPanel;
    public Button[] presetButtons;
    public Button resetCameraButton;
    public Button togglePanelButton;

    private Vector3 lastMousePosition;
    private bool isRotating = false;
    private bool controlsVisible = true;

    void Start()
    {
        SetupUIButtons();
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleZoom();
        HandlePresets();

        // Ограничиваем камеру в пределах сцены
        if (enableBounds)
        {
            ClampCameraPosition();
        }
    }

    void SetupUIButtons()
    {
        // Настраиваем кнопки пресетов
        for (int i = 0; i < presetButtons.Length; i++)
        {
            int index = i;
            if (presetButtons[i] != null)
            {
                presetButtons[i].onClick.AddListener(() => MoveToPreset(index));

                // Обновляем текст кнопки
                Text buttonText = presetButtons[i].GetComponentInChildren<Text>();
                if (buttonText != null && i < presetNames.Length)
                {
                    buttonText.text = presetNames[i];
                }
            }
        }

        // Кнопка сброса камеры
        if (resetCameraButton != null)
        {
            resetCameraButton.onClick.AddListener(ResetToDefault);
        }

        // Кнопка скрытия/показа панели
        if (togglePanelButton != null)
        {
            togglePanelButton.onClick.AddListener(ToggleControlsPanel);
        }
    }

    void HandleMovement()
    {
        float speed = moveSpeed;

        // Ускорение с Shift
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed *= shiftMultiplier;
        }

        // WASD - движение
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Q/E - движение вверх/вниз
        float lift = 0f;
        if (Input.GetKey(KeyCode.Q)) lift = -1f;
        if (Input.GetKey(KeyCode.E)) lift = 1f;

        Vector3 moveDirection = new Vector3(horizontal, lift, vertical);
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.Self);
    }

    void HandleRotation()
    {
        // Правая кнопка мыши - вращение
        if (Input.GetMouseButtonDown(1))
        {
            lastMousePosition = Input.mousePosition;
            isRotating = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            isRotating = false;
        }

        if (isRotating && Input.GetMouseButton(1))
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;

            // Вращение вокруг оси Y (горизонталь)
            transform.Rotate(Vector3.up, delta.x * rotateSpeed * Time.deltaTime, Space.World);

            // Вращение вокруг локальной оси X (вертикаль)
            transform.Rotate(Vector3.right, -delta.y * rotateSpeed * Time.deltaTime, Space.Self);

            lastMousePosition = Input.mousePosition;
        }

        // Клавиши для точного вращения
        if (Input.GetKey(KeyCode.Z)) transform.Rotate(Vector3.up, -rotateSpeed * 0.5f * Time.deltaTime, Space.World);
        if (Input.GetKey(KeyCode.X)) transform.Rotate(Vector3.up, rotateSpeed * 0.5f * Time.deltaTime, Space.World);
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            transform.Translate(Vector3.forward * scroll * zoomSpeed, Space.Self);
        }

        // Клавиши для точного zoom
        if (Input.GetKey(KeyCode.R)) transform.Translate(Vector3.forward * zoomSpeed * 0.5f * Time.deltaTime, Space.Self);
        if (Input.GetKey(KeyCode.F)) transform.Translate(Vector3.forward * -zoomSpeed * 0.5f * Time.deltaTime, Space.Self);
    }

    void HandlePresets()
    {
        // Цифровые клавиши для быстрых позиций
        for (int i = 0; i < presetPositions.Length && i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                MoveToPreset(i);
            }
        }

        // Клавиша 0 - общий вид
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            ResetToDefault();
        }

        // Скрытие/показа UI панели
        if (Input.GetKeyDown(KeyCode.H))
        {
            ToggleControlsPanel();
        }
    }

    void ClampCameraPosition()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
        pos.y = Mathf.Clamp(pos.y, minBounds.y, maxBounds.y);
        pos.z = Mathf.Clamp(pos.z, minBounds.z, maxBounds.z);
        transform.position = pos;
    }

    public void MoveToPreset(int presetIndex)
    {
        if (presetIndex >= 0 && presetIndex < presetPositions.Length && presetPositions[presetIndex] != null)
        {
            transform.position = presetPositions[presetIndex].position;
            transform.rotation = presetPositions[presetIndex].rotation;
            Debug.Log($"Камера перемещена: {presetNames[presetIndex]}");
        }
    }

    public void ResetToDefault()
    {
        // Возвращаем камеру в начальную позицию
        transform.position = new Vector3(0, 8, -12);
        transform.rotation = Quaternion.Euler(25, 0, 0);
        Debug.Log("Камера сброшена в начальное положение");
    }

    public void ToggleControlsPanel()
    {
        controlsVisible = !controlsVisible;
        if (cameraControlsPanel != null)
        {
            cameraControlsPanel.SetActive(controlsVisible);
        }
    }

    // Методы для UI кнопок
    public void SetPreset0() => MoveToPreset(0);
    public void SetPreset1() => MoveToPreset(1);
    public void SetPreset2() => MoveToPreset(2);
    public void SetPreset3() => MoveToPreset(3);
}