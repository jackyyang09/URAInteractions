using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectNameConverter : MonoBehaviour {

    public Transform theObject;
    public char charTBR; // To be replaced
    public char charR; // Replacement character

    public void Activate()
    {
        ChangeName(theObject);
    }

    public void ChangeName(Transform _theObject)
    {
        if (_theObject == null)
        {
            return;
        }

        char[] newName = new char[_theObject.name.Length];
        for (int i = 0; i < _theObject.name.Length; i++)
        {
            if (_theObject.name[i] == charTBR)
            {
                newName[i] = charR;
                continue;
            }
            newName[i] = _theObject.name[i];
        }

        _theObject.name = new string(newName);

        int index = 0;
        while (index < _theObject.childCount)
        {
            ChangeName(_theObject.GetChild(index));
            index++;
        }
    }
}
