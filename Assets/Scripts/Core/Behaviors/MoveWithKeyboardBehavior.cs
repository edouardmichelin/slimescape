using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Input Keys
public enum InputKeyboard
{
    arrows = 0,
    wasd = 1,
}

public enum Colors
{
    Blue = 0,
    Pink = 1,
    Yellow = 2
}

public class MoveWithKeyboardBehavior : AgentBehaviour
{
    public InputKeyboard inputKeyboard;
    public Colors color;
    
    public bool IsGemOwner
    {
        get;
        set;
    }

    void Start()
    {
        GameManager.Instance.RegisterPlayer(this);
        agent.SetVisualEffect(VisualEffect.VisualEffectConstAll, inputKeyboard == InputKeyboard.arrows ? Color.cyan : Color.magenta,  0);
    }

    public override Steering GetSteering()
    {
        float horizontal = Input.GetAxis($"Horizontal_{inputKeyboard}");
        float vertical = Input.GetAxis($"Vertical_{inputKeyboard}");
        
        Steering steering = new Steering();
        
        steering.linear = new Vector3(horizontal, 0, vertical) * agent.maxAccel;
        steering.linear = this.transform.parent.TransformDirection(Vector3.ClampMagnitude(steering.linear, agent.maxAccel));
        
        return steering;
    }
    
    // clears remaining haptic effects and enables BackdriveAssist
    public void MoveNormally()
    {
        agent.ClearHapticFeedback();
        agent.SetCasualBackdriveAssistEnabled(true);
    }

    // Will override backdriveAssist to false
    public void MoveOnStone()
    {
        agent.MoveOnStone();
    }

    public override void OnCelluloLongTouch(int key)
    {
        Debug.Log($"This is player {this.inputKeyboard} and you long touched led {key}");
        GameManager.Instance.TryUpdateReadyState(this);
    }
    
    public void SetColor(int color)
    {
        agent.SetVisualEffect(VisualEffect.VisualEffectConstAll,
            (Colors)color == Colors.Blue ? Color.cyan :
            (Colors)color == Colors.Pink ? Color.magenta :
            (Colors)color == Colors.Yellow ? Color.yellow : Color.gray,
            0);
    }

    public void SetControls(int control)
    {
        inputKeyboard = (InputKeyboard) control;
    }
    
    
    void OnCollisionEnter(Collision collisionInfo)
    {
        if (IsGemOwner)
        {
            GameObject collider = collisionInfo.collider.transform.parent.gameObject;
            if (collider.CompareTag(Config.TAG_DOG))
            {
                GameManager.Instance.TryUpdateScoreOf(collider, Config.POINTS_FOR_PLAYER_CAUGHT_BY_GEM_OWNER);
                GameManager.Instance.TryUpdateScoreOf(this.gameObject, -1 * Config.POINTS_FOR_PLAYER_CAUGHT_BY_GEM_OWNER);
                AudioManager.Instance.PlaySoundEffect("stealPoints");
                IsGemOwner = false;
            }
        }
    }
}
