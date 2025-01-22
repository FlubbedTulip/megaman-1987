using System;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIHealthBar : MonoSingleton<UIHealthBar>
    {
        [SerializeField] private Image mask;
        float _originalSize;
        [SerializeField] private HealthManager targetHealth;
        private float _maxHealth;
        void Start()
        {
            _originalSize = mask.rectTransform.rect.height;
            _maxHealth = targetHealth.GetMaxHealth();
        }

        private void OnEnable()
        {
            targetHealth.OnDamageTaken += HandleDamageTaken;
        }

        private void OnDisable()
        {
            targetHealth.OnDamageTaken -= HandleDamageTaken;
        }

        private void HandleDamageTaken(float currentHealth)
        {
            float value = currentHealth / _maxHealth;
            SetValue(value);
        }

        private void SetValue(float value)
        {
            mask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _originalSize * value);
        }
    }
}
