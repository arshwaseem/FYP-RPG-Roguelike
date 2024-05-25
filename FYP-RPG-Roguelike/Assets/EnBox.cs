using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnBox : TriggerBasic
{
    public override void TriggerInvoke()
    {
        base.TriggerInvoke();
        SceneManager.LoadScene("End");
    }
}
