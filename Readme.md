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
    public string Phone
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

## Manipulating data

```csharp

// Basic fetch

IEnumerable<User> userCollection = await FirestoreManager.Instance.Collection("users").GetAsync<User>();


// Fetch using listeners

CollectionReference collection = FirestoreManager.Instance.Collection("users");

SnapshotListener<User> userListener = new SnapshotListener<User>();
userListener.OnItemChange += ...
userListener.OnItemsUpdated += ...
await userListener.FetchAndListen();

List<User> realtimeUsers = userListener.GetItems().ToList();

// Write operations (extension methods)

User? user = realtimeUsers.Find(x=> x.Firstname == "John");

user.Firstname = "Peter";

user.Update();

if (user.Phone.Contains("+33"))
{
    user.Delete();
}

User newUser = new User(collection.Document());

newUser.Name = "Maria";

newUser.Insert();


```

## Data sanitize, incident report

```csharp
...
IncidentManager.OnIncident += OnIncident;

static void OnIncident(Incident incident)
{
      switch (incident)
      {
          case MissingFieldIncident missingFieldIncident:
              // Solve missing field on document incident
              break;
          case InvalidFieldTypeIncident invalidFieldTypeIncident:
               // Solve wrong field type on document
              break;
      }
}


```

## Package Dependencies

| Name                                                 |
| ---------------------------------------------------- |
| Marius Lumbroso _Author_ (https://github.com/Skinz3) |
