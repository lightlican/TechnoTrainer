using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class CameraUIController : MonoBehaviour
{
    [Header("Ссылка на контроллер камеры")]
    public AdvancedCameraController cameraController;

    [Header("Кнопки пресетов")]
    public Button[] presetButtons;

    [Header("Кнопки движения")]
    public Button moveForwardBtn;
    public Button moveBackBtn;
    public Button moveLeftBtn;
    public Button moveRightBtn;
    public Button moveUpBtn;
    public Button moveDownBtn;

    [Header("Кнопки вращения")]
    public Button rotateLeftBtn;
    public Button rotateRightBtn;
    public Button rotateUpBtn;
    public Button rotateDownBtn;

    [Header("Кнопки зума")]
    public Button zoomInBtn;
    public Button zoomOutBtn;

    [Header("Прочие кнопки")]
    public Button resetCameraBtn;
    public Button toggleUIBtn;

    private bool isMoving = false;
    private Vector3 currentMoveDirection = Vector3.zero;

    void Start()
    {
        SetupPresetButtons();
        SetupMovementButtons();
        SetupRotationButtons();
        SetupZoomButtons();
        SetupOtherButtons();
    }

    void Update()
    {
        // Непрерывное движение при зажатых кнопках
        if (isMoving && cameraController != null)
        {
            cameraController.transform.Translate(currentMoveDirection * cameraController.moveSpeed * Time.deltaTime, Space.Self);
        }
    }

    void SetupPresetButtons()
    {
        for (int i = 0; i < presetButtons.Length; i++)
        {
            int index = i;
            presetButtons[i].onClick.AddListener(() => cameraController.MoveToPreset(index));
        }
    }

    void SetupMovementButtons()
    {
        // Движение вперед/назад
        SetupButtonHold(moveForwardBtn, Vector3.forward);
        SetupButtonHold(moveBackBtn, Vector3.back);
        SetupButtonHold(moveLeftBtn, Vector3.left);
        SetupButtonHold(moveRightBtn, Vector3.right);
        SetupButtonHold(moveUpBtn, Vector3.up);
        SetupButtonHold(moveDownBtn, Vector3.down);
    }

    void SetupRotationButtons()
    {
        rotateLeftBtn.onClick.AddListener(() => cameraController.transform.Rotate(Vector3.up, -30, Space.World));
        rotateRightBtn.onClick.AddListener(() => cameraController.transform.Rotate(Vector3.up, 30, Space.World));
        rotateUpBtn.onClick.AddListener(() => cameraController.transform.Rotate(Vector3.right, -30, Space.Self));
        rotateDownBtn.onClick.AddListener(() => cameraController.transform.Rotate(Vector3.right, 30, Space.Self));
    }

    void SetupZoomButtons()
    {
        zoomInBtn.onClick.AddListener(() => cameraController.transform.Translate(Vector3.forward * 2f, Space.Self));
        zoomOutBtn.onClick.AddListener(() => cameraController.transform.Translate(Vector3.forward * -2f, Space.Self));
    }

    void SetupOtherButtons()
    {
        resetCameraBtn.onClick.AddListener(cameraController.ResetToDefault);
        toggleUIBtn.onClick.AddListener(() => gameObject.SetActive(false));
    }

    void SetupButtonHold(Button button, Vector3 direction)
    {
        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

        // Нажатие
        var pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((data) => { StartMoving(direction); });
        trigger.triggers.Add(pointerDown);

        // Отпускание
        var pointerUp = new EventTrigger.Entry();
        pointerUp.eventID = EventTriggerType.PointerUp;
        pointerUp.callback.AddListener((data) => { StopMoving(); });
        trigger.triggers.Add(pointerUp);
    }

    void StartMoving(Vector3 direction)
    {
        isMoving = true;
        currentMoveDirection = direction;
    }

    void StopMoving()
    {
        isMoving = false;
        currentMoveDirection = Vector3.zero;
    }
}