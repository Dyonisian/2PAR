using System;
using System.IO;
using UnityEngine;

using Niantic.ARDK.Networking;
using Niantic.ARDK.Networking.MultipeerNetworkingEventArgs;
using Niantic.ARDK.Utilities.BinarySerialization;
using Niantic.ARDK.Utilities.BinarySerialization.ItemSerializers;

namespace Niantic.ARDK.Templates 
{
    public class MessagingManager 
    {
        private IMultipeerNetworking _networking;
        private SharedSession _controller;
        private readonly MemoryStream _builderMemoryStream = new MemoryStream(24);
       
        private enum _MessageType : uint 
        {
            AskMoveObjectMessage,
            AskAnimateObjectTapMessage,
            AskAnimateObjectDistanceMessage,
            AskTriggerInteractive,
            ObjectPositionMessage,
            ObjectScaleMessage,
            ObjectRotationMessage,
            TriggerInteractive,
            AskHostDrawLine,
            AskHostAddLine,
            AskHostStopLine,
            DrawNewLineMessage,
            AddToLineMessage,
            StopLineMessage,
            InteractiveObjectPositionMessage,
            InteractiveObjectRotationMessage,
            PhaseMessage,
            AskPhase,
            BroadcastLookInteractiveActive,
            AskLookInteractiveActive
        }

       

        

        internal void InitializeMessagingManager(IMultipeerNetworking networking, SharedSession controller)
        {
            _networking = networking;
            _controller = controller;
            _networking.PeerDataReceived += OnDidReceiveDataFromPeer;
        }

        internal void AskHostToMoveObject(IPeer host, Vector3 position)
        {
            _networking.SendDataToPeer (
                (uint)_MessageType.AskMoveObjectMessage,
                SerializeVector3(position),
                host,
                TransportType.ReliableUnordered
            );
        }

        internal void BroadcastPhase(int v)
        {
            _networking.BroadcastData(
                (uint)_MessageType.PhaseMessage,
                SerializeUint(Convert.ToUInt16(v)),
                TransportType.UnreliableUnordered
            );
        }

        internal void AskPhase(IPeer host, int v)
        {
            _networking.SendDataToPeer(
                            (uint)_MessageType.AskPhase,
                            SerializeUint(Convert.ToUInt16(v)),
                            host,
                            TransportType.ReliableUnordered
                        );
        }

        internal void AskHostToTriggerObject(IPeer host, int index)
        {
            _networking.SendDataToPeer(
                            (uint)_MessageType.AskTriggerInteractive,
                            SerializeUint(Convert.ToUInt16(index)),
                            host,
                            TransportType.ReliableUnordered
                        );
        }

        internal void AskHostToAnimateObjectTap(IPeer host)
        {
            _networking.SendDataToPeer (
                (uint)_MessageType.AskAnimateObjectTapMessage,
                new byte[1], // Empty byte
                host,
                TransportType.ReliableUnordered
            );
        }

        internal void AskHostToAnimateObjectDistance(IPeer host)
        {
            _networking.SendDataToPeer (
                (uint)_MessageType.AskAnimateObjectDistanceMessage,
                new byte[1], // Empty byte
                host,
                TransportType.ReliableUnordered
            );
        }
        internal void AskHostToDrawNewLine(IPeer host, int fingerId, Vector3 touchPosition, Vector3 circlePos, float circleScale)
        {
            _networking.SendDataToPeer(
                (uint)_MessageType.AskHostDrawLine,
                SerializeUIntVector3Vector3Float(Convert.ToUInt16(fingerId),touchPosition,circlePos,circleScale),
                host,
                TransportType.ReliableUnordered
            );
        }

        internal void BroadcastLookInteractiveActive(int index)
        {
            _networking.BroadcastData(
                            (uint)_MessageType.BroadcastLookInteractiveActive,
                            SerializeUint(Convert.ToUInt16(index)),
                            TransportType.UnreliableUnordered
                        );
        }

