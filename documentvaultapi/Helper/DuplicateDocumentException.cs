namespace documentvaultapi.Helper
{
    public class DuplicateDocumentException : Exception
    {
        public Guid ExistingDocumentId { get; }

        public DuplicateDocumentException(Guid documentId)
            : base($"Duplicate document detected. Use this Document Id : {documentId}.")
        {
            ExistingDocumentId = documentId;
        }
    }
}
