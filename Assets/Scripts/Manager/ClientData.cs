using Firebase.Firestore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

[FirestoreData,Serializable]
public class ClientData
{
    private string UserName = "test";
    private int UserAge = 100;
    private string UserIC = "ABC123";
    private string Occupation = "Warrior";
    private string DateOfBirth;
    private Int32 UserGender = 0;
    private string MaritalStatus = "Single";
    private string Nationality = "Unknown";
    private int HandphoneNumber = 1234567890;
    private string EmailAddress = "";
    private string ResidentialAddress = "";
    private Int32 Langauge = 0;
    private Int32 UserCondition = 0;
    private string EmergencyName = "Emergency";
    private int EmergencyNumber = 987654321;
    private string EmergencyRelationship = "Friend";
    private int UserSession = 0;
    private string UserSessionNote = "";

    [FirestoreProperty]
    public string Name { get { return UserName; } set { UserName = value; } }
    [FirestoreProperty]
    public int Age { get { return UserAge; } set { UserAge = value; } }
    [FirestoreProperty]
    public string IC { get { return UserIC; } set { UserIC = value; } }
    [FirestoreProperty]
    public string Job { get { return Occupation; } set { Occupation = value; } }
    [FirestoreProperty]
    public string DOB { get { return DateOfBirth; } set { DateOfBirth = value; } }
    [FirestoreProperty]
    public Int32 Gender { get { return UserGender; } set { UserGender = value; } }
    [FirestoreProperty]
    public string Status { get { return MaritalStatus; } set { MaritalStatus = value; } }
    [FirestoreProperty]
    public string Country { get { return Nationality; } set { Nationality = value; } }
    [FirestoreProperty]
    public int Phone { get { return HandphoneNumber; } set { HandphoneNumber = value; } }
    [FirestoreProperty]
    public string Email { get { return EmailAddress; } set { EmailAddress = value; } }
    [FirestoreProperty]
    public string Address { get { return ResidentialAddress; } set { ResidentialAddress = value; } }
    [FirestoreProperty]
    public Int32 Language { get { return Langauge; } set { Langauge = value; } }
    [FirestoreProperty]
    public Int32 Condition { get { return UserCondition; } set { UserCondition = value; } }
    [FirestoreProperty]
    public int Session { get { return UserSession; } set { UserSession = value; } }
    [FirestoreProperty]
    public string SessionNote { get { return UserSessionNote; } set { UserSessionNote = value; } }
    [FirestoreProperty]
    public string EmergencyContactName { get { return EmergencyName; } set { EmergencyName = value; } }
    [FirestoreProperty]
    public int EmergencyContactNumber { get { return EmergencyNumber; } set { EmergencyNumber = value; } }
    [FirestoreProperty]
    public string EmergencyContactRelationship { get { return EmergencyRelationship; } set { EmergencyRelationship = value; } }
}


[FirestoreData, Serializable]
public class AppointmentData
{
    private Timestamp AppointmentDate;
    private string UserIC = "ABC1";

    [FirestoreProperty]
    public Timestamp Date { get { return AppointmentDate; } set { AppointmentDate = value; } }
    [FirestoreProperty]
    public string IC { get { return UserIC; } set { UserIC = value; } }
}

[Serializable]
public class Note
{
    public int id;
    public string noteText;
    public StrokeData strokeData;
}


[Serializable]
public class StrokeData
{
    public string color;                    // HEX
    public float thickness;
    public List<Vec3Data> linePoints = new();

    public List<Vector3> GetLinePoint()
    {
        List<Vector3> newLinePoints = new();

        for (int i = 0; i < linePoints.Count; i++)
        {
            newLinePoints.Add(new Vector3(linePoints[i].x, linePoints[i].y, linePoints[i].z));
        }

        return newLinePoints;
    }
}

[Serializable]
public struct Vec3Data
{
    public float x;
    public float y;
    public float z;

    public Vec3Data(Vector3 v)
    {
        x = v.x;
        y = v.y;
        z = v.z;
    }

    public Vector3 ToVector3() => new Vector3(x, y, z);
}

public enum UserCondition
{
    MSK = 0,
    NEURO
}