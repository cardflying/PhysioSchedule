using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseSystem : MonoBehaviour
{
    private FirebaseFirestore firestore;
    private int appointmentIndex = -1;

    public async UniTask Init()
    {
        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (dependencyStatus == DependencyStatus.Available)
        {
            firestore = FirebaseFirestore.DefaultInstance;
            //Debug.Log("Firebase Firestore initialized");
        }
        else
        {
            Debug.LogError("Firebase dependencies not resolved");
        }
    }

    /// <summary>
    /// save client data to cloud firestore
    /// </summary>
    /// <param name="clientData"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Get all client data from cloud firestore
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Update existing client data in cloud firestore
    /// </summary>
    /// <param name="clientData"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Book appointment to cloud firestore
    /// </summary>
    /// <param name="appointmentData"></param>
    /// <returns></returns>
    public async UniTask BookAppointment(AppointmentData appointmentData)
    {
        try
        {
            Query query = firestore.Collection("appointment_data");
            
            // 2. Save client data (auto ID)
            DocumentReference docRef = firestore.Collection("appointment_data").Document((appointmentIndex++).ToString());

            await docRef.SetAsync(appointmentData);

            //Debug.Log("Client data saved successfully");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("SaveClientDataToCloud failed: " + ex);
        }
    }

    /// <summary>
    /// cancel appointment from cloud firestore
    /// </summary>
    /// <param name="appointmentData"></param>
    /// <returns></returns>
    public async UniTask CancelAppointment(AppointmentData appointmentData)
    {
        try
        {
            Query query = firestore.Collection("appointment_data")
                                   .WhereEqualTo("IC", appointmentData.IC)
                                   .WhereEqualTo("Date", appointmentData.Date); // Ensure correct type

            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            foreach (var doc in snapshot.Documents)
            {
                AppointmentData data = doc.ConvertTo<AppointmentData>();
                Debug.Log($"Deleting document {doc.Id} with IC: {data.IC} and date:{data.Date}");

                await doc.Reference.DeleteAsync();
                Debug.Log($"Deleted document {doc}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Firestore delete failed: {e}");
        }
    }

    /// <summary>
    /// Get appointment list from cloud firestore based on date
    /// </summary>
    /// <param name="appointmentData"></param>
    /// <returns></returns>
    public async UniTask<List<AppointmentData>> GetAppointmentList(AppointmentData appointmentData)
    {
        List<AppointmentData> appointment = new List<AppointmentData>();

        try
        {
            DateTime day = appointmentData.Date.ToDateTime();
            DateTime start = day.ToUniversalTime();
            DateTime end = start.AddDays(1);
            //Debug.Log(start + " "+ end);
            Query query = firestore.Collection("appointment_data")
                                   //.WhereEqualTo("Date", appointmentData.Date);
                                   .WhereGreaterThanOrEqualTo("Date", start)
                                   .WhereLessThan("Date", end);
            
            QuerySnapshot snapshot = await query.GetSnapshotAsync();


            foreach (DocumentSnapshot doc in snapshot.Documents)
            {
                if (!doc.Exists) continue;

                AppointmentData data = doc.ConvertTo<AppointmentData>();
                appointment.Add(data);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Firestore delete failed: {e}");
        }

        return appointment;
    }

    /// <summary>
    /// Get appointment list from cloud firestore based on month
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public async UniTask<List<AppointmentData>> GetAppointmentList(DateTime date)
    {
        List<AppointmentData> appointment = new List<AppointmentData>();

        try
        {
            DateTime startLocal = new DateTime(date.Year, date.Month, 1, 0, 0, 0, DateTimeKind.Local);

            DateTime endLocal = startLocal.AddMonths(1);

            Query query = firestore.Collection("appointment_data")
            .WhereGreaterThanOrEqualTo("Date", Timestamp.FromDateTime(startLocal.ToUniversalTime()))
            .WhereLessThan("Date",Timestamp.FromDateTime(endLocal.ToUniversalTime()));

            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            foreach (DocumentSnapshot doc in snapshot.Documents)
            {
                if (!doc.Exists) continue;

                AppointmentData data = doc.ConvertTo<AppointmentData>();
                appointment.Add(data);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Firestore delete failed: {e}");
        }

        return appointment;
    }
}
