using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Photon.Pun;
using TMPro;
using Photon.Pun.Demo.Asteroids;
using Photon.Realtime;
using Unity.VisualScripting;
using JetBrains.Annotations;
using System.Runtime.CompilerServices;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField usernameText;
    public TMP_InputField passwordText;
    public TMP_InputField room_name;
    public TMP_InputField num_players;

    public TMP_InputField first_name;
    public TMP_InputField username_signup;
    public TMP_InputField password_signup;
    public TMP_InputField confirmpass_signup;

    public GameObject LobbyPanel;
    public GameObject LoginPanel;
    public GameObject create_room_panel;
    public GameObject room_list_panel;
    public GameObject signup_panel;

    private Dictionary<string, RoomInfo> roomlistData;
    private Dictionary<string, GameObject> roomlistGameObject;

    public GameObject room_list_prefab;
    public GameObject room_list_parent;

    [Header("InsideRoomPanel")]
    public GameObject inside_room_panel;
    public GameObject player_listitem_prefab;
    public GameObject player_listitem_parent;
    public GameObject playButton;

    private Dictionary<int, GameObject> playerlistGameObject;





    #region UnityMethods
    // Start is called before the first frame update
    void Start()
    {
        ActivatePanels(LoginPanel.name);
        roomlistData = new Dictionary<string, RoomInfo>();
        roomlistGameObject = new Dictionary<string, GameObject>();

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #endregion





    #region UIMethods
    
    public void OnloginClick()
    {
        string name = usernameText.text;
        string password = passwordText.text;

        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(password))
        {
            StartCoroutine(SendLoginRequest(name, password));
            //PhotonNetwork.LocalPlayer.NickName = name;
            //PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.Log("Empty Name");
        }
        //ActivatePanels(LobbyPanel.name);
    }

    public void OnloginPageSignupClick() 
    {
        ActivatePanels(signup_panel.name);
    }

    public void OnSignupClick()
    {
        string name = first_name.text;
        string username = username_signup.text;
        string password = password_signup.text;
        string cnfpass = confirmpass_signup.text;

        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(cnfpass))
        {
            if(password == cnfpass)
            {
                StartCoroutine(SendRegistrationRequest(name, username, password));
            }
            else
            {
                Debug.Log("Password do not match");
            }
            
        }
        else
        {
            Debug.Log("Empty fields");
        }
    }

    public void OnCreateRoomCLick()
    {
        ActivatePanels(create_room_panel.name);
    }

    public void OnClickCreate()
    {
        string roomname = room_name.text;
        if(string.IsNullOrEmpty(roomname))
        {
            roomname = roomname + Random.Range(0, 1000);
        }

        RoomOptions room_options = new RoomOptions();
        room_options.MaxPlayers = (byte)int.Parse(num_players.text);

        PhotonNetwork.CreateRoom(roomname, room_options);

    }

    public void OnClickCancel()
    {
        ActivatePanels(LobbyPanel.name);
    }

    public void OnclickRoomList()
    {
        if(!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
        ActivatePanels(room_list_panel.name);   
    }

    public void BackFromRoomList()
    {
        if(PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
        ActivatePanels(LobbyPanel.name);
    }

    public void BackfromPlayerList()
    {
        if(PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        ActivatePanels(LobbyPanel.name);
    }

    public void OnCliclPlayBtn()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("Game");
        }
    }

    #endregion










    #region Photon_callbacks

    public override void OnConnected()
    {
        Debug.Log("Connetect to Ineternet");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " is connected to photon");
        ActivatePanels(LobbyPanel.name);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.Name + " is created");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " joined room");
        ActivatePanels(inside_room_panel.name);

        if(playerlistGameObject==null)
        {
            playerlistGameObject = new Dictionary<int, GameObject>();
        }

        if(PhotonNetwork.IsMasterClient)
        {
            playButton.SetActive(true);
        }
        else
        {
            playButton.SetActive(false);
        }

        foreach(Player p in PhotonNetwork.PlayerList)
        {
            GameObject playerlistitem = Instantiate(player_listitem_prefab);
            playerlistitem.transform.SetParent(player_listitem_parent.transform);
            playerlistitem.transform.localScale = Vector3.one;

            playerlistitem.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = p.NickName;
            if(p.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                playerlistitem.transform.GetChild(1).gameObject.SetActive(true);            
            }
            else
            {
                playerlistitem.transform.GetChild(1).gameObject.SetActive(false);
            }

            playerlistGameObject.Add(p.ActorNumber, playerlistitem);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        GameObject playerlistitem = Instantiate(player_listitem_prefab);
        playerlistitem.transform.SetParent(player_listitem_parent.transform);
        playerlistitem.transform.localScale = Vector3.one;

        playerlistitem.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = newPlayer.NickName;
        if (newPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            playerlistitem.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            playerlistitem.transform.GetChild(1).gameObject.SetActive(false);
        }

        playerlistGameObject.Add(newPlayer.ActorNumber, playerlistitem);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Destroy(playerlistGameObject[otherPlayer.ActorNumber]);
        playerlistGameObject.Remove(otherPlayer.ActorNumber);

        if (PhotonNetwork.IsMasterClient)
        {
            playButton.SetActive(true);
        }
        else
        {
            playButton.SetActive(false);
        }
    }

    public override void OnLeftRoom()
    {
        ActivatePanels(LobbyPanel.name);
        foreach(GameObject obj in playerlistGameObject.Values)
        {
            Destroy(obj);
        }
        playerlistGameObject.Clear();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("room list update called");
        Clear_Room_List();

        foreach(RoomInfo rooms in roomList)
        {
            Debug.Log("Room name: " + rooms.Name);
            if(!rooms.IsOpen || !rooms.IsVisible || rooms.RemovedFromList)
            {
                if(roomlistData.ContainsKey(rooms.Name))
                {
                    roomlistData.Remove(rooms.Name);
                }
            }
            else
            {
                if(roomlistData.ContainsKey(rooms.Name))
                {
                    roomlistData[rooms.Name] = rooms;
                }
                else
                {
                    roomlistData.Add(rooms.Name, rooms);
                }
            } 
        }

        foreach(RoomInfo roomitem in roomlistData.Values) 
        {
            GameObject room_list_itemobject = Instantiate(room_list_prefab);
            room_list_itemobject.transform.SetParent(room_list_parent.transform);
            room_list_itemobject.transform.localScale = Vector3.one;

            room_list_itemobject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = roomitem.Name;
            room_list_itemobject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = roomitem.PlayerCount + "/" + roomitem.MaxPlayers;
            room_list_itemobject.transform.GetChild(2).gameObject.GetComponent<Button>().onClick.AddListener(() => Room_Join_from_List(roomitem.Name));

            roomlistGameObject.Add(roomitem.Name, room_list_itemobject);
        }
    }

    public override void OnLeftLobby()
    {
        Clear_Room_List();
        roomlistData.Clear();
    }

    #endregion









    #region public_methods

    public void Clear_Room_List()
    {
        if (roomlistGameObject.Count > 0)
        {
            foreach (var v in roomlistGameObject.Values)
            {
                Destroy(v);
            }
            roomlistGameObject.Clear();
        }
    }

    public void Room_Join_from_List(string room_name)
    {
        if(PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        PhotonNetwork.JoinRoom(room_name);
    }

    public void ActivatePanels(string panelName)
    {
        LoginPanel.SetActive(panelName.Equals(LoginPanel.name));
        LobbyPanel.SetActive(panelName.Equals(LobbyPanel.name));
        create_room_panel.SetActive(panelName.Equals(create_room_panel.name));
        room_list_panel.SetActive(panelName.Equals(room_list_panel.name));
        inside_room_panel.SetActive(panelName.Equals(inside_room_panel.name));
        signup_panel.SetActive(panelName.Equals(signup_panel.name));
    }

    #endregion










    #region backend_methods

    IEnumerator SendLoginRequest(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("loginuser", username);
        form.AddField("loginpass", password);

        string url = "http://localhost:7000/login";

        using (WWW www = new WWW(url, form))
        {
            yield return www;

            if (www.error != null)
            {
                Debug.LogError("Error: " + www.error);
            }
            else
            {
                Debug.Log("Server Response: " + www.text);

                if (www.text.Contains("Login successful"))
                {
                    Debug.Log("Login successful");
                    // Proceed with further actions, e.g., connect to Photon, activate panels, etc.
                    PhotonNetwork.LocalPlayer.NickName = username;
                    PhotonNetwork.ConnectUsingSettings();
                }
                else
                {
                    Debug.Log("Login failed");
                    // Display an error message to the user or handle the failure appropriately
                }
            }
        }
    }



    IEnumerator SendRegistrationRequest(string name, string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("registername", name);
        form.AddField("registeruser", username);
        form.AddField("registerpass", password);

        string url = "http://localhost:7000/register";

        using (WWW www = new WWW(url, form))
        {
            yield return www;

            if (www.error != null)
            {
                Debug.LogError("Error: " + www.error);
            }
            else
            {
                Debug.Log("Server Response: " + www.text);

                // Check if registration is successful based on the server response
                if (www.text.Contains("Registration successful"))
                {
                    Debug.Log("Registration successful");
                    // Handle success, e.g., show a success message, switch panels, etc.
                }
                else
                {
                    Debug.Log("Registration failed");
                    // Handle failure, e.g., display an error message to the user
                }
            }
        }
    }

    #endregion
    public string GetUsername()
    {
        return usernameText.text; // Assuming usernameText is the TMP_InputField for the username
    }

}
