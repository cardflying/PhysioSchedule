using Firebase.Firestore;
using System;
using System.Collections.Generic;
using UnityEngine;

[FirestoreData,Serializable]
public class ClientData
{
    public string DocumentId { get; set; }

    private string UserName = "test";
    private int UserAge = 100;
    private string UserIC = "ABC123";
    private string Occupation = "Warrior";
    private string DateOfBirth;
    private int UserGender = 0;
    private string MaritalStatus = "Single";
    private string Nationality = "Unknown";
    private string HandphoneNumber = "1234567890";
    private string EmailAddress = "";
    private string ResidentialAddress = "";
    private int Language = 0;
    private int UserCondition = 0;
    private string EmergencyName = "Emergency";
    private string EmergencyNumber = "987654321";
    private string EmergencyRelationship = "Friend";
    private int UserSession = 0;
    private string UserSessionNote = "";

    [FirestoreProperty("name")]
    public string Name { get => UserName; set => UserName = value; }

    [FirestoreProperty("age")]
    public int Age { get => UserAge; set => UserAge = value; }

    [FirestoreProperty("ic")]
    public string IC { get => UserIC; set => UserIC = value; }

    [FirestoreProperty("job")]
    public string Job { get => Occupation; set => Occupation = value; }

    [FirestoreProperty("dob")]
    public string DOB { get => DateOfBirth; set => DateOfBirth = value; }

    [FirestoreProperty("gender")]
    public int Gender { get => UserGender; set => UserGender = value; }

    [FirestoreProperty("status")]
    public string Status { get => MaritalStatus; set => MaritalStatus = value; }

    [FirestoreProperty("country")]
    public string Country { get => Nationality; set => Nationality = value; }

    [FirestoreProperty("phone")]
    public string Phone { get => HandphoneNumber; set => HandphoneNumber = value; }

    [FirestoreProperty("email")]
    public string Email { get => EmailAddress; set => EmailAddress = value; }

    [FirestoreProperty("address")]
    public string Address { get => ResidentialAddress; set => ResidentialAddress = value; }

    [FirestoreProperty("language")]
    public int LanguageCode { get => Language; set => Language = value; }

    [FirestoreProperty("condition")]
    public int Condition { get => UserCondition; set => UserCondition = value; }

    [FirestoreProperty("session")]
    public int Session { get => UserSession; set => UserSession = value; }

    [FirestoreProperty("sessionNote")]
    public string SessionNote { get => UserSessionNote; set => UserSessionNote = value; }

    [FirestoreProperty("emergencyName")]
    public string EmergencyContactName { get => EmergencyName; set => EmergencyName = value; }

    [FirestoreProperty("emergencyNumber")]
    public string EmergencyContactNumber { get => EmergencyNumber; set => EmergencyNumber = value; }

    [FirestoreProperty("emergencyRelationship")]
    public string EmergencyContactRelationship
    {
        get => EmergencyRelationship;
        set => EmergencyRelationship = value;
    }
}


[FirestoreData, Serializable]
public class AppointmentData
{
    public string DocumentId { get; set; }
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