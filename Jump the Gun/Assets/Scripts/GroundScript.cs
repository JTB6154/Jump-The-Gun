using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundScript : MonoBehaviour
{
    [SerializeField] Collider2D collider;
    void Start()
    {
        if (collider == null)
        {
            Debug.Log("collider automatically gotten");
            collider = gameObject.GetComponent<Collider2D>();
        }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
    }


}
