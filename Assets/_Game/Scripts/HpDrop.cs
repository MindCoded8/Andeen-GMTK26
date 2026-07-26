using System;
using MoreMountains.CorgiEngine;
using MoreMountains.Feedbacks;
using UnityEngine;
using Random = System.Random;

public class HpDrop : CharacterAbility
{
    
    [Header("Hp Drop Settings")]
    [SerializeField] [Min(1)] private int minDropAmount = 1;
    [SerializeField] [Min(1)] private int maxDropAmount = 5;
    [SerializeField] [Range(0f, 1f)] private float dropChance = 0.5f;

    [Header("Drop Events")] 
    [SerializeField] private bool dropOnHit;
    [SerializeField] private bool dropOnKill = true;

    [Header("Feedbacks")]
    [SerializeField] private MMF_Player dropFeedback;
    
    private Random _dropRandom;
    private Random _chanceRandom;

    protected override void Initialization()
    {
        base.Initialization();
        _dropRandom = new Random();
        _chanceRandom = new Random();
        _health.OnHit += HandleOnHit;
        _health.OnDeath += HandleOnDeath;
    }
    
    private void HandleOnHit()
    {
        if (!dropOnHit) return;
        TryDropHp();
    }
    
    private void HandleOnDeath()
    {
        if (!dropOnKill) return;
        TryDropHp();
    }

    private void TryDropHp()
    {
        var dropAmount = _dropRandom.NextDouble() * (maxDropAmount - minDropAmount) + minDropAmount;
        
        foreach( var player in LevelManager.Instance.Players)
        {
            if (player.ConditionState.CurrentState == CharacterStates.CharacterConditions.Dead) continue;
            if (_chanceRandom.NextDouble() > dropChance) continue;
        
            player.CharacterHealth.SetHealth((float)(player.CharacterHealth.CurrentHealth + dropAmount), gameObject);
            Debug.Log($"Dropped {dropAmount} HP for player {player.name}");
            dropFeedback?.PlayFeedbacks();
        }
    }
    
}
