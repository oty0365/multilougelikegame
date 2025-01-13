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
    //Loby->�÷��̾ ���ϴ� ������ ã�ų�, ���� �����
    //Relay->��Ī�� �÷��̾���� Relay�� JoinCode�� ����Ǿ� ��Ƽ ȯ�� ����
    private Lobby curLobby;
    [SerializeField] private TextMeshProUGUI roomId;
    private void Update()
    {
        if(curLobby != null)
        {
            roomId.text = curLobby.Id;
        }
    }
    private async void Start() //�񵿱�->���ÿ� �Ͼ�� �ʴ´�
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
            curLobby = await LobbyService.Instance.CreateLobbyAsync("������Ī(4)", 4);
            Debug.Log("���ο� �� ������:" + curLobby.Id);
            await AllocateRelayServerAndJoin(curLobby);
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
            Debug.Log("���� �Ϸ�" + curLobby.Id);
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
        Debug.Log("ȣ������ ���۵�");
    }
    private void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        Debug.Log("Ŭ�����̾�Ʈ�� ����Ǿ����ϴ�.");
    }

}
