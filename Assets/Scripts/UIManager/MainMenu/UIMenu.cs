using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEditor;

public class UIMenu : MonoBehaviour
{
    public GameObject MainMenuPanel;
    public GameObject SettingMenuPanel;
    public GameObject LoadingCanvas;

    [Header("Material Reference")]
    [SerializeField] private Material targetMaterial;

    [Header("Animation Settings")]
    [SerializeField] private float targetEmissionStrength = 3f;
    [SerializeField] private float emissionDuration = 2f;

    [Header("Settings References")]
    [SerializeField] private Animator CameraAnim;
    [SerializeField] private PlayerSettingsSO playerSettings;
    [SerializeField] private SensitivitySlider sensitivitySlider;

    [Header("Cursor Settings")]
    [SerializeField] private Animator Canvas_Animator; 
    [SerializeField] private RectTransform cursorTransform; 

    private bool isCursorActive = true;
    private Vector2 cursorPosition;
    private UnityEngine.EventSystems.EventSystem eventSystem;

    private void Start()
    {
        InitializeMainMenu();
        InitializeCursor();
    }

    private void InitializeCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        if (Canvas_Animator == null)
        {
            GameObject canvasObj = GameObject.Find("Canvas");
            if (canvasObj != null)
            {
                Canvas_Animator = canvasObj.GetComponent<Animator>();
            }
        }

        if (cursorTransform == null)
        {
            GameObject cursorObj = GameObject.Find("Cursor");
            if (cursorObj != null)
            {
                cursorTransform = cursorObj.GetComponent<RectTransform>();
            }
            else
            {
                CreateSimpleCursor();
            }
        }

        eventSystem = UnityEngine.EventSystems.EventSystem.current;

