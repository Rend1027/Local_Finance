using System;
using Firebase.Firestore;

[Serializable]
[FirestoreData]
public class TransactionModel
{
    [FirestoreProperty] public string Title { get; set; }
    [FirestoreProperty] public string Description { get; set; }
    [FirestoreProperty] public float Amount { get; set; }
    [FirestoreProperty] public bool IsIncome { get; set; }
    [FirestoreProperty] public long Timestamp { get; set; }
    [FirestoreProperty] public string Category { get; set; }
}
