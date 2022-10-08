using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityActions : Manager<UtilityActions>
{
    public List<AIAction> actions = new List<AIAction>();
    public Vector2 entropyRange;

    public AIAction GetBestActionForShip(Ship ship)
    {
        float bestScore = 0;
        AIAction bestAction = null;

        foreach (var action in actions)
        {
            if (!action.CanExecute(ship))
            {
                continue;
            }

            float score = action.GetScore(ship);
            score *= Random.Range(entropyRange.x, entropyRange.y);

            if (score > bestScore)
            {
                bestScore = score;
                bestAction = action;
            }
        }

        return bestAction;
    }
}