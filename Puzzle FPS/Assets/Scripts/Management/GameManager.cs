using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

	public bool EDITORMODE = true;

	[Header("Player")]
	[SerializeField]
	GameObject PlayerPrefab;
	public GameObject PlayerSpawnPos;

	public GameObject PlayerInstance;

    [Header("Game Components")]
    [SerializeField]
    PlayerController playerScript;
	[SerializeField]
	CameraControl playerCamera;

    [Header("Game State Variables")]
    [SerializeField]
    bool isPaused;
	

	GameEvent LevelOneEvent;
	
	GameEvent LevelTwoEvent;
	
	GameEvent LevelThreeEvent;

	//Bool to determine when a scene with the player in it has started (I.E. Not in the main menu or level selection.
	//This lets the script know it can start tracking game events like winning or losing.
	[SerializeField]
	bool playStarted;

    [Header("Event Tracking")]
    [SerializeField]
    int enemiesRemaining;

	#region GM Access Methods
	public PlayerController PlayerScript()
    {
        return playerScript;
    }
	public CameraControl CameraControl()
	{
		return playerCamera;
	}
	public bool IsPaused()
    {
        return isPaused;
    }
	public bool PlayStarted()
	{
		return playStarted;
	}
	public int EnemiesRemaining()
    {
        return enemiesRemaining;
    }
    #endregion
	private void Awake()
	{
		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	void Start()
	{
		BeginGame();
		InitializePlay();
	}
    private void LateUpdate()
    {
        if (playStarted)
        {
			ManagePlayerTasks();
        }
		
    }
    #region Public Methods

    #region Game Goal Methods
	public void UpdateEnemyCount(int amt)
    {
		enemiesRemaining += amt;
    }
    #endregion
    public void InitializePlay()
	{
		if (!EDITORMODE)
		{
			MenuManager.Instance.DeactivateAllMenus();
			SceneControl.Instance.LoadLevelOne();
		}
		GameEventManager.Instance.FindEvents();


		LevelSetup();
		HUDManager.Instance.ShowHUD();
		playStarted = true;
		

	}

    private void FetchEvents()
    {
		if (GameEventManager.Instance.HasEvents())
		{
			if (SceneControl.Instance.GetCurrentScene().name == "Level_One")
			{
				LevelOneEvent = GameEventManager.Instance.GameEvents[0];
			}
			else if (SceneControl.Instance.GetCurrentScene().name == "Level_Two")
			{
				LevelTwoEvent = GameEventManager.Instance.GameEvents[0];
			}
			else if (SceneControl.Instance.GetCurrentScene().name == "Level_Three")
			{
				LevelThreeEvent = GameEventManager.Instance.GameEvents[0];
			}
		}
	}

    public void LevelSetup()
	{
		FetchEvents();
		PlayerInstance = Instantiate(PlayerPrefab);
		
		playerScript = PlayerInstance.GetComponent<PlayerController>();
		playerCamera = Camera.main.GetComponent<CameraControl>();

		AssertPlayerPreferencesToScript();

		Cursor.lockState = CursorLockMode.Locked;

		playerCamera.ToggleCursorVisibility();
		MenuManager.Instance.CanToggleGameMenu = true;

		GameEventManager.Instance.GenerateEvents();
	}
    public void AssertPlayerPreferencesToScript()
	{
		//Will take the active values from Player Preferences and assign those settings to the variables
		//used in the player and camera scripts
		//Should be called right before the player is dropped in and gains control of the player.
		//Script values should be assigned from preferences, controls should be enabled and cursor hidden
		playerScript.HP = PlayerPreferences.Instance.HP;
		playerScript.MoveSpeed = PlayerPreferences.Instance.MoveSpeed;
		playerScript.SprintMod = PlayerPreferences.Instance.SprintMod;
		playerScript.JumpTimes = PlayerPreferences.Instance.JumpTimes;
		playerScript.JumpSpeed = PlayerPreferences.Instance.JumpSpeed;
		playerScript.PlayerGravity = PlayerPreferences.Instance.PlayerGravityStrength;
		playerScript.PlayerForce = PlayerPreferences.Instance.PlayerForceStrength;


		playerScript.zoomMax = PlayerPreferences.Instance.ZoomMax;
		playerScript.zoomInSpeed = PlayerPreferences.Instance.ZoomInSpeed;
		playerScript.zoomOutSpeed = PlayerPreferences.Instance.ZoomOutSpeed;


		playerScript.ShootRate = PlayerPreferences.Instance.ShootRate;
		playerScript.ShootDistance = PlayerPreferences.Instance.ShootDistance;
		playerScript.ShotDamage = PlayerPreferences.Instance.ShotDamage;

		playerCamera.HorizontalSensitivity = PlayerPreferences.Instance.SensitivityHorizontal;
		playerCamera.VeritcalSensitivity = PlayerPreferences.Instance.SensitivityVertical;
		playerCamera.VeticalLockMin = PlayerPreferences.Instance.VerticalLockMin;
		playerCamera.VeticalLockMax = PlayerPreferences.Instance.VerticalLockMax;
		playerCamera.InvertX = PlayerPreferences.Instance.InvertX;

	}

	public void RestartLevel()
	{
		ClearLevel();
		Debug.Log("Restarting Level");
		//Restart a level without going all the way back to the main menu
		SceneControl.Instance.SceneRestart_CurrentScene();

		//reload player and variable settings
		InitializePlay();
	}
	public void RestartGame()
	{
		playerCamera.ToggleCursorVisibility();
		ClearLevel();

		//Call to scene control to handle unloading anything we are currently in
		SceneControl.Instance.SceneRestart_Game();

		//This call loads the main menu scene and menus
		BeginGame();

		Cursor.lockState = CursorLockMode.Confined;
	}
	public void LoseGame()
	{
		Pause();

		MenuManager.Instance.DisplayLoseMenu();
	}
	public void WinGame()
	{
		Pause();

		MenuManager.Instance.DisplayWinMenu();
	}
	public void ToggleGameMenu()
	{
		if (MenuManager.Instance.GameMenuIsUp())
		{
			MenuManager.Instance.CloseGameMenu();
			UnPause();
			playerCamera.ToggleCursorVisibility();

		}
		else
		{
			MenuManager.Instance.DisplayGameMenu();
			Pause();
			playerCamera.ToggleCursorVisibility();
		}
	}
    #endregion
    #region Private Methods
    private void Pause()
    {
		isPaused = true;
		Time.timeScale = 0f;
		Cursor.lockState = CursorLockMode.Confined;
	}
	private void UnPause()
	{
		isPaused = false;
		Time.timeScale = 1f;
		Cursor.lockState = CursorLockMode.Locked;
	}
	void BeginGame()
	{
		if (!EDITORMODE)
		{
			isPaused = true;
			playStarted = false;

			//Deactivate any menus up from a possible last play
			DeactivateUI();

			SceneControl.Instance.LoadMainMenuScene();

			MenuManager.Instance.InitializeMenusText();

			MenuManager.Instance.DisplayMainMenu();
		} else
        {
			isPaused = false;
		}
	}
	private void DeactivateUI()
    {
		MenuManager.Instance.DeactivateAllMenus();
		HUDManager.Instance.CloseHUD();
	}
	private void ManagePlayerTasks()
	{
		//could probably only check this when an interaction happens or something
		if (GameEventManager.Instance.GameEvents.Count > 0)
		{
			//Track by highlighting active quest or event, remove or cross out when done, add new tasks as they appear.
			GameEventManager.Instance.UpdateEvents();
			if (SceneControl.Instance.GetCurrentScene().name == "Level_One")
			{
				if (LevelOneEvent.ReturnEventCompletion(LevelOneEvent.Conditions))
				{
					GameEventManager.Instance.EventsCompleted++;
					Debug.Log("Loading Level Two...");
					ClearLevel();
					SceneControl.Instance.LoadLevelTwo();
					LevelSetup();
				}
			}


			if (GameEventManager.Instance.EventsCompleted == 3)
			{
				WinGame();
			}
		}
	}
	private void ClearLevel()
    {
		GameEventManager.Instance.ClearEventListUI();
		GameEventManager.Instance.GameEvents.Clear();
		Destroy(PlayerInstance);
		PlayerSpawnPos = null;
	}
    #endregion
}
