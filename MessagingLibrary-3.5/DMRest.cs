﻿using System.Collections.Generic;
using System.Net;
using System.Security.Authentication;
using System.Text;
using Newtonsoft.Json;
using DMWeb_REST.Models;

namespace DMWeb_REST
{
    /// <summary>
    /// Class functions
    /// </summary>
    public class DMWeb
    {
        public static WebClient client = new WebClient();
        public static string _baseUrl = "";
        public static string _sessionKey = "";

        public Message.SendMessage sendMessagePayload = new Message.SendMessage();
        public Message.SaveDraftRequest saveDraftPayload = new Message.SaveDraftRequest();
        public List<Message.AttachmentsBody> attachmentPayload = new List<Message.AttachmentsBody>();
        public List<string> _base64 = new List<string>();

        public DMAccount Account = new DMAccount();
        public DMFolders Folders = new DMFolders();
        public DMMessage Message = new DMMessage();

        public const SslProtocols _Tls12 = (SslProtocols)0x00000C00;
        public const SecurityProtocolType Tls12 = (SecurityProtocolType)_Tls12;

        /// <summary>
        /// Default constructor that sets the _baseUrl to SecureMail
        /// </summary>
        public DMWeb()
        {
            _baseUrl = "https://ssl.datamotion.com/SecureMessagingApi";
            ServicePointManager.SecurityProtocol = Tls12;
        }