        internal void AskLookInteractiveActive(IPeer host, int index)
        {
            _networking.SendDataToPeer(
                                        (uint)_MessageType.AskLookInteractiveActive,
                                        SerializeUint(Convert.ToUInt16(index)),
                                        host,
                                        TransportType.ReliableUnordered
                                    );
        }

        internal void AskHostToAddToLine(IPeer host, int fingerId, Vector3 touchPosition)
        {
            _networking.SendDataToPeer(
                            (uint)_MessageType.AskHostAddLine,
                            SerializeUIntVector3(Convert.ToUInt16(fingerId), touchPosition),
                            host,
                            TransportType.ReliableUnordered
                        );
        }


        internal void AskHostToStopLine(IPeer host, int fingerId)
        {
            _networking.SendDataToPeer(
                                        (uint)_MessageType.AskHostStopLine,
                                        SerializeUint(Convert.ToUInt16(fingerId)),
                                        host,
                                        TransportType.ReliableUnordered
                                    );
        }        

        internal void BroadcastObjectPosition(Vector3 position) 
        {
            _networking.BroadcastData (
                (uint)_MessageType.ObjectPositionMessage,
                SerializeVector3(position),
                TransportType.UnreliableUnordered
            );
        }
        internal void BroadcastInteractiveObjectPosition(int i, Vector3 position)
        {
            _networking.BroadcastData(
                (uint)_MessageType.InteractiveObjectPositionMessage,
                SerializeUIntVector3(Convert.ToUInt16(i), position),
                TransportType.UnreliableUnordered
            );

        }
        internal void BroadcastObjectScale(Vector3 scale) 
        {
            _networking.BroadcastData (
                (uint)_MessageType.ObjectScaleMessage,
                SerializeVector3(scale),
                TransportType.UnreliableUnordered
            );
        }

        internal void BroadcastObjectRotation(Quaternion rotation) 
        {
            _networking.BroadcastData (
                (uint)_MessageType.ObjectRotationMessage,
                SerializeQuaternion(rotation),
                TransportType.UnreliableUnordered
            );
        }
        internal void BroadcastInteractiveObjectRotation(int i, Quaternion rotation)
        {
            _networking.BroadcastData(
               (uint)_MessageType.InteractiveObjectRotationMessage,
               SerializeUIntQuaternion(Convert.ToUInt16(i), rotation),
               TransportType.UnreliableUnordered
           );
        }        

        internal void BroadcastTriggerObject(int index)
        {
            _networking.BroadcastData(
                (uint)_MessageType.TriggerInteractive,
                SerializeUint(Convert.ToUInt16(index)),
                TransportType.UnreliableUnordered
            );
        }
        internal void BroadcastDrawNewLine(int fingerId, Vector3 touchPosition, Vector3 circlePos, float circleScale)
        {
            _networking.BroadcastData(
                            (uint)_MessageType.DrawNewLineMessage,
                            SerializeUIntVector3Vector3Float(Convert.ToUInt16(fingerId), touchPosition, circlePos, circleScale),
                            TransportType.UnreliableUnordered
                        );
        }

        

        internal void BroadcastAddToLine(int fingerId, Vector3 touchPosition)
        {
            _networking.BroadcastData(
                            (uint)_MessageType.AddToLineMessage,
                            SerializeUIntVector3(Convert.ToUInt16(fingerId), touchPosition),
                            TransportType.UnreliableUnordered
                        );
        }

        

        internal void BroadcastStopLine(int fingerId)
        {
            _networking.BroadcastData(
                           (uint)_MessageType.StopLineMessage,
                           SerializeUint(Convert.ToUInt16(fingerId)),
                           TransportType.UnreliableUnordered
                       );
        }

