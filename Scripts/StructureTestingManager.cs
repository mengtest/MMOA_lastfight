using UnityEngine;
using System.Collections;

using BlGame.Resource;
using BlGame.View;
using BlGame.GameEntity;
using BlGame.GameState;
using BlGame.FSM;
using BlGame;
using GameDefine;
public class StructureTestingManager: MonoBehaviour {
	private GameObject mPlayerObj;
	void Awake(){
		GameObject ui = GameObject.Find ("GameUI");
		ui.AddComponent<BlGameUI> ();
	}
	// Use this for initialization
	void Start () {
		this.LoadBattle ();


	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void LoadBattle(){
		//LoadScene.Instance.LoadAsignedSene ("Scenes/pvp_001")
		//ResourcesManager.HandleFinishLoadLevel += onFinishLoadLevel;

		DontDestroyOnLoad (GameObject.Find ("GameUI"));
		DontDestroyOnLoad (this);
		DontDestroyOnLoad (GameObject.Find("JxBlGame"));

		ResourcesManager.Instance.Init ();
		ResourcesManager.HandleFinishLoadLevel del = onFinishLoadLevel;
		ResourcesManager.Instance.loadLevel("cenes/pvp_001",del);

		//HeroTimeLimitWindow window = WindowManager.Instance.GetWindow (EWindowType.EMT_HeroTimeLimitWindow) as HeroTimeLimitWindow;
		//string path = GameDefine.GameConstDefine.LoadMonsterModels;
		//Entity entity;
		//entity.r
		Ientity player = new Iselfplayer(1001,EntityCampType.CampTypeA);
		player.entityType = EntityType.Player;
		player.ObjTypeID = 10003;//<szNOStr>10003</szNOStr>


		new EntityManager ();
		new PlayerManager ();

		//依赖Audio?
		new GameStateManager ();
		//GameStateManager.Instance.EnterDefaultState();
		//GameStateManager.Instance.ChangeGameStateTo(GameStateType.GS_Play);
		PlayState state = GameStateManager.Instance.GetCurState () as PlayState;

		Vector3 playerDefPosition = this.ConvertPosToVector3 (new Vector2 (21600, 7400));
		mPlayerObj = EntityManager.Instance.CreateEntityModel (player, 1001, new Vector3 (0, 0, 0), playerDefPosition);

		DontDestroyOnLoad (mPlayerObj);
		//PlayerManager.Instance.LocalPlayer = new 

		SkillWindow window = WindowManager.Instance.GetWindow (EWindowType.EMT_SkillWindow) as SkillWindow;
		window.Show();

		//摇杆
		//依赖于PlayerManager.Instance.LocalAccount
		//还依赖于PlayerManager.Instance.LocalPlayer.RealEntity
		PlayerManager.Instance.LocalAccount = (Iplayer)player;
		PlayerManager.Instance.LocalAccount.SetObjType (GameDefine.ObPlayerOrPlayer.PlayerType);//play state需要
		GamePlayWindow panel = WindowManager.Instance.GetWindow (EWindowType.EWT_GamePlayWindow) as GamePlayWindow;
		panel.Show ();

//		GameObject uiRoot = GameObject.Find ("GameUI");
//		ResourceUnit unit = ResourcesManager.Instance.loadImmediate ("Guis/UIMainWindow",ResourceType.PREFAB);
//		GameObject virtualPanel = GameObject.Instantiate (unit.Asset) as GameObject;
//		virtualPanel.transform.parent = uiRoot.transform;


//		child.transform.parent = 

	//	gameObject.transform.
		//gameObject.transform.
	}

	private void onFinishLoadLevel()
	{
		GameObject ui = GameObject.Find ("GameUI");
		Transform panel = ui.transform.FindChild ("Camera/UpdatePatch");
		panel.gameObject.SetActive (false);

		//必须在level加载完之后才有Camera.main
		//必须在初始化之后
		Iplayer player = PlayerManager.Instance.LocalAccount;
		player.OnFSMStateChange (EntityFreeFSM.Instance);
		AudioManager.Instance.StopHeroAudio();


		GameMethod.CreateCharacterController (player);//controller move重要

		//GameObject terrian = GameObject.Find ("GameObject");
		//mPlayerObj.transform.parent = terrian.transform;



	}

	private Vector3 ConvertPosToVector3(Vector2 loc)
	{
		if (loc != null) {
			float height = 60;//GetGlobalHeight()
			return new Vector3 ((float)loc.x / 100.0f, height, (float)loc.y / 100.0f);
		}
		else
			return Vector3.zero;
	}
}
