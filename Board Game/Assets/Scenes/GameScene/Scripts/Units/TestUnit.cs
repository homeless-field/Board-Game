using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUnit : UnitManager
{
    // PLACED ON THE UNIT OBJECT. CONTROLS ITS BEHAVIOR

    public override IEnumerator Turn()
    {
        foreach (Direction direction in turnSteps)
        {
            Vector3 startingPos = transform.localPosition;
            yield return StartCoroutine(base.gameManager.MoveUnitTo(transform, startingPos, startingPos + (directionMap[direction] * base.gameManager.tileSize), base.gameManager.tileTransitionCurve, base.gameManager.tileTransitionTime));
            yield return StartCoroutine(Attack());
        }
    }

    public override IEnumerator Attack()
    {
        animator.SetTrigger("AttackForward");
        attacking = true;

        while (attacking)
            yield return null;
    }
}
