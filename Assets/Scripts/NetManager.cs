using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEditor;
using UnityEngine;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Internal;
using System;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;


public class NetManager : MonoBehaviour
{
    //Loby->플레이어가 원하는 게임을 찾거나, 새겜 만들기
    //Relay->매칭된 플레이어들의 Relay의 JoinCode로 연결되어 멀티 환경 유지
    private Lobby curLobby;
    public int readyCount;
    public PlayerInfo playerInfo;
    private string token;
    [Header("로그인 패널 관련")]
    [SerializeField] private GameObject loginPannel;
    [Header("방상태 관련 tmp")]
    [SerializeField] private TextMeshProUGUI roomId;
    [SerializeField] private TextMeshProUGUI joinedPlayers;
    [SerializeField] private TextMeshProUGUI role;
    [SerializeField] private GameObject multiplayerBtn;
    [SerializeField] private GameObject multiplayerLobbyPannel;
    [Header("참가 플레이어 btn")]
    [SerializeField] private Button startBtn;
    [SerializeField] private TextMeshProUGUI readyTxt;




    private async void Start() //비동기->동시에 일어나지 않는다
    {
        await UnityServices.InitializeAsync();
        InitUI();
        PlayerAccountService.Instance.SignedIn += SignInWithUnity;
    }
    public async void GuestLogin()
    {
        try
        {
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            loginPannel.SetActive(false);
        }
        catch(AuthenticationException e) 
        {
            Debug.LogError("로그인 실패:" + e);
        }

    }
    async Task SignInWithUnityAsync(string accessToken)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUnityAsync(accessToken);
            Debug.Log("SignIn is successful.");
            playerInfo = AuthenticationService.Instance.PlayerInfo;
            loginPannel.SetActive(false);
            //SavePlayerData("IsFirstConnected","")
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }
    private async void SignInWithUnity()
    {
        try
        {
            var accessToken = PlayerAccountService.Instance.AccessToken;
            await SignInWithUnityAsync(accessToken);

        }
        catch
        {
            Debug.Log(":(");
        }
    }
    public async System.Threading.Tasks.Task SavePlayerData(string key, object value)
    {
        try
        {
            var data = new Dictionary<string, object> { { key, value } };
            await CloudSaveService.Instance.Data.Player.SaveAsync(data);
            Debug.Log($"Data saved: {key} = {value}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to save data: {ex.Message}");
        }
    }
    public void UnityLogined()
    {
        PlayerAccountService.Instance.StartSignInAsync();
       
    }

    public async void StartRandomMatchmaking()
    {
        multiplayerBtn.SetActive(false);
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            Debug.LogError("로그인되지 않았다");
            return;
        }
        curLobby = await FindAvilableLjobby();
        if (curLobby == null) {
            await CreateNewLobby();
        }
        else
        {
            await JoinLobby(curLobby.Id);
        }
    }
    private async Task<Lobby> FindAvilableLjobby()
    {
            try
            {
                var queryResponse = await LobbyService.Instance.QueryLobbiesAsync();
                if(queryResponse.Results.Count > 0)
                {
                    return queryResponse.Results[0];
                }
            }
            catch(LobbyServiceException e)
            {
                Debug.Log("로비 찾기 실패" + e);
            }
        return null;
        }
    private async Task CreateNewLobby()
    {
        try
        {
            await CreateLobbyWithHeartbeatAsync();
            //curLobby = await LobbyService.Instance.CreateLobbyAsync("랜덤매칭(4)", 4);
            UpdateLobbyOptions options = new UpdateLobbyOptions();
            options.IsPrivate = false;
            await LobbyService.Instance.UpdateLobbyAsync(curLobby.Id,options);
            Debug.Log("새로운 방 생성됨:" + curLobby.Id);
            ConnectToLobbyUpdate(curLobby.Id);
            await AllocateRelayServerAndJoin(curLobby);
            multiplayerLobbyPannel.SetActive(true);
            StartHost();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("로비 생성 실패" + e);
        }
    }
    private async Task JoinLobby(string lobbyId)
    {
        try
        {
            curLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
            ConnectToLobbyUpdate(curLobby.Id);
            Debug.Log("접속 완료" + curLobby.Id);
            multiplayerLobbyPannel.SetActive(true);
            StartClient();
        }
        catch (LobbyServiceException e) 
        {
                Debug.LogError("로비 참가 실패: " + e);
        }
    }
    private async Task AllocateRelayServerAndJoin(Lobby lobby)
    {
        try
        {
            var allocation = await RelayService.Instance.CreateAllocationAsync(lobby.MaxPlayers);
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log("Relay서버 할당 완료. JoinCode: " + joinCode);
        }
        catch(RelayServiceException e)
        {
            Debug.LogError("Relay 서버 할당 실패: " +e);

        }
    }
    private void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        ChangeText(curLobby.Id, "집회장");
        Debug.Log("호스팅이 시작됨");
        //InitReadyPlayers();
        InitUIOnJoined(true);
    }
    private void CheckMatchTotalPlayers()
    {
        if (NetworkManager.Singleton.ConnectedClients.Count== curLobby.MaxPlayers)
        {
            OnStartClicked();
        }
    }
    private void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        ChangeText(curLobby.Id, "참가자");
        Debug.Log("클라이이언트가 연결되었습니다.");
        InitUIOnJoined(false);
    }
    private void ChangeText(string id,string type)
    {
        roomId.text = "방 아이디: "+id;
        role.text = type;
    }
    private async void ConnectToLobbyUpdate(string lobbyId)
    {
        var callbacks = new LobbyEventCallbacks();
        await LobbyService.Instance.SubscribeToLobbyEventsAsync(lobbyId, callbacks);
        callbacks.LobbyChanged += OnLobbyUpdated;
        //callbacks.DataChanged += OnPlayerDataChanged;
    }

    /*private void OnPlayerDataChanged(Dictionary<string, ChangedOrRemovedLobbyValue<DataObject>> dictionary)
    {
        Debug.Log("!");
        if (dictionary.ContainsKey("IsReady") && dictionary["IsReady"].Value.ToString() == "true")
        {
            readyCount++;
        }
        var joinedCustomers = (int)curLobby.Players.Count - 1;
        readyPlayers.text = readyCount + "/" + joinedCustomers.ToString();
    }*/

    private void OnLobbyUpdated(ILobbyChanges changes)
    {
        joinedPlayers.text = curLobby.Players.Count+ "/" + curLobby.MaxPlayers;
        CheckMatchTotalPlayers();
    }
    private void InitUIOnJoined(bool isHost)
    {
        joinedPlayers.text = curLobby.Players.Count + "/" + curLobby.MaxPlayers;
        if (isHost)
        {
            startBtn.gameObject.SetActive(true);
            startBtn.onClick.AddListener(() => OnStartClicked());
        }
        else
        {
            readyTxt.gameObject.SetActive(true);
            //readyBtn.onClick.AddListener(() => OnReadyClicked());
        }
    }
    async Task<Lobby> CreateLobbyWithHeartbeatAsync()
    {
        string lobbyName = "무작위 여행(4)";
        int maxPlayers = 4;
        CreateLobbyOptions options = new CreateLobbyOptions();
        curLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
        StartCoroutine(HeartbeatLobbyCoroutine(curLobby.Id, 15));
        return curLobby;
    }

    private IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
    {
        var delay = new WaitForSecondsRealtime(waitTimeSeconds);

        while (true)
        {
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }
    private void InitUI()
    {
        multiplayerLobbyPannel.SetActive(false);
        startBtn.gameObject.SetActive(false);
        readyTxt.gameObject.SetActive(false);
    }

    /*public void OnReadyClicked()
    {
        Debug.Log("Ready");
        //SayReadyAsync();
        
    }*/
    public void OnStartClicked()
    {
        if(NetworkManager.Singleton.ConnectedClients.Count < 2)
        {
            Debug.Log("인원수 이슈");
        }
        else
        {
            ChangeSceneForAllPlayers();
        }
    }
    private void ChangeSceneForAllPlayers()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("InGame", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }
    /*private async void SayReadyAsync()
    {
        try
        {
            UpdatePlayerOptions options = new UpdatePlayerOptions();

            options.Data = new Dictionary<string, PlayerDataObject>()
        {
        {
            "IsReday", new PlayerDataObject(
                visibility: PlayerDataObject.VisibilityOptions.Public,
                value: "true")
        }
        };

            string playerId = AuthenticationService.Instance.PlayerId;

            var lobby = await LobbyService.Instance.UpdatePlayerAsync(curLobby.Id, playerId, options);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }*/
    /*private async void InitReadyPlayers()
    {
        try
        {
            UpdatePlayerOptions options = new UpdatePlayerOptions();

            options.Data = new Dictionary<string, PlayerDataObject>()
        {
        {
            "IsReday", new PlayerDataObject(
                visibility: PlayerDataObject.VisibilityOptions.Public,
                value: "false")
        }
        };

            string playerId = AuthenticationService.Instance.PlayerId;

            var lobby = await LobbyService.Instance.UpdatePlayerAsync(curLobby.Id, playerId, options);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }*/
    
}