        CenterCursor();
    }

    private void CreateSimpleCursor()
    {
        Canvas mainCanvas = FindObjectOfType<Canvas>();
        if (mainCanvas == null) return;

        GameObject cursorObj = new GameObject("MenuCursor");
        cursorObj.transform.SetParent(mainCanvas.transform, false);

        Image cursorImage = cursorObj.AddComponent<Image>();
        cursorImage.color = new Color(1, 1, 1, 0.8f);

        Texture2D tex = new Texture2D(32, 32);
        for (int y = 0; y < tex.height; y++)
        {
            for (int x = 0; x < tex.width; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(16, 16));
                Color color = dist < 12 ? Color.white : Color.clear;
                tex.SetPixel(x, y, color);
            }
        }
        tex.Apply();

        Sprite circleSprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
        cursorImage.sprite = circleSprite;

        cursorTransform = cursorObj.GetComponent<RectTransform>();
        cursorTransform.sizeDelta = new Vector2(24, 24);
        cursorTransform.SetAsLastSibling(); 
    }

    private void InitializeMainMenu()
    {
        CameraAnim.SetBool("Settings", false);
        CameraAnim.SetBool("Play", false);

        LoadingCanvas.SetActive(false);
        SetEmissionStrength(0f);

        if (MainMenuPanel != null) MainMenuPanel.SetActive(true);
        if (SettingMenuPanel != null) SettingMenuPanel.SetActive(false);

        if (playerSettings != null)
        {
            playerSettings.LoadSettings();
            Debug.Log("Configuración cargada en menú principal");
        }

        if (sensitivitySlider != null)
        {
            Debug.Log("SensitivitySlider encontrado en menú principal");
        }
        else
        {
            sensitivitySlider = FindObjectOfType<SensitivitySlider>(true);
            if (sensitivitySlider != null)
            {
                Debug.Log("SensitivitySlider encontrado automáticamente");
            }
        }
    }

    private void Update()
    {
        if (isCursorActive && cursorTransform != null)
        {
            UpdateCursorPosition();
            UpdateCursorAnimation();
            HandleCursorInput();
        }
    }

    private void UpdateCursorPosition()
    {
        // Mover cursor con el mouse
        Vector2 mousePos = Input.mousePosition;
        cursorTransform.position = mousePos;
        cursorPosition = mousePos;
    }

    private void UpdateCursorAnimation()
    {
        if (Canvas_Animator == null) return;

        bool isHoveringUI = IsHoveringUI();

        Canvas_Animator.SetBool("Inspect", isHoveringUI);

        if (cursorTransform != null)
        {
            if (isHoveringUI)
            {
                cursorTransform.localScale = Vector3.one * 1.1f;
            }
            else
            {
                cursorTransform.localScale = Vector3.one;
            }
        }
    }

    private bool IsHoveringUI()
    {
        if (eventSystem == null) return false;

        // Detectar si el mouse está sobre algún elemento UI
        var pointerData = new UnityEngine.EventSystems.PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };

        var results = new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();
        eventSystem.RaycastAll(pointerData, results);

        foreach (var result in results)
        {
            if (result.gameObject != cursorTransform?.gameObject)
                return true;
        }

        return false;
    }

    private void HandleCursorInput()
    {
        // Efecto visual al hacer clic
        if (cursorTransform != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                cursorTransform.localScale = Vector3.one * 0.8f;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                cursorTransform.localScale = Vector3.one;
            }
        }
    }

    public void CenterCursor()
    {
        if (cursorTransform != null)
        {
            cursorTransform.position = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        }
    }

    public void SetCursorActive(bool active)
    {
        isCursorActive = active;

        if (cursorTransform != null)
        {
            cursorTransform.gameObject.SetActive(active);
        }

        Cursor.visible = !active;

        if (Canvas_Animator != null)
        {
            Canvas_Animator.SetBool("Inspect", false);
        }
    }

    public void DestroySpecificObject(GameObject targetObject)
    {
        if (targetObject != null)
        {
            Destroy(targetObject);
            Debug.Log("Objeto específico destruido: " + targetObject.name);
        }
    }

    public void SettingActive()
    {
        CameraAnim.SetBool("Settings", true);
        SettingMenuPanel.SetActive(true);
        MainMenuPanel.SetActive(false);
        AudioManager.Instance?.SyncSlidersInScene();
    }

    public void SettingDesactive()
    {
        CameraAnim.SetBool("Settings", false);
        SettingMenuPanel.SetActive(false);
        MainMenuPanel.SetActive(true);
    }

    public void LoadLevelOne()
    {
        CurvedTMPDownloading downloading = FindObjectOfType<CurvedTMPDownloading>(true);

        if (downloading != null)
        {
            downloading.gameObject.SetActive(true);
            StartCoroutine(PlayDownloadingAndLoad(downloading));
        }
        else
        {
            SceneManager.LoadScene(1);
        }
    }

    private IEnumerator PlayDownloadingAndLoad(CurvedTMPDownloading anim)
    {
        CameraAnim.SetBool("Play", true);

        anim.loop = false;
        anim.repeatCount = 2;
        anim.StartAnimation();

        float estimatedTime = (anim.GetComponent<TextMeshPro>().text.Length * anim.letterInterval * 2f + anim.pauseBeforeRestart * 2f) * anim.repeatCount;

        yield return new WaitForSeconds(estimatedTime);

        if (targetMaterial != null)
        {
            SetEmissionStrength(targetEmissionStrength);

            yield return new WaitForSeconds(emissionDuration);

            CameraAnim.SetBool("Play", false);
            CameraAnim.SetBool("Settings", false);
        }
        else
        {
            Debug.LogWarning("No se asignó el material objetivo para cambiar la emisión");
        }

        LoadingCanvas.SetActive(true);
        SetEmissionStrength(0f);

        Cursor.visible = true;
        SetCursorActive(false);

        SceneManager.LoadScene(1);
    }

    private void SetEmissionStrength(float strength)
    {
        if (targetMaterial != null)
        {
            targetMaterial.SetFloat("_EmissionStrength", strength);
            Debug.Log($"Emission strength cambiado a: {strength}");
        }
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void SetPlayerSettings(PlayerSettingsSO settings)
    {
        playerSettings = settings;
    }

    private void OnDestroy()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (Canvas_Animator != null)
        {
            Canvas_Animator.SetBool("Inspect", false);
        }
    }
}