        private void OnDidReceiveDataFromPeer(PeerDataReceivedArgs args) 
        {
            var data = args.CopyData();
            switch ((_MessageType)args.Tag) 
            {   
                case _MessageType.AskMoveObjectMessage:
                    _controller.SharedObjectHolder.MoveObject(DeserializeVector3(data));
                    break;
                
                case _MessageType.AskAnimateObjectTapMessage:
                    _controller.SharedObjectHolder.ObjectInteraction.AnimateObjectTap();
                    break;
                
                case _MessageType.AskAnimateObjectDistanceMessage:
                    _controller.SharedObjectHolder.ObjectInteraction.AnimateObjectDistance();
                    break;
                case _MessageType.AskTriggerInteractive:
                    _controller.TriggerInteractiveObject(DeserializeUint(data));
                    break;

                case _MessageType.ObjectPositionMessage:
                    _controller.SetObjectPosition(DeserializeVector3(data));
                    break;

                case _MessageType.ObjectScaleMessage:
                    _controller.SetObjectScale(DeserializeVector3(data));
                    break;

                case _MessageType.ObjectRotationMessage:
                    _controller.SetObjectRotation(DeserializeQuaternion(data));
                    break;
                case _MessageType.TriggerInteractive:
                    _controller.TriggerInteractiveObject(DeserializeUint(data));
                    break;
                case _MessageType.AskHostDrawLine:
                case _MessageType.DrawNewLineMessage:
                    ushort index;
                    Vector3 touchPos;
                    Vector3 circlePos;
                    float circleScale;
                    DeserializeUintVector3Vector3Float(data, out index, out touchPos, out circlePos, out circleScale);
                    _controller._arDrawManager.DrawNewLine(index, touchPos, circlePos, circleScale);
                    break;
                case _MessageType.AskHostAddLine:
                case _MessageType.AddToLineMessage:
                    ushort index2;
                    Vector3 touchPos2;
                    DeserializeUintVector3(data, out index2, out touchPos2);
                    _controller._arDrawManager.AddToLine(index2, touchPos2);
                    break;
                case _MessageType.AskHostStopLine:
                case _MessageType.StopLineMessage:
                    _controller._arDrawManager.StopLine(DeserializeUint(data));
                    break;
                case _MessageType.InteractiveObjectPositionMessage:
                    ushort index3;
                    Vector3 pos3;
                    DeserializeUintVector3(data, out index3, out pos3);
                    _controller.SetInteractiveObjectPosition(index3, pos3);
                    break;
                case _MessageType.InteractiveObjectRotationMessage:
                    ushort index4;
                    Quaternion rot;
                    DeserializeUintQuaternion(data, out index4, out rot);
                    _controller.SetInteractiveObjectRotation(index4,rot);
                    break;
                case _MessageType.AskPhase:
                case _MessageType.PhaseMessage:
                    _controller.SetGamePhase(DeserializeUint(data));
                    break;
                case _MessageType.BroadcastLookInteractiveActive:
                case _MessageType.AskLookInteractiveActive:
                    _controller.ActivateLookInteractive(DeserializeUint(data));
                    break;
                default:
                    throw new ArgumentException("Received unknown tag from message");
            }
        }

        

        internal void Destroy() 
        {
            _networking.PeerDataReceived -= OnDidReceiveDataFromPeer;
        }

        private byte[] SerializeVector3(Vector3 vector) 
        {
            _builderMemoryStream.Position = 0;
            _builderMemoryStream.SetLength(0);

            using (var binarySerializer = new BinarySerializer(_builderMemoryStream))
                Vector3Serializer.Instance.Serialize(binarySerializer, vector);

                return _builderMemoryStream.ToArray();
        }

