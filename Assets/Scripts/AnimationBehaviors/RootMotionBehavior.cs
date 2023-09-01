using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootMotionBehavior : StateMachineBehaviour
{
    [Header("Movement/Rotation")]
    [SerializeField] bool lockMovementOnEnter = false; 
    [SerializeField] bool lockMovementOnExit = false; 
    [SerializeField] bool lockRotationOnEnter = false; 
    [SerializeField] bool lockRotationOnExit = false; 

    [Header("Root Motion")]
    [SerializeField] bool enableRootMotionOnEnter = true; 
    [SerializeField] bool enableRootMotionOnExit = false;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.applyRootMotion = enableRootMotionOnEnter;
        InputManager.Instance.MovementLocked = lockMovementOnEnter;
        InputManager.Instance.RotationLocked = lockRotationOnEnter;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.applyRootMotion = enableRootMotionOnExit;
        InputManager.Instance.MovementLocked = lockMovementOnExit;
        InputManager.Instance.RotationLocked = lockRotationOnExit;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
