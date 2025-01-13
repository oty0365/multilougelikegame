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

public class NetManager : MonoBehaviour
{
    //Loby->플레이어가 원하는 게임을 찾거나, 새겜 만들기
    //Relay->매칭된 플레이어들의 Relay의 JoinCode로 연결되어 멀티 환경 유지
    private Lobby curLobby;
    [SerializeField] private TextMeshProUGUI roomId;
    private void Update()
    {
        if(curLobby != null)
        {
            roomId.text = curLobby.Id;
        }
    }
    private async void Start() //비동기->동시에 일어나지 않는다
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }
    public async void StartMatchmaking()
    {
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
            curLobby = await LobbyService.Instance.CreateLobbyAsync("랜덤매칭(4)", 4);
            Debug.Log("새로운 방 생성됨:" + curLobby.Id);
            await AllocateRelayServerAndJoin(curLobby);
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
            Debug.Log("접속 완료" + curLobby.Id);
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
        Debug.Log("호스팅이 시작됨");
    }
    private void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        Debug.Log("클라이이언트가 연결되었습니다.");
    }

}
