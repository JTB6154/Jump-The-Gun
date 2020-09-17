using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    public GameObject player;
    public GameObject ammoBR1;
    public GameObject ammoBR2;
    public GameObject ammoRL1;
    public GameObject ammoRL2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int bigRecoilAmmo = player.GetComponent<CharacterController>().GetBigRecoilAmmo();
        int rocketAmmo = player.GetComponent<CharacterController>().GetRocketAmmo();


        if (bigRecoilAmmo == 2)
        {
            ammoBR1.SetActive(true);
            ammoBR2.SetActive(true);
        }
        else if (bigRecoilAmmo == 1)
        {
            ammoBR1.SetActive(true);
            ammoBR2.SetActive(false);
        }
        else
        {
            ammoBR1.SetActive(false);
            ammoBR2.SetActive(false);
        }

        if (rocketAmmo == 2)
        {
            ammoRL1.SetActive(true);
            ammoRL2.SetActive(true);
        }
        else if (rocketAmmo == 1)
        {
            ammoRL1.SetActive(true);
            ammoRL2.SetActive(false);
        }
        else
        {
            ammoRL1.SetActive(false);
            ammoRL2.SetActive(false);
        }
    }
}
