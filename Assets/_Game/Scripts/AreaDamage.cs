using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

public class AreaDamage : MonoBehaviour
{
    [SerializeField] private Collider2D areaCollider;

    [Header("Target")] 
    [SerializeField] private LayerMask damageableLayers;
    [SerializeField] private bool damageOnEnter = true;
    [SerializeField] private bool damageOnExit = true;
    [SerializeField] private bool damageOnStay = true;
    
    [Header("Damage")]
    [Range(1, 100)]
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private bool repeatDamageOverTime = true;
    [ShowIf("repeatDamageOverTime"), Min(1)]
    [SerializeField] private int amountOfRepeats;
    [Range(0.1f, 10f)]
    [SerializeField] private float timeBetweenRepeats = 1f;

    
    //events
    public delegate void OnAreaDamageDelegate();
    public OnAreaDamageDelegate OnHit;
    public OnAreaDamageDelegate OnDamageCaused;
    public OnAreaDamageDelegate OnNoDamageCaused;
    public OnAreaDamageDelegate OnAreaDamageKill;
    
    private Health _targetHealth;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Awake()
    {
        if (!areaCollider && !TryGetComponent(out areaCollider))
        {
            Debug.LogError(
                "AreaDamage: No Collider2D found on the GameObject or assigned in the inspector. The AreaDamage will not work");
        }
        else
        {
            areaCollider.isTrigger = true;
        }
    }
    

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!damageOnEnter) return;
        ApplyDamage(collision);
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if(!damageOnStay) return;
        ApplyDamage(collision);
    }
    
    public void OnTriggerExit2D(Collider2D collision)
    {
        if(!damageOnExit) return;
        ApplyDamage(collision);
    }

    private void ApplyDamage(Collider2D collision)
    {
        // if what we're colliding with isn't part of the target layers, we do nothing and exit
        if (!MMLayers.LayerInLayerMask(collision.gameObject.layer, damageableLayers))
        {
            return;
        }

        if (!collision.gameObject.TryGetComponent(out _targetHealth)) return;
        OnHit?.Invoke();
        
        if(_targetHealth.CanTakeDamageThisFrame())
        {
            OnDamageCaused?.Invoke();
            if (repeatDamageOverTime)
            {
                _targetHealth.DamageOverTime(damageAmount, gameObject, 0.1f, 1, 
                    Vector3.zero, null, amountOfRepeats, timeBetweenRepeats);
            }
            else
            {
                _targetHealth.Damage(damageAmount, gameObject, 0.1f, 1, Vector3.zero);
            }

            if (_targetHealth.CurrentHealth <= 0)
            {
                OnAreaDamageKill?.Invoke();
            }
        
        }
        else
        {
            _targetHealth.DamageZero();
            OnNoDamageCaused?.Invoke();
        }
    }
    
    
}
