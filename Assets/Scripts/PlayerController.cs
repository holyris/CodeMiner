using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class PlayerController : MonoBehaviour
{

    public GameObject grid;
    public GameObject buttons;
    public GameObject stop;
    public GameObject pickups;
    public LayerMask blockingLayer;         //  Layer sur lequel la collision sera comptee comme etant un mur
    public int direction;                   //  a configurer sur l'editeur unity



    private int nbOfBegin, nbOfEnd;
    private Sprite[] sprites;
    private SpriteRenderer spriteRenderer; //  on met le composant sprite dans un autre GameObject pour rotate librement

    private Vector2 startPosition;          //  la position dans laquelle le player demarre
    private int startDirection;             //  stock la direction de depart

    private Rigidbody2D rb2d;               //  rigidbody du player
    private BoxCollider2D boxCollider;      //  BoxCollider2D du player

    private float size;                     //  les dimensions du plateau de jeu
    private Animator animator;

    private List<GameObject> cartes;        //  stock les objets cartes (label)

    private int stackRank, stackIterator;   //  permettent aux mouvements de s'attendre entre eux

    private float inverseMoveTime;

    // Start is called before the first frame update
    void Start()
    {
        sprites = Resources.LoadAll<Sprite>("Sprites/miner");   //  on prend tous les sous-sprites
        spriteRenderer = GetComponent<SpriteRenderer>();        //  permettra de modif le sprite du player
        animator = GetComponent<Animator>();


        startPosition = transform.position;         //stock la position de depart
        startDirection = direction;                 //stock la direction de depart
        animator.SetFloat("Direction", direction);

        animator.SetTrigger("Stop");

        rb2d = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        size = grid.GetComponent<SpriteRenderer>().bounds.size.x;
        stackRank = 0;
        stackIterator = 1;
        inverseMoveTime = 1f / 0.1f;
        nbOfBegin = 0;
        nbOfEnd = 0;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Pickup"))
        {
            other.gameObject.SetActive(false); //   abusé non ?
        }
    }

    public void BeginMovements(List<GameObject> liste)
    {
        cartes = liste;
        MovementsInterpreter(0);
    }


    //  fonction qui interprete les cartes pour executer les mouvements dans le bon ordre
    private int MovementsInterpreter(int loopBegin)
    {
        int i = loopBegin;
        for (; i < cartes.Count; i++)
        {

            int argument = System.Convert.ToInt32(cartes[i].GetComponent<Label>().argument);
            string nom = cartes[i].name;

            //  debut loop
            if (nom.Equals("Begin"))
            {
                nbOfBegin++;
                loopBegin = i;

                //  pour que la boucle fasse 2 tour par defaut
                if (argument == 1)
                    argument = 2;

                for (int j = 0; j < argument; j++)
                    i = MovementsInterpreter(loopBegin + 1);
            }

            //  fin loop
            else if (nom.Equals("End"))
            {
                nbOfEnd++;
                if(nbOfBegin >= nbOfEnd)
                {
                    nbOfEnd--;
                    return i;                        
                }
            }

            //  avancer
            else if (nom.Equals("Avancer"))
            {
                stackRank++;
                StartCoroutine(Avancer(argument, stackRank));
            }

            //  tourner a droite
            else if (nom.Equals("Droite"))
            {
                stackRank++;
                StartCoroutine(Tourner(argument, stackRank, -1));
            }

            //  tourner a gauche
            else if (nom.Equals("Gauche"))
            {
                stackRank++;
                StartCoroutine(Tourner(argument, stackRank, 1));
            }

            else if (nom.Equals("Goal"))
            {
                stackRank++;
                
                StartCoroutine(ResetAtEnd(stackRank));
            }
        }
        //  pour eviter les warnings
        return i;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="loop">Le nombre de fois où l'action "avancer" va se faire</param>
    /// <param name="rank">la coroutine va attendre que rank soit atteint pour se lancer</param>
    /// <returns></returns>
    IEnumerator Avancer(int loop, int rank)
    {
        yield return new WaitUntil(() => rank==stackIterator); // attend que ça soit le tour de cette animation

        Vector3 start = new Vector3();
        Vector3 end = new Vector3();

        for (int i = 0; i < loop; i++)
        {

            animator.SetTrigger("Move");

            start = transform.position;

            end = transform.position + ( (Quaternion.AngleAxis(direction*90, Vector3.forward) * Vector3.up)  * (size / 8) );  //  la position de la destination du player

            //  si la voie est libre
            if(CheckCanMove(start, end))
            {
                float sqrDistance = (transform.position - end).sqrMagnitude;

                //  tant que la distance n'est pas (presque) nulle alors
                while (sqrDistance > 0.001f)
                {
                    Vector3 newPosition = Vector3.MoveTowards(rb2d.position, end, inverseMoveTime * Time.deltaTime * 0.3f);

                    rb2d.MovePosition(newPosition);

                    sqrDistance = (transform.position - end).sqrMagnitude;   //recalcul la distance restante
                    yield return null;

                }
            }
            animator.SetTrigger("Stop");

        }

        yield return new WaitForSeconds(0.5f);  //  pour eviter d'enchainer trop vite les mouvements

        stackIterator++;    //  pour lancer le coup d'envoi au prochain mouvement

    }



    IEnumerator Tourner(int loop, int rank, int angle)
    {
        yield return new WaitUntil(() => rank == stackIterator);
        
        for (int i = 0; i < loop; i++)
        {
            direction += angle;

            if (direction == -1)
                direction = 3;
            else if (direction == 4)
                direction = 0;

            animator.SetFloat("Direction", direction);
            animator.SetTrigger("Stop");

            yield return new WaitForSeconds(0.2f);

        }

        yield return new WaitForSeconds(0.5f);

        stackIterator++;
    }


    //  cette fonction permet d'attendre la fin des mouvements avant de reset
    IEnumerator ResetAtEnd(int rank)
    {
        yield return new WaitUntil(() => rank == stackIterator);
        Reset();
    }


    //  cette fonction reset le plateau de jeu, et est appelée quand on appuie sur le bouton stop
    public void Reset()
    {
        StopAllCoroutines();
        animator.ResetTrigger("Stop");
        transform.SetPositionAndRotation(startPosition, Quaternion.identity);
        direction = startDirection;
        Start();
        buttons.SetActive(true);
        stop.SetActive(false);

        foreach (Transform child in pickups.transform)
        {
            child.gameObject.SetActive(true);
        }


    }


    //  fonction qui check si la voie est libre
    //  si oui, alors renvoyer true
    private bool CheckCanMove(Vector3 start, Vector3 end)
    {
        //  on desactive son propre boxcollider, pour eviter que linecast le compte comme collision
        boxCollider.enabled = false;

        //  trace une ligne de start a end, hit = null si rien n'est rencontre
        RaycastHit2D hit = Physics2D.Linecast(start, end, blockingLayer);

        //  reactive le boxcollider
        boxCollider.enabled = true;

        if (hit.transform == null)
            return true;
        else return false;
    }


    private void ChangeSprite()
    {
        if(direction == 0)
            spriteRenderer.sprite = sprites[10];
        else if (direction == 1)
            spriteRenderer.sprite = sprites[4];
        else if (direction == 2)
            spriteRenderer.sprite = sprites[1];
        else if (direction == 3)
            spriteRenderer.sprite = sprites[7];


    }

    private void ChangeAnimation()
    {
        if (direction == 0)
            animator.SetTrigger("MoveUp");
        else if (direction == 1)
            animator.SetTrigger("MoveLeft");
        else if (direction == 2)
            animator.SetTrigger("MoveDown");
        else if (direction == 3)
            animator.SetTrigger("MoveRight");
    }

}
