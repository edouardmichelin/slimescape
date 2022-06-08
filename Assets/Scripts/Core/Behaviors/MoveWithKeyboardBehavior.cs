using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

//Input Keys
public enum InputKeyboard
{
    arrows = 0,
    wasd = 1,
}

public class MoveWithKeyboardBehavior : AgentBehaviour
{
    public InputKeyboard inputKeyboard;
    public Player.Colors color;
    
    public bool IsGemOwner { get; set; }
    private bool[] m_ledsTouchBegin;

    void Start()
    {
        m_ledsTouchBegin = new bool[Config.CELLULO_KEYS];
        GameManager.Instance.TryRegisterPlayer(this, inputKeyboard);
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

    public override void OnCelluloTouchBegan(int key)
    {
        Debug.Log($"This is player {this.inputKeyboard} and led {key} is pressed");
        m_ledsTouchBegin[key] = true;
    }

    public override void OnCelluloTouchReleased(int key)
    {
        Debug.Log($"This is player {this.inputKeyboard} and led {key} is released");
        m_ledsTouchBegin[key] = false;
    }
    
    public override void OnCelluloLongTouch(int key)
    {
        switch (m_ledsTouchBegin.Count(x => x))
        {
            case 1:
                Debug.Log("Easy has been selected");
                break;
            
            case 2:
                Debug.Log("Medium has been selected");
                break;
            
            case 3:
                Debug.Log("Hard has been selected");
                break;
            
            default:
                Debug.Log("Hard has been selected");
                break;
        }
        Debug.Log($"This is player {this.inputKeyboard} and you long touched led {key}");
        GameManager.Instance.TryUpdateReadyState(this);
    }

    public override void OnCelluloKidnapped()
    {
        GameManager.Instance.PlayerKidnapped();
    }

    public override void OnCelluloUnKidnapped()
    {
        GameManager.Instance.PlayerUnkidnapped();
    }
    
    public void SetColor(int c)
    {
		color = (Player.Colors)c;
        agent.SetVisualEffect(VisualEffect.VisualEffectConstAll,
            color == Player.Colors.Blue ? Color.cyan :
            color == Player.Colors.Pink ? Color.magenta :
            color == Player.Colors.Yellow ? Color.yellow : Color.gray,
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
