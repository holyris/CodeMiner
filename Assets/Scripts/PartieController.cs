using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class PartieController : MonoBehaviour
{

    public GameObject pickups;
    public GameObject mouseItems;
    public GameObject cameraItems;

    public Text textLevel;

    private int index;

    private bool modeCamera;
    private bool modeMouse;

    // Start is called before the first frame update
    void Start()
    {

        string sceneName = SceneManager.GetActiveScene().name;
        char truc = sceneName[sceneName.Length - 1];    //  prend le dernier char du nom de la scene
        index = truc - '0'; //converti char en int
        index--;


        MouseMode();
        modeCamera = false;
        modeMouse = true;

        SetTextLevel();

    }

    // Update is called once per frame
    void Update()
    {
        //  si tous les pickups sont desactivés
        if (pickups.GetComponentsInChildren<Transform>().GetLength(0) - 1 == 0)
        {
            NextLevel();
        }


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

        modeCamera = true;
        modeMouse = false;
        transform.position = new Vector3(-4, 0, 0);
    }

    public void MouseMode()
    {
        cameraItems.SetActive(false);
        mouseItems.SetActive(true);
        modeCamera = false;
        modeMouse = true;

    }

    public void ExitGame()
    {
        Application.Quit();
        
    }
}
