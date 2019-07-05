using System;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace SagaDemo.Common.DataAccess.RavenDb.Utilities
{
    public static class DocumentIdHelper
    {
        public static string GetDocumentId<TEntity>(IAsyncDocumentSession session, string id)
        {
            // TODO: Consider definig this for DocumentStore instead of session
            var separator = session.Advanced.DocumentStore.Conventions.IdentityPartsSeparator;
            var collectionName = session.Advanced.DocumentStore.Conventions.GetCollectionName(typeof(TEntity));

            return $"{collectionName}{separator}{id}";
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
                    $"An invalid document Id has been encountered while trying to convert the document identifier to entity identifier.\n" +
                    $"A valid document identifier must start with '{prefix}'.\n" +
                    $"The attempted value was '{documentId}'.");
            }

            return documentId.Substring(prefixLength);
        }

        public static string GetEntityId<TEntity>(IDocumentStore documentStore, string documentId)
        {
            var separator = documentStore.Conventions.IdentityPartsSeparator;
            var collectionName = documentStore.Conventions.GetCollectionName(typeof(TEntity));

            var prefix = collectionName + separator;
            var prefixLength = prefix.Length;

            if (!documentId.StartsWith(prefix))
            {
                throw new ArgumentException(
                    $"An invalid document Id has been encountered while trying to convert the document identifier to entity identifier.\n" +
                    $"A valid document identifier must start with '{prefix}'.\n" +
                    $"The attempted value was '{documentId}'.");
            }

            return documentId.Substring(prefixLength);
        }
    }
}