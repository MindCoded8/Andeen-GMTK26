using System.Collections;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

public class AreaHealing : MonoBehaviour
{
    [SerializeField] private Collider2D areaCollider;

    [Header("Target")] 
    [SerializeField] private LayerMask healingLayers;
    [SerializeField] private bool healOnEnter = true;
    [SerializeField] private bool healOnExit = true;
    [SerializeField] private bool healOnStay = true;
    
    [Header("Healing")]
    [Range(1, 100)]
    [SerializeField] private float healAmount = 10f;
    [SerializeField] private bool repeatHealOverTime = true;
    [ShowIf("repeatHealOverTime"), Min(1)]
    [SerializeField] private int amountOfRepeats;
    [Range(0.1f, 10f)]
    [SerializeField] private float timeBetweenRepeats = 1f;
    
    //events
    public delegate void OnAreaHealDelegate();
    public OnAreaHealDelegate OnHealCaused;
    
    private Health _targetHealth;
    private Coroutine _healCoroutine;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Awake()
    {
        if (!areaCollider && !TryGetComponent(out areaCollider))
        {
            Debug.LogError(
                "AreaHealing: No Collider2D found on the GameObject or assigned in the inspector. The AreaHealing will not work");
        }
        else
        {
            areaCollider.isTrigger = true;
        }
    }
    

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!healOnEnter) return;
        ApplyHealing(collision);
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if(!healOnStay) return;
        ApplyHealing(collision);
    }
    
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (_healCoroutine != null) StopCoroutine(_healCoroutine);
        if(!healOnExit) return;
        ApplyHealing(collision);
    }

    private void ApplyHealing(Collider2D collision)
    {
        // if what we're colliding with isn't part of the target layers, we do nothing and exit
        if (!MMLayers.LayerInLayerMask(collision.gameObject.layer, healingLayers))
        {
            return;
        }

        if (!collision.gameObject.TryGetComponent(out _targetHealth)) return;
        
        OnHealCaused?.Invoke();
        var newHealth  = _targetHealth.CurrentHealth + healAmount;
        if (repeatHealOverTime)
        {
            if (_healCoroutine != null) StopCoroutine(_healCoroutine);
            _healCoroutine = StartCoroutine(HealCoroutine());
        }
        else
        {
            _targetHealth.GetHealth(newHealth, gameObject);
        }
        
    }

    private IEnumerator HealCoroutine()
    {
        int repeats = 0;
        while (repeats < amountOfRepeats)
        {
            _targetHealth.GetHealth(_targetHealth.CurrentHealth + healAmount, gameObject);
            repeats++;
            yield return new WaitForSeconds(timeBetweenRepeats);
        }
        _healCoroutine = null;
    }
    
    
}
