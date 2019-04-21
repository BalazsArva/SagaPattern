using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Raven.Client.Documents.Session;

namespace SagaDemo.InventoryAPI.Utilities.Helpers
{
    public static class DocumentIdHelper
    {
        private static readonly NumberFormatInfo InvariantNumberFormat = CultureInfo.InvariantCulture.NumberFormat;

        public static string GetDocumentId<TEntity>(IAsyncDocumentSession session, int id)
        {
            var separator = session.Advanced.DocumentStore.Conventions.IdentityPartsSeparator;
            var collectionName = session.Advanced.DocumentStore.Conventions.GetCollectionName(typeof(TEntity));
            var idString = id.ToString(InvariantNumberFormat);

            return $"{collectionName}{separator}{idString}";
        }

        public static IEnumerable<string> GetDocumentIds<TEntity>(IAsyncDocumentSession session, IEnumerable<int> ids)
        {
            var separator = session.Advanced.DocumentStore.Conventions.IdentityPartsSeparator;
            var collectionName = session.Advanced.DocumentStore.Conventions.GetCollectionName(typeof(TEntity));

            return ids
                .Select(id => id.ToString(InvariantNumberFormat))
                .Select(id => $"{collectionName}{separator}{id}");
        }

        public static int GetEntityId<TEntity>(IAsyncDocumentSession session, string documentId)
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

            var unprefixedId = documentId.Substring(prefixLength);
            if (int.TryParse(unprefixedId, NumberStyles.Integer, InvariantNumberFormat, out var numericId))
            {
                return numericId;
            }
            else
            {
                throw new ArgumentException(
                    $"An invalid document Id has been encountered while trying to convert the document identifier to numeric identifier.\n" +
                    $"A valid document identifier must not contain any non-numeric characters after the collection name and identity parts separator ('{prefix}').\n" +
                    $"The attempted value was '{documentId}'.");
            }
        }

        public static IEnumerable<int> GetEntityIds<TEntity>(IAsyncDocumentSession session, IEnumerable<string> documentIds)
        {
            var separator = session.Advanced.DocumentStore.Conventions.IdentityPartsSeparator;
            var collectionName = session.Advanced.DocumentStore.Conventions.GetCollectionName(typeof(TEntity));

            var prefix = collectionName + separator;
            var prefixLength = prefix.Length;

            foreach (var documentId in documentIds)
            {
                if (!documentId.StartsWith(prefix))
                {
                    throw new ArgumentException(
                        $"An invalid document Id has been encountered while trying to convert document identifiers to numeric identifiers.\n" +
                        $"A valid document identifier must start with '{prefix}' to be considered valid.\n" +
                        $"The attempted value was '{documentId}'.");
                }

                var unprefixedId = documentId.Substring(prefixLength);
                if (int.TryParse(unprefixedId, NumberStyles.Integer, InvariantNumberFormat, out var numericId))
                {
                    yield return numericId;
                }
                else
                {
                    throw new ArgumentException(
                        $"An invalid document Id has been encountered while trying to convert document identifiers to numeric identifiers.\n" +
                        $"A valid document identifier must not contain any non-numeric characters after the collection name and identity parts separator ('{prefix}').\n" +
                        $"The attempted value was '{documentId}'.");
                }
            }
        }
    }
}