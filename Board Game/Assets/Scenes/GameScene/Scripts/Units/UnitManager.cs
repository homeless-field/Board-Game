using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitManager : MonoBehaviour
{
    // NOT PLACED ON ANY OBJECT. CONSISTENT CODE BETWEEN UNIT TYPES

    [System.NonSerialized] public GameManager gameManager;
    public string description;
    public List<Direction> turnSteps = new List<Direction>();
    public enum Direction { Left, Right, Forward, Back };
    public abstract IEnumerator Turn(); // RUNS FOR THE ENTIRETY OF THE UNIT'S TURN CYCLE
    public abstract IEnumerator Attack(); // ATTACK ABILITY
    public Animator animator;
    public bool attacking;

    public Dictionary<Direction, Vector3> directionMap = new Dictionary<Direction, Vector3>()
    {
        { Direction.Left, Vector3.left },
        { Direction.Right, Vector3.right },
        { Direction.Forward, Vector3.forward },
        { Direction.Back, Vector3.back }
    };

    private void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }
}
