using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : UnitManager
{
    // PLACED ON THE UNIT OBJECT. CONTROLS ITS BEHAVIOR

    public override IEnumerator Turn()
    {
        Debug.Log("Turn");
        Vector2 inputVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // WAIT UNTIL ONLY ONE KEY IS PRESSED
        while (inputVector == Vector2.zero || (inputVector.x != 0 && inputVector.y != 0))
        {
            inputVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            yield return null;
        }

        Vector3 startingPos = transform.localPosition;
        Vector3 targetPos = Vector3.zero;

        if (inputVector.x < 0)
            targetPos = startingPos + (directionMap[Direction.Left] * base.gameManager.tileSize);
        else if (inputVector.x > 0)
            targetPos = startingPos + (directionMap[Direction.Right] * base.gameManager.tileSize);
        else if (inputVector.y < 0)
            targetPos = startingPos + (directionMap[Direction.Back] * base.gameManager.tileSize);
        else if (inputVector.y > 0)
            targetPos = startingPos + (directionMap[Direction.Forward] * base.gameManager.tileSize);

        yield return StartCoroutine(base.gameManager.MoveUnitTo(transform, startingPos, targetPos, base.gameManager.tileTransitionCurve, base.gameManager.tileTransitionTime));
    }

    public override IEnumerator Attack()
    {
        yield return null;
    }
}
