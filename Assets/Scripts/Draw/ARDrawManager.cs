using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ARDrawManager : MonoBehaviour
{
    [SerializeField]
    private LineSettings _lineSettings = null;

    [SerializeField]
    private UnityEvent OnDraw = null;

    
    [SerializeField]
    private Camera _arCamera = null;


    private Dictionary<int, ARLine> _lines = new Dictionary<int, ARLine>();

    private bool CanDraw { get; set; }

    [SerializeField]
    Niantic.ARDK.Templates.SharedSession _sharedSession;
    void Update()
    {
#if !UNITY_EDITOR
        DrawOnTouch();
#else
        DrawOnMouse();
#endif
    }

        public void ToggleDraw()
    {
        StopCoroutine(ToggleDrawAfterDelay());
        StartCoroutine(ToggleDrawAfterDelay());
    }
    IEnumerator ToggleDrawAfterDelay()
    {
        yield return new WaitForSeconds(1);
        CanDraw = !CanDraw;
    }

    void DrawOnTouch()
    {
        if (!CanDraw) return;

        int tapCount = Input.touchCount > 1 && _lineSettings.allowMultiTouch ? Input.touchCount : 1;

        if(Input.touchCount>0)
        for (int i = 0; i < tapCount; i++)
        {
            
            Touch touch = Input.GetTouch(i);
            Vector3 touchPosition = _arCamera.ScreenToWorldPoint(new Vector3(Input.GetTouch(i).position.x, Input.GetTouch(i).position.y, _lineSettings.distanceFromCamera));

            RaycastHit hit;
            if (Physics.Raycast(_arCamera.ScreenPointToRay(Input.GetTouch(i).position, Camera.MonoOrStereoscopicEye.Mono), out hit))
            {
                touchPosition = hit.point;
            }
            else
                {
                    return;
                }


            if (touch.phase == TouchPhase.Began)
            {
                OnDraw?.Invoke();
                
                ARLine line = new ARLine(_lineSettings);
                _lines.Add(touch.fingerId, line);
                line.AddNewLineRenderer(transform, _sharedSession._arNetworking.ARSession.AddAnchor(transform.localToWorldMatrix), touchPosition);
            }
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                _lines[touch.fingerId].AddPoint(touchPosition);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                _lines.Remove(touch.fingerId);
            }
        }
    }

    void DrawOnMouse()
    {
        if (!CanDraw) return;

        Vector3 mousePosition = _arCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _lineSettings.distanceFromCamera));

        RaycastHit hit;
        if (Physics.Raycast(_arCamera.ScreenPointToRay(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono), out hit))
        {
            mousePosition = hit.point;
        }
        if (Input.GetMouseButton(0))
        {
            OnDraw?.Invoke();

            if (_lines.Keys.Count == 0)
            {
                ARLine line = new ARLine(_lineSettings);
                _lines.Add(0, line);
                line.AddNewLineRenderer(transform, null, mousePosition);
            }
            else
            {
                _lines[0].AddPoint(mousePosition);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _lines.Remove(0);
        }
    }



    GameObject[] GetAllLinesInScene()
    {
        return GameObject.FindGameObjectsWithTag("Line");
    }

    public void ClearLines()
    {
        GameObject[] lines = GetAllLinesInScene();
        foreach (GameObject currentLine in lines)
        {
            LineRenderer line = currentLine.GetComponent<LineRenderer>();
            Destroy(currentLine);
        }
    }
}
