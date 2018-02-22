namespace Messaging_Library.Models
{
    public class FolderModels
    {
        /// <summary>
        /// Structure for Folder object
        /// </summary>
        public class FolderRequest
        {
            public int FolderId { get; set; }
            public string FolderName { get; set; }
            public int FolderType { get; set; }
            public string FolderTypeDescription { get; set; }
            public bool IsSystemFolder { get; set; }
            public int TotalMessages { get; set; }
            public int TotalSize { get; set; }
        }

        /// <summary>
        /// Classes associated with folders
        /// </summary>
        public class Folder
        {
            /// <summary>
            /// Create an array (Folders) of type Folder which has the data types of the payload being received
            /// </summary>
            public FolderRequest[] Folders { get; set; }
        }

        public class FolderResponse
        {
            public int FolderId { get; set; }
        }

    }
}
