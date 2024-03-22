# Firestore.ORM

Firestore.ORM is a lightweight object-relational mapping (ORM) product for [Firestore](https://firebase.google.com/) : It provides an additional layer of abstraction on top of the existing Firestore SDK. (Google.Cloud.Firestore).

One of the main advantages of this project is to provide a solid model structure base in order to ensure the integrity of the data present in Firestore. It also provides utility methods encapsulating the SDK functions in order to simplify writing and reading, serialization and synchronization.

[![NuGet latest version](https://badgen.net/nuget/v/Firestore.ORM/latest)](https://www.nuget.org/packages/Firestore.ORM/)

- https://www.nuget.org/packages/Firestore.ORM/

## Model

- All class from the model must derive from the type `Firestore.ORM.FirestoreDocument`. This abstract class is wrapping data provided by the SDK (such as `Google.Cloud.Firestore.DocumentReference`) and additional data provided by the ORM, such as a list of corruption errors if the data obtained from Firebase does not match the structure expected by the model.

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

## Connecting to Firebase

The initialization function establishes the connection with Firebase and introspects the code to discover the model.

```csharp
Initialize(string projectId, FirestoreClient client, Assembly assembly,
            MappingBehavior mappingBehavior = MappingBehavior.Strict)
```

The 'assembly' parameter indicates the assembly where the model is located. 'mappingBehavior' corresponds to the level of tolerance that the ORM adopts when remote data structure is not valid compared to the model. In the case of strict tolerance, the ORM throws if any data is corrupted.

```csharp

var client = new FirestoreClientBuilder
{
    CredentialsPath = "credentials.json"
}.Build();

FirestoreManager.Instance.Initialize("my-project", client, Assembly.GetExecutingAssembly());


```

## Manipulating data

### Basic fetch

```csharp
IEnumerable<User> userCollection = await FirestoreManager.Instance.Collection("users").GetAsync<User>();
```

### Listeners

```csharp
CollectionReference collection = FirestoreManager.Instance.Collection("users");

SnapshotListener<User> userListener = new SnapshotListener<User>();
userListener.OnItemChange += ...
userListener.OnItemsUpdated += ...

/*
  First fetch the collection and start to listen to the changes
*/
await userListener.FetchAndListen();

/*
  Provide a snapshot of the collection,
  internal collection inside the listener is still in real time
*/

List<User> users = userListener.GetItems().ToList();

```

### Writing

```csharp
User? user = users.Find(x=> x.Firstname == "John");

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

Incident management allows you to verify that the data present in Firebase respects the structure expected by the model. It ensures the structuring of data.
This feature is only available with mappingBehavior : 'MappingBehavior.Souple'.

```csharp

IncidentManager.OnIncident += OnIncident;

static void OnIncident(Incident incident)
{
      switch (incident)
      {
          case MissingFieldIncident missingFieldIncident:
              // Solve missing field on document
              break;
          case InvalidFieldTypeIncident invalidFieldTypeIncident:
               // Solve wrong field type on document
              break;
      }
}


```

## Contributors

| Name                                                 |
| ---------------------------------------------------- |
| Marius Lumbroso _Author_ (https://github.com/Skinz3) |
