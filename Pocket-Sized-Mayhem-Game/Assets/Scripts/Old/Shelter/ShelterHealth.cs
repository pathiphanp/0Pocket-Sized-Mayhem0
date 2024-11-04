using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ShelterHealth : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] public float _currentHealth = 100f;

    public float Percentage => _currentHealth / _maxHealth;
    public bool IsAlive => _currentHealth >= 1f;

    public void Damage(float damage)
    {
        if (!IsAlive)
        {
            return;
        }
        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, _maxHealth);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals(14))
        {
            Damage(5);
        }
    }
}