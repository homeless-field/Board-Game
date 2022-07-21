using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    // PLACED ON PHYSICAL CAMERA OBJECT. MANAGES CAMERA BEHAVIOR AND TRANSITIONS

    [System.NonSerialized] public GameObject currentCam;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Vector3 perspectiveCamDistanceScale, intermediaryCamDistanceScale, informationalCamDistanceScale;
    public bool transitioning = false;

    private Vector3 CamOffset(Vector3 distanceScale, RoomManager roomManager)
    {
        float roomMagnitude = gameManager.SimpleMagnitude(roomManager.roomSize);
        Vector3 camOffset = gameManager.tileSize * roomMagnitude * distanceScale;

        return camOffset;
    }

    // TRANSITIONS FROM ONE CAMERA TO ANOTHER, GOING THROUGH ANOTHER
    private IEnumerator TransitionWithIntermediary(Transform currentBoard, int currentIndex, int targetIndex, int intermediaryIndex)
    {
        if (transitioning) yield break;
        transitioning = true;

        GameObject currentObj = currentBoard.GetChild(currentIndex).gameObject;
        GameObject intermediaryObj = currentBoard.GetChild(intermediaryIndex).gameObject;
        GameObject targetObj = currentBoard.GetChild(targetIndex).gameObject;

        currentObj.SetActive(false);
        intermediaryObj.SetActive(true);

        while (CinemachineCore.Instance.IsLive(currentObj.GetComponent<CinemachineVirtualCamera>()))
            yield return null;

        intermediaryObj.SetActive(false);
        targetObj.gameObject.SetActive(true);
        currentCam = targetObj;
        transitioning = false;
    }

    // HANDLES THE CAMERA TRANSITION TO THE SPECIFIED ROOM
    public IEnumerator TransitionToBoard(GameObject board)
    {
        if (transitioning) yield break;
        transitioning = true;

        GameObject nextCam;
        if (currentCam == null)
        {
            nextCam = board.transform.GetChild(1).gameObject;
            nextCam.SetActive(true);
        }
        else
        {
            nextCam = board.transform.GetChild(currentCam.transform.GetSiblingIndex()).gameObject;
            currentCam.SetActive(false);
            nextCam.SetActive(true);

            while (CinemachineCore.Instance.IsLive(currentCam.GetComponent<CinemachineVirtualCamera>()))
                yield return null;
        }

        currentCam = nextCam;
        transitioning = false;
    }

    private void Start()
    {
        // SET ALL THE CAMERAS TO HAVE THE CORRECT VALUES
        foreach (GameObject board in gameManager.setupScript.gameBoards)
        {
            RoomManager roomManager = board.GetComponent<RoomManager>();
            board.transform.GetChild(1).GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = CamOffset(perspectiveCamDistanceScale, roomManager);
            
            float orthoSize = CamOffset(informationalCamDistanceScale, roomManager).y;
            board.transform.GetChild(3).GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = orthoSize;

            CinemachineVirtualCamera intermediaryCam = board.transform.GetChild(2).GetComponent<CinemachineVirtualCamera>();
            intermediaryCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = orthoSize * 114;
            intermediaryCam.m_Lens.FarClipPlane += orthoSize * 114;
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Transition Camera"))
        {
            // DECIDE WHICH TRANSITION TO DO BASED ON THE ACTIVE CAMERA
            if (currentCam.transform.GetSiblingIndex() == 1)
                StartCoroutine(TransitionWithIntermediary(currentCam.transform.parent, 1, 3, 2));
            else if (currentCam.transform.GetSiblingIndex() == 3)
                StartCoroutine(TransitionWithIntermediary(currentCam.transform.parent, 3, 1, 2));
        }
    }
}