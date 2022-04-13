using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

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
    public Button SettingBtn;
    public Text _NickName;

    [Header ("Collection")]
    public GameObject Collection;

    [Header ("Setting")]
    public GameObject Setting;


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

    #region Settings
    public void OnSettingPanel()
    {
        Setting.SetActive(true);
    }

    public void OffSettingPanel()
    {
        Setting.SetActive(false);
    }

    public void ScreenSizeDropDown(int index)
    {
        switch (index)
        {
            case 0:
                Screen.SetResolution(960, 540, false);
                break;
            case 1:
                Screen.SetResolution(1280, 720, false);
                break;
            case 2:
                Screen.SetResolution(1600, 900, false);
                break;
            case 3:
                Screen.SetResolution(1920, 1080, false);
                break;
        }
    }
    #endregion

}
