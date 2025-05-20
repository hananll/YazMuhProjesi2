using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HakimTablet : MonoBehaviour
{
    public GameObject tanikPaneli;


    public void PaneliAcKapat()
    {
        tanikPaneli.SetActive(!tanikPaneli.activeSelf);
    }

}
