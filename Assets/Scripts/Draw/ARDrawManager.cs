using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Niantic.ARDK.Utilities;

public class ARDrawManager : MonoBehaviour
{
    [SerializeField]
    private LineSettings _lineSettings = null;

    [SerializeField]
    private UnityEvent OnDraw = null;


    [SerializeField]
    private Camera _arCamera = null;

    [SerializeField]
    ProtectionCircle _protectionCircle;


    private Dictionary<int, ARLine> _lines = new Dictionary<int, ARLine>();

    private bool CanDraw { get; set; }

    Vector2 _lastTouchPos = new Vector2(0, 0);

    [SerializeField]
    Niantic.ARDK.Templates.SharedSession _sharedSession;
    void Update()
    {
        Debug.DrawRay(_arCamera.ScreenPointToRay(_lastTouchPos, Camera.MonoOrStereoscopicEye.Mono).origin, _arCamera.ScreenPointToRay(_lastTouchPos, Camera.MonoOrStereoscopicEye.Mono).direction);

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
        bool isHit = false;

        if (Input.touchCount > 0)
            for (int i = 0; i < tapCount; i++)
            {

                Touch touch = Input.GetTouch(i);
                Vector3 touchPosition = _arCamera.ScreenToWorldPoint(new Vector3(Input.GetTouch(i).position.x, Input.GetTouch(i).position.y, _lineSettings.distanceFromCamera));
                _lastTouchPos = touch.position;
                RaycastHit hit;
                if (Physics.Raycast(_arCamera.ScreenPointToRay(Input.GetTouch(i).position, Camera.MonoOrStereoscopicEye.Mono), out hit))
                {
                    touchPosition = hit.point;
                    Debug.Log("Drawing on " + hit.collider.name);
                    isHit = true;
                }
                var currentFrame = _sharedSession._arNetworking.ARSession.CurrentFrame;
                if (currentFrame == null) return;
                if (_sharedSession._camera == null) return;

                var hitTestResults = currentFrame.HitTest(
                            _sharedSession._camera.pixelWidth,
                            _sharedSession._camera.pixelHeight,
                            _lastTouchPos,
                            Niantic.ARDK.AR.HitTest.ARHitTestResultType.All
                        );

                if (hitTestResults.Count > 0)
                {
                    isHit = true;
                    touchPosition = hitTestResults[0].WorldTransform.ToPosition();
                }

                if (!isHit)
                    return;


                if (touch.phase == TouchPhase.Began)
                {
                    OnDraw?.Invoke();

                    ARLine line = gameObject.AddComponent<ARLine>();
                    line.settings = _lineSettings;
                    _lines.Add(touch.fingerId, line);
                    line.AddNewLineRenderer(transform, _sharedSession._arNetworking.ARSession.AddAnchor(transform.localToWorldMatrix), touchPosition);
                    Vector3 spawnPos = touchPosition;
                    spawnPos -= spawnPos - _arCamera.transform.position;
                    spawnPos.y = touchPosition.y;
                    _protectionCircle.transform.position = spawnPos;
                    float scale = Vector3.Distance(touchPosition, _arCamera.transform.position);
                    _protectionCircle.transform.localScale = new Vector3(scale, scale, scale);
                    _protectionCircle.Reset();
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

        bool isHit = false;
        Vector3 mousePosition = _arCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _lineSettings.distanceFromCamera));
        _lastTouchPos = Input.mousePosition;

        RaycastHit hit;
        if (Physics.Raycast(_arCamera.ScreenPointToRay(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono), out hit))
        {
            mousePosition = hit.point;

            Debug.Log("Mouse Drawing on " + hit.collider.name);
            isHit = true;

        }
        var currentFrame = _sharedSession._arNetworking.ARSession.CurrentFrame;
        if (currentFrame == null) return;
        if (_sharedSession._camera == null) return;

        var hitTestResults = currentFrame.HitTest(
                    _sharedSession._camera.pixelWidth,
                    _sharedSession._camera.pixelHeight,
                    _lastTouchPos,
                    Niantic.ARDK.AR.HitTest.ARHitTestResultType.All
                );

        if (hitTestResults.Count > 0)
        {
            isHit = true;
            mousePosition = hitTestResults[0].WorldTransform.ToPosition();
        }

        if (!isHit)
            return;

        if (Input.GetMouseButton(0))
        {
            OnDraw?.Invoke();

            if (_lines.Keys.Count == 0)
            {
                ARLine line = gameObject.AddComponent<ARLine>();
                line.settings = _lineSettings;
                _lines.Add(0, line);
                line.AddNewLineRenderer(transform, _sharedSession._arNetworking.ARSession.AddAnchor(transform.localToWorldMatrix), mousePosition);
                Vector3 spawnPos = mousePosition;
                spawnPos -= spawnPos - _arCamera.transform.position;
                spawnPos.y = mousePosition.y;
                _protectionCircle.transform.position = spawnPos;
                float scale = Vector3.Distance(mousePosition, _arCamera.transform.position);
                _protectionCircle.transform.localScale = new Vector3(scale, scale, scale);
                _protectionCircle.Reset();
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
