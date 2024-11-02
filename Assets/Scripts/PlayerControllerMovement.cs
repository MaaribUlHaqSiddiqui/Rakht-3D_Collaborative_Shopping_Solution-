using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

/*
public class PlayerController : MonoBehaviourPun, IPunObservable
{
    public float moveSpeed = 5f;
    public float turnSpeed = 10f;
    public Transform cameraTransform; // Reference to the camera transform
    public Vector3 cameraOffset = new Vector3(0f, 2f, -5f); // Adjust this offset as needed

    private Rigidbody rb;
    private Vector3 networkPosition;
    private Quaternion networkRotation;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (!photonView.IsMine)
        {
            rb.isKinematic = true; // Disable rigidbody for non-local players
            if (cameraTransform != null)
            {
                cameraTransform.gameObject.SetActive(false); // Disable non-local players' cameras
            }
        }
        else if (cameraTransform != null)
        {
            cameraTransform.SetParent(transform); // Set the camera as a child of the local player
            cameraTransform.localPosition = cameraOffset;
            cameraTransform.localRotation = Quaternion.identity;
        }
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            ProcessInputs();
        }
        else
        {
            SmoothMove();
        }
    }

    private void ProcessInputs()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0f, vertical) * moveSpeed * Time.deltaTime;
        rb.MovePosition(transform.position + transform.TransformDirection(movement));

        if (movement.magnitude > 0.1f)
        {
            Quaternion newRotation = Quaternion.LookRotation(movement);
            rb.MoveRotation(Quaternion.Lerp(rb.rotation, newRotation, turnSpeed * Time.deltaTime));
        }
    }

    private void SmoothMove()
    {
        transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 10);
        transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * 10);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}




public class PlayerMovement : MonoBehaviourPunCallbacks
{
    private PhotonView photonView;
    private Rigidbody2D rigidbody2D;

    public Transform playerCamera;
    private Vector3 movementVector;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");

            movementVector = new Vector3(horizontalInput, 0f, verticalInput);

            Quaternion targetRotation = Quaternion.LookRotation(movementVector);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.5f);

            if (movementVector.magnitude > 0)
            {
                rigidbody2D.AddRelativeForce(movementVector * 5f);

                photonView.RPC("SyncMovement", RpcTarget.AllBuffered, movementVector, transform.rotation.eulerAngles.y);
            }
            else
            {
                rigidbody2D.velocity = Vector2.zero;
            }
        }
    }

    [PunRPC]
    void SyncMovement(Vector3 movementVector, float rotationY)
    {
        if (!photonView.IsMine)
        {
            rigidbody2D.AddRelativeForce(movementVector * 5f);
            transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
        }
    }

    public void SetCameraPosition(Transform playerTransform)
    {
        Vector3 cameraPosition = playerTransform.position + new Vector3(0f, 3f, -2f);

        transform.position = cameraPosition;
    }
}*/



public class player : MonoBehaviour
{
    public Animator playerAnim;
    public Rigidbody playerRigid;
    public float w_speed, wb_speed, olw_speed, rn_speed, ro_speed;
    public bool walking;
    public Transform playerTrans;

    private PhotonView photonView;


    void FixedUpdate()
    {
        photonView = GetComponent<PhotonView>() ;
        // Check if this instance owns the GameObject before applying movement
        if (photonView.IsMine)
        {
            if (Input.GetKey(KeyCode.W))
            {
                playerRigid.velocity = transform.forward * w_speed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                playerRigid.velocity = -transform.forward * wb_speed * Time.deltaTime;
            }
            else
            {
                // If neither W nor S keys are pressed, set velocity to zero
                playerRigid.velocity = Vector3.zero;
            }
        }
    }
    void Update()
    {
        photonView = GetComponent<PhotonView>();
        if (photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                playerAnim.SetTrigger("walk");
                playerAnim.ResetTrigger("idle");
                walking = true;
                //steps1.SetActive(true);
            }
            if (Input.GetKeyUp(KeyCode.W))
            {
                playerAnim.ResetTrigger("walk");
                playerAnim.SetTrigger("idle");
                walking = false;
                //steps1.SetActive(false);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                playerAnim.SetTrigger("walk");
                playerAnim.ResetTrigger("idle");
                //steps1.SetActive(true);
            }
            if (Input.GetKeyUp(KeyCode.S))
            {
                playerAnim.ResetTrigger("walk");
                playerAnim.SetTrigger("idle");
                //steps1.SetActive(false);
            }
            if (Input.GetKey(KeyCode.A))
            {
                playerTrans.Rotate(0, -ro_speed * Time.deltaTime, 0);
            }
            if (Input.GetKey(KeyCode.D))
            {
                playerTrans.Rotate(0, ro_speed * Time.deltaTime, 0);
            }
        }
    }
}
