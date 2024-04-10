using Firestore.ORM.Reflect;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Firestore.ORM.Extensions
{
    public static class DocumentExtensions
    {
        public static async Task<bool> Exists(this Query query)
        {
            var res = await query.Limit(1).GetSnapshotAsync();
            return res.Count > 0;
        }
        public static async Task Insert<T>(this T item) where T : FirestoreDocument
        {
            await item.Reference.CreateAsync(FirestoreManager.Instance.ToFirestore(item));
        }
        public static async Task Update<T>(this T item) where T : FirestoreDocument
        {
            await item.Reference.SetAsync(FirestoreManager.Instance.ToFirestore(item), SetOptions.MergeAll);
        }
        public static async Task Delete<T>(this T item) where T : FirestoreDocument
        {
            await item.Reference.DeleteAsync();
        }
        public static async Task<List<T>> GetAsync<T>(this Query query) where T : FirestoreDocument
        {
            return await FirestoreManager.Instance.GetAsync<T>(query);
        }

        public static List<T> Get<T>(this Query query) where T : FirestoreDocument
        {
            return FirestoreManager.Instance.Get<T>(query);
        }
    }
}
