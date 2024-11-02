using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ShowName : MonoBehaviourPunCallbacks
{
    private PhotonView _view;

    void Start()
    {
        _view = GetComponent<PhotonView>();

        if (_view != null && _view.Owner != null)
        {
            Text textComponent = GetComponent<Text>();
            if (textComponent != null)
            {
                textComponent.text = _view.Owner.NickName;
            }
            else
            {
                Debug.LogError("Text component is missing on the GameObject.");
            }
        }
        else
        {
            Debug.LogError("PhotonView component or Owner is missing.");
        }
    }

    void Update()
    {

    }
}
