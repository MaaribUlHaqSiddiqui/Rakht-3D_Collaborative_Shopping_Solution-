using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CameraControllerDeactivate : MonoBehaviourPunCallbacks
{
    private PhotonView _view;

    void Start()
    {
        _view = GetComponent<PhotonView>();
        if (!_view.IsMine)
        {
            gameObject.SetActive(false);
        }
    }

    void Update()
    {

    }
}
