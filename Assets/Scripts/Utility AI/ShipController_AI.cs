using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShipController_AI : BaseShipController
{
    public bool initializeOnStart;
    
    private AIArchetype _archetype;
    private float _locomotionTime, _interactionTime;

    private const float _timeEntropy = 0.25f;

    [ShowInInspector, ReadOnly]
    private AIAction locomotion, interaction;
    private void Start()
    {
        if (initializeOnStart)
        {
            Initialize(GetComponent<Ship>());
        }
    }

    public override void Initialize(Ship ship, params object[] data)
    {
        base.Initialize(ship);
        foreach (var obj in data)
        {
            if (obj is AIArchetype archetype)
            {
                _archetype = archetype;
            }
        }

        if (_archetype == null)
        {
            _archetype = Resources.Load("AI/defaultArchetype") as AIArchetype;
        }
        
    }

    public override void Deinitialize()
    {
        base.Deinitialize();
        Destroy(this);
    }

    private void FixedUpdate()
    {
        if (Time.time - _locomotionTime >= _archetype.decisionPeriod)
        {
            _locomotionTime = Time.time + Random.Range(-_timeEntropy, _timeEntropy);

            AIAction actionToExecute = GetBestAction(_archetype.locomotionActions);
            if (actionToExecute != null)
            {
                locomotion = actionToExecute;
                actionToExecute.Execute(_ship);
            }
        }
        
        if (Time.time - _interactionTime >= _archetype.decisionPeriod)
        {
            _interactionTime = Time.time + Random.Range(-_timeEntropy, _timeEntropy);

            AIAction actionToExecute = GetBestAction(_archetype.interactionActions);
            if (actionToExecute != null)
            {
                interaction = actionToExecute;
                actionToExecute.Execute(_ship);
            }
        }
    }
    
    public AIAction GetBestAction(AIAction[] actions)
    {
        float bestScore = 0;
        AIAction bestAction = null;

        foreach (var action in actions)
        {
            if (!action.CanExecute(_ship))
            {
                continue;
            }

            float score = action.GetScore(_ship);
            score *= Random.Range(_archetype.entropyMin, _archetype.entropyMax);

            if (score > bestScore)
            {
                bestScore = score;
                bestAction = action;
            }
        }

        return bestAction;
    }
}
