using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    // NOT PLACED ON ANY OBJECT. CONSISTENT CODE BETWEEN ROOM TYPES

    [System.NonSerialized] public float tileSize;
    [System.NonSerialized] public Vector2Int roomSize;
    [System.NonSerialized] public GameManager gameManager;
    [SerializeField] private Vector2Int roomSizeLimits;
    public Tile[,] room;

    // REPRESENTS A SINGLE TILE OF THE ROOM
    public struct Tile
    {
        public Unit unit;

        public Tile(Unit unit)
        {
            this.unit = unit;
        }
    }

    // REPRESENTS A SINGLE UNIT
    public struct Unit
    {
        public GameObject gameObject;
        public UnitManager script;
        public GameManager.Team team;

        public Unit(GameObject gameObject, UnitManager script, GameManager.Team team)
        {
            this.gameObject = gameObject;
            this.script = script;
            this.team = team;
        }
    }

    // LOOPS THROUGH EVERY UNIT ONCE TO GIVE THEM THEIR TURN
    public virtual IEnumerator TurnCycle()
    {
        yield return StartCoroutine(gameManager.playerTransform.GetComponent<UnitManager>().Turn());
    }

    // CREATES A UNIT AT THE SPECIFIED LOCATION RELATIVE TO THE BOARD
    public Unit CreateUnit(GameObject prefab, Vector2Int boardPosition, GameManager.Team team)
    {
        GameObject thisObject = Instantiate(prefab, transform, false);
        thisObject.transform.localPosition = gameManager.BoardToLocalPos(boardPosition, thisObject.transform.position.y);

        Unit thisUnit = new Unit(thisObject, thisObject.transform.GetComponent<UnitManager>(), team);
        room[boardPosition.x, boardPosition.y].unit = thisUnit;

        return thisUnit;
    }

    private void Awake()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        roomSize = new Vector2Int(Random.Range(roomSizeLimits.x, roomSizeLimits.y + 1), Random.Range(roomSizeLimits.x, roomSizeLimits.y + 1));
        room = new Tile[roomSize.x, roomSize.y];
    }
}
