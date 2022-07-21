using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetup : MonoBehaviour
{
    // PLACED ON GAME MANAGER. SETS UP THE SCENE

    [System.NonSerialized] public List<GameObject> gameBoards = new List<GameObject>();
    [SerializeField] private Room rooms;
    [SerializeField] private Vector2Int roomCountLimits;
    [SerializeField] private Vector2 minBoardDistance, maxBoardDistance;
    [SerializeField] private float minBoardRotation, maxBoardRotation;
    [SerializeField] private GameManager gameManager;
    private float prevBoardX;

    // CONTAINS THE VARIOUS ROOM TYPES. JUST FOR ORGANIZATION
    [System.Serializable]
    private struct Room
    {
        public GameObject empty;
        public GameObject loot;
        public GameObject combat;
        public GameObject boss;

        public Room(GameObject empty, GameObject loot, GameObject combat, GameObject boss)
        {
            this.empty = empty;
            this.loot = loot;
            this.combat = combat;
            this.boss = boss;
        }
    }

    // CREATES THE SPECIFIED NUMBER OF ROOMS AND ADDS THEM TO GAMEBOARDS
    private void CreateRooms(int count)
    {
        int lootRoomIndex = Random.Range(1, count - 2);

        for (int i = 0; i < count; i++)
        {
            Vector3 boardPosition = new Vector3(Random.Range(minBoardDistance.x, maxBoardDistance.x) + prevBoardX, 0, Random.Range(minBoardDistance.y, maxBoardDistance.y));
            Vector3 boardRotation = new Vector3(0, Random.Range(minBoardRotation, maxBoardRotation), 0);

            if (i == 0)
                gameBoards.Add(Instantiate(rooms.empty, boardPosition, Quaternion.Euler(boardRotation)));
            else if (i == count - 1)
                gameBoards.Add(Instantiate(rooms.boss, boardPosition, Quaternion.Euler(boardRotation)));
            else if (i == lootRoomIndex)
                gameBoards.Add(Instantiate(rooms.loot, boardPosition, Quaternion.Euler(boardRotation)));
            else
                gameBoards.Add(Instantiate(rooms.combat, boardPosition, Quaternion.Euler(boardRotation)));

            prevBoardX = boardPosition.x;
        }
    }

    private void Awake()
    {
        int roomCount = Random.Range(roomCountLimits.x, roomCountLimits.y);
        CreateRooms(roomCount);

        RoomManager roomManager = gameBoards[0].GetComponent<RoomManager>();
        gameManager.playerTransform = roomManager.CreateUnit(gameManager.playerPrefab, new Vector2Int(0, roomManager.roomSize.y / 2), GameManager.Team.PlayerTeam).gameObject.transform;

        StartCoroutine(gameManager.camManager.TransitionToBoard(gameBoards[0]));
        StartCoroutine(roomManager.TurnCycle());
    }
}
