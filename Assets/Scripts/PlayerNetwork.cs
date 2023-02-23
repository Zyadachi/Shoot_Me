using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
 [SerializeField] private Transform spawnedObjectPrefab;
 private Transform spawnedObjectTransform;
  public override void OnNetworkSpawn()
  {
        randomNumer.OnValueChanged += (MyCustomData previousValue, MyCustomData newValue) => 
        {
            Debug.Log(OwnerClientId +"; " + newValue._int + "; " + newValue._bool + "; " + newValue.message);
        };
  }
  private NetworkVariable<MyCustomData> randomNumer = new NetworkVariable<MyCustomData>(new MyCustomData {
    _int = 56,
    _bool = true,
  }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
  
  public struct MyCustomData : INetworkSerializable
  {
    public int _int;
    public bool _bool;
    public FixedString128Bytes message;

        public void NetworkSerialize<G>(BufferSerializer<G> serializer) where G : IReaderWriter
        {
            serializer.SerializeValue(ref _int);
            serializer.SerializeValue(ref _bool);
            serializer.SerializeValue(ref message);
        }
    }
  private void Update()
  {
    if(!IsOwner) return;

    if (Input.GetKeyDown(KeyCode.G))
    {
        spawnedObjectTransform = Instantiate(spawnedObjectPrefab);
        spawnedObjectTransform.GetComponent<NetworkObject>().Spawn(true);
        //TestClientRpc(new ClientRpcParams{Send = new ClientRpcSendParams {TargetClientIds = new List<ulong>{1}}});
        /*
        randomNumer.Value = new MyCustomData {
          _int = 10,
          _bool = false,
          message = "Doigrasz się!",
        };
        */
    }

    if (Input.GetKeyDown(KeyCode.H))
    {
      spawnedObjectTransform.GetComponent<NetworkObject>().Despawn(true);
      Destroy(spawnedObjectTransform.gameObject);
    }

    Vector3 moveDir = new Vector3(0,0,0);

    if(Input.GetKey(KeyCode.W)) moveDir.z = +1f;
    if(Input.GetKey(KeyCode.S)) moveDir.z = -1f;
    if(Input.GetKey(KeyCode.A)) moveDir.x = -1f;
    if(Input.GetKey(KeyCode.D)) moveDir.x = +1f;

    float moveSpeed = 3f;
    transform.position += moveDir * moveSpeed * Time.deltaTime;
  }
 /* [ServerRpc]
  private void TestServerRpc(ServerRpcParams serverRpcParams)
  {
      Debug.Log("TestServerRpc" + OwnerClientId + "; " + serverRpcParams.Receive.SenderClientId);
  }

  [ClientRpc]
  private void TestClientRpc(ClientRpcParams clientRpcParams)
  {
      Debug.Log("TestClientRpc");
  }*/
}
