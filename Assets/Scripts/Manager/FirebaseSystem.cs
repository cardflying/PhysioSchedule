using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseSystem : MonoBehaviour
{
    private FirebaseFirestore firestore;
    private FirebaseAuth firebaseAuth;
    private DocumentReference docRef;

    public async UniTask Init()
    {
        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (dependencyStatus != DependencyStatus.Available)
        {
            Debug.LogError($"Firebase dependencies not resolved: {dependencyStatus}");
            return;
        }

        firestore = FirebaseFirestore.DefaultInstance;

        firebaseAuth = FirebaseAuth.DefaultInstance;

        if (firebaseAuth.CurrentUser == null)
        {
            try
            {
                await firebaseAuth.SignInAnonymouslyAsync();
                Debug.Log("Signed in anonymously");
            }
            catch (Exception e)
            {
                Debug.LogError($"Anonymous sign-in failed: {e}");
                return;
            }
        }
        
        var user = firebaseAuth.CurrentUser;
        //Debug.Log($"Firebase UID: {user.UserId}");
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
            if (firebaseAuth == null || firestore == null)
                throw new Exception("Firebase not initialized");

            if (firebaseAuth.CurrentUser == null)
                throw new Exception("User not authenticated");

            await firestore.Collection("client_data").AddAsync(clientData);
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
                data.DocumentId = doc.Id;
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
            if (string.IsNullOrEmpty(clientData.DocumentId))
            {
                Debug.LogError("Cannot update client data: DocumentId is missing.");
                return;
            }

            // Directly update using DocumentId
            await firestore
                .Collection("client_data")
                .Document(clientData.DocumentId)
                .SetAsync(clientData, SetOptions.MergeAll);

            Debug.Log($"Updated client data: {clientData.DocumentId}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("UpdateClientDataInCloud failed: " + ex);
        }
    }

    /// <summary>
    /// Book appointment to cloud firestore
    /// </summary>
    /// <param name="appointmentData"></param>
    /// <returns></returns>
    public async UniTask<string> BookAppointment(AppointmentData appointmentData)
    {
        try
        {
            docRef = firestore.Collection("appointment_data").Document();

            await docRef.SetAsync(appointmentData);

            //Debug.Log("Client data saved successfully");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("SaveClientDataToCloud failed: " + ex);
        }

        return docRef.Id;
    }

    /// <summary>
    /// cancel appointment from cloud firestore
    /// </summary>
    /// <param name="appointmentData"></param>
    /// <returns></returns>
    public async UniTask CancelAppointment(AppointmentData appointmentData)
    {
        if (string.IsNullOrEmpty(appointmentData.DocumentId))
        {
            Debug.LogError("CancelAppointment failed: DocumentId is null or empty");
            return;
        }

        try
        {
            await firestore.Collection("appointment_data").Document(appointmentData.DocumentId).DeleteAsync();
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
                data.DocumentId = doc.Id;
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
        if (firebaseAuth.CurrentUser == null)
            await firebaseAuth.SignInAnonymouslyAsync();

        List<AppointmentData> appointment = new List<AppointmentData>();

        DateTime startLocal = new DateTime(date.Year, date.Month, 1, 0, 0, 0, DateTimeKind.Local);
        DateTime endLocal = startLocal.AddMonths(1);

        Query query = firestore.Collection("appointment_data")
            .WhereGreaterThanOrEqualTo("Date", Timestamp.FromDateTime(startLocal.ToUniversalTime()))
            .WhereLessThan("Date", Timestamp.FromDateTime(endLocal.ToUniversalTime()));

        QuerySnapshot snapshot = await query.GetSnapshotAsync();

        foreach (var doc in snapshot.Documents)
            if (doc.Exists) appointment.Add(doc.ConvertTo<AppointmentData>());

        return appointment;
    }

    /// <summary>
    /// Copy collection from source to target for firestore
    /// </summary>
    /// <param name="sourceCollection"></param>
    /// <param name="targetCollection"></param>
    /// <returns></returns>
    public async UniTask CopyCollection(string sourceCollection, string targetCollection)
    {
        QuerySnapshot snapshot =
            await firestore.Collection(sourceCollection).GetSnapshotAsync();

        WriteBatch batch = firestore.StartBatch();

        foreach (DocumentSnapshot doc in snapshot.Documents)
        {
            DocumentReference targetDoc =
                firestore.Collection(targetCollection).Document(doc.Id);

            batch.Set(targetDoc, doc.ToDictionary());
        }

        await batch.CommitAsync();
    }
}
