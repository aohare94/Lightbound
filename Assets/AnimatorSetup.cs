using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

public class AnimatorSetup : MonoBehaviour
{
    public Animator animator;

    void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogWarning("Animator component is not found. Adding an Animator component.");
                animator = gameObject.AddComponent<Animator>();
            }
        }

        SetupAnimator(animator);
    }

    void SetupAnimator(Animator animator)
    {
        if (animator == null)
        {
            Debug.LogError("Animator is not assigned.");
            return;
        }

        AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;
        if (controller == null)
        {
            controller = AnimatorController.CreateAnimatorControllerAtPath("Assets/Animator/WillowAnimator.controller");
            animator.runtimeAnimatorController = controller;
        }

        AddParameterIfNotExists(controller, "StartTransform", AnimatorControllerParameterType.Trigger);
        AddParameterIfNotExists(controller, "StartSpin", AnimatorControllerParameterType.Trigger);
        AddParameterIfNotExists(controller, "StartReverse", AnimatorControllerParameterType.Trigger);
        AddParameterIfNotExists(controller, "SpinDuration", AnimatorControllerParameterType.Float);
        AddParameterIfNotExists(controller, "isMoving", AnimatorControllerParameterType.Bool);
        AddParameterIfNotExists(controller, "CurrentFrame", AnimatorControllerParameterType.Float);

        var stateMachine = controller.layers[0].stateMachine;
        var idleState = AddStateIfNotExists(stateMachine, "WillowSouthIdle", "Assets/Animations/WillowSouthIdle.anim");
        var walkState = AddStateIfNotExists(stateMachine, "WillowSouthWalk", "Assets/Animations/WillowSouthWalk.anim");
        var swordTransState = AddStateIfNotExists(stateMachine, "WillowSouthSwordTrans", "Assets/Animations/WillowSouthSwordTrans.anim");
        var spinState = AddStateIfNotExists(stateMachine, "SwordSpin", "Assets/Animations/SwordSpin.anim");

        AddTransitionIfNotExists(idleState, walkState, "isMoving", true);
        AddTransitionIfNotExists(walkState, idleState, "isMoving", false);
        AddTransitionIfNotExists(idleState, swordTransState, "StartTransform");
        AddTransitionIfNotExists(walkState, swordTransState, "StartTransform");
        AddTransitionIfNotExists(swordTransState, spinState, "StartSpin");
        AddTransitionIfNotExists(swordTransState, idleState, "StartReverse");
        AddTransitionIfNotExists(spinState, idleState, "StartReverse");
        AddTransitionIfNotExists(spinState, walkState, "isMoving", true);

        controller.layers[0].stateMachine.defaultState = idleState;
    }

    void AddParameterIfNotExists(AnimatorController controller, string paramName, AnimatorControllerParameterType type)
    {
        foreach (var param in controller.parameters)
        {
            if (param.name == paramName) return;
        }
        controller.AddParameter(paramName, type);
    }

    AnimatorState AddStateIfNotExists(AnimatorStateMachine stateMachine, string stateName, string animationPath)
    {
        foreach (var state in stateMachine.states)
        {
            if (state.state.name == stateName) return state.state;
        }
        var newState = stateMachine.AddState(stateName);
        newState.motion = AssetDatabase.LoadAssetAtPath<AnimationClip>(animationPath);
        return newState;
    }

    void AddTransitionIfNotExists(AnimatorState fromState, AnimatorState toState, string condition, bool conditionValue = true)
    {
        foreach (var transition in fromState.transitions)
        {
            if (transition.destinationState == toState && transition.conditions.Length == 1 &&
                transition.conditions[0].parameter == condition &&
                transition.conditions[0].mode == (conditionValue ? AnimatorConditionMode.If : AnimatorConditionMode.IfNot))
                return;
        }
        var newTransition = fromState.AddTransition(toState);
        newTransition.AddCondition(conditionValue ? AnimatorConditionMode.If : AnimatorConditionMode.IfNot, 0, condition);
        newTransition.hasExitTime = false;
    }
}
