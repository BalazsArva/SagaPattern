using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Client.Documents.Session;

namespace SagaDemo.Common.DataAccess.RavenDb.Utilities
{
    public static class DocumentIdHelper
    {
        public static string GetDocumentId<TEntity>(IAsyncDocumentSession session, string id)
        {
            var separator = session.Advanced.DocumentStore.Conventions.IdentityPartsSeparator;
            var collectionName = session.Advanced.DocumentStore.Conventions.GetCollectionName(typeof(TEntity));

            return $"{collectionName}{separator}{id}";
        }

        public static IEnumerable<string> GetDocumentIds<TEntity>(IAsyncDocumentSession session, IEnumerable<string> ids)
        {
            var separator = session.Advanced.DocumentStore.Conventions.IdentityPartsSeparator;
            var collectionName = session.Advanced.DocumentStore.Conventions.GetCollectionName(typeof(TEntity));

            return ids.Select(id => $"{collectionName}{separator}{id}");
        }

        public static string GetEntityId<TEntity>(IAsyncDocumentSession session, string documentId)
        {
            var separator = session.Advanced.DocumentStore.Conventions.IdentityPartsSeparator;
            var collectionName = session.Advanced.DocumentStore.Conventions.GetCollectionName(typeof(TEntity));

            var prefix = collectionName + separator;
            var prefixLength = prefix.Length;

            if (!documentId.StartsWith(prefix))
            {
                throw new ArgumentException(
                    $"An invalid document Id has been encountered while trying to convert the document identifier to numeric identifies.\n" +
                    $"A valid document identifier must start with '{prefix}' to be considered valid.\n" +
                    $"The attempted value was '{documentId}'.");
            }

            return documentId.Substring(prefixLength);
        }
    }
}