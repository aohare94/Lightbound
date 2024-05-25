using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Linq;

public class WillowAnimationSetup : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component is not found or assigned.");
            return;
        }

        SetupAnimator();
    }

    void SetupAnimator()
    {
        string controllerPath = "Assets/AnimatorControllers/Willow/WillowMovementController.controller";
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);

        controller.AddParameter("Horizontal", AnimatorControllerParameterType.Float);
        controller.AddParameter("Vertical", AnimatorControllerParameterType.Float);
        controller.AddParameter("Speed", AnimatorControllerParameterType.Float);

        AddAnimationState(controller, "WillowIdleS", "Assets/Animations/Willow/Idle/IdleS_Clip");
        AddAnimationState(controller, "WillowIdleN", "Assets/Animations/Willow/Idle/IdleN_Clip");
        AddAnimationState(controller, "WillowIdleWE", "Assets/Animations/Willow/Idle/IdleWE_Clip");
        AddAnimationState(controller, "WillowWalkS", "Assets/Animations/Willow/Walk/WalkS_Clip");
        AddAnimationState(controller, "WillowWalkN", "Assets/Animations/Willow/Walk/WalkN_Clip");
        AddAnimationState(controller, "WillowWalkWE", "Assets/Animations/Willow/Walk/WalkWE_Clip");

        SetupTransitions(controller);

        animator.runtimeAnimatorController = controller;
        Debug.Log("Animator setup complete.");
    }

    void AddAnimationState(AnimatorController controller, string stateName, string path)
    {
        var state = controller.layers[0].stateMachine.AddState(stateName);
        AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path + ".anim");
        if (clip != null)
        {
            state.motion = clip;
        }
        else
        {
            Debug.LogError($"AnimationClip at path {path}.anim not found.");
        }
    }

    void SetupTransitions(AnimatorController controller)
    {
        var rootStateMachine = controller.layers[0].stateMachine;

        AnimatorState idleStateS = FindState(rootStateMachine, "WillowIdleS");
        AnimatorState walkStateS = FindState(rootStateMachine, "WillowWalkS");
        AnimatorState idleStateN = FindState(rootStateMachine, "WillowIdleN");
        AnimatorState walkStateN = FindState(rootStateMachine, "WillowWalkN");
        AnimatorState idleStateWE = FindState(rootStateMachine, "WillowIdleWE");
        AnimatorState walkStateWE = FindState(rootStateMachine, "WillowWalkWE");

        AddTransition(idleStateS, walkStateS, "Speed", 0, true);
        AddTransition(walkStateS, idleStateS, "Speed", 0, false);
        AddTransition(idleStateN, walkStateN, "Speed", 0, true);
        AddTransition(walkStateN, idleStateN, "Speed", 0, false);
        AddTransition(idleStateWE, walkStateWE, "Speed", 0, true);
        AddTransition(walkStateWE, idleStateWE, "Speed", 0, false);
    }

    AnimatorState FindState(AnimatorStateMachine stateMachine, string stateName)
    {
        foreach (var state in stateMachine.states)
        {
            if (state.state.name == stateName)
            {
                return state.state;
            }
        }
        Debug.LogError($"State {stateName} not found.");
        return null;
    }

    void AddTransition(AnimatorState fromState, AnimatorState toState, string parameterName, float threshold, bool greaterThan)
    {
        var transition = fromState.AddTransition(toState);
        if (greaterThan)
        {
            transition.AddCondition(AnimatorConditionMode.Greater, threshold, parameterName);
        }
        else
        {
            transition.AddCondition(AnimatorConditionMode.Less, threshold, parameterName);
        }
        transition.hasExitTime = false;
    }
}
