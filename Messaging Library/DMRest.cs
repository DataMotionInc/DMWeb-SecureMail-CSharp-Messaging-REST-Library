using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Net.Http;
using Newtonsoft.Json;
using Messaging_Library.Models;
using MimeKit;
using System.Text;

namespace DMWeb_REST
{
    /// <summary>
    /// Class functions
    /// </summary>
    public class DMWeb
    { 
        public static string _baseUrl = "";
        public static string sessionKey = "";
        

        public MessagingModels.GetSendMessage sendMessagePayload = new MessagingModels.GetSendMessage();
        public List<string> base64 = new List<string>();

        public DMAccount Account = new DMAccount();
        public DMFolders Folders = new DMFolders();
        public DMMessage Message = new DMMessage();
        public DMMessage.MID MID = new DMMessage.MID();


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
            public async Task<string> LogOn(AccountModels.AccountLogOn model)
            {
                HttpClient client = new HttpClient();

                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync(_baseUrl + "/Account/Logon", model);
                    response.EnsureSuccessStatusCode();
                    sessionKey = await response.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException ex)
                {
                    throw ex;
                }

                AccountModels.AccountSessionKey temp = JsonConvert.DeserializeObject<AccountModels.AccountSessionKey>(sessionKey);
                sessionKey = temp.SessionKey;

                client.DefaultRequestHeaders.Add("X-Session-Key", sessionKey);

                return temp.SessionKey;

            }

            /// <summary>
            /// Displays the account details to the user
            /// </summary>
            /// <returns>HttpResponseMessage deserialized into AccountResponses object</returns>
            public async Task<AccountModels.AccountDetails> Details()
            {
                HttpClient client = new HttpClient();
                
                client.DefaultRequestHeaders.Add("X-Session-Key", sessionKey);

                try
                {
                    HttpResponseMessage response = await client.GetAsync(_baseUrl + "/Account/Details");
                    response.EnsureSuccessStatusCode();
                    string details = await response.Content.ReadAsStringAsync();
                    AccountModels.AccountDetails temp = JsonConvert.DeserializeObject<AccountModels.AccountDetails>(details);
                    return temp;
                }
                catch(HttpRequestException ex)
                {
                    throw ex;  
                }
            }

