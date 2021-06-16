using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomObjectSelector : MonoBehaviour
{
    public bool randomColour;
    public Material[] materials;
    public RandomItems[] randomItems;

    void Start()
    {
        StartCoroutine(LoadItems());
    }

    IEnumerator LoadItems()
    {
        Material newMat = materials[Random.Range(0, materials.Length)];
        foreach (RandomItems i in randomItems)
        {
            if (randomColour)
            {
                i.SetMaterial(newMat);
            }
            i.ChooseRandomObject();
            yield return new WaitForEndOfFrame();
        }
    }
}

[System.Serializable]
public class RandomItems
{
    public bool assignMaterial;
    public GameObject[] gameObjects;
    private Material mat;

    public void SetMaterial(Material m)
    {
        mat = m;
    }

    public void ChooseRandomObject()
    {
        int randomObj = Random.Range(0, gameObjects.Length);
        gameObjects[randomObj].SetActive(true);
        if (assignMaterial)
        {
            gameObjects[randomObj].GetComponent<Renderer>().material = mat;
        }
    }
}