        private Vector3 DeserializeVector3(byte[] data) 
        {
            using(var readingStream = new MemoryStream(data))
                using (var binaryDeserializer = new BinaryDeserializer(readingStream))
                    return Vector3Serializer.Instance.Deserialize(binaryDeserializer);
        }
        private byte[] SerializeUint(UInt16 i)
        {
            _builderMemoryStream.Position = 0;
            _builderMemoryStream.SetLength(0);
            using (var binarySerializer = new BinarySerializer(_builderMemoryStream))
                UInt16Serializer.Instance.Serialize(binarySerializer, i);
            return _builderMemoryStream.ToArray();
        }
        private UInt16 DeserializeUint(byte[] data)
        {
            using (var readingStream = new MemoryStream(data))
            using (var binaryDeserializer = new BinaryDeserializer(readingStream))
                return UInt16Serializer.Instance.Deserialize(binaryDeserializer);
        }

        private byte[] SerializeQuaternion(Quaternion quat) 
        {
            _builderMemoryStream.Position = 0;
            _builderMemoryStream.SetLength(0);

            using (var binarySerializer = new BinarySerializer(_builderMemoryStream))
                QuaternionSerializer.Instance.Serialize(binarySerializer, quat);

                return _builderMemoryStream.ToArray();
        }
        private byte[] SerializeUIntQuaternion(ushort i, Quaternion rotation)
        {
            using (var stream = new MemoryStream())
            {
                using (var serializer = new BinarySerializer(stream))
                {
                    serializer.Serialize(i);
                    serializer.Serialize(rotation);
                    return stream.ToArray();
                }
            }
        }
        private void DeserializeUintQuaternion(byte[] data, out UInt16 index, out Quaternion rotation)
        {
            using (var stream = new MemoryStream(data))
            {
                using (var deserializer = new BinaryDeserializer(stream))
                {
                    index = (ushort)deserializer.Deserialize();
                    rotation = (Quaternion)deserializer.Deserialize();
                    // The number and order of the Deserialize() calls should match the Serialize() calls.
                }
            }
        }

        private Quaternion DeserializeQuaternion(byte[] data) 
        {
            using(var readingStream = new MemoryStream(data))
                using (var binaryDeserializer = new BinaryDeserializer(readingStream))
                    return QuaternionSerializer.Instance.Deserialize(binaryDeserializer);
        }
        
        private byte[] SerializeUIntVector3Vector3Float(ushort v, Vector3 touchPosition, Vector3 circlePos, float circleScale)
        {
            using (var stream = new MemoryStream())
            {
                using (var serializer = new BinarySerializer(stream))
                {
                    serializer.Serialize(v);
                    serializer.Serialize(touchPosition);
                    serializer.Serialize(circlePos);
                    serializer.Serialize(circleScale);
                    return stream.ToArray();
                }
            }
        }

        private byte[] SerializeUIntVector3(ushort v, Vector3 touchPosition)
        {
            using (var stream = new MemoryStream())
            {
                using (var serializer = new BinarySerializer(stream))
                {
                    serializer.Serialize(v);
                    serializer.Serialize(touchPosition);                    
                    return stream.ToArray();
                }
            }
        }
        private void DeserializeUintVector3(byte[] data, out UInt16 index, out Vector3 touchPosition)
        {
            using (var stream = new MemoryStream(data))
            {
                using (var deserializer = new BinaryDeserializer(stream))
                {
                    index = (ushort)deserializer.Deserialize();
                    touchPosition = (Vector3)deserializer.Deserialize();                   
                    // The number and order of the Deserialize() calls should match the Serialize() calls.
                }
            }
        }

        private void DeserializeUintVector3Vector3Float(byte[] data, out UInt16 index, out Vector3 touchPosition, out Vector3 circlePos, out float circleScale)
        {
            using (var stream = new MemoryStream(data))
            {
                using (var deserializer = new BinaryDeserializer(stream))
                {
                    index = (ushort)deserializer.Deserialize(); 
                    touchPosition = (Vector3)deserializer.Deserialize();
                    circlePos = (Vector3)deserializer.Deserialize();
                    circleScale = (float)deserializer.Deserialize();
                }
            }
        }
    }
}