            /// <summary>
            /// Allows user to change their account's password
            /// </summary>
            /// <param name="model">Model of type AccountChangePassword contains string OldPassword and string NewPassword</param>
            /// <returns>HttpResponseMessage</returns>
            public async Task<string> ChangePassword(AccountModels.AccountChangePassword model)
            {
                HttpClient client = new HttpClient();
                
                client.DefaultRequestHeaders.Add("X-Session-Key", sessionKey);

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
            /// Removes the Session Key when the user chooses to log out
            /// </summary>
            /// <returns>HttpResponseMessage</returns>
            public async Task<string> LogOut()
            {
                HttpClient client = new HttpClient();
                
                client.DefaultRequestHeaders.Add("X-Session-Key", sessionKey);

                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync(_baseUrl + "/Account/Logout", "");
                    response.EnsureSuccessStatusCode();
                    client.DefaultRequestHeaders.Remove("X-Session-Key");
                    sessionKey = "";

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
            public async Task<FolderModels.Folder> List()
            {
                HttpClient client = new HttpClient();
                
                client.DefaultRequestHeaders.Add("X-Session-Key", sessionKey);

                try
                {
                    HttpResponseMessage response = await client.GetAsync(_baseUrl + "/Folder/List");
                    response.EnsureSuccessStatusCode();
                    string stringFolders = await response.Content.ReadAsStringAsync();

                    FolderModels.Folder folderResponse = JsonConvert.DeserializeObject<FolderModels.Folder>(stringFolders);

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
            public async Task<string> Create(FolderModels.FolderRequest model)
            {
                HttpClient client = new HttpClient();
                
                client.DefaultRequestHeaders.Add("X-Session-Key", sessionKey);

                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync(_baseUrl + "/Folder", model);
                    response.EnsureSuccessStatusCode();

                    string responseString = await response.Content.ReadAsStringAsync();

                    FolderModels.FolderResponse fid = JsonConvert.DeserializeObject<FolderModels.FolderResponse>(responseString);

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
                HttpClient client = new HttpClient();
                
                client.DefaultRequestHeaders.Add("X-Session-Key", sessionKey);

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
            public async Task<MessagingModels.GetInboxMIDResponse> GetInboxMessageIds(MessagingModels.GetInboxMIDRequest model)
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("X-Session-Key", sessionKey);
                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync(_baseUrl + "/Message/GetInboxMessageIds", model);
                    response.EnsureSuccessStatusCode();
                    string messageIdsString = await response.Content.ReadAsStringAsync();

                    MessagingModels.GetInboxMIDResponse inboxResponse = JsonConvert.DeserializeObject<MessagingModels.GetInboxMIDResponse>(messageIdsString);

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
            public async Task<MessagingModels.GetMessageSummariesAndUnreadMessages> GetMessageSummaries(MessagingModels.GetMessageSummariesRequest model)
            {
                HttpClient client = new HttpClient();
                
                client.DefaultRequestHeaders.Add("X-Session-Key", sessionKey);

                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync(_baseUrl + "/Message/GetMessageSummaries", model);
                    response.EnsureSuccessStatusCode();
                    string summariesString = await response.Content.ReadAsStringAsync();

                    MessagingModels.GetMessageSummariesAndUnreadMessages summariesResponse = JsonConvert.DeserializeObject<MessagingModels.GetMessageSummariesAndUnreadMessages>(summariesString);

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
            public async Task<MessagingModels.GetMessageSummariesAndUnreadMessages> GetUnreadMessages(bool LastMIDReceived, string MID)
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("X-Session-Key", sessionKey);

                if (LastMIDReceived == false)
                {
                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(_baseUrl + "/Message/Inbox/Unread");
                        string unreadResponseString = await response.Content.ReadAsStringAsync();
                        response.EnsureSuccessStatusCode();

                        MessagingModels.GetMessageSummariesAndUnreadMessages unreadResponse = JsonConvert.DeserializeObject<MessagingModels.GetMessageSummariesAndUnreadMessages>(unreadResponseString);

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

                        MessagingModels.GetMessageSummariesAndUnreadMessages unreadResponse = JsonConvert.DeserializeObject<MessagingModels.GetMessageSummariesAndUnreadMessages>(unreadResponseString);
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
            public async Task<MessagingModels.SearchInboxResponse> SearchInbox(MessagingModels.SearchInbox model)
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("X-Session-Key", sessionKey);

                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync(_baseUrl + "/Message/Inbox/Search", model);
                    string searchInboxResponseString = await response.Content.ReadAsStringAsync();
                    response.EnsureSuccessStatusCode();

                    MessagingModels.SearchInboxResponse searchInboxResponseObject = JsonConvert.DeserializeObject<MessagingModels.SearchInboxResponse>(searchInboxResponseString);

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
            public async Task<MessagingModels.MetadataResponse> GetMessageMetadata(string MessageId)
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("X-Session-Key", sessionKey);
                
                try
                {
                    HttpResponseMessage response = await client.GetAsync(_baseUrl + "/Message/" + MessageId + "/Metadata");
                    string messageMetadataString = await response.Content.ReadAsStringAsync();
                    response.EnsureSuccessStatusCode();

                    MessagingModels.MetadataResponse messageMetadata = JsonConvert.DeserializeObject<MessagingModels.MetadataResponse>(messageMetadataString);

                    return messageMetadata;
                }
                catch(HttpRequestException ex)
                {
                    throw ex;
                }
            }

            public class MID
            {
                /// <summary>
                /// Used to retract a message
                /// </summary>
                /// <param name="model">Model contains string messageId</param>
                /// <returns>HttpResponseMessage(null if successful) </returns>
                public async Task<string> Retract(MessagingModels.MessageOperations model)
                {
                    HttpClient client = new HttpClient();
                    
                    client.DefaultRequestHeaders.Add("X-Session-Key", sessionKey);

                    try
                    {
                        string messageId = model.MessageId.ToString();
                        HttpResponseMessage response = await client.PostAsJsonAsync(_baseUrl + "/Message/" + messageId + "/Retract", model);
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
                /// <param name="model">Model contains string messageId</param>
                /// <returns>HttpResponseMessage(null if successful)</returns>
                public async Task<string> Move(MessagingModels.MessageOperations model)
                {
                    HttpClient client = new HttpClient();
                    
                    client.DefaultRequestHeaders.Add("X-Session-Key", sessionKey);
                    client.DefaultRequestHeaders.Add("Accept", "*/*");
                    client.DefaultRequestHeaders.Add("Connection", "close");
                    client.DefaultRequestHeaders.Add("cache-control", "no-cache");
                    client.DefaultRequestHeaders.Add("accept-encoding", "gzip, deflate");

                    try
                    {
                        string messageId = model.MessageId.ToString();
                        HttpResponseMessage response = await client.PostAsJsonAsync(_baseUrl + "/Message/" + messageId + "/Move/", model);
                        response.EnsureSuccessStatusCode();

                        return await response.Content.ReadAsStringAsync();
                    }
                    catch (HttpRequestException ex)
                    {
                        throw ex;
                    }
                }
            }

            /// <summary>
            /// Used to send a message
            /// </summary>
            /// <param name="model">Model contains multiple parameters</param>
            /// <returns>MessageID as an integer</returns>
            public async Task<int> Send(MessagingModels.GetSendMessage model)
            {
                HttpClient client = new HttpClient();
                
                client.DefaultRequestHeaders.Add("X-Session-Key", sessionKey);

                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync(_baseUrl + "/Message", model);
                    response.EnsureSuccessStatusCode();

                    string responseString = await response.Content.ReadAsStringAsync();
                    
                    MessagingModels.SendMessageResponse mid = JsonConvert.DeserializeObject<MessagingModels.SendMessageResponse>(responseString);

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
            public async Task<string> Delete(string mid, bool permanentlyDeleteCheck)
            {
                //MessagingModels.MessageOperations model
                HttpClient client = new HttpClient();
                
                client.DefaultRequestHeaders.Add("X-Session-Key", sessionKey);
                client.DefaultRequestHeaders.Add("Accept", "*/*");
                client.DefaultRequestHeaders.Add("Connection", "close");
                client.DefaultRequestHeaders.Add("cache-control", "no-cache");
                client.DefaultRequestHeaders.Add("accept-encoding", "gzip, deflate");
                client.BaseAddress = new Uri("https://ssl.datamotion.com/SecureMessagingApi/");
                string messageId = mid;
                if (permanentlyDeleteCheck == true)
                {
                    try
                    {
                        HttpResponseMessage response = await client.DeleteAsync(_baseUrl + "/Message/" + messageId + "?Permanently=true");
                        response.EnsureSuccessStatusCode();

                        return await response.Content.ReadAsStringAsync();
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

                        return await response.Content.ReadAsStringAsync();
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
            /// <returns>GetSendMessage object</returns>
            public async Task<MessagingModels.GetSendMessage> Get(string messageID)
            {
                HttpClient client = new HttpClient();
                
                client.DefaultRequestHeaders.Add("X-Session-Key", sessionKey);

                try
                {
                    HttpResponseMessage response = await client.GetAsync(_baseUrl + "/Message/" + messageID);
                    response.EnsureSuccessStatusCode();
                    string messageString = await response.Content.ReadAsStringAsync();

                    MessagingModels.GetSendMessage messageResponse = JsonConvert.DeserializeObject<MessagingModels.GetSendMessage>(messageString);

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
            public async Task<MessagingModels.MimeMessageRequestandResponse> GetaMimeMessage(string messageId)
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("X-Session-Key", sessionKey);
                try
                {
                    HttpResponseMessage response = await client.GetAsync(_baseUrl + "/Message/" + messageId + "/Mime");
                    string mimeString = await response.Content.ReadAsStringAsync();
                    response.EnsureSuccessStatusCode();

                    MessagingModels.MimeMessageRequestandResponse mimeMessage = JsonConvert.DeserializeObject<MessagingModels.MimeMessageRequestandResponse>(mimeString);

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
            public async Task<string> SendMimeMessage(MessagingModels.GetSendMessage model, string location)
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("X-Session-Key", sessionKey);

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
                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync(_baseUrl + "/Message/Mime", mimeMessageObject);
                    response.EnsureSuccessStatusCode();
                    string responseString = await response.Content.ReadAsStringAsync();

                    MessagingModels.MimeMessageRequestandResponse mid = JsonConvert.DeserializeObject<MessagingModels.MimeMessageRequestandResponse>(responseString);

                    return mid.MessageId.ToString();
                }
                catch (HttpRequestException ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Used to convert a file to Base64 string
        /// </summary>
        /// <param name="location">string of file location</param>
        /// <returns></returns>
        public string ConvertToBase64(string location)
        {
            byte[] imageArray = System.IO.File.ReadAllBytes(location);
            string base64 = Convert.ToBase64String(imageArray);
            return base64;
        }

        /// <summary>
        /// Used to convert base64 string into original file with choice of save location
        /// </summary>
        /// <param name="base64">string base64</param>
        /// <param name="FileName">string file name</param>
        public void ConvertFromBase64(string base64, string FileName)
        {
            byte[] imageBytes = Convert.FromBase64String(base64);

            SaveFileDialog dlg = new SaveFileDialog();
            //string type = Path.GetExtension(dlg.FileName);
            dlg.FileName = FileName; 
            dlg.ShowDialog();

            System.IO.FileInfo location = new System.IO.FileInfo(dlg.FileName);
            File.WriteAllBytes(location.ToString(), imageBytes);
        }
    }
}