using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseSystem : MonoBehaviour
{
    private FirebaseFirestore firestore;
    private int totalClient = 0;

    private void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                firestore = FirebaseFirestore.DefaultInstance;
                //Debug.Log("Firebase Firestore initialized");
            }
            else
            {
                Debug.LogError("Firebase dependencies not resolved");
            }
        });
    }

    public async UniTask SaveClientDataToCloud(ClientData clientData)
    {
        try
        {
            Query query = firestore.Collection("client_data");
            AggregateQuery countQuery = query.Count;

            AggregateQuerySnapshot snapshot = await countQuery.GetSnapshotAsync(AggregateSource.Server);
            long totalPlayers = snapshot.Count;

            // 2. Save client data (auto ID)
            DocumentReference docRef = firestore.Collection("client_data/").Document((totalPlayers++).ToString());

            await docRef.SetAsync(clientData);

            //Debug.Log("Client data saved successfully");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("SaveClientDataToCloud failed: " + ex);
        }
    }

    public async UniTask<List<ClientData>> LoadClientDataFromCloud()
    {
        List<ClientData> players = new List<ClientData>();

        try
        {
            QuerySnapshot snapshot = await firestore.Collection("client_data").GetSnapshotAsync();

            foreach (DocumentSnapshot doc in snapshot.Documents)
            {
                if (!doc.Exists) continue;

                ClientData data = doc.ConvertTo<ClientData>();
                players.Add(data);
            }

            //Debug.Log($"Loaded {players.Count} players");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to load players: " + ex);
        }

        return players;
    }

    public async UniTask UpdateClientDataInCloud(ClientData clientData)
    {
        try
        {
            Query query = firestore.Collection("client_data").WhereEqualTo("IC", clientData.IC);
            
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            if (snapshot.Count > 0)
            {
                DocumentReference clientDoc = null;
                using (var enumerator = snapshot.Documents.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        clientDoc = enumerator.Current.Reference;
                    }
                }
                if (clientDoc != null)
                {
                    await clientDoc.SetAsync(clientData, SetOptions.MergeAll);
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("SaveClientDataToCloud failed: " + ex);
        }
    }
}
