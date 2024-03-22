# Firestore.ORM

Firestore.ORM is a lightweight object-relational mapping (ORM) product for [Firestore](https://firebase.google.com/) : It provides an additional layer of abstraction on top of the existing Firestore SDK. (Google.Cloud.Firestore).

It allows you to structure the data contained in Firebase and to ensure its itegrity. It also provides utility methods encapsulating the SDK functions in order to simplify writing and reading, serialization and synchronization.

[![NuGet latest version](https://badgen.net/nuget/v/Firestore.ORM/latest)](https://www.nuget.org/packages/Firestore.ORM/) https://www.nuget.org/packages/Firestore.ORM/

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

## Connecting to Firebase

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

Incident management allows you to verify that the data present in Firebase respects the structure expected by the model. It ensures the structuring of data

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

## Package Dependencies

| Name                                                 |
| ---------------------------------------------------- |
| Marius Lumbroso _Author_ (https://github.com/Skinz3) |
