using UnityEngine;
using UnityEngine.UI;

using Niantic.ARDK.AR.ARSessionEventArgs;
using Niantic.ARDK.AR.HitTest;
using Niantic.ARDK.AR.Networking;
using Niantic.ARDK.AR.Networking.ARNetworkingEventArgs;
using Niantic.ARDK.Extensions;
using Niantic.ARDK.Networking;
using Niantic.ARDK.Networking.MultipeerNetworkingEventArgs;
using Niantic.ARDK.Utilities;
using Niantic.ARDK.Utilities.Input.Legacy;

namespace Niantic.ARDK.Templates 
{
    public class SharedInteractionController : MonoBehaviour 
    {
        public SharedSession SharedSession;
        public float TriggerDistance = 1.5f;
        bool _isObjectPlaced = false;

        private void Update() 
        {
            float distance = Vector3.Distance (SharedSession.SharedObjectHolder.transform.position, Camera.main.gameObject.transform.position);

            if (distance <= TriggerDistance) 
            {
                if (SharedSession._isHost)
                { }//SharedSession.SharedObjectHolder.ObjectInteraction.AnimateObjectDistance();
                else //SharedSession._messagingManager.AskHostToAnimateObjectDistance(SharedSession._host);
                { }
            }

            if (PlatformAgnosticInput.touchCount <= 0) return;

            var touch = PlatformAgnosticInput.GetTouch(0);
            if (touch.phase == TouchPhase.Began) 
            {
                TouchBegan(touch);
            }
        }

        private void TouchBegan(Touch touch) 
        {
            var currentFrame = SharedSession._arNetworking.ARSession.CurrentFrame;
            if (currentFrame == null) return;
            if (SharedSession._camera == null) return;

            var worldRay = SharedSession._camera.ScreenPointToRay(touch.position);
            RaycastHit hit;

            if (Physics.Raycast(worldRay, out hit, 1000f)) 
            {   
                {
                    //Debug.Log(hit.collider.name);
                    InteractiveObject iObject = hit.transform.GetComponent<InteractiveObject>();
                    if(iObject)
                    {
                        iObject.Trigger();
                        if (SharedSession._isHost)
                        {
                            SharedSession._messagingManager.BroadcastTriggerObject(iObject._index);
                        }
                        else
                        {
                            SharedSession._messagingManager.AskHostToTriggerObject(SharedSession._host, iObject._index);
                        }
                    }
                }
            } 
            else 
            {                
                var hitTestResults = currentFrame.HitTest (
                    SharedSession._camera.pixelWidth, 
                    SharedSession._camera.pixelHeight, 
                    touch.position, 
                    ARHitTestResultType.EstimatedHorizontalPlane
                );

                if (hitTestResults.Count <= 0) return;

                _isObjectPlaced = true;
                var position = hitTestResults[0].WorldTransform.ToPosition();

                if (SharedSession._isHost) 
                {
                    if (!SharedSession.SharedObjectHolder.gameObject.activeSelf && SharedSession._isStable) 
                    {
                        SharedSession.SharedObjectHolder.gameObject.SetActive(true);
                    }
                    if(!_isObjectPlaced)
                    SharedSession.SharedObjectHolder.MoveObject(position);
                }
                else 
                {   
                    if(!_isObjectPlaced)
                    SharedSession._messagingManager.AskHostToMoveObject(SharedSession._host, position);
                }
            }
        }
    }
}
