using System.Collections.Generic;
using UnityEngine; 

/// <summary>
/// Manages the configuration and constants of the project. 
/// </summary>
public static class Config
{
    //Global variables
    public static int controlPanelSelectedCellulo = 0; 

    // Scanner
    public const float REFRESH_TIMER = 1f; // seconds after which it will stop and start scanning to refresh

    ///////////////////
    //// CONSTANTS ////
    ///////////////////
    
    // Game Constants
    public const float GAME_DURATION = 60f;
    public const float MAX_GAME_DURATION = 600f;
    public const float MIN_ROLE_TIME = 10f;
    public const float MAX_ROLE_TIME = 18f;

    // Tags
    public const string TAG_DOG = "Dog";
    public const string TAG_SHEEP = "Sheep";
    public const string TAG_GHOST = "Ghost";
    public const string TAG_BORDER = "Border";
    public const string TAG_START_PROMPT = "StartGamePrompt";
    
    //Starting positions
    public const float PLAYER1_STARTPOS_X = 2.5f;
    public const float PLAYER1_STARTPOS_Y = -3.37f;
    public const float PLAYER1_STARTPOS_THETA = 0f;
    
    public const float PLAYER2_STARTPOS_X = 2.5f;
    public const float PLAYER2_STARTPOS_Y = -7.02f;
    public const float PLAYER2_STARTPOS_THETA = 0f;
    
    public const float SLIME_STARTPOS_X = 8.8f;
    public const float SLIME_STARTPOS_Y = -5.08f;
    public const float SLIME_STARTPOS_THETA = 0f;

    // Points
    public const int POINTS_FOR_PLAYER_CAUGHT_BY_GHOST = -1;
    public const int POINTS_FOR_SHEEP_IN_RING = 1;
    public const int POINTS_FOR_PLAYER_CAUGHT_BY_GEM_OWNER = -2;
    
    // Spawn intervals
    public const float SPAWNER_GEMS_MIN_TIME_INTERVAL = 10f;
    public const float SPAWNER_GEMS_MAX_TIME_INTERVAL = 15f;
    public const float SPAWNER_GEMS_DIFFUCULTY_DELTA_TIME_INTERVAL = 5f;
    
    // Slime constant
    public const float SLIME_SPEED_SLOW = 0.8f;
    public const float SLIME_SPEED_DEFAULT = 0.85f;
    public const float SLIME_SPEED_FAST = 0.92f;

    // Cellulo constant
    public const int CELLULO_KEYS = 6;
    public const float DEFAULT_CONTROL_PERIOD = 0.1f;
    public const float goalPoseThreshold = 1f;
    public const float goalRotationThreshold = 10f;

    ///////////////////
    //// SCALING ////
    ///////////////////

    /* Scaling between the real map and the virtual one; 
    The default is that 10 units in unity = min(x dimension of real map, y dimension of real map) */
    public static MapOriginPosition ORIGIN = MapOriginPosition.TopLeft;
    public static MapOrientation ORIENTATION = MapOrientation.XisLarger;
    public static int REAL_MAP_DIMENSION_X = 841 ; // in mm 
    public static int REAL_MAP_DIMENSION_Y = 1189; // in mm 

    public static float GetCelluloScale(){
        return 1000.0f/ Mathf.Min(REAL_MAP_DIMENSION_X,REAL_MAP_DIMENSION_Y);
    }

    public static float UNITY_MAP_DIMENSION_X = REAL_MAP_DIMENSION_X<=REAL_MAP_DIMENSION_Y?10:10* REAL_MAP_DIMENSION_X/ REAL_MAP_DIMENSION_Y; 
    public static float UNITY_MAP_DIMENSION_Y = REAL_MAP_DIMENSION_Y<= REAL_MAP_DIMENSION_X ? 10 : 10 * REAL_MAP_DIMENSION_Y / REAL_MAP_DIMENSION_X;
    
