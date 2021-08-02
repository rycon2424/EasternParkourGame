using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialTipTrigger : MonoBehaviour
{
    public GameObject popups;
    public bool triggered;
    [TextArea(5,5)] public string tip;
    public Sprite tipImage;
    [Space]
    public Image tutorialImage;
    public Text tutorialText;

    private void Start()
    {
        tutorialImage.sprite = tipImage;
        tutorialText.text = tip;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
        {
            return;
        }
        PlayerBehaviour pb = other.GetComponent<PlayerBehaviour>();
        if (pb != null)
        {
            triggered = true;
            popups.SetActive(true);
            PauseSystem.instance.Pause(false);
        }
    }

    public void ClosePopup()
    {
        popups.SetActive(false);
        PauseSystem.instance.Resume();
    }

    private void OnDrawGizmos()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        DrawBoxCollider(Color.red, boxCollider);
    }

    private void DrawBoxCollider(Color gizmoColor, BoxCollider boxCollider, float alphaForInsides = 0.3f)
    {
        var color = gizmoColor;
        
        Gizmos.matrix = Matrix4x4.TRS(this.transform.TransformPoint(boxCollider.center), this.transform.rotation, this.transform.lossyScale);
        
        Gizmos.color = color;
        Gizmos.DrawWireCube(Vector3.zero, boxCollider.size);
        
        color.a *= alphaForInsides;
        Gizmos.color = color;
        Gizmos.DrawCube(Vector3.zero, boxCollider.size);
    }
}
