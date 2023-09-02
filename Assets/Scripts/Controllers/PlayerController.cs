using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{


    private CharacterController characterController;
    private MovementBehavior movementBehavior;
    private Animator animator;

    void Start()
    {
        movementBehavior = GetComponent<MovementBehavior>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        movementBehavior.HandleMovement();

        if (InputManager.Instance.ShouldUseSkill1) 
            animator.SetTrigger("Skill1");


        if (InputManager.Instance.ShouldUseSkill2)
            animator.SetTrigger("Skill2");
    }

}