    public static float GetRatioUnityToRealInX(){
        UNITY_MAP_DIMENSION_X = REAL_MAP_DIMENSION_X<=REAL_MAP_DIMENSION_Y?10:10* REAL_MAP_DIMENSION_X/ REAL_MAP_DIMENSION_Y; 
        return REAL_MAP_DIMENSION_X/UNITY_MAP_DIMENSION_X;
    }
    public static float GetRatioUnityToRealInY(){
        UNITY_MAP_DIMENSION_Y = REAL_MAP_DIMENSION_Y<= REAL_MAP_DIMENSION_X ? 10 : 10 * REAL_MAP_DIMENSION_Y / REAL_MAP_DIMENSION_X;
        return REAL_MAP_DIMENSION_Y/UNITY_MAP_DIMENSION_Y;
    }


    public static Vector3 RealToUnityPositionScale(float x, float y){
        Vector3 position = Vector3.zero; 
        switch(ORIGIN){
            case MapOriginPosition.TopLeft:
                position.x = x/GetRatioUnityToRealInX();
                position.z = -y/GetRatioUnityToRealInY();
            break; 
            case MapOriginPosition.TopRight:
                position.x = (REAL_MAP_DIMENSION_Y - y)/GetRatioUnityToRealInY();
                position.z = -x/GetRatioUnityToRealInX(); 
            break; 
            case MapOriginPosition.BottomRight:
                position.x = (REAL_MAP_DIMENSION_X - x)/GetRatioUnityToRealInX(); 
                position.z = -(REAL_MAP_DIMENSION_Y - y)/GetRatioUnityToRealInY();
            break; 
            case MapOriginPosition.BottomLeft: 
                position.x = y/GetRatioUnityToRealInY(); 
                position.z = -(REAL_MAP_DIMENSION_X- x)/GetRatioUnityToRealInX();
            break; 
        }
        return position;
    }
    public static Vector2 UnityToRealPositionScale(float x, float y){
        Vector2 position = Vector2.zero; 
        switch(ORIGIN){
            case MapOriginPosition.TopLeft:
                position.x = x*GetRatioUnityToRealInX();
                position.y = -y*GetRatioUnityToRealInY();
            break; 
            case MapOriginPosition.TopRight:
                position.y = REAL_MAP_DIMENSION_Y - x*GetRatioUnityToRealInY();
                position.x = -y*GetRatioUnityToRealInX(); 
            break; 
            case MapOriginPosition.BottomRight:
                position.x = REAL_MAP_DIMENSION_X - x*GetRatioUnityToRealInX(); 
                position.y = REAL_MAP_DIMENSION_Y + y*GetRatioUnityToRealInY();
            break; 
            case MapOriginPosition.BottomLeft: 
                position.y = x*GetRatioUnityToRealInY(); 
                position.x = REAL_MAP_DIMENSION_X + y*GetRatioUnityToRealInX();
            break;

        }
        return position;
    }
        public static Vector2 UnityToRealVelocityScale(float x, float y){
        Vector2 position = Vector2.zero; 
        switch(ORIGIN){
            case MapOriginPosition.TopLeft:
                position.x = x*GetRatioUnityToRealInX();
                position.y = -y*GetRatioUnityToRealInY();
            break; 
            case MapOriginPosition.TopRight:
                position.y = -x*GetRatioUnityToRealInY();
                position.x = -y*GetRatioUnityToRealInX(); 
            break; 
            case MapOriginPosition.BottomRight:
                position.x = -x*GetRatioUnityToRealInX(); 
                position.y = y*GetRatioUnityToRealInY();
            break; 
            case MapOriginPosition.BottomLeft: 
                position.y = x*GetRatioUnityToRealInY(); 
                position.x = y*GetRatioUnityToRealInX();
            break;

        }
        return position;
    }

}
public enum MapOriginPosition {
    TopLeft =0, TopRight, BottomLeft, BottomRight
}
public enum MapOrientation { 
    XisLarger = 0, YisLarger
}
