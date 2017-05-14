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
		//实际上创建场景的player实例
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


		Ientity diren = new Iplayer (1002, EntityCampType.CampTypeB);
		diren.entityType = EntityType.Player;
		diren.ObjTypeID = 10004;
		Vector3 direnPosition = this.ConvertPosToVector3 (new Vector2 (21600, 7430));
		GameObject direnObject = EntityManager.Instance.CreateEntityModel (diren, 1002, new Vector3 (0, 0, 0), direnPosition);
		DontDestroyOnLoad (direnObject);

		System.Collections.Generic.List<string> sources = new System.Collections.Generic.List<string>();
		//sources.Add("Media/Effect/Model/Materials/guangquan.tga");
		sources.Add	("Audio/sounddead/Nvyao5_Dead");//mp3
		sources.Add ("effect/ui_effect/Remove_cooling_effect");//prefab
		sources.Add ("Audio/sounddead/Nvyao5_Attack");
		//sources.Add ("effect/skill/release/sword_ex");//skill manager id : 140026
		ResourceCache.Instance.preLoadResources(sources);

		ResourceCache.Instance.preLoadResources (10004);
	}
	
	private void onFinishLoadLevel()
	{
		GameObject ui = GameObject.Find ("GameUI");
		Transform panel = ui.transform.FindChild ("Camera/UpdatePatch");
		panel.gameObject.SetActive (false);

		//必须在level加载完之后才有Camera.main
		//必须在初始化之后
		Iplayer player = PlayerManager.Instance.LocalAccount;
		//set target这个实现方法放在FreeFSM前面，因为FressFSM会update camera，需要有target才好update
		GameObject mainCamera = GameObject.FindGameObjectWithTag ("MainCamera");
		mainCamera.GetComponent<SmoothFollow> ().target = player.objTransform;

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
