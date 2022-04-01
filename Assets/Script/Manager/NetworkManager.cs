using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("NickName")]
    public GameObject NickNamePanel;
    public InputField NickNameInput;
    public Button CheckButton;
    public Text NetworkState;

    [Header("Main")]
    public GameObject Main;
    public Button MatchBtn;
    public Button CollectionBtn;
    public Button ExitBtn;
    public Text _NickName;

    [Header ("Collection")]
    public GameObject Collection;


    #region MonoBehavior
    private void Awake()
    {
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
    }

    private void Start()
    {
        //포톤 닉네임 존재여부
        if (PhotonNetwork.NickName == "")
        {
            NickNamePanel.SetActive(true);
        }
        else
            NickNamePanel.SetActive(false);
    }

    private void Update()
    {
        NetworkState.text = PhotonNetwork.NetworkClientState.ToString();
    }
    #endregion

    #region Network
    public override void OnConnectedToMaster()
    {
        //로비 접속시 실행
        PhotonNetwork.NickName = NickNameInput.text;
        NickNamePanel.SetActive(false);
        _NickName.text = PhotonNetwork.NickName;

    }

    public void Lobby() => PhotonNetwork.ConnectUsingSettings();

    public override void OnJoinedLobby()
    {
        NickNamePanel.SetActive(false);
        Main.SetActive(true);
    }

    public void Room() => PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 2 }, null);

    public override void OnJoinedRoom()
    {
        SceneManager.LoadScene(1);
    }
    #endregion

    #region Collection
    public void OnCollectionPanel()
    {
        Collection.SetActive(true);
    }
    public void OffCollectionPanel()
    {
        Collection.SetActive(false);
    }
#endregion
}
