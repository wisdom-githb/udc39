using UnityEngine;
using UnityEngine.UI;

public sealed class LevelGameManager : MonoBehaviour
{
    [Header("Gameplay")]
    [SerializeField] private Transform player;
    [SerializeField] private SphereController playerController;
    [SerializeField] private Collectible[] collectibles;
    [SerializeField] private GameObject exitDoor;

    [Header("UI")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private Text hudText;
    [SerializeField] private Text finalTimeText;
    [SerializeField] private Button startButton;
    [SerializeField] private Button restartButton;

    private Vector3 playerStartPosition;
    private Quaternion playerStartRotation;
    private float runStartedAt;
    private float elapsedTime;
    private int collectedCount;
    private bool isRunning;

    public static LevelGameManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        playerStartPosition = player.position;
        playerStartRotation = player.rotation;
        startButton.onClick.AddListener(StartGame);
        restartButton.onClick.AddListener(RestartGame);
        ShowStartScreen();
    }

    private void OnDestroy()
    {
        startButton.onClick.RemoveListener(StartGame);
        restartButton.onClick.RemoveListener(RestartGame);

        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Update()
    {
        if (!isRunning)
        {
            return;
        }

        elapsedTime = Time.realtimeSinceStartup - runStartedAt;
        UpdateHud();
    }

    public void TryCollect(Collectible collectible)
    {
        if (!isRunning || collectible.IsCollected)
        {
            return;
        }

        collectible.MarkCollected();
        collectedCount++;
        UpdateHud();

        if (collectedCount >= collectibles.Length)
        {
            exitDoor.SetActive(true);
        }
    }

    public void TryCompleteLevel()
    {
        if (!isRunning || collectedCount < collectibles.Length)
        {
            return;
        }

        elapsedTime = Time.realtimeSinceStartup - runStartedAt;
        isRunning = false;
        playerController.ResetMotion();
        playerController.enabled = false;
        finalTimeText.text = "LEVEL COMPLETE!\nTime: " + elapsedTime.ToString("0.00") + " seconds";
        levelCompletePanel.SetActive(true);
    }

    private void StartGame()
    {
        BeginRun();
    }

    private void RestartGame()
    {
        BeginRun();
    }

    private void BeginRun()
    {
        elapsedTime = 0f;
        runStartedAt = Time.realtimeSinceStartup;
        collectedCount = 0;
        isRunning = true;

        player.position = playerStartPosition;
        player.rotation = playerStartRotation;
        playerController.ResetMotion();
        playerController.enabled = true;

        foreach (Collectible collectible in collectibles)
        {
            collectible.ResetCollectible();
        }

        exitDoor.SetActive(false);
        startPanel.SetActive(false);
        levelCompletePanel.SetActive(false);
        hudPanel.SetActive(true);
        UpdateHud();
    }

    private void ShowStartScreen()
    {
        isRunning = false;
        playerController.ResetMotion();
        playerController.enabled = false;
        exitDoor.SetActive(false);
        hudPanel.SetActive(false);
        levelCompletePanel.SetActive(false);
        startPanel.SetActive(true);

        foreach (Collectible collectible in collectibles)
        {
            collectible.ResetCollectible();
        }
    }

    private void UpdateHud()
    {
        hudText.text = "Time: " + elapsedTime.ToString("0.00") + "   Cubes: " + collectedCount + "/" + collectibles.Length;
    }
}
