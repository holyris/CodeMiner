using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;



public class UIController : MonoBehaviour
{
    public GameObject partie;
    public GameObject mouseItems;
    public GameObject cameraItems;
    public GameObject labels;
    public Text textLevel;

    private int index;


    // Start is called before the first frame update
    void Start()
    {

        string sceneName = SceneManager.GetActiveScene().name;
        index = Convert.ToInt32(sceneName); //converti string en int
        index--;

        SetTextLevel();

    }

    // Update is called once per frame
    void Update()
    {


        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }

    }

    public void NextLevel()
    {
        //  si ce n'est pas le dernier niveau alors
        if (index != SceneManager.sceneCountInBuildSettings - 1)
            SceneManager.LoadScene(index + 1);
        else
            SceneManager.LoadScene(0);

    }

    public void PreviousLevel()
    {
        //si ce n'est pas le premier niveau alors
        if (index != 0)
            SceneManager.LoadScene(index - 1);
        else
            SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
    }



    void SetTextLevel()
    {
        textLevel.text = "Niveau " + (index + 1).ToString();
    }


    public void CameraMode()
    {
        mouseItems.SetActive(false);
        cameraItems.SetActive(true);

        transform.position = new Vector3(-4, 0, 0);
    }

    public void MouseMode()
    {
        cameraItems.SetActive(false);
        mouseItems.SetActive(true);


    }

    public void ExitGame()
    {
        Application.Quit();
        
    }
}
