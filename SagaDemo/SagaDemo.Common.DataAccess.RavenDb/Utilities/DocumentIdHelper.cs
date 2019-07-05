using Raven.Client.Documents;

namespace SagaDemo.Common.DataAccess.RavenDb.Utilities
{
    public static class DocumentIdHelper
    {
        public static string GetDocumentId<TEntity>(IDocumentStore documentStore, string entityId)
        {
            var collectionName = documentStore.Conventions.GetCollectionName(typeof(TEntity));
            var separator = documentStore.Conventions.IdentityPartsSeparator;

            var prefix = collectionName + separator;

            if (entityId.StartsWith(prefix))
            {
                return entityId;
            }

            return prefix + entityId;
        }

        public static string GetEntityId<TEntity>(IDocumentStore documentStore, string documentId)
        {
            var separator = documentStore.Conventions.IdentityPartsSeparator;
            var collectionName = documentStore.Conventions.GetCollectionName(typeof(TEntity));

            var prefix = collectionName + separator;
            var prefixLength = prefix.Length;

            if (!documentId.StartsWith(prefix))
            {
                return documentId;
            }

            return documentId.Substring(prefixLength);
        }
    }
}