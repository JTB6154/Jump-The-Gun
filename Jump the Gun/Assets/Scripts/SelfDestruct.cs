using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float lifetime;
    private float timer;

    public bool IsDestroyedWhenInvisible;
    public float timeToDestroyAfterInvisible;

    // Update is called once per frame
    void Update()
    {
        if (IsDestroyedWhenInvisible)
        {
            DestroyWhenInvisible();
        }

        else
        {
            timer += Time.deltaTime;
            if (timer >= lifetime)
            {
                Destroy(gameObject);
            }
        }
    }

    private void DestroyWhenInvisible()
    {
        if (!MiscHelper.IsVisibleFrom_BoxCollider2D(GetComponent<BoxCollider2D>(), Camera.main))
        {
            StartCoroutine(DestroyAfterInvisible());
        }
    }

    private IEnumerator DestroyAfterInvisible()
    {
        float timer = 0f;
        while (timer <= timeToDestroyAfterInvisible)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}
