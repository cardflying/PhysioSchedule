using Firebase.Firestore;
using System;

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
    private string EmergencyName = "Emergency";
    private int EmergencyNumber = 987654321;
    private string EmergencyRelationship = "Friend";

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