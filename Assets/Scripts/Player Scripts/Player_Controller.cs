using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


[RequireComponent(typeof(Player_Movement))]
public class Player_Controller : MonoBehaviour
{
    public LayerMask moveMask;

    public Interactable focus;
    PlayerManager playerManager;

    CharacterAnimator anim;
    Player_Stats stats;
    CharacterCombat combat;

    public bool activeCharacter;

    Camera cam;

    Player_Movement movement;

    private void Awake()
    {
        stats = GetComponent<Player_Stats>();
        combat = GetComponent<CharacterCombat>();
        anim = GetComponent<CharacterAnimator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerManager = PlayerManager.instance;

        cam = Camera.main;
        movement = GetComponent<Player_Movement>();
    }

    // Update is called once per frame
    void Update()
    {   
        if (activeCharacter == false)
        {
            if (focus == null)
            {
                Collider[] collidersNearby = Physics.OverlapSphere(transform.position, 5);
                if (collidersNearby != null)
                {
                    for (int i = 0; i < collidersNearby.Length; i++)
                    {
                        if (collidersNearby[i].TryGetComponent(out Enemy enemy))
                        {
                            SetFocus(enemy);
                            return;
                        }
                    }
                }
                movement.FollowTarget(playerManager.activePerson.gameObject, 5);

                if(anim != null) anim.characterAnim.SetBool("basicAttack", false);
            }
           
            return;
        }

        if (stats.dead || playerManager.gameOver) return;

        if (combat.CastTime > 0) return;

        if (EventSystem.current.IsPointerOverGameObject()) return;

        //shoots ray from mouse position, then moves player to target position
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100, moveMask))
            {
                //Left mouse button click move
                RemoveFocus();
                movement.MovetoPoint(hit.point);

            }
        }

        //shoots ray from mouse position, if it hits an Interactable object, then that becomes the player's focus
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                //Interactable interactable = hit.collider.GetComponent<Interactable>();
                if (hit.collider.TryGetComponent(out Interactable interactable))
                {
                    SetFocus(interactable);
                }
            }
        }
    }

    //Sets the player's focus
    //if there was a focus already, then stop focusing on old focus
    void SetFocus(Interactable newFocus)
    {
        if (newFocus != focus)
        {
            if (focus != null)
                focus.OnDefocused(gameObject);

            focus = newFocus;

            //Moves character to focus location
            movement.FollowTarget(newFocus);
        }

        //Tells interactable that they are being focused by the player
        newFocus.OnFocused(transform);
    }

    //removes the player's current focus
    public void RemoveFocus()
    {
        if (focus != null)
            focus.OnDefocused(gameObject);

        focus = null;
        movement.StopFollowTarget();
        //Debug.Log("focus removed should stop following target");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 direction = transform.TransformDirection(Vector3.forward) * 2;
        Gizmos.DrawRay(transform.position, direction);
    }
}
