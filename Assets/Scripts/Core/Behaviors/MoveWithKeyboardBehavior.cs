using System.Linq;
using UnityEngine;

//Input Keys
public enum InputKeyboard
{
    arrows = 0,
    wasd = 1,
}

// Ids
public enum PlayerId
{
    Player_1 = 1,
    Player_2 = 2,
}

public class MoveWithKeyboardBehavior : AgentBehaviour
{
    public PlayerId id; 
    public InputKeyboard inputKeyboard;
    public Player.Colors color;
    
    public bool IsGemOwner { get; set; }
    private bool[] m_ledsTouchBegin;

    void Start()
    {
        m_ledsTouchBegin = new bool[Config.CELLULO_KEYS];
        
        GameManager.Instance.TryRegisterPlayer(this, id, inputKeyboard);
        SetColor(inputKeyboard == InputKeyboard.arrows ? Player.Colors.Blue : Player.Colors.Pink);
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

    public void TeleportTo(MoveWithKeyboardBehavior otherPlayer)
    {
        // swap colors
        (otherPlayer.color, color) = (color, otherPlayer.color);
        /*otherPlayer.SetColor(color);
        SetColor(otherColor);*/

        // swap ids
        if (id == PlayerId.Player_1)
        {
            id = PlayerId.Player_2;
            otherPlayer.id = PlayerId.Player_1;
        }
        else
        {
            id = PlayerId.Player_1;
            otherPlayer.id = PlayerId.Player_2;
        }
        
        // swap controls
        if (inputKeyboard == InputKeyboard.wasd)
        {
            inputKeyboard = InputKeyboard.arrows;
            otherPlayer.inputKeyboard = InputKeyboard.wasd;
        }
        else
        {
            inputKeyboard = InputKeyboard.wasd;
            otherPlayer.inputKeyboard = InputKeyboard.arrows;
        }
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

    public void GoToStartPosition()
    {
        if (id == PlayerId.Player_1)
        {
            agent.SetGoalPose(
                Config.PLAYER1_STARTPOS_X, 
                Config.PLAYER1_STARTPOS_Y, 
                Config.PLAYER1_STARTPOS_THETA, 
                3, 
                3);
        }
        else
        {
            agent.SetGoalPose(
                Config.PLAYER2_STARTPOS_X, 
                Config.PLAYER2_STARTPOS_Y, 
                Config.PLAYER2_STARTPOS_THETA, 
                3, 
                3);
        }
    }

    public override void OnGoalPoseReached()
    {
        Debug.Log("Player reached his starting position");
    }

    public override void OnCelluloConnect()
    {
        GoToStartPosition();
    }

    public override void OnCelluloTouchBegan(int key)
    {
        m_ledsTouchBegin[key] = true;
    }

    public override void OnCelluloTouchReleased(int key)
    {
        m_ledsTouchBegin[key] = false;
    }
    
    public override void OnCelluloLongTouch(int key)
    {
        switch (m_ledsTouchBegin.Count(x => x))
        {
            case 1:
                GameManager.Instance.SetGameDifficulty(Difficulty.Easy);
                break;
            
            case 2:
                GameManager.Instance.SetGameDifficulty(Difficulty.Normal);
                break;
            
            case 3:
                GameManager.Instance.SetGameDifficulty(Difficulty.Hard);
                break;
            
            default:
                GameManager.Instance.SetGameDifficulty(Difficulty.Normal);
                break;
        }
        Debug.Log($"This is player {this.inputKeyboard} and you long touched led {key}");
        GameManager.Instance.TryUpdateReadyState(id);
    }

    public override void OnCelluloKidnapped()
    {
        GameManager.Instance.PlayerKidnapped(id);
    }

    public override void OnCelluloUnKidnapped()
    {
        GameManager.Instance.PlayerUnkidnapped(id);
    }
    
    public void SetColor(int c)
    {
        color = (Player.Colors) c;
        agent.SetVisualEffect(VisualEffect.VisualEffectConstAll,
            color == Player.Colors.Blue ? Color.cyan :
            color == Player.Colors.Pink ? Color.magenta :
            color == Player.Colors.Yellow ? Color.yellow : Color.gray,
            0);
    }
    
    public void SetColor(Player.Colors c)
    {
        color = c;
        GameManager.Instance.TryUpdatePlayerColor(id, color);
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
