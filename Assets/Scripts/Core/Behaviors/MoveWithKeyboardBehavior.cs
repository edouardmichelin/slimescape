using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Input Keys
public enum InputKeyboard
{
    arrows = 0,
    wasd = 1
}
public class MoveWithKeyboardBehavior : AgentBehaviour
{
    public InputKeyboard inputKeyboard;

    void Start()
    {
        agent.SetVisualEffect(
            VisualEffect.VisualEffectConstAll, 
            inputKeyboard == InputKeyboard.wasd ? Color.blue : Color.yellow, 
            0);
        agent.SetCasualBackdriveAssistEnabled(true);
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

    //clears remaining haptic effects and enables BackdriveAssist
    public void MoveNormally()
    {
        agent.ClearHapticFeedback();
        agent.SetCasualBackdriveAssistEnabled(true);
    }

    //Will override backdriveAssist to false
    public void MoveOnStone(bool boolean)
    {
        agent.MoveOnStone();
    }

}
