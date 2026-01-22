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
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
            {
                return;
            }
            
            transforms.Clear();
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

        [Rpc(SendTo.Everyone)]
        protected void ResetTransformRpc()
        {
            foreach (var obj in objects)
            {
                if (obj == null || !transforms.TryGetValue(obj, out var data))
                {
                    continue;
                }
                
                var (pos, scale, rotation) = data;
                
                obj.GetComponent<Collider>().enabled = false;
                
                obj.SetPosition(pos);
                obj.SetRotation(rotation);
                obj.transform.localScale = scale;
            }
            
            Invoke(nameof(EnableColliders), 2f);
        }

        private void EnableColliders()
        {
            foreach (var obj in objects)
            {
                obj.GetComponent<Collider>().enabled = true;
            }
        }
    }
}