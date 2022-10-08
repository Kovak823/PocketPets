using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultySetter : MonoBehaviour
{
    public static bool isDiffEasy;
    public static bool isDiffNormal;
    public static bool isDiffHard;

    public void PlayEasy()
    {
        isDiffEasy = true;
        isDiffNormal = false;
        isDiffHard = false;
    }

    public void PlayNormal()
    {
        isDiffEasy = false;
        isDiffNormal = true;
        isDiffHard = false;
    }

    public void PlayHard()
    {
        isDiffEasy = false;
        isDiffNormal = false;
        isDiffHard = true;
    }
}
