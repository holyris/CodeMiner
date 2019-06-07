using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class LabelsController : MonoBehaviour
{
    public GameObject labelPrefab;
    public GameObject player;
    public GameObject buttons;
    public GameObject stop;
    
    private float carteSize;
    private int index;
    private string carteClicked;
    private List<GameObject> labels;
    private List<GameObject> arguments;
    private GameObject label;
    private Vector3 lastVector, incrementVector;

    private float width, height;
    // Start is called before the first frame update
    void Start()
    {

        index = 0;
        RectTransform rectTrans = GetComponent<RectTransform>();
        width = rectTrans.rect.width;
        height = rectTrans.rect.height;
        lastVector = new Vector3(-width/2 + width*0.2f, height/2 - height*0.15f);      //  ici on l'initialise à la position du premier label
        incrementVector = new Vector3(0,0,0);
        carteClicked = "";
        carteSize = 0;
        labels = new List<GameObject>();
        arguments = new List<GameObject>();


    }

    /// <summary>
    /// fonction called when onClick()
    /// </summary>
    public void AddCarte()
    {

        carteClicked = EventSystem.current.currentSelectedGameObject.name;

        //  quand la carte precedente est begin, alors la carte sera placee sur le cote bas
        //  mais pas pour les cartes end et goal

        if (index != 0)
        {
            if (labels[index - 1].name.Equals("Begin"))
            {
                if (!carteClicked.Equals("End"))
                    incrementVector = new Vector3(carteSize, -carteSize/2);
            }
            else
            {
                if (carteClicked.Equals("End"))
                {
                    // si c'est sur le bord de gauche
                    if (lastVector.x <= (-width / 2 + (width * 0.2f))+1)
                    
                        incrementVector = new Vector3(0, -carteSize);  

                    else
                        incrementVector = new Vector3(-carteSize, -carteSize / 2);

                }
                else
                    incrementVector = new Vector3(0, -carteSize);
            }

        }
        else
        {
            incrementVector = new Vector3(0, -carteSize);
        }

        lastVector = lastVector + incrementVector;

        //  si le prochain label est dans la zone 
        if(lastVector.y > -height/2 + (height*0.15f) && lastVector.x < width/2 - (width * 0.2f))
        {
            label = Instantiate(labelPrefab, transform, false);
            label.transform.localPosition = lastVector;
            carteSize = label.GetComponent<RectTransform>().rect.width;

            label.name = carteClicked;
            label.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + carteClicked);
            labels.Add(label);
            arguments.Add(null); // permet de remplir la liste arguments


            index++;

            CheckGoal();
        }
        else
        {
            lastVector = lastVector - incrementVector;
        }
        
    }


    public void DeleteLastCarte()
    {
        if (index > 0) //prevent IndexOutOfRangeException for empty list
        {
            index--;

            
            Destroy(labels[index]); //  destruction de l'object
            Destroy(arguments[index]);

            labels.RemoveAt(index); //  suppression de l'emplacement de la liste            
            arguments.RemoveAt(index);

            if (index > 0)
                lastVector = labels[index - 1].transform.localPosition;


        }
        else Start();

        if (labels.Count==0) Start();

    }

    public void DeleteAllCarte()
    {
        for(int i = 0; i < labels.Count; i++)
            Destroy(labels[i]);
        for (int i = 0; i < arguments.Count; i++)
            Destroy(arguments[i]);
        
        labels.Clear();
        arguments.Clear();

        Start();

    }

    public void AddArgument()
    {
        
        //  regarde s'il existe déjà une carte
        if(labels.Count > 0)
        {
            carteClicked = EventSystem.current.currentSelectedGameObject.name;

            //  si le label precedent n'est pas end ni goal
            if (!labels[index - 1].name.Equals("End") && !labels[index - 1].name.Equals("Goal"))
            {
                //  si le label precedent n'a pas d'argument
                if (labels[index - 1].GetComponent<Label>().argument.Equals("1"))
                {
                    if (labels[index - 1].name.Equals("Begin"))
                        incrementVector = new Vector3(-carteSize, 0);
                    else
                        incrementVector = new Vector3(carteSize, 0);


                    label = Instantiate(labelPrefab, transform, false);
                    label.transform.localPosition = labels[index - 1].transform.localPosition + incrementVector;
                    label.name = carteClicked;
                    label.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + carteClicked);
                    arguments.Insert(index - 1, label);
                    
                }

                else
                {
                   arguments[index-1].GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + carteClicked);

                }
            }

            labels[index - 1].GetComponent<Label>().argument = carteClicked;

        }
    }

    public void CheckGoal()
    {
        if (labels[index-1].name.Equals("Goal"))
        {
            player.GetComponent<PlayerController>().BeginMovements(labels);
            DesactivateButtons();
            DeleteLastCarte();
        }
    }

    public void DesactivateButtons()
    {
        buttons.SetActive(false);
        stop.SetActive(true);
    }

}
