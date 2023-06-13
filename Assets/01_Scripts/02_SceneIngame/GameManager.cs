using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.AI;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour, Observer
{
    Scene stageScene;
    //카메라, 포인트 지점 저장, Find된 적 리스트
    List<Transform> cameraPos = new List<Transform>();
    List<Transform> pointPos = new List<Transform>();
    List<Transform> spawnPos = new List<Transform>();
    Dictionary<GameObject, List<Transform>> enemyDatas = new Dictionary<GameObject, List<Transform>>();

    //스테이지
    public GameObject _Stage;
    //스테이지 내 모든 자식 객체.
    GameObject[] StageChilds;

    //RF,LF,LB,RB
    GameObject squadField;

    [SerializeField]
    Camera mainCamera;

    // 모든 캐릭터 프리팹 배열!
    [SerializeField]
    GameObject[] allCharacterArr;

    // 타겟 객체 저장 배열
    GameObject[] targetEnemys;

    // 스쿼드 분대원
    public GameObject[] squadCharac;

    // navMeshAgent배열
    NavMeshAgent[] navMeshAgents;
    int[] nDestinationIndex;

    bool isGameEnd;

    // 카메라 무빙 위치 포지션, 카메라 관리 인덱스
    private int cameraTargetIndex;

    // 엔드페이즈 UI 컴포넌트 및 제어 변수
    [SerializeField]
    TextMeshProUGUI txt_EndText;
    [SerializeField]
    Image img_EndTextBgr;
    [SerializeField]
    TextMeshProUGUI txt_EndText2;
    [SerializeField]
    Image img_EndTextBgr2;
    bool isCallScene;
    bool isClear;

    static GameManager instance;

    public static GameManager Instance
    {
        get { return instance; }
    }


    private void Awake()
    {
        isClear=true;
        isCallScene = false;
        img_EndTextBgr.gameObject.SetActive(false);
        txt_EndText.gameObject.SetActive(false);
        txt_EndText2.gameObject.SetActive(false);
        img_EndTextBgr2.gameObject.SetActive(false);
        cameraTargetIndex = 0;
        isGameEnd =false;
        instance = this;
        squadCharac = new GameObject[4];
        navMeshAgents = new NavMeshAgent[4];
        targetEnemys = new GameObject[4];
        nDestinationIndex = new int[4];

    }

    void Start()
    {
        SceneFinder();
        ObjectSet();

        #region 스쿼드 Set 처리

        mainCamera.transform.position = cameraPos[0].position;
        mainCamera.transform.rotation = cameraPos[0].rotation;
        cameraTargetIndex++;

        // 스쿼드 포지션
        Transform rightFront = squadField.transform.GetChild(0);
        Transform leftFront = squadField.transform.GetChild(1);
        Transform leftBack = squadField.transform.GetChild(2);
        Transform rightBack = squadField.transform.GetChild(3);

        //스쿼드 캐릭터의 프리팹을 생성하고, 캐릭터 스크립트를 할당해줍니다.
        for(int i=0; i<allCharacterArr.Length; i++)
        {
            for(int j=0; j< squadCharac.Length; j++)
            {
                if (SquadSceneManager.SQUAD_Characters[j] != null)
                {
                    if(SquadSceneManager.SQUAD_Characters[j].editorName == allCharacterArr[i].gameObject.name)
                    {
                        // 프리팹을 생성하고 캐릭터 매니저 스크립트를 가져옵니다.
                        // 이름을 변경하고, 컴포넌트를 가져와 캐릭터 데이터를 Set, NavMesh를 배열에 저장시킵니다.
                        string prefabName = allCharacterArr[i].name.Replace("(Clone)", "");
                        squadCharac[j] = Instantiate(allCharacterArr[i], _Stage.transform);
                        squadCharac[j].name = prefabName;
                        CharacterManager charManager = squadCharac[j].GetComponent<CharacterManager>();
                        NavMeshAgent navMeshAgent = squadCharac[j].GetComponent<NavMeshAgent>();

                        // 캐릭터 매니저 스크립트의 myCharacter 변수에 캐릭터 클래스 데이터를 할당합니다.
                        charManager.myCharacter = SquadSceneManager.SQUAD_Characters[j];
                        navMeshAgents[j] = navMeshAgent;

                        // Subject 객체들에게 GameManager를 Observer로 인식하도록, 배열에 저장시킵니다.
                        CharacterViewRange charSubject = squadCharac[j].GetComponent<CharacterViewRange>();
                        charSubject.Attach(this);

                        // enemyDatas Dictionary에 키값을 추가하고 데이터를 초기화합니다.
                        if (!enemyDatas.ContainsKey(squadCharac[j]))
                        {
                            enemyDatas.Add(squadCharac[j], null);
                        }
                    }
                }
            }
        }
        #endregion


        // 몬스터 생성함수 호출
        EnemyCreateSetManager ecsm = GameObject.Find("EnemyCreateSetManager").GetComponent<EnemyCreateSetManager>();
        ecsm.CreateMonsterFunc(spawnPos, _Stage);


        // 스쿼드 포지션 배치
        for (int i = 0; i < squadCharac.Length; i++)
        {
            if (squadCharac[i] != null)
            {
                Debug.Log("squadCharac" + i + "position : " + squadCharac[i].transform.position);
                if (i == 0) squadCharac[i].transform.position = rightFront.position;
                else if (i == 1) squadCharac[i].transform.position = leftFront.position;
                else if (i == 2) squadCharac[i].transform.position = leftBack.position;
                else if (i == 3) squadCharac[i].transform.position = rightBack.position;
                Debug.Log("squadCharac" + i + "position : " + squadCharac[i].transform.position);
            }
        }
        SoundManager.Instance.PlayBGM(SoundManager.eTYPE_BGM.InGame,0.3f);
        Debug.Log("cameraPos : " + cameraPos.Count);
    }

    private void Update()
    {
        //IsCharacterArrive();
        if (!isGameEnd)
        {
            // enemyDatas의 킷값(캐릭터)가 null이라면, 타겟도 null로 바꾼다.
            for (int i = 0; i < enemyDatas.Count; i++)
            {
                var pair = enemyDatas.ElementAt(i);
                if (pair.Key == null)
                {
                    enemyDatas[pair.Key] = null;
                    targetEnemys[i] = null;
                }
            }
            
            AI_Move();
        }
        else
        {
            if (isCallScene == false)
                GameEnd();
        }

    }

    private void LateUpdate()
    {
        CameraMover();
    }


    #region 옵저버 패턴

    public void Notify(List<Transform> data)
    {
    }

    public void FindEnemyData(List<Transform> data, GameObject charac)
    {

        if (enemyDatas.ContainsKey(charac))
        {
            enemyDatas[charac] = data;
        }
        else
        {
            enemyDatas.Add(charac, data);
        }

    }

    #endregion


    #region 게임 엔딩
    void IsCharacterArrive()
    {
        
        for(int i=0; i< squadCharac.Length; i++)
        {
            if (squadCharac[i] != null)
                return;
        }
        // 모든 캐릭터가 null이면 게임 패배
        isClear = false;
        isGameEnd = true;
    }

    void GameEnd()
    {
        isCallScene = true;
        // 엔드 텍스트 UI 활성화
        txt_EndText.gameObject.SetActive(true);
        img_EndTextBgr.gameObject.SetActive(true);
        if (isClear==false)
        {
            txt_EndText.text = "미션 실패!";
            Invoke("ReturnToLobby", 5f);
        }
        else
        {
            int level = 0;
            string stageName = _Stage.name;
            string levelString = stageName.Substring(5); // "Stage" 이후의 문자열 추출
            int.TryParse(levelString, out level);        // 스테이지 단계 인트형으로 초기화

            if(UI_Manager.currentStageLevel < level + 1)
                UI_Manager.currentStageLevel = level + 1;    // 다음 단계 해금을 위한 전역변수 설정

            //퀘스트 보상
            QuestReward_Manager qrm = GameObject.Find("QuestReward_Manager").GetComponent<QuestReward_Manager>();
            qrm.ClearRewardFunc(level);

            txt_EndText.text = "미션 성공!";
            Reward(level);
        }
    }

    void Reward(int level)
    {
        // 보상 UI창 출력
        txt_EndText2.gameObject.SetActive(true);
        img_EndTextBgr2.gameObject.SetActive(true);

        // 골드 보상 Income
        UI_Manager.GOLD += level * 1000;

        for (int i = 0; i < SquadSceneManager.SQUAD_Characters.Count; i++)
        {
            if (SquadSceneManager.SQUAD_Characters[i] == null)
                continue;

            // 보상을 줄 스쿼드 캐릭터 찾기
            ChracterClass squadCharacter = SquadSceneManager.SQUAD_Characters[i];

            // UI_Manager.myCharacterList에서 일치하는 캐릭터를 찾습니다.
            ChracterClass myCharacter = UI_Manager.myCharacterList.Find(character => character.characterID == squadCharacter.characterID);
            ChracterLevelUp(myCharacter,level);

        }

        txt_EndText2.text = "보상\n" + "골드 : " + level * 1000 + "\n" + "캐릭터 경험치 : " + level * 100;
        Invoke("ReturnToLobby", 5f);
    }

    void ChracterLevelUp(ChracterClass character, int level)
    {
        if (character != null)
        {
            int currentExp = level * 100;
            int maxLevel = character.characMaxExp.Length;

            // 배열의 인덱스가 캐릭터 레벨을 초과하지 않도록 처리
            int maxExpIndex = Mathf.Min(character.characterLevel - 1, maxLevel - 1);
            int maxExp = character.characMaxExp[maxExpIndex];
            while (currentExp >= maxExp)
            {
                // 캐릭터 보상
                currentExp -= maxExp;
                character.characterLevel++;
                character.characCurrentExp = currentExp;
                character.characterCurrnetHP += 100;
                character.characterMaxHP+= 100;
                character.characterDef += 5;
                character.characterPower+= 15;

                // 배열의 인덱스가 캐릭터 레벨을 초과하지 않도록 처리
                maxExpIndex = Mathf.Min(character.characterLevel - 1, maxLevel - 1);
                maxExp = character.characMaxExp[maxExpIndex];
            }

            character.characCurrentExp = currentExp;
        }
    }
    void ReturnToLobby()
    {
        SceneLoadManager._instance.SceneLoadder("10_StartMissionUI_Scene");
    }


    #endregion




    #region AI

    #region AI 이동
    void AI_Move()
    {
        for (int i = 0; i < navMeshAgents.Length; i++)
        {
            // 전투 페이즈 유무 파악용 부울 변수
            bool isBattle = false;
            if (navMeshAgents[i] == null)
                continue;

            CharacterManager charManager = squadCharac[i].GetComponent<CharacterManager>();

            // 본인 캐릭터 Range 범위 내에 적이 존재한다면, 전투 페이즈로 넘어갑니다.
            if (enemyDatas.ContainsKey(squadCharac[i]) && enemyDatas[squadCharac[i]] != null && enemyDatas[squadCharac[i]].Count > 0)
            {
                isBattle = true;
                navMeshAgents[i].isStopped = true;

                // 전투 페이즈로 돌입
                AI_Attack(charManager);
            }
            else if (enemyDatas.ContainsKey(squadCharac[i]) && enemyDatas[squadCharac[i]] != null && enemyDatas[squadCharac[i]].Count <= 0)
            {
                if (targetEnemys[i] == null)
                {
                    // 아군 캐릭터의 Range 범위 내에 적을 탐색합니다.
                    foreach (var enemyData in enemyDatas)
                    {
                        if (enemyData.Value == null || enemyData.Value.Count == 0)
                        {
                            // 적이 없는 경우, 다음 루프로 넘깁니다.
                            continue;
                        }

                        // 적이 있을 경우, Target으로 삼습니다.
                        isBattle = true;
                        targetEnemys[i] = enemyData.Value[0].gameObject;
                        break;
                    }
                }
                else
                {
                    isBattle = true;
                }

                if (isBattle)
                {
                    if (targetEnemys[i] != null && enemyDatas.ContainsKey(squadCharac[i]) && enemyDatas[squadCharac[i]].Contains(targetEnemys[i].transform))
                    {
                        // 적을 탐지했고, 타겟이 캐릭터 본인의 enemyDatas[squadCharac[i]] 리스트에 포함된 경우
                        // 이동을 멈추고 전투 페이즈로 진입합니다.
                        navMeshAgents[i].isStopped = true;
                        AI_TargetToAttack(charManager, targetEnemys[i]);
                    }
                    else if (targetEnemys[i] != null)
                    {
                        // 타겟이 enemyDatas[squadCharac[i]] 리스트에 포함되지 않은 경우
                        // 이동을 시작합니다.
                        navMeshAgents[i].SetDestination(targetEnemys[i].transform.position);
                        charManager.setAniControl_Int(ChracterClass.eChracter_State.Move);
                        navMeshAgents[i].isStopped = false;
                    }
                }
                else
                {
                    // 적이 없다면 이동을 다시 시작합니다.
                    navMeshAgents[i].isStopped = false;
                    charManager.setAniControl_Int(ChracterClass.eChracter_State.Move);
                }
            } // else

            if (navMeshAgents[i].remainingDistance <= navMeshAgents[i].stoppingDistance + 1.5f && isBattle ==false)
            {
                if (nDestinationIndex[i] >= pointPos.Count)
                {
                    // 적 추적으로 인하여, 포인트 배열의 범위를 초과했음에도 최종목적지에 도착하지 않았을 경우, 인덱스를 보정합니다.
                    if (nDestinationIndex[i] > pointPos.Count - 1 && Vector3.Distance(squadCharac[i].transform.position, pointPos[pointPos.Count - 1].position) > 1.5f)
                    {
                        // pointPos의 원소들 중 squadCharac[i].transform.position의 위치와 가장 가까운 인덱스 번째 장소를 선택
                        float minDistance = float.MaxValue;
                        int closestIndex = -1;

                        for (int j = 0; j < pointPos.Count; j++)
                        {
                            // 반복문을 돌며, 각 포인트간 거리를 계산하여, 현재 위치로부터 가장 가까운 지점을 포인트로 삼습니다.
                            float distance = Vector3.Distance(squadCharac[i].transform.position, pointPos[j].position);
                            if (distance < minDistance)
                            {
                                minDistance = distance;
                                closestIndex = j;
                            }
                        }

                        if (closestIndex != -1)
                        {
                            nDestinationIndex[i] = closestIndex;
                        }
                    }

                    if(Vector3.Distance(squadCharac[i].transform.position, pointPos[pointPos.Count - 1].position) <= 1.5f)
                    {
                        // 목적지에 도착했을 경우 멈춥니다.
                        navMeshAgents[i].isStopped = true;
                        charManager.setAniControl_Int(ChracterClass.eChracter_State.None);
                        isGameEnd = true;
                    }
                }
                else
                {
                    navMeshAgents[i].isStopped = false;
                    // 인덱스 값을 보정하고 destination을 초기화 합니다.
                    navMeshAgents[i].SetDestination(pointPos[nDestinationIndex[i]].position);
                    nDestinationIndex[i]++;

                    charManager.setAniControl_Int(ChracterClass.eChracter_State.Move);
                }
            }
        }
    }
    #endregion

    #region AI 공격
    void AI_Attack(CharacterManager character)
    {
        character.setAniControl_Int(ChracterClass.eChracter_State.Attack);
        if(character.getIsSkill_Ing() == false)
        {
            // 루프 플래그 확인 후, 함수 호출
            character.SkillLoopOn();
        }
    }

    void AI_TargetToAttack(CharacterManager character, GameObject target)
    {
        character.setAniControl_Int(ChracterClass.eChracter_State.Attack);
        // 캐릭터 매니저의 타겟을 세팅
        character.setTarget(target);
        if(character.getIsSkill_Ing() == false)
        {
            // 루프 플래그 확인 후, 함수 호출
            character.SkillLoopOn();
        }
    }
    #endregion

    #endregion




    #region 카메라 무빙

    void CameraMover()
    {
        float cameraMoveSpeed = 0.2f;

        // 카메라가 마지막 위치에 도착한 경우, 이동을 멈춥니다.
        if (cameraTargetIndex >= cameraPos.Count)
            return;

        // 현재 위치에서 다음 위치로 부드럽게 이동합니다.
        Vector3 desiredPosition = Vector3.Lerp(mainCamera.transform.position, cameraPos[cameraTargetIndex].position, Time.deltaTime * cameraMoveSpeed);

        // 캐릭터의 Z축 위치에 맞춰 카메라의 Z축 위치를 조정합니다.
        if(squadCharac != null)
        {
            for(int i=0; i< squadCharac.Length; i++)
            {
                if (squadCharac[i] != null)
                {
                    desiredPosition.z = squadCharac[i].transform.position.z;
                    break;
                }
            }
        }
        
        // 카메라의 포지션과 로테이션을 세팅합니다.
        mainCamera.transform.position = desiredPosition;
        mainCamera.transform.rotation = cameraPos[cameraTargetIndex].rotation;

        // 다음 위치에 충분히 가까워지면 다음 위치로 이동합니다.
        if (Vector3.Distance(mainCamera.transform.position, cameraPos[cameraTargetIndex].position) < 1.2f)
        {
            Debug.Log("cameraPos.Index : " + cameraTargetIndex);
            cameraTargetIndex++;
        }
    }

    #endregion




    #region 최초 씬로드 데이터 셋
    void SceneFinder()
    {
        // 현재 씬을 가져옵니다.
        Scene currentScene = SceneManager.GetActiveScene();

        // 모든 로드된 씬을 가져옵니다.
        Scene[] loadedScenes = new Scene[SceneManager.sceneCount];
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            loadedScenes[i] = SceneManager.GetSceneAt(i);
        }

        // 첫 번째로 발견된 현재 씬이 아닌 씬을 가져옵니다.
        Scene targetScene = currentScene;
        foreach (Scene scene in loadedScenes)
        {
            if (scene != currentScene)
            {
                targetScene = scene;
                break;
            }
        }

        stageScene = targetScene;
        // 타겟 씬의 이름을 출력합니다.
        Debug.Log(targetScene.name);
    }

    void ObjectSet()
    {
        GameObject[] rootObjects = stageScene.GetRootGameObjects();

        foreach (GameObject item in rootObjects)
        {
            if (item.CompareTag("Stage"))
            {
                _Stage = item;
                break;
            }
        }

        // LINQ 메서드 체인으로 Where() 메서드를 호출합니다.
        // Select() 메서드를 호출해서 각각의 Transform 컴포넌트에 대응하는 GameObject를 선택합니다.
        // GameObject는 Component나 MonoBehaviour를 상속받지 않으므로, GameObject에 대해 GetComponent 메서드를 호출하면 위와 같은 예외가 발생
        IEnumerable<GameObject> stageChilds = _Stage.GetComponentsInChildren<Transform>()
        .Where(t => t.gameObject != _Stage.gameObject)
        .Select(t => t.gameObject);

        // ToArray() 메서드를 호출해서, 선택된 GameObject를 배열로 변환 저장합니다.
        StageChilds = stageChilds.ToArray();

        for (int i = 0; i < StageChilds.Length; i++)
        {
            if (StageChilds[i].gameObject.CompareTag("CameraPos"))
            {
                cameraPos.Add(StageChilds[i].transform);
            }

            if (StageChilds[i].gameObject.CompareTag("Point"))
            {
                pointPos.Add(StageChilds[i].transform);
            }


            if (StageChilds[i].gameObject.CompareTag("SquadField"))
            {
                squadField = StageChilds[i];
            }

            if (StageChilds[i].gameObject.CompareTag("SpawnPoint"))
            {
                spawnPos.Add(StageChilds[i].transform);
            }
        }


    }
    #endregion

    public bool GetIsGameEnd()
    {
        return isGameEnd;
    }


    public void ToLobbyBtn()
    {
        SoundManager.Instance.PlayEffect_OnMng(SoundManager.eTYPE_EFFECT.BUTTON);
        SceneLoadManager._instance.SceneLoadder("10_StartMissionUI_Scene");
        SceneManager.UnloadSceneAsync(stageScene);
    }


    private void OnDestroy()
    {
        foreach (GameObject item in squadCharac)
        {
            CharacterViewRange charSubject = item?.GetComponent<CharacterViewRange>();
            charSubject?.Detach(this);
        }
    }


}
