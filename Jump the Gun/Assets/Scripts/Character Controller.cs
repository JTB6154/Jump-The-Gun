using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [Header ("Character Objects")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] GameObject character;

    // Start is called before the first frame update
    void Start()
    {
        if (character == null)
        {
            character = gameObject;
        }

        if (rb == null)
        {
            rb = character.GetComponent<Rigidbody2D>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