        /// <summary>
        /// Non-default constructor that allows the host URL to be changed
        /// </summary>
        /// <param name="url">The string of the destination URL</param>
        public DMWeb(string url)
        {
            _baseUrl = url;
            ServicePointManager.SecurityProtocol = Tls12;
        }
        public class DMAccount
        {
            /// <summary>
            /// Retrieve a sessionkey for the user
            /// </summary>
            /// <param name="model">Model of type AccountLogOn contains string UserIdOrEmail and string Password</param>
            /// <returns>HttpResponseMessage deserialized into AccountSessionKey object</returns>
            public string LogOn(Account.LogOn model)
            {
                string jsonString = JsonConvert.SerializeObject(model);
                byte[] jsonByteArray = Encoding.UTF8.GetBytes(jsonString);

                client.Headers.Add("Content-Type", "application/json");
                client.Headers.Remove("X-Session-Key");

                try
                {
                    string response = Encoding.UTF8.GetString(client.UploadData(_baseUrl + "/Account/Logon", "POST", jsonByteArray));

                    Account.AccountSessionKey temp = JsonConvert.DeserializeObject<Account.AccountSessionKey>(response);
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
            public Account.AccountDetails Details()
            {
                client.Headers.Add("Content-Type", "application/json");
                
                try
                {
                    string response = client.DownloadString(_baseUrl + "/Account/Details");
                    Account.AccountDetails temp = JsonConvert.DeserializeObject<Account.AccountDetails>(response);
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
            public string ChangePassword(Account.ChangePassword model)
            {
                string jsonString = JsonConvert.SerializeObject(model);
                byte[] jsonByteArray = Encoding.UTF8.GetBytes(jsonString);

                client.Headers.Add("Content-Type", "application/json");
                
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

                client.Headers.Add("Content-Type", "application/json");
                
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
            public Folder.ListFolders List()
            {
                client.Headers.Add("Content-Type", "application/json");
                
                try
                {
                    string response = client.DownloadString(_baseUrl + "/Folder/List");
                    Folder.ListFolders folderResponse = JsonConvert.DeserializeObject<Folder.ListFolders>(response);

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
            public string Create(Folder.Create model)
            {
                string jsonString = JsonConvert.SerializeObject(model);
                byte[] jsonByteArray = Encoding.UTF8.GetBytes(jsonString);

                client.Headers.Add("Content-Type", "application/json");
                
                try
                {
                    string response = Encoding.UTF8.GetString(client.UploadData(_baseUrl + "/Folder", jsonByteArray));
                    Folder.FolderResponse fid = JsonConvert.DeserializeObject<Folder.FolderResponse>(response);

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

                client.Headers.Add("Content-Type", "application/json");
                
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
            public Message.GetInboxMIDResponse GetInboxMessageIds(Message.GetInboxMIDRequest model)
            {
                string jsonString = JsonConvert.SerializeObject(model);
                byte[] jsonByteArray = Encoding.UTF8.GetBytes(jsonString);

                client.Headers.Add("Content-Type", "application/json");
                
                try
                {
                    string response = Encoding.UTF8.GetString(client.UploadData(_baseUrl + "/Message/GetInboxMessageIds", "POST", jsonByteArray));

                    Message.GetInboxMIDResponse inboxResponse = JsonConvert.DeserializeObject<Message.GetInboxMIDResponse>(response);

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
            public Message.GetMessageSummaries GetMessageSummaries(Message.GetMessageSummariesRequest model)
            {
                string jsonString = JsonConvert.SerializeObject(model);
                byte[] jsonByteArray = Encoding.UTF8.GetBytes(jsonString);

                client.Headers.Add("Content-Type", "application/json");
                
                try
                {
                    string response = Encoding.UTF8.GetString(client.UploadData(_baseUrl + "/Message/GetMessageSummaries", "POST", jsonByteArray));

                    Message.GetMessageSummaries summariesResponse = JsonConvert.DeserializeObject<Message.GetMessageSummaries>(response);

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
            public Message.GetUnreadMessages GetUnreadMessages(bool LastMIDReceived, string MID)
            {
                client.Headers.Add("Content-Type", "application/json");
                
                if (LastMIDReceived == false)
                {
                    try
                    {
                        string response = client.DownloadString(_baseUrl + "/Message/Inbox/Unread");

                        Message.GetUnreadMessages unreadResponse = JsonConvert.DeserializeObject<Message.GetUnreadMessages>(response);

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

                        Message.GetUnreadMessages unreadResponse = JsonConvert.DeserializeObject<Message.GetUnreadMessages>(response);

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
            public Message.SearchInboxResponse SearchInbox(Message.SearchInbox model)
            {
                string jsonString = JsonConvert.SerializeObject(model);
                byte[] jsonByteArray = Encoding.UTF8.GetBytes(jsonString);

                client.Headers.Add("Content-Type", "application/json");
                
                try
                {
                    string response = Encoding.UTF8.GetString(client.UploadData(_baseUrl + "/Message/Inbox/Search", "POST", jsonByteArray));

                    Message.SearchInboxResponse searchInboxResponseObject = JsonConvert.DeserializeObject<Message.SearchInboxResponse>(response);

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
            public Message.MetadataResponse GetMessageMetadata(string MessageId)
            {
                client.Headers.Add("Content-Type", "application/json");
                                
                try
                {
                    string response = client.DownloadString(_baseUrl + "/Message/" + MessageId + "/Metadata");

                    Message.MetadataResponse messageMetadata = JsonConvert.DeserializeObject<Message.MetadataResponse>(response);

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
            public string Retract(string messageId)
            {
                string jsonString = JsonConvert.SerializeObject("");
                byte[] jsonByteArray = Encoding.UTF8.GetBytes(jsonString);

                client.Headers.Add("Content-Type", "application/json");
                
                try
                {
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
            public string Move(Message.MoveMessageRequest model, string messageId)
            {
                string jsonString = JsonConvert.SerializeObject(model);
                byte[] jsonByteArray = Encoding.UTF8.GetBytes(jsonString);

                client.Headers.Add("Content-Type", "application/json");
                
                try
                {
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
            public int Send(Message.SendMessage model)
            {
                string jsonString = JsonConvert.SerializeObject(model);
                byte[] jsonByteArray = Encoding.UTF8.GetBytes(jsonString);

                client.Headers.Add("Content-Type", "application/json");
                
                try
                {
                    string response = Encoding.UTF8.GetString(client.UploadData(_baseUrl + "/Message", "POST", jsonByteArray));

                    Message.SendMessageResponse mid = JsonConvert.DeserializeObject<Message.SendMessageResponse>(response);

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
            public Message.DeleteMessageResponse Delete(string mid, bool permanentlyDeleteCheck)
            {
                string jsonString = JsonConvert.SerializeObject("");
                byte[] jsonByteArray = Encoding.UTF8.GetBytes(jsonString);

                client.Headers.Add("Content-Type", "application/json");
                
                //MessagingModels.MessageOperations model
                string messageId = mid;
                if (permanentlyDeleteCheck == true)
                {
                    try
                    {
                        string response = Encoding.UTF8.GetString(client.UploadData(_baseUrl + "/Message/" + messageId + "?Permanently=true", "DELETE", jsonByteArray));
                        Message.DeleteMessageResponse deletedResponse = JsonConvert.DeserializeObject<Message.DeleteMessageResponse>(response);

                        return deletedResponse;
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
                        Message.DeleteMessageResponse deletedResponse = JsonConvert.DeserializeObject<Message.DeleteMessageResponse>(response);

                        return deletedResponse;
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
            public Message.GetMessage Get(string messageID)
            {
                client.Headers.Add("Content-Type", "application/json");
                
                try
                {
                    string response = client.DownloadString(_baseUrl + "/Message/" + messageID);

                    Message.GetMessage messageResponse = JsonConvert.DeserializeObject<Message.GetMessage>(response);

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
            public Message.GetMimeMessageResponse GetaMimeMessage(string messageId)
            {
                client.Headers.Add("Content-Type", "application/json");
                
                try
                {
                    string response = client.DownloadString(_baseUrl + "/Message/" + messageId + "/Mime");
                    Message.GetMimeMessageResponse mimeMessage = JsonConvert.DeserializeObject<Message.GetMimeMessageResponse>(response);

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
            public string SendMimeMessage(string mimeString)
            {
                client.Headers.Add("Content-Type", "application/json");
                
                Message.SendMimeMessageRequest mimeMessageObject = new Message.SendMimeMessageRequest();
                mimeMessageObject.MimeMessage = mimeString;

                string jsonString = JsonConvert.SerializeObject(mimeMessageObject);
                byte[] jsonByteArray = Encoding.UTF8.GetBytes(jsonString);

                try
                {
                    string response = Encoding.UTF8.GetString(client.UploadData(_baseUrl + "/Message/Mime", "POST", jsonByteArray));

                    Message.SendMimeMessageResponse mid = JsonConvert.DeserializeObject<Message.SendMimeMessageResponse>(response);

                    return mid.MessageId.ToString();
                }
                catch (WebException ex)
                {
                    throw ex;
                }
            }

            // 5.43

            /// <summary>
            /// Gets a message without attachment data
            /// </summary>
            /// <param name="messageId"></param>
            /// <returns>MessagingModels.GetMessageWithoutAttachmentDataResponse object</returns>
            public Message.GetMessageWithoutAttachmentDataResponse GetMessageWithoutAttachmentData(int messageId)
            {
                client.Headers.Add("Content-Type", "application/json");
                
                try
                {
                    string response = client.DownloadString(_baseUrl + "/Message/" + messageId + "/NoAttachmentData");
                    Message.GetMessageWithoutAttachmentDataResponse responseObject = JsonConvert.DeserializeObject<Message.GetMessageWithoutAttachmentDataResponse>(response);
                    return responseObject;
                }
                catch (WebException ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// Gets only the attachment data from a message 
            /// </summary>
            /// <param name="attachmentId"></param>
            /// <returns>MessagingModels.GetAttachmentResponse object</returns>
            public Message.GetAttachmentResponse GetAttachment(int attachmentId)
            {
                client.Headers.Add("Content-Type", "application/json");
                
                try
                {
                    string response = client.DownloadString(_baseUrl + "/Message/" + attachmentId + "/Attachment");
                    Message.GetAttachmentResponse responseObject = JsonConvert.DeserializeObject<Message.GetAttachmentResponse>(response);
                    return responseObject;
                }
                catch (WebException ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// Gets the message summaries with metadata
            /// </summary>
            /// <param name="model"></param>
            /// <returns>MessagingModels.GetMessageSummariesWithMetadataResponse object</returns>
            public Message.GetMessageSummariesWithMetadataResponse GetMessageSummariesWithMetadata(Message.GetMessageSummariesWithMetadataRequest model)
            {
                string jsonString = JsonConvert.SerializeObject(model);
                byte[] jsonByteArray = Encoding.UTF8.GetBytes(jsonString);

                client.Headers.Add("Content-Type", "application/json");
                
                try
                {
                    string response = Encoding.UTF8.GetString(client.UploadData(_baseUrl + "/Message/GetMessageSummariesWithMetadata", "POST", jsonByteArray));
                    Message.GetMessageSummariesWithMetadataResponse responseObject = JsonConvert.DeserializeObject<Message.GetMessageSummariesWithMetadataResponse>(response);
                    return responseObject;
                }
                catch (WebException ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// Save a message as a draft
            /// </summary>
            /// <param name="model"></param>
            /// <returns>MessagingModels.SaveDraftResponse object</returns>
            public Message.SaveDraftResponse SaveDraft(Message.SaveDraftRequest model)
            {
                string jsonString = JsonConvert.SerializeObject(model);
                byte[] jsonByteArray = Encoding.UTF8.GetBytes(jsonString);

                client.Headers.Add("Content-Type", "application/json");
                
                try
                {
                    string response = Encoding.UTF8.GetString(client.UploadData(_baseUrl + "/Message/SaveDraft", "POST", jsonByteArray));
                    Message.SaveDraftResponse responseObject = JsonConvert.DeserializeObject<Message.SaveDraftResponse>(response);
                    return responseObject;
                }
                catch (WebException ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// Send a draft
            /// </summary>
            /// <param name="messageId"></param>
            /// <returns>MessagingModels.SendDraftResponse object</returns>
            public Message.SendDraftResponse SendDraft(int messageId)
            {
                string jsonString = JsonConvert.SerializeObject("");
                byte[] jsonByteArray = Encoding.UTF8.GetBytes(jsonString);

                client.Headers.Add("Content-Type", "application/json");
                
                try
                {
                    string response = Encoding.UTF8.GetString(client.UploadData(_baseUrl + "/Message/" + messageId + "/SendDraft", "POST", jsonByteArray));
                    Message.SendDraftResponse responseObject = JsonConvert.DeserializeObject<Message.SendDraftResponse>(response);
                    return responseObject;
                }
                catch (WebException ex)
                {
                    throw ex;
                }
            }
        }
    }
}