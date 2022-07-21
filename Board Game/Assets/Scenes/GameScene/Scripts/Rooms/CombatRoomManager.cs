using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatRoomManager : RoomManager
{
    // PLACED ON ROOM PARENT. CONTROLS ROOMS WITH COMBAT

    [SerializeField] private int enemyUnitCutoff;
    [Range(0.0f, 1.0f)][SerializeField] private float enemyUnitChance;
    [SerializeField] private GameObject testUnit;
    private List<UnitManager> playerTeam = new List<UnitManager>(), enemyTeam = new List<UnitManager>();

    // LOOPS THROUGH EVERY UNIT ONCE TO GIVE THEM THEIR TURN
    public override IEnumerator TurnCycle()
    {
        yield return StartCoroutine(base.TurnCycle());

        foreach (UnitManager unit in playerTeam)
            yield return StartCoroutine(unit.Turn());

        foreach (UnitManager unit in enemyTeam)
            yield return StartCoroutine(unit.Turn());
    }

    private void Start()
    {
        for (int x = room.GetLength(0) - enemyUnitCutoff - 1; x < room.GetLength(0); x++)
        {
            for (int y = 0; y < room.GetLength(1); y++)
            {
                if (Random.Range(0.0f, 1.0f) < enemyUnitChance)
                {
                    Unit thisUnit = base.CreateUnit(testUnit, new Vector2Int(x, y), GameManager.Team.EnemyTeam);
                    enemyTeam.Add(thisUnit.script);
                }
            }
        }
    }
}