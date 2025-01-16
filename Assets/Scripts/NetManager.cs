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
using WebSocketSharp.Net;


public class NetManager : MonoBehaviour
{
    //Loby->�÷��̾ ���ϴ� ������ ã�ų�, ���� �����
    //Relay->��Ī�� �÷��̾���� Relay�� JoinCode�� ����Ǿ� ��Ƽ ȯ�� ����
    private Lobby curLobby;
    public int readyCount;
    public PlayerInfo playerInfo;
    private string token;
    private HttpListener _listener;
    [Header("�α��� �г� ����")]
    [SerializeField] private GameObject loginPannel;
    [Header("�κ� �г�")]
    [SerializeField] private GameObject lobbyPannel;
    [Header("����� ���� tmp")]
    [SerializeField] private TextMeshProUGUI roomId;
    [SerializeField] private TextMeshProUGUI joinedPlayers;
    [SerializeField] private TextMeshProUGUI role;
    [SerializeField] private GameObject multiplayerBtn;
    [SerializeField] private GameObject multiplayerLobbyPannel;
    [Header("���� �÷��̾� btn")]
    [SerializeField] private Button startBtn;
    [SerializeField] private TextMeshProUGUI readyTxt;




    private async void Start() //�񵿱�->���ÿ� �Ͼ�� �ʴ´�
    {
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.ClearSessionToken();
        InitUI();
        PlayerAccountService.Instance.SignedIn -= SignInWithUnity;
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
            Debug.LogError("�α��� ����:" + e);
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
            Debug.Log(accessToken);
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
            Debug.LogError("�α��ε��� �ʾҴ�");
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
                Debug.Log("�κ� ã�� ����" + e);
            }
        return null;
        }
    private async Task CreateNewLobby()
    {
        try
        {
            await CreateLobbyWithHeartbeatAsync();
            //curLobby = await LobbyService.Instance.CreateLobbyAsync("������Ī(4)", 4);
            UpdateLobbyOptions options = new UpdateLobbyOptions();
            options.IsPrivate = false;
            await LobbyService.Instance.UpdateLobbyAsync(curLobby.Id,options);
            Debug.Log("���ο� �� ������:" + curLobby.Id);
            ConnectToLobbyUpdate(curLobby.Id);
            await AllocateRelayServerAndJoin(curLobby);
            multiplayerLobbyPannel.SetActive(true);
            StartHost();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("�κ� ���� ����" + e);
        }
    }
    private async Task JoinLobby(string lobbyId)
    {
        try
        {
            curLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
            ConnectToLobbyUpdate(curLobby.Id);
            Debug.Log("���� �Ϸ�" + curLobby.Id);
            multiplayerLobbyPannel.SetActive(true);
            StartClient();
        }
        catch (LobbyServiceException e) 
        {
                Debug.LogError("�κ� ���� ����: " + e);
        }
    }
    private async Task AllocateRelayServerAndJoin(Lobby lobby)
    {
        try
        {
            var allocation = await RelayService.Instance.CreateAllocationAsync(lobby.MaxPlayers);
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log("Relay���� �Ҵ� �Ϸ�. JoinCode: " + joinCode);
        }
        catch(RelayServiceException e)
        {
            Debug.LogError("Relay ���� �Ҵ� ����: " +e);

        }
    }
    private void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        ChangeText(curLobby.Id, "��ȸ��");
        Debug.Log("ȣ������ ���۵�");
        //InitReadyPlayers();
        InitUIOnJoined(true);
    }
    private void CheckMatchTotalPlayers()
    {

        if (NetworkManager.Singleton.ConnectedClients.Count== curLobby.MaxPlayers || curLobby.Data["IsReady"].Value.ToString()=="True")
        {
            lobbyPannel.SetActive(false);
        }
    }
    private void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        ChangeText(curLobby.Id, "������");
        Debug.Log("Ŭ�����̾�Ʈ�� ����Ǿ����ϴ�.");
        InitUIOnJoined(false);
    }
    private void ChangeText(string id,string type)
    {
        roomId.text = "�� ���̵�: "+id;
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
        var createOptions = new CreateLobbyOptions
        {
            Data = new Dictionary<string, DataObject>
                    {
                        { "IsReady", new DataObject(DataObject.VisibilityOptions.Public, "false") }
                    }
        };
        string lobbyName = "������ ����(4)";
        int maxPlayers = 4;
        curLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers,createOptions);
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
            Debug.Log("�ο��� �̽�");
        }
        else
        {
            Ready();
            Ready();
        }
    }
    /*private void ChangeSceneForAllPlayers()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("InGame", LoadSceneMode.Single);
        }
    }*/
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
    public async void Ready()
    {
        try
        {
            curLobby = await LobbyService.Instance.GetLobbyAsync(curLobby.Id);

            bool isReady;
            isReady = bool.Parse(curLobby.Data["IsReady"].Value);
            isReady = true;
            
            var updatedData = new Dictionary<string, DataObject>
            {
                { "IsReady", new DataObject(DataObject.VisibilityOptions.Public, isReady.ToString()) }
            };

            curLobby = await LobbyService.Instance.UpdateLobbyAsync(curLobby.Id, new UpdateLobbyOptions
            {
                Data = updatedData
            });

            Debug.Log("�κ� �����Ͱ� ���������� ������Ʈ�Ǿ����ϴ�.");
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError($"�κ� ������ ������Ʈ ����: {e.Message}");
        }
    }
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
