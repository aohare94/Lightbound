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

        AddAnimationState(controller, "WillowIdleS", "Assets/Animations/Willow/Idle/IdleS.aseprite");
        AddAnimationState(controller, "WillowIdleN", "Assets/Animations/Willow/Idle/IdleN.aseprite");
        AddAnimationState(controller, "WillowIdleWE", "Assets/Animations/Willow/Idle/IdleWE.aseprite");
        AddAnimationState(controller, "WillowWalkS", "Assets/Animations/Willow/Walk/WalkS.aseprite");
        AddAnimationState(controller, "WillowWalkN", "Assets/Animations/Willow/Walk/WalkN.aseprite");
        AddAnimationState(controller, "WillowWalkWE", "Assets/Animations/Willow/Walk/WalkWE.aseprite");

        SetupTransitions(controller);

        animator.runtimeAnimatorController = controller;
        Debug.Log("Animator setup complete.");
    }

    void AddAnimationState(AnimatorController controller, string stateName, string path)
    {
        var state = controller.layers[0].stateMachine.AddState(stateName);
        AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
        state.motion = clip;
    }

    void SetupTransitions(AnimatorController controller)
    {
        var rootStateMachine = controller.layers[0].stateMachine;

        AnimatorState idleStateS = rootStateMachine.states.FirstOrDefault(s => s.state.name == "WillowIdleS").state;
        AnimatorState walkStateS = rootStateMachine.states.FirstOrDefault(s => s.state.name == "WillowWalkS").state;
        AnimatorState idleStateN = rootStateMachine.states.FirstOrDefault(s => s.state.name == "WillowIdleN").state;
        AnimatorState walkStateN = rootStateMachine.states.FirstOrDefault(s => s.state.name == "WillowWalkN").state;
        AnimatorState idleStateWE = rootStateMachine.states.FirstOrDefault(s => s.state.name == "WillowIdleWE").state;
        AnimatorState walkStateWE = rootStateMachine.states.FirstOrDefault(s => s.state.name == "WillowWalkWE").state;

        AddTransition(idleStateS, walkStateS, "Speed", 0, true);
        AddTransition(walkStateS, idleStateS, "Speed", 0, false);
        AddTransition(idleStateN, walkStateN, "Speed", 0, true);
        AddTransition(walkStateN, idleStateN, "Speed", 0, false);
        AddTransition(idleStateWE, walkStateWE, "Speed", 0, true);
        AddTransition(walkStateWE, idleStateWE, "Speed", 0, false);
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
