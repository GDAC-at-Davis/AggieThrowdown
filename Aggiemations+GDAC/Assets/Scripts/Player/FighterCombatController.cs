using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class FighterCombatController : MonoBehaviour
{
    public const float FreezeFrameDuration = 0.12f;

    private static Coroutine FreezeFrameCorout;
    public event Action<AttackInstance, bool> OnHitByAttack;
    public event Action<AttackInstance> OnKilledAttack;

    public int IsInvincible { get; private set; }
    public bool IsSuperArmor { get; private set; }

    private int playerIndex;
    private FighterHitbox hitbox;
    private FighterConfigSO config;

    public int CurrentHealth { get; private set; }

    public struct AttackInstance
    {
        public AttackConfig attackConfig;
        public Vector2 position;
        public int dir;
        public int sourcePlayerIndex;
    }

    public void Initialize(FighterConfigSO config, int playerIndex, FighterHitbox hitbox)
    {
        this.playerIndex = playerIndex;
        this.hitbox = hitbox;
        this.config = config;

        hitbox.OnHitByAttack += ReceiveAttack;
    }

    public void SetInvincible(bool value)
    {
        IsInvincible += value ? 1 : -1;
    }

    public void SetSuperArmor(bool value)
    {
        IsSuperArmor = value;
    }

    public bool PerformAttack(AttackConfig attackConfig, Vector2 position, int dir)
    {
        attackConfig.DrawHurtboxDebug(position, dir, 0.5f);

        // overlap box to check for collisions
        var allHits = new HashSet<Collider2D>();
        foreach (var hurtbox in attackConfig.Hurtboxes)
        {
            var offset = new Vector2(hurtbox.Offset.x * dir, hurtbox.Offset.y);
            var centerPosition = position + offset;
            var hits = Physics2D.OverlapBoxAll(
                centerPosition,
                hurtbox.Size,
                dir * hurtbox.Angle,
                LayerMask.GetMask("Player"));
            allHits.AddRange(hits.ToList());
        }

        var attackInstance = new AttackInstance
        {
            attackConfig = attackConfig,
            position = position,
            dir = dir,
            sourcePlayerIndex = playerIndex
        };

        // attempt to damage all hit hitboxes
        foreach (var hit in allHits)
        {
            var hitFighter = hit.GetComponent<FighterHitbox>();
            if (hitFighter != null)
            {
                var didHit = hitFighter.ReceiveAttack(attackInstance);
                if (didHit)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool ReceiveAttack(AttackInstance instance)
    {
        if (IsInvincible > 0)
        {
            return false;
        }

        if (instance.sourcePlayerIndex == playerIndex)
        {
            return false;
        }

        OnHitByAttack?.Invoke(instance, IsSuperArmor);

        CurrentHealth -= instance.attackConfig.PointsAwarded;

        if (CurrentHealth <= 0)
        {
            OnKilledAttack?.Invoke(instance);
        }

        // Freeze frame
        if (FreezeFrameCorout != null)
        {
            StopCoroutine(FreezeFrameCorout);
        }

        MainCameraController.Instance.SendImpulse(Vector2.down);

        FreezeFrameCorout = StartCoroutine(FreezeTime(FreezeFrameDuration));


        return true;
    }

    private IEnumerator FreezeTime(float duration)
    {
        Time.timeScale = 0.05f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1;
        FreezeFrameCorout = null;
    }
}