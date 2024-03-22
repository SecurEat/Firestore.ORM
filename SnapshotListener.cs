using Google.Api;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firestore.ORM
{
    public class SnapshotListener<T> where T : FirestoreDocument
    {
        public delegate void OnChangeDelegate(T? oldItem, T? item, DocumentChange.Type type);

        public delegate void OnItemsUpdatedDelegate(T[] items);

        public event OnChangeDelegate? OnItemChange;

        public event OnItemsUpdatedDelegate? OnItemsUpdated;

        private Query Query
        {
            get;
            set;
        }
        public T[] Items
        {
            get
            {
                return m_items_safe.ToArray();
            }
        }

        private List<T> m_items_safe;

        private List<T> m_items;

        private bool TriggerEvents
        {
            get;
            set;
        }

        private List<Action> Events
        {
            get;
            set;
        }
        public SnapshotListener(Query query)
        {
            this.Query = query;
            this.m_items = new List<T>();
            this.m_items_safe = new List<T>();
            this.OnItemChange = null;
            this.OnItemsUpdated = null;
            Events = new List<Action>();
        }
        public void DispatchEvents()
        {
            foreach (var evt in Events)
            {
                evt();
            }

            Events.Clear();

            TriggerEvents = true;
        }

        public async Task FetchAndListenAsync(bool triggerEvents = true)
        {
            TriggerEvents = triggerEvents;

            var tcs = new TaskCompletionSource<bool>();


            var listener = Query.Listen((QuerySnapshot snap) =>
            {
                var oldItems = m_items.ToArray();
                m_items.Clear();

                foreach (var snapshot in snap)
                {
                    var dictionary = snapshot.ToDictionary();
                    T? element = FirestoreManager.Instance.FromFirestore<T>(snapshot.Reference, dictionary);

                    m_items.Add(element);
                }

                if (OnItemChange != null)
                {
                    foreach (var change in snap.Changes)
                    {
                        T? oldItem = null;
                        T? newItem = null;

                        if (change.NewIndex.HasValue)
                        {
                            newItem = m_items[change.NewIndex!.Value];
                        }

                        if (change.OldIndex.HasValue)
                        {
                            oldItem = oldItems[change.OldIndex.Value];
                        }



                        if (newItem == null || newItem.Incidents.Count == 0)
                        {
                            if (TriggerEvents)
                            {
                                OnItemChange.Invoke(oldItem, newItem, change.ChangeType);
                            }
                            else
                            {
                                Events.Add(() => { OnItemChange.Invoke(oldItem, newItem, change.ChangeType); });
                            }
                        }
                    }
                }

                m_items_safe = m_items.Where(x => x.Incidents.Count == 0).ToList();
                if (!tcs.Task.IsCompleted)
                {
                    tcs.SetResult(true);
                }

                if (TriggerEvents)
                {
                    OnItemsUpdated?.Invoke(Items);
                }
                else
                {
                    Events.Add(() => { OnItemsUpdated?.Invoke(Items); });
                }
            });

            await tcs.Task;
        }


        public T? GetById(string id)
        {
            return Items.FirstOrDefault(x => x.Id == id);
        }
        public T[] GetItemsWithIncidentsOnly()
        {
            return m_items.Where(x => x.Incidents.Count > 0).ToArray();
        }
        public T[] GetAllItems()
        {
            return m_items.ToArray();
        }
    }
}
