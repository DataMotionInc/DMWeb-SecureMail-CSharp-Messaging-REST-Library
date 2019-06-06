using System.Net;
using System.IO;
using System.Threading;
using System.Reflection;
using NUnit.Framework;
using DMWeb_REST;
using DMWeb_REST.Models;
using System;

namespace Consolidated_Unit_Tests
{
    public class Context
    {
        public static DMWeb dmWeb = new DMWeb();
        public static string userName;
        public static string password;
        public static int mid;
        public static string attachmentId;
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
        [Test, Order(1)]
        [Category("No Session Key")]
        [Category("GetMessageWithoutAttachmentData")]
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

        [Test, Order(2)]
        [Category("No Session Key")]
        [Category("GetMessageWithoutAttachmentData")]
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


        [Test, Order(3)]
        [Category("No Session Key")]
        [Category("GetAttachment")]
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

        [Test, Order(4)]
        [Category("No Session Key")]
        [Category("GetAttachment")]
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

        [Test, Order(5)]
        [Category("No Session Key")]
        [Category("GetMessageSumariesWithMetadata")]
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

        [Test, Order(6)]
        [Category("No Session Key")]
        [Category("GetMessageSumariesWithMetadata")]
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

        [Test, Order(7)]
        [Category("No Session Key")]
        [Category("GetMessageSumariesWithMetadata")]
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

        [Test, Order(8)]
        [Category("No Session Key")]
        [Category("GetMessageSumariesWithMetadata")]
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

        [Test, Order(9)]
        [Category("No Session Key")]
        [Category("SendDraft")]
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

        [Test, Order(10)]
        [Category("No Session Key")]
        [Category("GetMessageSumariesWithMetadata")]
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
        [Test, Order(11)]
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

        [Test, Order(12)]
        public void SendMessagePositiveTest()
        {
            string[] lines = System.IO.File.ReadAllLines(_messageDataPath);

            string str3 = lines[2];
            string[] linesplit3 = str3.Split(':');
            string toAddress = linesplit3[1];

            byte[] imageBytes = File.ReadAllBytes(_testDataPath);
            string base64String = Convert.ToBase64String(imageBytes);
            Context.mid = Context.dmWeb.Message.Send(new Message.SendMessage { To = { toAddress }, Subject = "543 Message", Attachments = { new Message.AttachmentsBody { AttachmentBase64 = base64String, ContentType = "text/plain", FileName = "test.txt" } } });
            Thread.Sleep(5000);
        }

        [Test, Order(13)]
        [Category("GetMessageSumariesWithoutAttachmentData")]
        public void GetMessageWithoutAttachmentDataWithSessionKeyTrueMID()
        {
            Message.GetMessageWithoutAttachmentDataResponse response = Context.dmWeb.Message.GetMessageWithoutAttachmentData(Context.mid);
            Context.attachmentId = response.Attachments[0].AttachmentId;
        }

        [Test]
        [Category("GetMessageSumariesWithoutAttachmentData")]
        public void GetMessageWithoutAttachmentDataWithSessionKeyFalseMID()
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

        [Test, Order(14)]
        [Category("GetAttachment")]
        public void GetAttachmentWithSessionKeyTrueAttachmentId()
        {
            Context.dmWeb.Message.GetAttachment(int.Parse(Context.attachmentId));
        }

        [Test]
        [Category("GetAttachment")]
        public void GetAttachmentWithSessionKeyFalseAttachmentId()
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

        [Test]
        [Category("GetMessageSumariesWithMetadata")]
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
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test]
        [Category("GetMessageSumariesWithMetadata")]
        public void GetMessageSummariesWithMetadataWithSessionKeyFalseFIDTrueLMID()
        {
            Message.GetMessageSummariesWithMetadataRequest request = new Message.GetMessageSummariesWithMetadataRequest();
            request.FolderId = 12345;
            request.LastMessageIdReceived = Context.mid;

            try
            {
                Context.dmWeb.Message.GetMessageSummariesWithMetadata(request);
            }
            catch (WebException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test]
        [Category("GetMessageSumariesWithMetadata")]
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
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test]
        [Category("GetMessageSumariesWithMetadata")]
        public void GetMessageSummariesWithMetadataWithSessionKeyTrueFIDTrueLMID()
        {
            Message.GetMessageSummariesWithMetadataRequest request = new Message.GetMessageSummariesWithMetadataRequest();
            request.FolderId = 1;
            request.LastMessageIdReceived = Context.mid;

            Context.dmWeb.Message.GetMessageSummariesWithMetadata(request);
        }

        [Test]
        [Category("SendDraft")]
        public void SendDraftWithSessionKeyTrueMID()
        {
            Message.SaveDraftRequest request = new Message.SaveDraftRequest();
            request.To.Add("user1@dmweb.citest.com");
            request.Subject = "No Session Key True MID Test";

            Message.SaveDraftResponse response = Context.dmWeb.Message.SaveDraft(request);
            Context.dmWeb.Message.SendDraft(response.MessageId);
        }

        [Test]
        [Category("SendDraft")]
        public void SendDraftWithSessionKeyFalseMID()
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
    }
}
