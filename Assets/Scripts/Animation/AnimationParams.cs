using UnityEngine;

public static class AnimationParams
{
    ////////////////////////////////1. STATES //////////////////////////////////////  
  
                                    //+ Movement States
    public static readonly int Jump_Start_State = Animator.StringToHash("Jump Start");
    public static readonly int Jump_End_State = Animator.StringToHash("Jump End");
    public static readonly int Roll_State = Animator.StringToHash("Roll");
    public static readonly int Swimming_State = Animator.StringToHash("Swimming");
    public static readonly int Locomotion_State = Animator.StringToHash("Locomotion");

                                    //+ Crafting States
    public static readonly int Dig_State = Animator.StringToHash("Dig");
    public static readonly int Hammering_State = Animator.StringToHash("Hammering");
    public static readonly int Axe_State = Animator.StringToHash("Axe");
    public static readonly int Mining_State = Animator.StringToHash("Mining");

                                    //+ Fishing States
    public static readonly int Casting_State = Animator.StringToHash("Casting");
    public static readonly int Reeling_State = Animator.StringToHash("Reeling");
    public static readonly int Caught_State = Animator.StringToHash("Caught");

                                //+ Regular Stuffs States
    public static readonly int Carry_State = Animator.StringToHash("Carry");
    public static readonly int Doing_State = Animator.StringToHash("Doing");
    public static readonly int Watering_State = Animator.StringToHash("Watering");

    ////////////////////////////////2. PARAMATERS //////////////////////////////////

                                    //+ Movement Params
    public static readonly int Speed_Param = Animator.StringToHash("Speed");
    public static readonly int IsGround_Param = Animator.StringToHash("isGround");
}
