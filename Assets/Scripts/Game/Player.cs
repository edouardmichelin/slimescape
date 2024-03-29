using UnityEngine;

public class Player
{
    public bool IsReady { get; set; }
    
    public int Score { get; set; }
    
    public MoveWithKeyboardBehavior Behavior { get; set; }

    public PlayerId Id { get; set; }
    
    public InputKeyboard Controls { get; set; }

    public Player.Colors Color { get; set; }
    
    public bool IsKidnapped { get; set; }

    public enum Colors
    {
        Blue = 0,
        Pink = 1,
        Yellow = 2
    }
}
