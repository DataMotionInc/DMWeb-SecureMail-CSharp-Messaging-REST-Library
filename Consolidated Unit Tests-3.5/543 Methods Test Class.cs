using System.Net;
using System.IO;
using System.Threading;
using System.Reflection;
using NUnit.Framework;
using DMWeb_REST;
using DMWeb_REST.Models;

namespace Consolidated_Unit_Tests
{
    public class Context
    {
        public static DMWeb dmWeb = new DMWeb();
        public static string userName;
        public static string password;
    }

    [TestFixture]
    public class v543_Tests
    {
        static string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string rootPath = Directory.GetParent(assemblyFolder).Parent.Parent.FullName;
        private static string _consolidatedPath = Path.Combine(rootPath, "Consolidated Unit Tests");
        private static string _documentsPath = Path.Combine(_consolidatedPath, "Test Documents");
        private string _messageDataPath = Path.Combine(_documentsPath, "MessageData.txt");
        private string _testDataPath = Path.Combine(_documentsPath, "test.txt");

        #region No Session Key
        public void GetMessageWithoutAttachmentDataNoSessionKeyTrueMID()
        {
            try
            {
                Context.dmWeb.Message.GetMessageWithoutAttachmentData(74);
            }
            catch (WebException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        public void GetMessageWithoutAttachmentDataNoSessionKeyFalseMID()
        {
            try
            {
                Context.dmWeb.Message.GetMessageWithoutAttachmentData(1234);
            }
            catch (WebException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        public void GetAttachmentNoSessionKeyTrueAttachmentId()
        {
            try
            {
                Context.dmWeb.Message.GetAttachment(126);
            }
            catch (WebException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        public void GetAttachmentNoSessionKeyFalseAttachmentId()
        {
            try
            {
                Context.dmWeb.Message.GetAttachment(12345);
            }
            catch (WebException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        public void GetMessageSummariesWithMetadataNoSessionKeyFalseFIDFalseLMID()
        {
            Message.GetMessageSummariesWithMetadataRequest request = new Message.GetMessageSummariesWithMetadataRequest();
            request.FolderId = 12345;
            request.LastMessageIdReceived = 12345;

            try
            {
                Context.dmWeb.Message.GetMessageSummariesWithMetadata(request);
            }
            catch (WebException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        public void GetMessageSummariesWithMetadataNoSessionKeyFalseFIDTrueLMID()
        {
            Message.GetMessageSummariesWithMetadataRequest request = new Message.GetMessageSummariesWithMetadataRequest();
            request.FolderId = 12345;
            request.LastMessageIdReceived = 74;

            try
            {
                Context.dmWeb.Message.GetMessageSummariesWithMetadata(request);
            }
            catch (WebException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        public void GetMessageSummariesWithMetadataNoSessionKeyTrueFIDFalseLMID()
        {
            Message.GetMessageSummariesWithMetadataRequest request = new Message.GetMessageSummariesWithMetadataRequest();
            request.FolderId = 1;
            request.LastMessageIdReceived = 12345;

            try
            {
                Context.dmWeb.Message.GetMessageSummariesWithMetadata(request);
            }
            catch (WebException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        public void GetMessageSummariesWithMetadataNoSessionKeyTrueFIDTrueLMID()
        {
            Message.GetMessageSummariesWithMetadataRequest request = new Message.GetMessageSummariesWithMetadataRequest();
            request.FolderId = 1;
            request.LastMessageIdReceived = 74;

            try
            {
                Context.dmWeb.Message.GetMessageSummariesWithMetadata(request);
            }
            catch (WebException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        public void SendDraftNoSessionKeyTrueMID()
        {
            try
            {
                Message.SaveDraftRequest request = new Message.SaveDraftRequest();
                request.To.Add("user1@dmweb.citest.com");
                request.Subject = "No Session Key True MID Test";

                Message.SaveDraftResponse response = Context.dmWeb.Message.SaveDraft(request);
                Context.dmWeb.Message.SendDraft(response.MessageId);
            }
            catch (WebException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        public void SendDraftNoSessionKeyFalseMID()
        {
            try
            {
                Context.dmWeb.Message.SendDraft(12345);
            }
            catch (WebException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }
        #endregion

        #region Session Key Exists
        public void LogOn()
        {
            string[] lines = File.ReadAllLines(_messageDataPath);

            string str = lines[0];
            string[] linesplit = str.Split(':');
            Context.userName = linesplit[1];

            string str2 = lines[1];
            string[] linesplit2 = str2.Split(':');
            Context.password = linesplit2[1];

            string sessionKey = Context.dmWeb.Account.LogOn(new Account.LogOn { UserIdOrEmail = Context.userName, Password = Context.password });
            Assert.AreNotEqual(string.Empty, sessionKey);

            Thread.Sleep(2000);
        }

        public void GetMessageWithoutAttachmentDataWithSessionKeyTrueMID()
        {
            try
            {
                Context.dmWeb.Message.GetMessageWithoutAttachmentData(74);
            }
            catch (WebException ex)
            {
                Assert.IsTrue(ex.Message.Contains("200"));
            }
        }

        public void GetMessageWithoutAttachmentDataWithSessionKeyFalseMID()
        {
            try
            {
                Context.dmWeb.Message.GetMessageWithoutAttachmentData(1234);
            }
            catch (WebException ex)
            {
                Assert.IsTrue(ex.Message.Contains("400"));
            }
        }

        public void GetAttachmentWithSessionKeyTrueAttachmentId()
        {
            try
            {
                Context.dmWeb.Message.GetAttachment(126);
            }
            catch (WebException ex)
            {
                Assert.IsTrue(ex.Message.Contains("200"));
            }
        }

        public void GetAttachmentWithSessionKeyFalseAttachmentId()
        {
            try
            {
                Context.dmWeb.Message.GetAttachment(12345);
            }
            catch (WebException ex)
            {
                Assert.IsTrue(ex.Message.Contains("400"));
            }
        }

        public void GetMessageSummariesWithMetadataWithSessionKeyFalseFIDFalseLMID()
        {
            Message.GetMessageSummariesWithMetadataRequest request = new Message.GetMessageSummariesWithMetadataRequest();
            request.FolderId = 12345;
            request.LastMessageIdReceived = 12345;

            try
            {
                Context.dmWeb.Message.GetMessageSummariesWithMetadata(request);
            }
            catch (WebException ex)
            {
                Assert.IsTrue(ex.Message.Contains("400"));
            }
        }

        public void GetMessageSummariesWithMetadataWithSessionKeyFalseFIDTrueLMID()
        {
            Message.GetMessageSummariesWithMetadataRequest request = new Message.GetMessageSummariesWithMetadataRequest();
            request.FolderId = 12345;
            request.LastMessageIdReceived = 74;

            try
            {
                Context.dmWeb.Message.GetMessageSummariesWithMetadata(request);
            }
            catch (WebException ex)
            {
                Assert.IsTrue(ex.Message.Contains("400"));
            }
        }

        public void GetMessageSummariesWithMetadataWithSessionKeyTrueFIDFalseLMID()
        {
            Message.GetMessageSummariesWithMetadataRequest request = new Message.GetMessageSummariesWithMetadataRequest();
            request.FolderId = 1;
            request.LastMessageIdReceived = 12345;

            try
            {
                Context.dmWeb.Message.GetMessageSummariesWithMetadata(request);
            }
            catch (WebException ex)
            {
                Assert.IsTrue(ex.Message.Contains("400"));
            }
        }

        public void GetMessageSummariesWithMetadataWithSessionKeyTrueFIDTrueLMID()
        {
            Message.GetMessageSummariesWithMetadataRequest request = new Message.GetMessageSummariesWithMetadataRequest();
            request.FolderId = 1;
            request.LastMessageIdReceived = 74;

            try
            {
                Context.dmWeb.Message.GetMessageSummariesWithMetadata(request);
            }
            catch (WebException ex)
            {
                Assert.IsTrue(ex.Message.Contains("200"));
            }
        }

        public void SendDraftWithSessionKeyTrueMID()
        {
            try
            {
                Message.SaveDraftRequest request = new Message.SaveDraftRequest();
                request.To.Add("user1@dmweb.citest.com");
                request.Subject = "No Session Key True MID Test";

                Message.SaveDraftResponse response = Context.dmWeb.Message.SaveDraft(request);
                Context.dmWeb.Message.SendDraft(response.MessageId);
            }
            catch (WebException ex)
            {
                Assert.IsTrue(ex.Message.Contains("200"));
            }
        }

        public void SendDraftWithSessionKeyFalseMID()
        {
            try
            {
                Context.dmWeb.Message.SendDraft(12345);
            }
            catch (WebException ex)
            {
                Assert.IsTrue(ex.Message.Contains("400"));
            }
        }
        #endregion
    }
}
