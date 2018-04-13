using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Messaging_Library.Models;
using System.Net;
using System.Text;
using MimeKit;

namespace DMWeb_REST
{
    /// <summary>
    /// Class functions
    /// </summary>
    public class DMWeb
    { 
        public static string _baseUrl = "";
        public static string _sessionKey = "";
        

        public MessagingModels.SendMessage sendMessagePayload = new MessagingModels.SendMessage();
        public List<string> _base64 = new List<string>();

        public DMAccount Account = new DMAccount();
        public DMFolders Folders = new DMFolders();
        public DMMessage Message = new DMMessage();


        /// <summary>
        /// Default constructor that sets the _baseUrl to SecureMail
        /// </summary>
        public DMWeb()
        {
            _baseUrl = "https://ssl.datamotion.com/SecureMessagingApi";
        }

        /// <summary>
        /// Non-default constructor that allows the host URL to be changed
        /// </summary>
        /// <param name="url">The string of the destination URL</param>
        public DMWeb(string url)
        {
            _baseUrl = url;
        }
        public class DMAccount
        {
            /// <summary>
            /// Retrieve a sessionkey for the user
            /// </summary>
            /// <param name="model">Model of type AccountLogOn contains string UserIdOrEmail and string Password</param>
            /// <returns>HttpResponseMessage deserialized into AccountSessionKey object</returns>
            public string LogOn(AccountModels.LogOn model)
            {
                string jsonString = JsonConvert.SerializeObject(model);
                byte[] jsonByteArray = Encoding.UTF8.GetBytes(jsonString);

                WebClient client = new WebClient();
                client.Headers.Add("Content-Type", "application/json");

                try
                {
                    string response = Encoding.UTF8.GetString(client.UploadData(_baseUrl + "/Account/Logon", "POST", jsonByteArray));

                    AccountModels.AccountSessionKey temp = JsonConvert.DeserializeObject<AccountModels.AccountSessionKey>(response);
                    _sessionKey = temp.SessionKey;

                    client.Headers.Add("X-Session-Key", _sessionKey);

                    return temp.SessionKey;
                }
                catch (WebException ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// Displays the account details to the user
            /// </summary>
            /// <returns>HttpResponseMessage deserialized into AccountResponses object</returns>
            public AccountModels.AccountDetails Details()
            {
                WebClient client = new WebClient();
                client.Headers.Add("Content-Type", "application/json");
                client.Headers.Add("X-Session-Key", _sessionKey);

                try
                {
                    string response = client.DownloadString(_baseUrl + "/Account/Details");
                    AccountModels.AccountDetails temp = JsonConvert.DeserializeObject<AccountModels.AccountDetails>(response);
                    return temp;
                }
                catch(WebException ex)
                {
                    throw ex;  
                }
            }

            /// <summary>
            /// Allows user to change their account's password
            /// </summary>
            /// <param name="model">Model of type AccountChangePassword contains string OldPassword and string NewPassword</param>
            /// <returns>HttpResponseMessage</returns>
            public string ChangePassword(AccountModels.ChangePassword model)
            {
                string jsonString = JsonConvert.SerializeObject(model);
                byte[] jsonByteArray = Encoding.UTF8.GetBytes(jsonString);

                WebClient client = new WebClient();
                client.Headers.Add("Content-Type", "application/json");
                client.Headers.Add("X-Session-Key", _sessionKey);

                try
                {
                    string response = Encoding.UTF8.GetString(client.UploadData(_baseUrl + "/Account/ChangePassword", "POST", jsonByteArray));

                    return response;
                }
                catch (WebException ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// Removes the Session Key when the user chooses to log out and invalidates the supplied SessionKey on our Saas
            /// </summary>
            /// <returns>HttpResponseMessage</returns>
            public string LogOut()
            {
                string jsonString = JsonConvert.SerializeObject("");
                byte[] jsonByteArray = Encoding.UTF8.GetBytes(jsonString);

                WebClient client = new WebClient();

                client.Headers.Add("Content-Type", "application/json");
                client.Headers.Add("X-Session-Key", _sessionKey);

                try
                {
                    string response = Encoding.UTF8.GetString(client.UploadData(_baseUrl + "/Account/Logout", "POST", jsonByteArray));
                    client.Headers.Remove("X-Session-Key");
                    _sessionKey = "";

                    return response;
                }
                catch (WebException ex)
                {
                    throw ex;
                }
            }
        }

        public class DMFolders
        {
            /// <summary>
            /// Displays the details of a folder
            /// </summary>
            /// <returns>HttpResponseMessage deserialized into FolderResponses object</returns>
            public FolderModels.Folder List()
            {
                WebClient client = new WebClient();
                client.Headers.Add("Content-Type", "application/json");
                client.Headers.Add("X-Session-Key", _sessionKey);

                try
                {
                    string response = client.DownloadString(_baseUrl + "/Folder/List");
                    FolderModels.Folder folderResponse = JsonConvert.DeserializeObject<FolderModels.Folder>(response);

                    return folderResponse;
                }
                catch (WebException ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// Creates a folder of the chosen type and name by the user
            /// </summary>
            /// <param name="model">Model of type Folder contains string FolderName and int FolderType</param>
            /// <returns>FolderID as a string</returns>
            public string Create(FolderModels.Create model)
            {
                string jsonString = JsonConvert.SerializeObject(model);
                byte[] jsonByteArray = Encoding.UTF8.GetBytes(jsonString);

                WebClient client = new WebClient();
                client.Headers.Add("Content-Type", "application/json");
                client.Headers.Add("X-Session-Key", _sessionKey);

                try
                {
                    string response = Encoding.UTF8.GetString(client.UploadData(_baseUrl + "/Folder", jsonByteArray));
                    FolderModels.FolderResponse fid = JsonConvert.DeserializeObject<FolderModels.FolderResponse>(response);

                    return fid.FolderId.ToString();
                }

                catch (WebException ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// Deletes a folder of the user's choice
            /// </summary>
            /// <param name="FolderID">string FolderId</param>
            /// <returns>HttpResponseMessage</returns>
            public string Delete(string FolderID)
            {
                string jsonString = JsonConvert.SerializeObject("");
                byte[] jsonByteArray = Encoding.UTF8.GetBytes(jsonString);

                WebClient client = new WebClient();
                client.Headers.Add("Content-Type", "application/json");
                client.Headers.Add("X-Session-Key", _sessionKey);

                try
                {
                    string response = Encoding.UTF8.GetString(client.UploadData(_baseUrl + "/Folder/" + FolderID, "DELETE", jsonByteArray));

                    return response;
                }
                catch (WebException ex)
                {
                    throw ex;
                }
            }
        }

        public class DMMessage
        {
            /// <summary>
            /// Displays the MessageIds in the inbox
            /// </summary>
            /// <param name="model"></param>
            /// <returns>GetInboxMIDResponse object</returns>
            public MessagingModels.GetInboxMIDResponse GetInboxMessageIds(MessagingModels.GetInboxMIDRequest model)
            {
                string jsonString = JsonConvert.SerializeObject(model);
                byte[] jsonByteArray = Encoding.UTF8.GetBytes(jsonString);

                WebClient client = new WebClient();
                client.Headers.Add("Content-Type", "application/json");
                client.Headers.Add("X-Session-Key", _sessionKey);

                try
                {
                    string response = Encoding.UTF8.GetString(client.UploadData(_baseUrl + "/Message/GetInboxMessageIds", "POST", jsonByteArray));

                    MessagingModels.GetInboxMIDResponse inboxResponse = JsonConvert.DeserializeObject<MessagingModels.GetInboxMIDResponse>(response);

                    return inboxResponse;
                }
                catch(WebException ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// Displays the MessageSummaries of a message
            /// </summary>
            /// <param name="model">Model of type GetMessageSummariesRequest contains int FolderId and int LastMessageIDReceived</param>
            /// <returns>HttpResponseMessage deserialized into SummariesResponseBody object</returns>
            public MessagingModels.GetMessageSummaries GetMessageSummaries(MessagingModels.GetMessageSummariesRequest model)
            {
                string jsonString = JsonConvert.SerializeObject(model);
                byte[] jsonByteArray = Encoding.UTF8.GetBytes(jsonString);

                WebClient client = new WebClient();
                client.Headers.Add("Content-Type", "application/json");
                client.Headers.Add("X-Session-Key", _sessionKey);

                try
                {
                    string response = Encoding.UTF8.GetString(client.UploadData(_baseUrl + "/Message/GetMessageSummaries", "POST", jsonByteArray));

                    MessagingModels.GetMessageSummaries summariesResponse = JsonConvert.DeserializeObject<MessagingModels.GetMessageSummaries>(response);

                    return summariesResponse;
                }
                catch(WebException ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// Displays the unread messages for a user
            /// </summary>
            /// <param name="LastMIDReceived">Receive only messages created since the ID reference</param>
            /// <param name="MID">MessageID</param>
            /// <returns>GetMessageSummariesResponse object</returns>
            public MessagingModels.GetUnreadMessages GetUnreadMessages(bool LastMIDReceived, string MID)
            {
                WebClient client = new WebClient();
                client.Headers.Add("Content-Type", "application/json");
                client.Headers.Add("X-Session-Key", _sessionKey);


                if (LastMIDReceived == false)
                {
                    try
                    {
                        string response = client.DownloadString(_baseUrl + "/Message/Inbox/Unread");

                        MessagingModels.GetUnreadMessages unreadResponse = JsonConvert.DeserializeObject<MessagingModels.GetUnreadMessages>(response);

                        return unreadResponse;
                    }
                    catch(WebException ex)
                    {
                        throw ex;
                    }
                }
                else
                {
                    try
                    {
                        string response = client.DownloadString(_baseUrl + "/Message/Inbox/Unread/?After=" + MID);

                        MessagingModels.GetUnreadMessages unreadResponse = JsonConvert.DeserializeObject<MessagingModels.GetUnreadMessages>(response);

                        return unreadResponse;
                    }
                    catch(WebException ex)
                    {
                        throw ex;
                    }
                }
            }

            /// <summary>
            /// Searches the user's inbox, based on filter parameters
            /// </summary>
            /// <param name="model"></param>
            /// <returns>searchInboxResponse object</returns>
            public MessagingModels.SearchInboxResponse SearchInbox(MessagingModels.SearchInbox model)
            {
                string jsonString = JsonConvert.SerializeObject(model);
                byte[] jsonByteArray = Encoding.UTF8.GetBytes(jsonString);

                WebClient client = new WebClient();
                client.Headers.Add("Content-Type", "application/json");
                client.Headers.Add("X-Session-Key", _sessionKey);

                try
                {
                    string response = Encoding.UTF8.GetString(client.UploadData(_baseUrl + "/Message/Inbox/Search", "POST", jsonByteArray));

                    MessagingModels.SearchInboxResponse searchInboxResponseObject = JsonConvert.DeserializeObject<MessagingModels.SearchInboxResponse>(response);

                    return searchInboxResponseObject;
                }
                catch (WebException ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// Gets the metadata of a selected message
            /// </summary>
            /// <param name="MessageId"></param>
            /// <returns>MetadataResponse object</returns>
            public MessagingModels.MetadataResponse GetMessageMetadata(string MessageId)
            {
                WebClient client = new WebClient();
                client.Headers.Add("Content-Type", "application/json");
                client.Headers.Add("X-Session-Key", _sessionKey);
                
                try
                {
                    string response = client.DownloadString(_baseUrl + "/Message/" + MessageId + "/Metadata");

                    MessagingModels.MetadataResponse messageMetadata = JsonConvert.DeserializeObject<MessagingModels.MetadataResponse>(response);

                    return messageMetadata;
                }
                catch(WebException ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// Used to retract a message
            /// </summary>
            /// <param name="model">Model contains string messageId</param>
            /// <returns>HttpResponseMessage(null if successful) </returns>
            public string Retract(MessagingModels.MessageOperations model)
            {
                string jsonString = JsonConvert.SerializeObject(model);
                byte[] jsonByteArray = Encoding.UTF8.GetBytes(jsonString);

                WebClient client = new WebClient();
                client.Headers.Add("Content-Type", "application/json");
                client.Headers.Add("X-Session-Key", _sessionKey);

                try
                {
                    string messageId = model.MessageId.ToString();

                    string response = Encoding.UTF8.GetString(client.UploadData(_baseUrl + "/Message/" + messageId + "/Retract", "POST", jsonByteArray));

                    return response;
                }
                catch (WebException ex)
                {
                    throw ex;
                }
            } 

            /// <summary>
            /// Used to move a message
            /// </summary>
            /// <param name="model">Model contains string messageId</param>
            /// <returns>HttpResponseMessage(null if successful)</returns>
            public string Move(MessagingModels.MessageOperations model)
            {
                string jsonString = JsonConvert.SerializeObject(model);
                byte[] jsonByteArray = Encoding.UTF8.GetBytes(jsonString);

                WebClient client = new WebClient();
                client.Headers.Add("Content-Type", "application/json");
                client.Headers.Add("X-Session-Key", _sessionKey);

                try
                {
                    string messageId = model.MessageId.ToString();

                    string response = Encoding.UTF8.GetString(client.UploadData(_baseUrl + "/Message/" + messageId + "/Move/", "POST", jsonByteArray));

                    return response;
                }
                catch (WebException ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// Used to send a message
            /// </summary>
            /// <param name="model">Model contains multiple parameters</param>
            /// <returns>MessageID as an integer</returns>
            public int Send(MessagingModels.SendMessage model)
            {
                string jsonString = JsonConvert.SerializeObject(model);
                byte[] jsonByteArray = Encoding.UTF8.GetBytes(jsonString);

                WebClient client = new WebClient();
                client.Headers.Add("Content-Type", "application/json");
                client.Headers.Add("X-Session-Key", _sessionKey);

                try
                {
                    string response = Encoding.UTF8.GetString(client.UploadData(_baseUrl + "/Message", "POST", jsonByteArray));

                    MessagingModels.SendMessageResponse mid = JsonConvert.DeserializeObject<MessagingModels.SendMessageResponse>(response);

                    return mid.MessageId;
                }
                catch (WebException ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// Used to delete a message
            /// </summary>
            /// <param name="model">Model contains messageId</param>
            /// <param name="permanentlyDeleteCheck">Boolean value used to delete a message permanently or to trash</param>
            /// <returns>HttpResponseMessage</returns>
            public string Delete(string mid, bool permanentlyDeleteCheck)
            {
                string jsonString = JsonConvert.SerializeObject("");
                byte[] jsonByteArray = Encoding.UTF8.GetBytes(jsonString);

                WebClient client = new WebClient();
                client.Headers.Add("Content-Type", "application/json");
                client.Headers.Add("X-Session-Key", _sessionKey);

                //MessagingModels.MessageOperations model
                string messageId = mid;
                if (permanentlyDeleteCheck == true)
                {
                    try
                    {
                        string response = Encoding.UTF8.GetString(client.UploadData(_baseUrl + "/Message/" + messageId + "?Permanently=true", "DELETE", jsonByteArray));

                        return response;
                    }
                    catch (WebException ex)
                    {
                        throw ex;
                    }
                }
                else
                {
                    try
                    {
                        string response = Encoding.UTF8.GetString(client.UploadData(_baseUrl + "/Message/" + messageId, "DELETE", jsonByteArray));

                        return response;
                    }
                    catch (WebException ex)
                    {
                        throw ex;
                    }
                }
            }

            /// <summary>
            /// Used to display a specific message
            /// </summary>
            /// <param name="messageID">string messageID of selected message</param>
            /// <returns>GetMessage object</returns>
            public MessagingModels.GetMessage Get(string messageID)
            {
                WebClient client = new WebClient();
                client.Headers.Add("Content-Type", "application/json");
                client.Headers.Add("X-Session-Key", _sessionKey);

                try
                {
                    string response = client.DownloadString(_baseUrl + "/Message/" + messageID);

                    MessagingModels.GetMessage messageResponse = JsonConvert.DeserializeObject<MessagingModels.GetMessage>(response);

                    return messageResponse;
                }
                catch (WebException ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// Retrieves a Mime Message
            /// </summary>
            /// <param name="messageId"></param>
            /// <returns>MimeMessageRequestandResponse object</returns>
            public MessagingModels.MimeMessageRequestandResponse GetaMimeMessage(string messageId)
            {
                WebClient client = new WebClient();
                client.Headers.Add("Content-Type", "application/json");
                client.Headers.Add("X-Session-Key", _sessionKey);

                try
                {
                    string response = client.DownloadString(_baseUrl + "/Message/" + messageId + "/Mime");
                    MessagingModels.MimeMessageRequestandResponse mimeMessage = JsonConvert.DeserializeObject<MessagingModels.MimeMessageRequestandResponse>(response);

                    return mimeMessage;
                }
                catch (WebException ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// Send a Mime message
            /// </summary>
            /// <param name="model"></param>
            /// <param name="location"></param>
            /// <returns>Mime MessageID as a string</returns>
            public string SendMimeMessage(MessagingModels.SendMessage model, string location)
            {
                WebClient client = new WebClient();
                client.Headers.Add("Content-Type", "application/json");
                client.Headers.Add("X-Session-Key", _sessionKey);

                var message = new MimeMessage();

                message.From.Add(new MailboxAddress(model.From));

                foreach (string str in model.To)
                {
                    message.To.Add(new MailboxAddress(str));
                }

                foreach (string str in model.Cc)
                {
                    message.Cc.Add(new MailboxAddress(str));
                }

                foreach (string str in model.Bcc)
                {
                    message.Bcc.Add(new MailboxAddress(str));
                }

                message.Subject = model.Subject;
                string messageString = model.TextBody;

                var body = new TextPart("plain") { Text = @messageString };

                var attachment = new MimePart("", "")
                {
                    ContentObject = new ContentObject(File.OpenRead(location), ContentEncoding.Default),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = Path.GetFileName(location)
                };

                var multipart = new Multipart("mixed");
                multipart.Add(body);
                multipart.Add(attachment);

                message.Body = multipart;

                MessagingModels.MimeMessageRequestandResponse mimeMessageObject = new MessagingModels.MimeMessageRequestandResponse();
                mimeMessageObject.MimeMessage = message.ToString();

                string jsonString = JsonConvert.SerializeObject(mimeMessageObject);
                byte[] jsonByteArray = Encoding.UTF8.GetBytes(jsonString);

                try
                {
                    string response = Encoding.UTF8.GetString(client.UploadData(_baseUrl + "/Message/Mime", "POST", jsonByteArray));

                    MessagingModels.MimeMessageRequestandResponse mid = JsonConvert.DeserializeObject<MessagingModels.MimeMessageRequestandResponse>(response);

                    return mid.MessageId.ToString();
                }
                catch (WebException ex)
                {
                    throw ex;
                }
            }
        }
    }
}