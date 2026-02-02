using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleLifeBar : MonoBehaviour
{
    public GameObject heartPrefab;
    public List<Image> lifePoints = new List<Image>();

    public GameObject lifeTarget;
    private IHealth _lifeTargetHealth;


    private void Start()
    {
        _lifeTargetHealth = lifeTarget.GetComponent<IHealth>();
        if (_lifeTargetHealth == null)
        {
            Debug.LogError($"Life target : {lifeTarget.name} does not have an IHealth component.");
            return;
        }
        _lifeTargetHealth.OnHealthChanged.AddListener(RefreshLife);

        InitializeLifeBar();
        RefreshLife();
    }

    private void InitializeLifeBar()
    {
        int heartCount = Mathf.CeilToInt(_lifeTargetHealth.MaxHealth * 0.5f);

        DeleteChildrens();
        for (int i = 0; i < heartCount; i++)
        {
            var heart = Instantiate(heartPrefab, transform);
            var lifePoint = heart.GetComponentsInChildren<Image>();

            foreach (var img in lifePoint)
            {
                lifePoints.Add(img);
            }
        }
    }
    private void DeleteChildrens()
    {
        int childCount = transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        lifePoints.Clear();
    }

    [ContextMenu("Refresh")]
    private void RefreshLife()
    {
        for (int i = 0; i < lifePoints.Count; i++)
        {
            lifePoints[i].gameObject.SetActive(i < _lifeTargetHealth.CurrentHealth);
        }
    }
}
