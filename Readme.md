# Firestore.ORM

Firestore.ORM is a lightweight object-relational mapping (ORM) product for [Firestore](https://firebase.google.com/) : It provides an additional layer of abstraction on top of the existing Firestore SDK. (Google.Cloud.Firestore).

It allows you to structure the data contained in Firebase and to ensure its itegrity.

[![NuGet latest version](https://badgen.net/nuget/v/Flex/latest)](https://www.nuget.org/packages/Flex/)

# Exemple

## Model

```csharp

  public class User : FirestoreDocument
  {
    public User(DocumentReference reference) : base(reference)
    {

    }

    [Field("email")]
    public string Email
    {
        get;
        set;
    }

    [Field("firstname")]
    public string Firstname
    {
        get;
        set;
    }

    [Field("phone", FieldNullability.Nullable)]
    public string Name
    {
        get;
        set;
    }


    [Field("role")]
    public int Role
    {
        get;
        set;
    } = 1;

    [Field("claims")]
    public List<DocumentReference> Claims
    {
      get;
      set;
    }


}
```

## Connecting to firebase

```csharp

var client = new FirestoreClientBuilder
{
    CredentialsPath = "credentials.json"
}.Build();

FirestoreManager.Instance.Initialize("my-project", client, Assembly.GetExecutingAssembly());


```

## Fetching data

```csharp

// Basic fetch

IEnumerable<User> userCollection = await FirestoreManager.Instance.Collection("users").GetAsync<User>();


// Fetch using listeners

SnapshotListener<User> userListener = new SnapshotListener<User>(FirestoreManager.Instance.Collection("users"));
userListener.OnItemChange += ...
userListener.OnItemsUpdated += ...
await userListener.FetchAndListen();

IEnumerable<User> realtimeUsers = userListener.GetItems();


```

# Package Dependencies

| Name                                                 |
| ---------------------------------------------------- |
| Marius Lumbroso (Author) (https://github.com/Skinz3) |
