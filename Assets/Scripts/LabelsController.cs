using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;


public class LabelsController : MonoBehaviour
{
    public GameObject labelPrefab;
    public GameObject buttons;
    public GameObject stop;

    private Transform player;
    private float carteSize;
    private int index;
    private GameObject carteClicked;
    private List<GameObject> labels;
    private List<GameObject> arguments;
    private GameObject label;
    private Vector3 lastVector, incrementVector;

    private float width, height;
    // Start is called before the first frame update
    void Start()
    {
        player = transform.parent.parent.GetComponent<UIController>().partie.transform.Find("Player");
        index = 0;
        RectTransform rectTrans = GetComponent<RectTransform>();
        width = rectTrans.rect.width;
        height = rectTrans.rect.height;
        lastVector = new Vector3(-width/2 + width*0.2f, height/2 - height*0.15f);      //  ici on l'initialise à la position du premier label
        incrementVector = new Vector3(0,0,0);
        carteSize = 0;
        labels = new List<GameObject>();
        arguments = new List<GameObject>();
    }

    /// <summary>
    /// fonction called when onClick()
    /// </summary>
    public void AddCarte()
    {

        carteClicked = EventSystem.current.currentSelectedGameObject;
        string carteName = carteClicked.name;

        Text childText = carteClicked.GetComponentInChildren<Text>();
        int nbOfCarteLeft = Convert.ToInt32(childText.text);

        //  si il reste des actions pour ce bouton alors
        if(nbOfCarteLeft != 0)
        {
            childText.text = Convert.ToString(nbOfCarteLeft - 1);

            //  quand la carte precedente est begin, alors la carte sera placee sur le cote bas
            //  mais pas pour les cartes end et goal
            if (index != 0)
            {
                if (labels[index - 1].name.Equals("Begin"))
                {
                    if (!carteName.Equals("End"))
                        incrementVector = new Vector3(carteSize, -carteSize / 2);
                    else incrementVector = new Vector3(0, -carteSize);
                }
                else
                {
                    if (carteName.Equals("End"))
                    {
                        // si c'est sur le bord de gauche
                        if (lastVector.x <= (-width / 2 + (width * 0.2f)) + 1)

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
            if (lastVector.y > -height / 2 + (height * 0.15f) && lastVector.x < width / 2 - (width * 0.2f))
            {
                label = Instantiate(labelPrefab, transform, false);
                label.transform.localPosition = lastVector;
                carteSize = label.GetComponent<RectTransform>().rect.width;

                label.name = carteName;
                label.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + carteName);
                labels.Add(label);
                arguments.Add(null); // permet de remplir la liste arguments

                index++;
            }
            else
            {
                lastVector = lastVector - incrementVector;
            }
        }
        
        
    }


    public void DeleteLastCarte()
    {

        if (index > 0) //prevent IndexOutOfRangeException for empty list
        {
            index--;
            string carteName = labels[index].GetComponent<Image>().sprite.name; //  on recup le nom du fichier source du sprite du label supprimé

            Transform child = buttons.transform.Find(carteName);    //  on recup l'enfant de buttons qui a le meme nom que le fichier source
            Text childText = child.GetComponentInChildren<Text>();  //  on recup le text de l'enfant
            int nbOfCarteLeft = Convert.ToInt32(childText.text);    //  on le converti en int
            childText.text = Convert.ToString(nbOfCarteLeft + 1);   //  on le decremente

            Destroy(labels[index]); //  destruction de l'object
            Destroy(arguments[index]);

            labels.RemoveAt(index); //  suppression de l'emplacement de la liste            
            arguments.RemoveAt(index);

            if (index > 0)
                lastVector = labels[index - 1].transform.localPosition;


        }
        else Start();


        //  si il n'y a plus de label affiché, on reset le script
        if (labels.Count == 0) Start();


    }

    public void DeleteAllCarte()
    {
        for(int i = 0; i < labels.Count; i++)
        {
            Destroy(labels[i]);

            string carteName = labels[i].GetComponent<Image>().sprite.name; //  on recup le nom du fichier source du sprite du label supprimé

            Transform child = buttons.transform.Find(carteName);    //  on recup l'enfant de buttons qui a le meme nom que le fichier source
            Text childText = child.GetComponentInChildren<Text>();  //  on recup le text de l'enfant
            childText.text = Convert.ToString("2");                 //  numero de base
        }
        for (int i = 0; i < arguments.Count; i++)
        {
            Destroy(arguments[i]);
        }

        labels.Clear();
        arguments.Clear();

        Start();
        
    }

    public void AddArgument()
    {
        
        //  regarde s'il existe déjà une carte
        if(labels.Count > 0)
        {
            carteClicked = EventSystem.current.currentSelectedGameObject;
            string carteName = carteClicked.name;

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
                    label.name = carteName;
                    label.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + carteName);
                    arguments.Insert(index - 1, label);
                    
                }

                else
                {
                   arguments[index-1].GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + carteName);

                }
            }

            labels[index - 1].GetComponent<Label>().argument = carteName;

        }
    }

    //  fonction appelee quand on appuie sur start
    public void Goal()
    {
        
        player.GetComponent<PlayerController>().BeginMovements(labels);
        DesactivateButtons();
        
    }

    //  fonction appelee quand on appuie sur stop
    public void Stop()
    {
        player.GetComponent<PlayerController>().Reset();

    }

    public void DesactivateButtons()
    {
        buttons.SetActive(false);
        stop.SetActive(true);
    }

    public void ActivateButtons()
    {
        buttons.SetActive(true);
        stop.SetActive(false);
    }

}
