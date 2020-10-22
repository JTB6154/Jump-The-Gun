using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class platformScript : MonoBehaviour
{
    bool state = false; //start in the off position
    public bool State
    {
        get
        {
            return state;
        }
    }
    bool rotationActive = false; //start inactive
    bool translationAcitve = false;
    [Header("Uses Switch: toggle off if this platform does not use a switch")]
    [SerializeField] bool usesSwitch = true;

    [Header("Objects")]
    [SerializeField] GameObject target;

    [Header("Translation")]
    [SerializeField] bool translationEnabled = false;
    [SerializeField] Vector3 totatlTranslation = Vector3.zero;
    [SerializeField] [Range(0f, 25f)] float translationSpeed = 5f;
    Vector3 startPos;
    float maxDistance, currentDistance;

    [Header("Rotation")]
    [SerializeField] bool rotationEnabled = false;
    [SerializeField] [Range(0f, 360f)] float rotationInDegrees = 0f;
    [SerializeField] [Range(0f, 180f)] float degreesPerSecond = 15f;
    float startRotation;
    float currentRotation, maxRotation;

    void Start()
    {
        maxDistance = totatlTranslation.magnitude;
        maxRotation = rotationInDegrees;
        if (target == null)
        {
            Debug.Log("Game object for moving platform " + gameObject.name + " gotten automatically");
            target = gameObject;
        }

        startPos = target.transform.position;
        startRotation = transform.rotation.eulerAngles.z;

        //reverse the translation and rotation so they go the right way the first time
        totatlTranslation = -totatlTranslation;
        rotationInDegrees = -rotationInDegrees;

        rotationActive = !usesSwitch && rotationEnabled;
        translationAcitve = !usesSwitch && translationEnabled;
    }

    void Update()
    {
        if (!translationAcitve && !rotationActive) return; //if the platform isn't active, leave 

        if (translationEnabled) UpdateTranslation(Time.deltaTime);

        if (rotationEnabled) UpdateRotation(Time.deltaTime);


        
    }
    public void UpdateTranslation(float dt)
    {
        if (!translationAcitve) return;

        currentDistance += dt * translationSpeed;
        float percentDistance = currentDistance / maxDistance;

        if (percentDistance >= 1f) //if we've passed the max distance
        {
            //disable the movement if a switch is not in use
            translationAcitve = !usesSwitch;
            currentDistance = 0; //reset program for next time
            target.transform.position = startPos + totatlTranslation.normalized * maxDistance; //move the object
            startPos = target.transform.position;
            //reverse total translation so that toggling a second time returns to original position
            totatlTranslation = -totatlTranslation;
            return;
            
        }

        target.transform.position = startPos + totatlTranslation.normalized * currentDistance;

    }
    public void UpdateRotation(float dt)
    {
        if (!rotationActive) return;

        currentRotation += dt * degreesPerSecond;
        float percentRotation = currentRotation / maxRotation;
        if (percentRotation >= 1f)
        {
            rotationActive = !usesSwitch;
            currentRotation = 0;
            target.transform.rotation = Quaternion.Euler(0, 0, startRotation + rotationInDegrees); 
            startRotation = startRotation + rotationInDegrees;
            //reverse so toggling returns to initial position
            rotationInDegrees = -rotationInDegrees;
            return;
        }

        target.transform.rotation = Quaternion.Euler(0, 0, startRotation + rotationInDegrees * percentRotation);

    }


    public void ToggleState()
    {
        if (translationAcitve || rotationActive || !usesSwitch) 
        {            
            return; 
        }

        state = !state;
        if (rotationEnabled)
        {
            rotationActive = true;
        }
        if (translationEnabled) 
        { 
            translationAcitve = true;
            
        }
    }
}
