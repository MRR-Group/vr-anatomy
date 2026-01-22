using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

namespace XRMultiplayer
{
    public class NetworkInteractableResetManager : NetworkBehaviour
    {
        [SerializeField]
        private List<NetworkRigidbody> objects;

        private Dictionary<NetworkRigidbody, (Vector3, Vector3, Quaternion)> transforms;
        
        private void Start()
        {
            objects ??=  new List<NetworkRigidbody>();
            transforms ??= new Dictionary<NetworkRigidbody, (Vector3, Vector3, Quaternion)>();
            
            SaveTransforms();
        }

        private void SaveTransforms()
        {
            foreach (var  obj in objects)
            {
                transforms.Add(obj, (obj.transform.position, obj.transform.localScale, obj.transform.rotation));
            }
        }

        public void ResetTransforms()
        {
            ResetTransformRpc();
        }

        [Rpc(SendTo.Server)]
        protected void ResetTransformRpc()
        {
            foreach (var obj in objects)
            {
                var (pos, scale, rotation) = transforms[obj];
                
                obj.SetPosition(pos);
                obj.SetRotation(rotation);
                obj.transform.localScale = scale;
            }
        }
    }
}