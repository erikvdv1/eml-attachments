using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace EmlAttachments
{
    /// <summary>
    /// A class for representing an email (eml file).
    /// </summary>
    class Email
    {
        /// <summary>
        /// The original file name of the .eml file.
        /// </summary>
        public string FileName { private set; get; }

        /// <summary>
        /// The boundary string of the email.
        /// </summary>
        public string Boundary
        {
            get { return _boundary ?? (_boundary = GetBoundary()); }
        }

        /// <summary>
        /// The text content of the email.
        /// </summary>
        public string Content
        {
            get { return _content ?? (_content = GetContent()); }
        }

        /// <summary>
        /// The attachments contained in the email.
        /// </summary>
        public IEnumerable<Attachment> Attachments
        {
            get { return _attachments ?? (_attachments = GetAttachments()); }
        }

        /// <summary>
        /// A private variable thats holds the attachments.
        /// </summary>
        private IEnumerable<Attachment> _attachments;

        /// <summary>
        /// A private variable thats holds the boudary string.
        /// </summary>
        private string _boundary;

        /// <summary>
        /// A private variable thats holds the content.
        /// </summary>
        private string _content;

        /// <summary>
        /// Creates a new email object from the specified .eml file.
        /// </summary>
        /// <param name="file">The path to the .eml file.</param>
        public Email(string file)
        {
            // Check if file exists.
            if (!File.Exists(file))
                throw new FileNotFoundException("File not found", file);

            FileName = file;
        }

        /// <summary>
        /// Tries to find the boundary string in the .eml file.
        /// </summary>
        /// <returns>The boundary string if found, otherwise null.</returns>
        private string GetBoundary()
        {
            Regex regex = new Regex("boundary=\"(.*?)\"", RegexOptions.None);
            Match match = regex.Match(Content);
            if (match.Success)
                return match.Groups[1].Value;
            return null;
        }

        /// <summary>
        /// Gets the text content of the .eml file.
        /// </summary>
        /// <returns>The text content.</returns>
        private string GetContent()
        {
            return File.ReadAllText(FileName);
        }

        /// <summary>
        /// Tries to find the attachments encoded as Base64 in the .eml file.
        /// </summary>
        /// <returns>The list of attachments found.</returns>
        private IEnumerable<Attachment> GetAttachments()
        {
            IList<Attachment> attachments = new List<Attachment>();

            // Check if we have a valid boundary
            if(Boundary == null)
                return attachments;

            // Split email content on boundary
            string[] parts = Content.Split(new [] { Boundary }, StringSplitOptions.None);

            // Parse each part
            foreach (var part in parts)
            {
                // Split on two new line chars to distinguish header and content
                string[] headerAndContent = part.Trim(new [] {'\r', '\n', '-', ' '})
                    .Split(new [] { Environment.NewLine + Environment.NewLine }, StringSplitOptions.None);

                // Not a valid split
                if (headerAndContent.Length != 2)
                    continue;

                // Valid header and content
                string header = headerAndContent[0];
                string content = headerAndContent[1];

                // Look for a valid file name string
                Regex regex = new Regex("filename=\"(.*?)\"");
                Match match = regex.Match(header);
                if(!match.Success)
                    continue;

                string fileName = match.Groups[1].Value;
                byte[] raw;

                try
                {
                    // Try to convert the Base64 content to bytes
                    raw = Convert.FromBase64String(content);
                }
                catch (Exception)
                {
                    continue;
                }

                // Successful conversion, add attachment to the list
                attachments.Add(new Attachment(fileName, raw));
            }

            // Return all attachments found
            return attachments;
        }

        /// <summary>
        /// Saves all attachments of this email to the output directory.
        /// </summary>
        /// <param name="outputDirectory">The output directory.</param>
        /// <returns>The number of files saved.</returns>
        public int SaveAttachments(string outputDirectory)
        {
            // Keep track of total number attachments
            int count = 0;

            // Extract each attachment
            foreach (var attachment in Attachments)
            {
                // Write bytes to output file
                string path = Path.Combine(outputDirectory, attachment.FileName);
                File.WriteAllBytes(path, attachment.Content);
                count++;
            }

            // Return count
            return count;
        }
    }
}
