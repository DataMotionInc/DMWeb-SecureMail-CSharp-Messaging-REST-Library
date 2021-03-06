﻿using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using DMWeb_REST.Models;
using System;

namespace DMWeb_REST
{
    /// <summary>
    /// Class functions
    /// </summary>
    public class DMWeb
    {
        public static HttpClient client = new HttpClient();
        public static string _baseUrl = "";
        public static string _sessionKey = "";

        public Message.SendMessage sendMessagePayload = new Message.SendMessage();
        public Message.SaveDraftRequest saveDraftPayload = new Message.SaveDraftRequest();
        public List<Message.AttachmentsBody> attachmentPayload = new List<Message.AttachmentsBody>();
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
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            if (client.Timeout == TimeSpan.FromMinutes(0))
            {
                client.Timeout = TimeSpan.FromMinutes(30);
            }
        }

        /// <summary>
        /// Non-default constructor that allows the host URL to be changed
        /// </summary>
        /// <param name="url">The string of the destination URL</param>
        public DMWeb(string url)
        {
            _baseUrl = url;
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            if (client.Timeout == TimeSpan.FromMinutes(0))
            {
                client.Timeout = TimeSpan.FromMinutes(30);
            }
        }
        public class DMAccount
        {
            /// <summary>
            /// Retrieve a sessionkey for the user
            /// </summary>
            /// <param name="model">Model of type AccountLogOn contains string UserIdOrEmail and string Password</param>
            /// <returns>HttpResponseMessage deserialized into AccountSessionKey object</returns>
            public async Task<string> LogOn(Account.LogOn model)
            {
                try
                {
                    client.DefaultRequestHeaders.Remove("X-Session-Key");

                    HttpResponseMessage response = await client.PostAsJsonAsync(_baseUrl + "/Account/Logon", model);
                    response.EnsureSuccessStatusCode();
                    _sessionKey = await response.Content.ReadAsStringAsync();

                    Account.AccountSessionKey temp = JsonConvert.DeserializeObject<Account.AccountSessionKey>(_sessionKey);
                    _sessionKey = temp.SessionKey;
                    client.DefaultRequestHeaders.Add("X-Session-Key", _sessionKey);

                    return temp.SessionKey;
                }
                catch (HttpRequestException ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// Displays the account details to the user
            /// </summary>
            /// <returns>HttpResponseMessage deserialized into AccountResponses object</returns>
            public async Task<Account.AccountDetails> Details()
            {                
                string details = "";

                try
                {
                    HttpResponseMessage response = await client.GetAsync(_baseUrl + "/Account/Details");
                    response.EnsureSuccessStatusCode();
                    details = await response.Content.ReadAsStringAsync();
                    Account.AccountDetails temp = JsonConvert.DeserializeObject<Account.AccountDetails>(details);
                    return temp;
                }
                catch(HttpRequestException ex)
                {
                    string errorMsg = JsonConvert.DeserializeObject<string>(details);
                    throw ex;  

                }
            }

            /// <summary>
            /// Allows user to change their account's password
            /// </summary>
            /// <param name="model">Model of type AccountChangePassword contains string OldPassword and string NewPassword</param>
            /// <returns>HttpResponseMessage</returns>
            public async Task<string> ChangePassword(Account.ChangePassword model)
            {                
                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync(_baseUrl + "/Account/ChangePassword", model);
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// Removes the Session Key when the user chooses to log out and invalidates the supplied SessionKey on our Saas
            /// </summary>
            /// <returns>HttpResponseMessage</returns>
            public async Task<string> LogOut()
            {                
                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync(_baseUrl + "/Account/Logout", "");
                    response.EnsureSuccessStatusCode();
                    client.DefaultRequestHeaders.Remove("X-Session-Key");
                    _sessionKey = "";

                    return await response.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException ex)
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
            public async Task<Folder.ListFolders> List()
            {                
                try
                {
                    HttpResponseMessage response = await client.GetAsync(_baseUrl + "/Folder/List");
                    response.EnsureSuccessStatusCode();
                    string stringFolders = await response.Content.ReadAsStringAsync();

                    Folder.ListFolders folderResponse = JsonConvert.DeserializeObject<Folder.ListFolders>(stringFolders);

                    return folderResponse;
                }
                catch (HttpRequestException ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// Creates a folder of the chosen type and name by the user
            /// </summary>
            /// <param name="model">Model of type Folder contains string FolderName and int FolderType</param>
            /// <returns>FolderID as a string</returns>
            public async Task<string> Create(Folder.Create model)
            {                
                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync(_baseUrl + "/Folder", model);
                    response.EnsureSuccessStatusCode();

                    string responseString = await response.Content.ReadAsStringAsync();

                    Folder.FolderResponse fid = JsonConvert.DeserializeObject<Folder.FolderResponse>(responseString);

                    return fid.FolderId.ToString();
                }

                catch (HttpRequestException ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// Deletes a folder of the user's choice
            /// </summary>
            /// <param name="FolderID">string FolderId</param>
            /// <returns>HttpResponseMessage</returns>
            public async Task<string> Delete(string FolderID)
            {                
                try
                {
                    HttpResponseMessage response = await client.DeleteAsync(_baseUrl + "/Folder/" + FolderID);
                    response.EnsureSuccessStatusCode();

                    return await response.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException ex)
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
            public async Task<Message.GetInboxMIDResponse> GetInboxMessageIds(Message.GetInboxMIDRequest model)
            {
                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync(_baseUrl + "/Message/GetInboxMessageIds", model);
                    response.EnsureSuccessStatusCode();
                    string messageIdsString = await response.Content.ReadAsStringAsync();

                    Message.GetInboxMIDResponse inboxResponse = JsonConvert.DeserializeObject<Message.GetInboxMIDResponse>(messageIdsString);

                    return inboxResponse;
                }
                catch(HttpRequestException ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// Displays the MessageSummaries of a message
            /// </summary>
            /// <param name="model">Model of type GetMessageSummariesRequest contains int FolderId and int LastMessageIDReceived</param>
            /// <returns>HttpResponseMessage deserialized into SummariesResponseBody object</returns>
            public async Task<Message.GetMessageSummaries> GetMessageSummaries(Message.GetMessageSummariesRequest model)
            {                
                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync(_baseUrl + "/Message/GetMessageSummaries", model);
                    response.EnsureSuccessStatusCode();
                    string summariesString = await response.Content.ReadAsStringAsync();

                    Message.GetMessageSummaries summariesResponse = JsonConvert.DeserializeObject<Message.GetMessageSummaries>(summariesString);

                    return summariesResponse;
                }
                catch(HttpRequestException ex)
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
            public async Task<Message.GetUnreadMessages> GetUnreadMessages(bool LastMIDReceived, string MID)
            {
                if (LastMIDReceived == false)
                {
                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(_baseUrl + "/Message/Inbox/Unread");
                        string unreadResponseString = await response.Content.ReadAsStringAsync();
                        response.EnsureSuccessStatusCode();

                        Message.GetUnreadMessages unreadResponse = JsonConvert.DeserializeObject<Message.GetUnreadMessages>(unreadResponseString);

                        return unreadResponse;
                    }
                    catch(HttpRequestException ex)
                    {
                        throw ex;
                    }
                }
                else
                {
                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(_baseUrl + "/Message/Inbox/Unread/?After=" + MID);
                        string unreadResponseString = await response.Content.ReadAsStringAsync();
                        response.EnsureSuccessStatusCode();

                        Message.GetUnreadMessages unreadResponse = JsonConvert.DeserializeObject<Message.GetUnreadMessages>(unreadResponseString);
                        return unreadResponse;
                    }
                    catch(HttpRequestException ex)
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
            public async Task<Message.SearchInboxResponse> SearchInbox(Message.SearchInbox model)
            {
                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync(_baseUrl + "/Message/Inbox/Search", model);
                    string searchInboxResponseString = await response.Content.ReadAsStringAsync();
                    response.EnsureSuccessStatusCode();

                    Message.SearchInboxResponse searchInboxResponseObject = JsonConvert.DeserializeObject<Message.SearchInboxResponse>(searchInboxResponseString);

                    return searchInboxResponseObject;
                }
                catch (HttpRequestException ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// Gets the metadata of a selected message
            /// </summary>
            /// <param name="MessageId"></param>
            /// <returns>MetadataResponse object</returns>
            public async Task<Message.MetadataResponse> GetMessageMetadata(string MessageId)
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(_baseUrl + "/Message/" + MessageId + "/Metadata");
                    string messageMetadataString = await response.Content.ReadAsStringAsync();
                    response.EnsureSuccessStatusCode();

                    Message.MetadataResponse messageMetadata = JsonConvert.DeserializeObject<Message.MetadataResponse>(messageMetadataString);

                    return messageMetadata;
                }
                catch(HttpRequestException ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// Used to retract a message
            /// </summary>
            /// <param name="messageId">The messageId of the message being retracted</param>
            /// <returns>HttpResponseMessage(null if successful) </returns>
            public async Task<string> Retract(string messageId)
            {                    
                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync(_baseUrl + "/Message/" + messageId + "/Retract", "");
                    response.EnsureSuccessStatusCode();

                    return await response.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// Used to move a message
            /// </summary>
            /// <param name="model">MoveMessageRequest model</param>
            /// <param name="messageId">The messageId being moved</param>
            /// <returns>HttpResponseMessage(null if successful)</returns>
            public async Task<string> Move(Message.MoveMessageRequest model, string messageId)
            {                    
                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync(_baseUrl + "/Message/" + messageId + "/Move/", model);
                    response.EnsureSuccessStatusCode();

                    return await response.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// Used to send a message
            /// </summary>
            /// <param name="model">Model contains multiple parameters</param>
            /// <returns>MessageID as an integer</returns>
            public async Task<int> Send(Message.SendMessage model)
            { 
                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync(_baseUrl + "/Message", model);
                    response.EnsureSuccessStatusCode();

                    string responseString = await response.Content.ReadAsStringAsync();

                    Message.SendMessageResponse mid = JsonConvert.DeserializeObject<Message.SendMessageResponse>(responseString);

                    return mid.MessageId;
                }
                catch (HttpRequestException ex)
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
            public async Task<Message.DeleteMessageResponse> Delete(string mid, bool permanentlyDeleteCheck)
            {
                string messageId = mid;
                if (permanentlyDeleteCheck == true)
                {
                    try
                    {
                        HttpResponseMessage response = await client.DeleteAsync(_baseUrl + "/Message/" + messageId + "?Permanently=true");
                        response.EnsureSuccessStatusCode();
                        string responseString = await response.Content.ReadAsStringAsync();

                        Message.DeleteMessageResponse deletedResponse = JsonConvert.DeserializeObject<Message.DeleteMessageResponse>(responseString);
                        return deletedResponse;
                    }
                    catch (HttpRequestException ex)
                    {
                        throw ex;
                    }
                }
                else
                {
                    try
                    {
                        HttpResponseMessage response = await client.DeleteAsync(_baseUrl + "/Message/" + messageId);
                        response.EnsureSuccessStatusCode();
                        string responseString = await response.Content.ReadAsStringAsync();

                        Message.DeleteMessageResponse deletedResponse = JsonConvert.DeserializeObject<Message.DeleteMessageResponse>(responseString);
                        return deletedResponse;
                    }
                    catch (HttpRequestException ex)
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
            public async Task<Message.GetMessage> Get(string messageID)
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(_baseUrl + "/Message/" + messageID);
                    response.EnsureSuccessStatusCode();
                    string messageString = await response.Content.ReadAsStringAsync();

                    Message.GetMessage messageResponse = JsonConvert.DeserializeObject<Message.GetMessage>(messageString);

                    return messageResponse;
                }
                catch (HttpRequestException ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// Retrieves a Mime Message
            /// </summary>
            /// <param name="messageId"></param>
            /// <returns>MimeMessageRequestandResponse object</returns>
            public async Task<Message.GetMimeMessageResponse> GetaMimeMessage(string messageId)
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(_baseUrl + "/Message/" + messageId + "/Mime");
                    string mimeString = await response.Content.ReadAsStringAsync();
                    response.EnsureSuccessStatusCode();

                    Message.GetMimeMessageResponse mimeMessage = JsonConvert.DeserializeObject<Message.GetMimeMessageResponse>(mimeString);

                    return mimeMessage;
                }
                catch (HttpRequestException ex)
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
            public async Task<string> SendMimeMessage(string mimeString)
            {
                Message.SendMimeMessageRequest mimeMessageObject = new Message.SendMimeMessageRequest();
                mimeMessageObject.MimeMessage = mimeString;

                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync(_baseUrl + "/Message/Mime", mimeMessageObject);
                    response.EnsureSuccessStatusCode();
                    string responseString = await response.Content.ReadAsStringAsync();

                    Message.SendMimeMessageResponse mid = JsonConvert.DeserializeObject<Message.SendMimeMessageResponse>(responseString);

                    return mid.MessageId.ToString();
                }
                catch (HttpRequestException ex)
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
            public async Task<Message.GetMessageWithoutAttachmentDataResponse> GetMessageWithoutAttachmentData(int messageId)
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(_baseUrl + "/Message/" + messageId + "/NoAttachmentData");
                    response.EnsureSuccessStatusCode();
                    string responseString = await response.Content.ReadAsStringAsync();

                    Message.GetMessageWithoutAttachmentDataResponse responseObject = JsonConvert.DeserializeObject<Message.GetMessageWithoutAttachmentDataResponse>(responseString);
                    return responseObject;
                }
                catch (HttpRequestException ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// Gets only the attachment data from a message 
            /// </summary>
            /// <param name="attachmentId"></param>
            /// <returns>MessagingModels.GetAttachmentResponse object</returns>
            public async Task<Message.GetAttachmentResponse> GetAttachment(int attachmentId)
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(_baseUrl + "/Message/" + attachmentId + "/Attachment");
                    response.EnsureSuccessStatusCode();
                    string responseString = await response.Content.ReadAsStringAsync();

                    Message.GetAttachmentResponse responseObject = JsonConvert.DeserializeObject<Message.GetAttachmentResponse>(responseString);
                    return responseObject;
                }
                catch (HttpRequestException ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// Gets the message summaries with metadata
            /// </summary>
            /// <param name="model"></param>
            /// <returns>MessagingModels.GetMessageSummariesWithMetadataResponse object</returns>
            public async Task<Message.GetMessageSummariesWithMetadataResponse> GetMessageSummariesWithMetadata(Message.GetMessageSummariesWithMetadataRequest model)
            {
                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync(_baseUrl + "/Message/GetMessageSummariesWithMetadata", model);
                    response.EnsureSuccessStatusCode();
                    string responseString = await response.Content.ReadAsStringAsync();

                    Message.GetMessageSummariesWithMetadataResponse responseObject = JsonConvert.DeserializeObject<Message.GetMessageSummariesWithMetadataResponse>(responseString);
                    return responseObject;
                }
                catch (HttpRequestException ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// Save a message as a draft
            /// </summary>
            /// <param name="model"></param>
            /// <returns>MessagingModels.SaveDraftResponse object</returns>
            public async Task<Message.SaveDraftResponse> SaveDraft(Message.SaveDraftRequest model)
            {
                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync(_baseUrl + "/Message/SaveDraft", model);
                    response.EnsureSuccessStatusCode();
                    string responseString = await response.Content.ReadAsStringAsync();

                    Message.SaveDraftResponse responseObject = JsonConvert.DeserializeObject<Message.SaveDraftResponse>(responseString);
                    return responseObject;
                }
                catch (HttpRequestException ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// Send a draft
            /// </summary>
            /// <param name="messageId"></param>
            /// <returns>MessagingModels.SendDraftResponse object</returns>
            public async Task<Message.SendDraftResponse> SendDraft(int messageId)
            {
                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync(_baseUrl + "/Message/" + messageId + "/SendDraft", "");
                    response.EnsureSuccessStatusCode();
                    string responseString = await response.Content.ReadAsStringAsync();

                    Message.SendDraftResponse responseObject = JsonConvert.DeserializeObject<Message.SendDraftResponse>(responseString);
                    return responseObject;
                }
                catch (HttpRequestException ex)
                {
                    throw ex;
                }
            }
        }
    }
}