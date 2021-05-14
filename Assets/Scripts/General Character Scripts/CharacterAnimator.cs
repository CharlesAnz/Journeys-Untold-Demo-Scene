using UnityEngine;
using UnityEngine.AI;

public class CharacterAnimator : MonoBehaviour
{
    const float locoAnimSmoothTime = 0.1f;

    public Animator characterAnim;
    NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        characterAnim = GetComponentInChildren<Animator>();
    }


    private void Update()
    {
        float speedPercent = 0;
        if (agent.speed > 0)
            speedPercent = agent.velocity.magnitude / agent.speed;
        //characterAnim.SetFloat("speedPercent", speedPercent, locoAnimSmoothTime, Time.deltaTime);
        characterAnim.SetFloat("speedPercent", speedPercent, locoAnimSmoothTime, Time.deltaTime);
    }
}
