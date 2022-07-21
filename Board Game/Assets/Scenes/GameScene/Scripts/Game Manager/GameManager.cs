using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // PLACED ON GAME MANAGER. MANAGES GENERAL GAME BEHAVIOR

    [System.NonSerialized] public float tileSize = 2.5f; // Update animations when changing this
    [System.NonSerialized] public Transform playerTransform;
    public enum Team { PlayerTeam, EnemyTeam };
    public AnimationCurve boardTransitionCurve, tileTransitionCurve;
    public float boardTransitionTime, tileTransitionTime;
    public CameraManager camManager;
    public GameObject playerPrefab;
    public GameSetup setupScript;
    private int currentRoom = 0;

    // TRANSITIONS TO THE SPECIFIED ROOM
    public IEnumerator EnterRoom(int current, int next)
    {
        GameObject board = setupScript.gameBoards[next];
        MovePlayerToNewBoard(board.transform, board.GetComponent<RoomManager>());
        yield return StartCoroutine(camManager.TransitionToBoard(board));

        StartCoroutine(board.GetComponent<RoomManager>().TurnCycle());
    }

    // RETURNS THE LARGEST VALUE IN A VECTOR2
    public float SimpleMagnitude(Vector2 vector)
    {
        if (vector.x > vector.y)
            return vector.x;
        else if (vector.y > vector.x)
            return vector.y;
        else
            return vector.x;
    }

    // CONVERTS A BOARD POSITION TO A LOCAL POSITION
    public Vector3 BoardToLocalPos(Vector2Int boardPos, float height)
    {
        return new Vector3(boardPos.x * tileSize, height, boardPos.y * tileSize);
    }

    // CONVERTS A LOCAL POSITION TO A BOARD POSITION
    public Vector2Int LocalToBoardPos (Vector2 localPos)
    {
        return new Vector2Int(Mathf.RoundToInt(localPos.x / tileSize), Mathf.RoundToInt(localPos.y / tileSize));
    }

    // PROPERLY SETS THE PLAYER'S PARENT AND SUCH WHILE MOVING IT
    private void MovePlayerToNewBoard(Transform newBoard, RoomManager roomScript)
    {
        Vector3 targetPos = newBoard.TransformPoint(BoardToLocalPos(new Vector2Int(0, roomScript.roomSize.y / 2), playerTransform.position.y));

        playerTransform.SetParent(newBoard, true);
        StartCoroutine(MoveUnitTo(playerTransform, playerTransform.position, targetPos, boardTransitionCurve, boardTransitionTime, true));
        StartCoroutine(SmoothRotate(playerTransform, playerTransform.localRotation, Quaternion.Euler(Vector3.zero), boardTransitionTime));
    }

    // SMOOTHLY ROTATES BETWEEN TWO ROTATIONS
    private IEnumerator SmoothRotate(Transform unit, Quaternion startingRot, Quaternion targetRot, float duration)
    {
        float currentTime = 0.0f;
        while (currentTime < duration)
        {
            float adjustedTime = currentTime / duration;

            unit.localRotation = Quaternion.Lerp(startingRot, targetRot, adjustedTime);
            currentTime += Time.deltaTime;
            yield return null;
        }

        unit.localRotation = targetRot;
    }

    // SMOOTHLY MOVES A UNIT BETWEEN TWO POINTS, WHILE ANIMATING THE HEIGHT
    public IEnumerator MoveUnitTo(Transform unit, Vector3 startingPos, Vector3 targetPos, AnimationCurve heightCurve, float duration, bool worldSpace = false)
    {
        float currentTime = 0.0f;

        while (currentTime < duration)
        {
            float adjustedTime = currentTime / duration;

            // SMOOTHLY MOVE TOWARD THE TARGET POSITION
            Vector3 unitPosition = Vector3.Lerp(startingPos, targetPos, Mathf.SmoothStep(0.0f, 1.0f, adjustedTime));
            unitPosition.y += heightCurve.Evaluate(adjustedTime);

            if (worldSpace)
                unit.position = unitPosition;
            else
                unit.localPosition = unitPosition;

            currentTime += Time.deltaTime;
            yield return null;
        }

        if (worldSpace)
            unit.position = targetPos;
        else
            unit.localPosition = targetPos;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Next Room"))
        {
            if (camManager.transitioning) return;

            currentRoom += 1;
            StartCoroutine(EnterRoom(0, currentRoom));
        }
    }
}
