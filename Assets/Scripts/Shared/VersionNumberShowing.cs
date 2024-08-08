using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VersionNumberShowing : MonoBehaviour
{
    public Text version;
    public TextMeshProUGUI version1;

    void OnEnable()
    {
        if(version)
            version.text = string.Format("Version: {0}", Application.version);
        else
            version1.text = string.Format("Version: {0}", Application.version);
    }
}
