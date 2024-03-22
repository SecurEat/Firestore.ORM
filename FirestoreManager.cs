using Firestore.ORM.Attributes;
using Firestore.ORM.Extensions;
using Firestore.ORM.Logging;
using Firestore.ORM.Logging.Incidents;
using Firestore.ORM.Reflect;
using Firestore.ORM.Utils;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Google.Type;
using System.Collections;
using System.Reflection;
using DateTime = System.DateTime;

namespace Firestore.ORM
{
    public class FirestoreManager : Singleton<FirestoreManager>
    {
        private Dictionary<Type, CollectionDefinition> Definitions
        {
            get;
            set;
        }
        private FirestoreDb FirestoreDb
        {
            get;
            set;
        }

        private Assembly TargetAssembly
        {
            get;
            set;
        }

        public MappingBehavior MappingBehavior
        {
            get;
            private set;
        }
        public void Initialize(string projectId, FirestoreClient client, Assembly assembly,
            MappingBehavior mappingBehavior = MappingBehavior.Strict)
        {
            FirestoreDb = FirestoreDb.Create(projectId, client);
            TargetAssembly = assembly;
            MappingBehavior = mappingBehavior;
            BuildDefinitions();

        }


        private void BuildDefinitions()
        {
            Definitions = new Dictionary<Type, CollectionDefinition>();

            foreach (var type in TargetAssembly.GetTypes())
            {
                if (typeof(FirestoreDocument).IsAssignableFrom(type) && !type.IsAbstract)
                {
                    Dictionary<FieldAttribute, PropertyInfo> dict = new Dictionary<FieldAttribute, PropertyInfo>();

                    var properties = type.GetProperties();

                    foreach (var property in properties)
                    {
                        FieldAttribute attribute = property.GetCustomAttribute<FieldAttribute>()!;

                        if (attribute != null)
                        {
                            dict.Add(attribute, property);
                        }
                    }

                    Definitions.Add(type, new CollectionDefinition(type, dict));

                }
            }
        }

        public CollectionReference Collection(string path)
        {
            return FirestoreDb.Collection(path);
        }
        public CollectionDefinition CollectionDefinition(Type type)
        {
            return Definitions[type];
        }

        public async Task<List<T>> Get<T>(Query query) where T : FirestoreDocument
        {
            List<T> result = new List<T>();
            var snapshot = await query.GetSnapshotAsync();

            foreach (var item in snapshot)
            {
                result.Add(FromFirestore<T>(item.Reference, item.ToDictionary()));
            }

            return result;
        }
        public async Task<T> Get<T>(DocumentReference reference) where T : FirestoreDocument
        {
            var snapshot = await reference.GetSnapshotAsync();
            return FromFirestore<T>(reference, snapshot.ToDictionary());
        }
        private IList ConvertList(List<object> value, Type targetType)
        {
            if (value == null)
            {
                return null;
            }


            if (targetType == typeof(string))
            {
                return value.Select(x => x.ToString()).ToList();
            }

            if (targetType == typeof(int))
            {
                return value.Select(x => (int)Convert.ChangeType(x, typeof(int))).ToList();
            }
            if (targetType == typeof(DocumentReference))
            {
                return value.Select(x => (DocumentReference)x).ToList();
            }

            return value;
        }
        public T FromFirestore<T>(DocumentReference reference, Dictionary<string, object> values) where T : FirestoreDocument
        {
            Type type = typeof(T);

            var definition = Definitions[type];
            var obj = (T)Activator.CreateInstance(type, new object[] { reference })!;

            if (obj is DefaultFirestoreDocument defaultFirestoreDocument)
            {
                defaultFirestoreDocument.Data = values;
                return obj;
            }

            foreach (var key in definition.Properties.Keys)
            {
                PropertyInfo propertyInfo = definition.Properties[key];

                var propertyType = propertyInfo.PropertyType;

                Type? underlyingType = Nullable.GetUnderlyingType(propertyType);

                if (underlyingType != null)
                {
                    propertyType = underlyingType;
                }

                if (!values.ContainsKey(key.Name) || values[key.Name] == null)
                {
                    if (key.Nullability == FieldNullability.NonNullable) //&& propertyInfo.GetValue(obj) == null)
                    {
                        obj.Incidents.Add(new MissingFieldIncident(obj, propertyInfo, key));
                    }
                    continue;
                }

                var value = values[key.Name];

                try
                {
                    if (propertyType.IsGenericType &&
                        propertyType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                    {
                        // nothing todo
                    }
                    else if (propertyType.IsListType())
                    {
                        var collectionType = propertyType.GenericTypeArguments[0];


                        if (!(value is IList))
                        {
                            obj.Incidents.Add(new InvalidFieldTypeIncident(obj, propertyInfo, value));
                            continue;
                        }
                        value = ConvertList((List<object>)value, collectionType);
                    }
                    else if (propertyType == typeof(DateTime))
                    {
                        value = ((Timestamp)value).ToDateTime();
                    }
                    else if (propertyType == typeof(int))
                    {
                        value = Convert.ChangeType(value, typeof(int));
                    }

                    propertyInfo.SetValue(obj, value);
                }
                catch (Exception ex)
                {
                    obj.Incidents.Add(new InvalidFieldTypeIncident(obj, propertyInfo, value));

                }
            }

            foreach (var incident in obj.Incidents)
            {
                IncidentManager.DeclareIncident(incident);
            }

            return obj;
        }

        public Dictionary<string, object> ToFirestoreCompiled<T>(T item)
        {
            var type = item.GetType();

            var definition = Definitions[type];
            var func = definition.Converter;
            Dictionary<string, object> result = func(item);
            return result;
        }
        public Dictionary<string, object> ToFirestore(FirestoreDocument item)
        {
            Dictionary<string, object?> result = new Dictionary<string, object?>();

            var type = item.GetType();

            var definition = Definitions[type];

            foreach (var pair in definition.Properties)
            {
                result.Add(pair.Key.Name, pair.Value.GetValue(item));
            }
            return result;
        }
        public FirestoreDb GetFirestoreDb()
        {
            return FirestoreDb;
        }
    }
}