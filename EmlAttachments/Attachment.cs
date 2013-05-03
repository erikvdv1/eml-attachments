namespace EmlAttachments
{
    /// <summary>
    /// A class for representing an attachment.
    /// </summary>
    public class Attachment
    {
        /// <summary>
        /// The file name of the attachment.
        /// </summary>
        public string FileName { private set; get; }

        /// <summary>
        /// The raw contents of the attachment.
        /// </summary>
        public byte[] Content { private set; get; }

        /// <summary>
        /// Creates a new attachment with the specified file name and contents.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <param name="content">The raw content.</param>
        public Attachment(string fileName, byte[] content)
        {
            FileName = fileName;
            Content = content;
        }
    }
}
