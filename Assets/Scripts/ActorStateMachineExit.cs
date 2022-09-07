using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorStateMachineExit : StateMachineBehaviour
{
    private Actor actor = null;


    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        if (actor == null)
        {
            actor = animator.transform.gameObject.GetComponent<Actor>();
        }
        actor.OnStateMachineExit();
    }
}
