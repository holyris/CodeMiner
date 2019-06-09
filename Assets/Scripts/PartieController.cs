using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartieController : MonoBehaviour
{


    public GameObject pickups;
    public GameObject canvas;

    // Update is called once per frame
    void Update()
    {
        //  si tous les pickups sont desactivés
        if (pickups.GetComponentsInChildren<Transform>().GetLength(0) - 1 == 0)
        {
            canvas.GetComponent<UIController>().NextLevel();
        }
    }
}
