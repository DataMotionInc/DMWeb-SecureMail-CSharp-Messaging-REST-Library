﻿using System.Net.Http;
using Messaging_Library.Models;
using NUnit.Framework;
using Newtonsoft;
using DMWeb_REST;
using System.IO;
using System.Threading;

namespace Messaging_Library.TestFixtures.UnitTestClass
{
    public class Context
    {
        public static DMWeb dmWeb = new DMWeb();
        public static string folderId;
        public static int sendDeleteMID;
        public static int moveMID;
        public static string mimeMessageId;
         
        public static string userName;
        public static string password;

    }

    //To enter user credentials, go to the project folder
    //Data/LogInData.txt
    [TestFixture]
    public class AccountTests
    {
        [Test, Order(1)]
        [Category("LogOn")]
        [Category("No Session Key")]
        public void LogOnEmptyFieldsTest()
        {
            try
            {
                Context.dmWeb.Account.LogOn(new AccountModels.LogOn { }).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("400"));
            }
        }

        [Test, Order(2)]
        [Category("LogOn")]
        [Category("No Session Key")]
        public void LogOnOnlyUsernameOrIdFieldTest()
        {

            try
            {
                Context.dmWeb.Account.LogOn(new AccountModels.LogOn { UserIdOrEmail = Context.userName, Password = "" }).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("400"));
            }
        }

        [Test, Order(3)]
        [Category("LogOn")]
        [Category("No Session Key")]
        public void LogOnOnlyPasswordFieldTest()
        {
            Context.userName = "";

            try
            {
                Context.dmWeb.Account.LogOn(new AccountModels.LogOn { UserIdOrEmail = "", Password = Context.password }).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("400"));
            }
        }

        [Test, Order(4)]
        [Category("LogOn")]
        [Category("No Session Key")]
        public void LogOnInvalidFieldsNegativeTest()
        {
            try
            {
                Context.dmWeb.Account.LogOn(new AccountModels.LogOn { UserIdOrEmail = Context.userName + "0", Password = Context.password + "0" }).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test, Order(5)]
        [Category("LogOn")]
        [Category("No Session Key")]
        public void LogOnOnlyBadUsernameFieldTest()
        {
            try
            {
                Context.dmWeb.Account.LogOn(new AccountModels.LogOn { UserIdOrEmail = Context.userName + "0", Password = Context.password }).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                //Returns "The Password field is required
                Assert.IsTrue(ex.Message.Contains("400"));
            }
        }

        [Test, Order(6)]
        [Category("LogOn")]
        [Category("No Session Key")]
        public void LogOnOnlyBadPasswordFieldTest()
        {
            try
            {
                Context.dmWeb.Account.LogOn(new AccountModels.LogOn { UserIdOrEmail = Context.userName, Password = Context.password + "0" }).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                //Needs UserIdOrEmail message returned
                Assert.IsTrue(ex.Message.Contains("400"));
            }
        }

        [Test, Order(7)]
        [Category("Details")]
        [Category("No Session Key")]
        public void DisplayDetailsWithoutSessionKeyTest()
        {
            try
            {
                Context.dmWeb.Account.Details().GetAwaiter().GetResult().ToString();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test, Order(8)]
        [Category("ChangePassword")]
        [Category("No Session Key")]
        public void ChangePasswordWithoutSessionKeyTest()
        {
            try
            {
                Context.dmWeb.Account.ChangePassword(new AccountModels.ChangePassword { OldPassword = "test#password", NewPassword = "test#password2" }).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                //Call executes propery, but is false because OldPassword is not provided
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test, Order(9)]
        [Category("LogOut")]
        [Category("No Session Key")]
        public void LogOutWithoutSessionKey()
        {
            try
            {
                Context.dmWeb.Account.LogOut().GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test, Order(10)]
        [Category("Create Folder")]
        [Category("No Session Key")]
        public void CreateNoSessionKeyTest()
        {
            try
            {
                Context.dmWeb.Folders.Create(new FolderModels.Create { FolderName = "UnitTest2", FolderId = 0 }).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test, Order(11)]
        [Category("Delete Folder")]
        [Category("No Session Key")]
        public void DeleteFolderWithoutSessionKey()
        {
            try
            {
                Context.dmWeb.Folders.Delete("85905").GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test, Order(12)]
        [Category("List Folders")]
        [Category("No Session Key")]
        public void ListFoldersWithOutSessionKey()
        {
            try
            {
                Context.dmWeb.Folders.List().GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test, Order(13)]
        [Category("Delete Message")]
        [Category("No Session Key")]
        public void DeleteMessageNoSessionKeyTest()
        {
            int MID = 36848566;

            try
            {
                Context.dmWeb.Message.Delete(MID.ToString(), permanentlyDeleteCheck: false).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test, Order(14)]
        [Category("Get Inbox MID")]
        [Category("No Session Key")]
        public void GetInboxMIDNoSessionKey()
        {
            try
            {
                Context.dmWeb.Message.GetInboxMessageIds(new MessagingModels.GetInboxMIDRequest { FolderId = 0, MessageListFilter = 0, MustHaveAttachments = false }).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test, Order(15)]
        [Category("Get Message Metadata")]
        [Category("No Session Key")]
        public void GetMessageMetadataNoSessionKeyTest()
        {
            string MIDString = 22722701.ToString();

            try
            {
                Context.dmWeb.Message.GetMessageMetadata(MIDString).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test, Order(16)]
        [Category("Get Message Summaries")]
        [Category("No Session Key")]
        public void GetMessageSummariesNoSessionKeyTest()
        {
            try
            {
                Context.dmWeb.Message.GetMessageSummaries(new MessagingModels.GetMessageSummariesRequest { FolderId = 0, LastMessageIDReceived = 0 }).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test, Order(17)]
        [Category("Get Message")]
        [Category("No Session Key")]
        public void GetMessageNoSessionKeyTest()
        {
            string MIDString = 22722701.ToString();

            try
            {
                Context.dmWeb.Message.Get(MIDString).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test, Order(18)]
        [Category("Get Mime Message")]
        [Category("No Session Key")]
        public void GetMimeMessageNoSessionKeyTest()
        {
            try
            {
                string messageId = 36896426.ToString();

                Context.dmWeb.Message.GetaMimeMessage(messageId).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test, Order(19)]
        [Category("Move Message")]
        [Category("No Session Key")]
        public void MoveMessagNoSessionKeyTest()
        {
            int MID = 36848566;
            int DFID = 2;

            try
            {
                Context.dmWeb.Message.Move(new MessagingModels.MessageOperations { MessageId = MID, DestinationFolderId = DFID }).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test, Order(20)]
        [Category("Retract Message")]
        [Category("No Session Key")]
        public void RetractMessageNoSessionKeyTest()
        {
            int MID = 36860224;

            try
            {
                Context.dmWeb.Message.Retract(new MessagingModels.MessageOperations { MessageId = MID }).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test, Order(21)]
        [Category("Search Inbox")]
        [Category("No Session Key")]
        public void SearchInboxNoSessionKeyTest()
        {
            try
            {
                Context.dmWeb.Message.SearchInbox(new MessagingModels.SearchInbox { PageNum = 1, PageSize = 1 }).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test, Order(22)]
        [Category("Send Message")]
        [Category("No Session Key")]

        public void SendMessageNoSessionKey()
        {
            string toAddress = "";
            try
            {
                Context.dmWeb.Message.Send(new MessagingModels.SendMessage { To = { toAddress } }).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }
        [Test, Order(23)]
        [Category("Send Mime Message")]
        [Category("No Session Key")]

        public void SendMimeMessageNoSessionKeyTest()
        {
            string[] lines = System.IO.File.ReadAllLines("Data\\MessageData.txt");

            string str3 = lines[2];
            string[] linesplit3 = str3.Split(':');
            string toAddress = linesplit3[1]; 

            string str4 = lines[3];
            string[] linesplit4 = str4.Split(':');
            string fromAddress = linesplit4[1]; 

            try
            {
                string location = Path.Combine(TestContext.CurrentContext.TestDirectory, @"Test Documents\test.txt");
                Context.dmWeb.Message.SendMimeMessage(new MessagingModels.SendMessage { To = { toAddress }, From = fromAddress, Subject = "Mime Test Spam", TextBody = "Mime Message Test" }, location).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }
        [Test, Order(24)]
        [Category("Get Unread Messages")]
        [Category("No Session Key")]
        public void GetUnreadMessagesNoSessionKeyTest()
        {
            try
            {
                Context.dmWeb.Message.GetUnreadMessages(LastMIDReceived: false, MID: 34174277.ToString()).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test, Order(25)]
        [Category("LogOn")]
        public void LogOnPositiveTest()
        {
            string[] lines = System.IO.File.ReadAllLines("Data\\MessageData.txt");

            string str = lines[0];
            string[] linesplit = str.Split(':');
            Context.userName = linesplit[1];

            string str2 = lines[1];
            string[] linesplit2 = str2.Split(':');
            Context.password = linesplit2[1];
            string sessionKey = Context.dmWeb.Account.LogOn(new AccountModels.LogOn { UserIdOrEmail = Context.userName, Password = Context.password }).GetAwaiter().GetResult();
            Assert.AreNotEqual(string.Empty, sessionKey);
        }

        [Test, Order(26)]
        [Category("Create Folder")]
        public void CreateFolderPositiveTest()
        {
            Context.folderId = Context.dmWeb.Folders.Create(new FolderModels.Create { FolderName = "Unit Test 2", FolderType = 0 }).GetAwaiter().GetResult();
        }

        [Test, Order(27)]
        [Category("Delete Folder")]
        public void DeleteFolderWithSessionKey()
        {
            Context.dmWeb.Folders.Delete(Context.folderId).GetAwaiter().GetResult();
        }

        [Test, Order(28)]
        [Category("Send Message")]
        //Will send a message each time function is called
        public void SendMessagePositiveTest()
        {
            string[] lines = System.IO.File.ReadAllLines("Data\\MessageData.txt");

            string str3 = lines[2];
            string[] linesplit3 = str3.Split(':');
            string toAddress = linesplit3[1];

            Context.sendDeleteMID = Context.dmWeb.Message.Send(new MessagingModels.SendMessage { To = { toAddress }, Subject = "Positive test"}).GetAwaiter().GetResult();
            Thread.Sleep(5000);
        }

        [Test, Order(29)]
        [Category("Delete Message")]
        public void DeleteMessageTrueMIDFalseBoolTest()

        {
            Thread.Sleep(5000);
            Context.dmWeb.Message.Delete(Context.sendDeleteMID.ToString(), permanentlyDeleteCheck: false).GetAwaiter().GetResult();
        }

        [Test, Order(30)]
        [Category("Move Message")]
        public void MoveMessagePositiveTest()
        {
            string[] lines = System.IO.File.ReadAllLines("Data\\MessageData.txt");

            string str3 = lines[2];
            string[] linesplit3 = str3.Split(':');
            string toAddress = linesplit3[1];

            Context.moveMID = Context.dmWeb.Message.Send(new MessagingModels.SendMessage { To = { toAddress }, Subject = "Move message test" }).GetAwaiter().GetResult();
            Thread.Sleep(5000);

            int DFID = 2;

            Context.dmWeb.Message.Move(new MessagingModels.MessageOperations { MessageId = Context.moveMID, DestinationFolderId = DFID }).GetAwaiter().GetResult();
        }

        [Test, Order(31)]
        [Category("Get Message")]
        public void GetMessagePositiveTest()
        {
            string[] lines = System.IO.File.ReadAllLines("Data\\MessageData.txt");

            string str3 = lines[2];
            string[] linesplit3 = str3.Split(':');
            string toAddress = linesplit3[1];

            int getMID = Context.dmWeb.Message.Send(new MessagingModels.SendMessage { To = { toAddress }, Subject = "Get message test" }).GetAwaiter().GetResult();
            Thread.Sleep(5000);

            Context.dmWeb.Message.Get(getMID.ToString()).GetAwaiter().GetResult();
        }

        [Test, Order(32)]
        [Category("Get Message Metadata")]
        public void GetMessageMetadataPositiveTest()
        {
            string[] lines = System.IO.File.ReadAllLines("Data\\MessageData.txt");

            string str3 = lines[2];
            string[] linesplit3 = str3.Split(':');
            string toAddress = linesplit3[1];

            int getMetadataMID = Context.dmWeb.Message.Send(new MessagingModels.SendMessage { To = { toAddress }, Subject = "Get message test" }).GetAwaiter().GetResult();
            Thread.Sleep(5000);

            Context.dmWeb.Message.GetMessageMetadata(getMetadataMID.ToString()).GetAwaiter().GetResult();
        }


        [Test, Order(33)]
        [Category("Send Mime Message")]
        public void SendMimeMessagePositiveTest()
        {
            string[] lines = System.IO.File.ReadAllLines("Data\\MessageData.txt");

            string str3 = lines[2];
            string[] linesplit3 = str3.Split(':');
            string toAddress = linesplit3[1];

            string str4 = lines[3];
            string[] linesplit4 = str4.Split(':');
            string fromAddress = linesplit4[1];

            string location = Path.Combine(TestContext.CurrentContext.TestDirectory, @"Test Documents\test.txt");
            Context.mimeMessageId = Context.dmWeb.Message.SendMimeMessage(new MessagingModels.SendMessage { To = { toAddress }, From = fromAddress, Subject = "Mime Test WITH SESSION KEY", TextBody = "Mime Message Test" }, location).GetAwaiter().GetResult();
        }

        [Test, Order(34)]
        [Category("Get Mime Message")]
        public void GetMimeMessagePositiveTest()
        {
            Context.dmWeb.Message.GetaMimeMessage(Context.mimeMessageId).GetAwaiter().GetResult();
        }

        [Test]
        [Category("Details")]
        public void DisplayDetailsWithSessionKeyTest()
        {
            Context.dmWeb.Account.Details().GetAwaiter().GetResult();
        }

        [Test]
        [Category("ChangePassword")]
        [Ignore("Ignore test: Avoid changing password")]
        public void ChangePasswordPositiveTest()
        {
            Context.dmWeb.Account.ChangePassword(new AccountModels.ChangePassword { OldPassword = Context.password, NewPassword = "temp#pass" }).GetAwaiter().GetResult();
        }

        [Test]
        [Category("ChangePassword")]
        public void ChangePasswordEmptyFieldsTest()
        {
            try
            {
                Context.dmWeb.Account.ChangePassword(new AccountModels.ChangePassword { OldPassword = "", NewPassword = "" }).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("400"));
            }
        }

        [Test]
        [Category("ChangePassword")]
        public void ChangePasswordOnlyOldPasswordFieldTest()
        {
            try
            {
                Context.dmWeb.Account.ChangePassword(new AccountModels.ChangePassword { OldPassword = Context.password, NewPassword = "" }).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("400"));
            }
        }

        [Test]
        [Category("ChangePassword")]
        public void ChangePasswordOnlyNewPasswordFieldTest()
        {
            try
            {
                Context.dmWeb.Account.ChangePassword(new AccountModels.ChangePassword { OldPassword = "", NewPassword = "newtest#password" }).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("400"));
            }
        }

        [Test]
        [Category("ChangePassword")]
        public void ChangePasswordSamePasswordsTest()
        {
            try
            {
                Context.dmWeb.Account.ChangePassword(new AccountModels.ChangePassword { OldPassword = Context.password, NewPassword = Context.password }).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test]
        [Category("ChangePassword")]
        //Will send message saying password is changed despite remaining the same
        public void ChangePasswordInvalidFieldsTest()
        {
            try
            {
                Context.dmWeb.Account.ChangePassword(new AccountModels.ChangePassword { OldPassword = "false1", NewPassword = "false2" }).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test]
        [Category("ChangePassword")]
        public void ChangePasswordFalseOldPasswordOnlyTest()
        {
            try
            {
                Context.dmWeb.Account.ChangePassword(new AccountModels.ChangePassword { OldPassword = "false", NewPassword = "" }).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("400"));
            }
        }

        //May be same as OnlyNewPasswordFieldTest()
        [Test]
        [Category("ChangePassword")]
        public void ChangePasswordFalseNewPasswordOnlyTest()
        {
            try
            {
                Context.dmWeb.Account.ChangePassword(new AccountModels.ChangePassword { OldPassword = "", NewPassword = "false" }).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("400"));
            }
        }

        [Test]
        [Category("Create Folder")]
        public void CreateFolderEmptyFieldsTest()
        {
            try
            {
                Context.dmWeb.Folders.Create(new FolderModels.Create { }).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("400"));
            }
        }

        [Test]
        [Category("Create Folder")]
        public void CreateOnlyFolderNameTest()
        {
            try
            {
                Context.dmWeb.Folders.Create(new FolderModels.Create { FolderName = "Unit Test" }).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test]
        [Category("Create Folder")]
        public void CreateOnlyFolderTypeTest()
        {
            try
            {
                Context.dmWeb.Folders.Create(new FolderModels.Create { FolderType = 0 }).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("400"));
            }
        }

        [Test]
        [Category("Create Folder")]
        public void CreateFalseFolderIDTest()
        {
            try
            {
                Context.dmWeb.Folders.Create(new FolderModels.Create { FolderName = "UnitTest", FolderType = 15 }).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test]
        [Category("Delete Folder")]
        public void DeleteFolderInvalidFID()
        {
            try
            {
                Context.dmWeb.Folders.Delete("1234").GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }

        }

        [Test]
        [Category("List Folders")]
        public void ListFoldersWithSessionKey()
        {
            Context.dmWeb.Folders.List().GetAwaiter().GetResult();
        }

        [Test]
        [Category("Delete Message")]
        public void DeleteMessageTrueMIDTrueBoolTest()
        {
            string[] lines = System.IO.File.ReadAllLines("Data\\MessageData.txt");

            string str3 = lines[2];
            string[] linesplit3 = str3.Split(':');
            string toAddress = linesplit3[1];

            int mid = Context.dmWeb.Message.Send(new MessagingModels.SendMessage { To = { toAddress }, Subject = "True bool: Delete Message" }).GetAwaiter().GetResult();
            Thread.Sleep(5000);

            Context.dmWeb.Message.Delete(mid.ToString(), permanentlyDeleteCheck: true).GetAwaiter().GetResult();
        }

        [Test]
        [Category("Delete Message")]
        public void DeleteMessageFalseMIDFalseBoolTest()
        {
            int mid = 1561646;

            try
            {
                Context.dmWeb.Message.Delete(mid.ToString(), permanentlyDeleteCheck: false).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test]
        [Category("Delete Message")]
        public void DeleteMessageFalseMIDTrueBoolTest()
        {
            int MID = 36848563;
            try
            {
                Context.dmWeb.Message.Delete(MID.ToString(), permanentlyDeleteCheck: true).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test]
        [Category("Get Inbox MID")]
        public void GetInboxMIDNoFieldWithSessionKeyTest()
        {
            Context.dmWeb.Message.GetInboxMessageIds(new MessagingModels.GetInboxMIDRequest { }).GetAwaiter().GetResult();
        }

        [Test]
        [Category("Get Inbox MID")]
        public void GetInboxMIDAllFieldsTest()
        {
            Context.dmWeb.Message.GetInboxMessageIds(new MessagingModels.GetInboxMIDRequest { FolderId = 1, MessageListFilter = 0, MustHaveAttachments = false }).GetAwaiter().GetResult();
        }

        [Test]
        [Category("Get Inbox MID")]
        public void GetInboxMIDOnlyFIDTest()
        {
            Context.dmWeb.Message.GetInboxMessageIds(new MessagingModels.GetInboxMIDRequest { FolderId = 1 }).GetAwaiter().GetResult();
        }

        [Test]
        [Category("Get Inbox MID")]
        public void GetInboxMIDOnlyMLFTest()
        {
            Context.dmWeb.Message.GetInboxMessageIds(new MessagingModels.GetInboxMIDRequest { MessageListFilter = 1 }).GetAwaiter().GetResult();
        }

        [Test]
        [Category("Get Inbox MID")]
        public void GetInboxMIDOnlyMHATest()
        {
            Context.dmWeb.Message.GetInboxMessageIds(new MessagingModels.GetInboxMIDRequest { MustHaveAttachments = false }).GetAwaiter().GetResult();
        }

        [Test]
        [Category("Get Inbox MID")]
        public void GetInboxMIDFalseFIDTest()
        {
            try
            {
                Context.dmWeb.Message.GetInboxMessageIds(new MessagingModels.GetInboxMIDRequest { FolderId = 214 }).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test]
        [Category("Get Inbox MID")]
        public void GetInboxMIDFalseMHFTest()
        {
            try
            {
                Context.dmWeb.Message.GetInboxMessageIds(new MessagingModels.GetInboxMIDRequest { MessageListFilter = 12 }).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test]
        [Category("Get Message Metadata")]
        public void GetMessageMetadataInvalidMIDTest()
        {
            string invalidMIDString = 15612.ToString();
            try
            {
                Context.dmWeb.Message.GetMessageMetadata(invalidMIDString).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test]
        [Category("Get Message Summaries")]
        public void GetMessageSummariesPositiveTest()
        {
            Context.dmWeb.Message.GetMessageSummaries(new MessagingModels.GetMessageSummariesRequest { FolderId = 1, LastMessageIDReceived = 22722701 }).GetAwaiter().GetResult();
        }

        [Test]
        [Category("Get Message Summaries")]
        public void GetMessageSummariesOnlyFIDTest()
        {
            Context.dmWeb.Message.GetMessageSummaries(new MessagingModels.GetMessageSummariesRequest { FolderId = 1 }).GetAwaiter().GetResult();
        }

        [Test]
        [Category("Get Message Summaries")]
        public void GetMessageSummariesInvalidFieldsTest()
        {
            try
            {
                Context.dmWeb.Message.GetMessageSummaries(new MessagingModels.GetMessageSummariesRequest { FolderId = 12, LastMessageIDReceived = 12 }).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test]
        [Category("Get Message Summaries")]
        public void GetMessageSummariesFalseFIDTest()
        {
            try
            {
                Context.dmWeb.Message.GetMessageSummaries(new MessagingModels.GetMessageSummariesRequest { FolderId = 12 }).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test]
        public void GetMessageInvalidMIDTest()
        {
            string invalidMIDString = 15612.ToString();

            try
            {
                Context.dmWeb.Message.Get(invalidMIDString).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test]
        [Category("Get Mime Message")]
        public void GetMimeMessageFalseMIDTest()
        {
            try
            {
                string messageId = 36896182.ToString();

                Context.dmWeb.Message.GetaMimeMessage(messageId).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test]
        [Category("Move Message")]
        public void MoveMessageMIDOnlyTest()
        {
            int MID = 36848566;

            try
            {
                Context.dmWeb.Message.Move(new MessagingModels.MessageOperations { MessageId = MID }).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test]
        [Category("Move Message")]
        public void MoveMessageFIDOnlyTest()
        {
            int DFID = 1;

            try
            {
                Context.dmWeb.Message.Move(new MessagingModels.MessageOperations { DestinationFolderId = DFID }).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                //400 status code because missing MID from model
                Assert.IsTrue(ex.Message.Contains("400"));
            }
        }

        [Test]
        [Category("Move Message")]
        public void MoveMessageTrueMIDFalseFIDTest()
        {
            int MID = 36848566;

            try
            {
                Context.dmWeb.Message.Move(new MessagingModels.MessageOperations { MessageId = MID, DestinationFolderId = 15143 }).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test]
        [Category("Move Message")]
        public void MoveMessageTrueFIDFalseMIDTest()
        {
            int MID = 36848541;
            int DFID = 1;

            try
            {
                Context.dmWeb.Message.Move(new MessagingModels.MessageOperations { MessageId = MID, DestinationFolderId = DFID }).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test]
        [Category("Move Message")]
        public void MoveMessageFalseMIDFalseFIDTest()
        {
            int MID = 36848541;
            int DFID = 15513;

            try
            {
                Context.dmWeb.Message.Move(new MessagingModels.MessageOperations { MessageId = MID, DestinationFolderId = DFID }).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test]
        [Category("Retract Message")]
        public void RetractMessagePositiveTest()
        {
            string[] lines = System.IO.File.ReadAllLines("Data\\MessageData.txt");

            string str3 = lines[2];
            string[] linesplit3 = str3.Split(':');
            string toAddress = linesplit3[1];


            int MID = Context.dmWeb.Message.Send(new MessagingModels.SendMessage { To = { toAddress }, Subject = "Retract Message Test" }).GetAwaiter().GetResult();

            Context.dmWeb.Message.Retract(new MessagingModels.MessageOperations { MessageId = MID }).GetAwaiter().GetResult();
        }

        [Test]
        [Category("Retract Message")]
        public void RetractMessageFalseMIDTest()
        {
            int MID = 36860224;

            try
            {
                Context.dmWeb.Message.Retract(new MessagingModels.MessageOperations { MessageId = MID }).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test]
        [Category("Search Inbox")]
        public void SearchInboxPositiveTest()
        {
            Context.dmWeb.Message.SearchInbox(new MessagingModels.SearchInbox { PageNum = 1, PageSize = 1 }).GetAwaiter().GetResult();
        }

        [Test]
        [Category("Search Inbox")]
        public void SearchInboxOnlyPageNumTest()
        {
            try
            {
                Context.dmWeb.Message.SearchInbox(new MessagingModels.SearchInbox { PageNum = 1 }).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test]
        [Category("Search Inbox")]
        public void SearchInboxOnlyPageSizeTest()
        {
            try
            {
                Context.dmWeb.Message.SearchInbox(new MessagingModels.SearchInbox { PageSize = 1 }).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                //Does not return error because information is still returned
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test]
        [Category("Search Inbox")]
        public void SearchInboxFalsePageNumTest()
        {
            try
            {
                Context.dmWeb.Message.SearchInbox(new MessagingModels.SearchInbox { PageNum = 1561, PageSize = 1 }).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                //No exception caught because API automatically goes to the last page
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test]
        [Category("Search Inbox")]
        public void SearchInboxFalsePageSizeTest()
        {
            try
            {
                Context.dmWeb.Message.SearchInbox(new MessagingModels.SearchInbox { PageNum = 1, PageSize = 1561 }).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test]
        [Category("Search Inbox")]
        public void SearchInboxNegativeTest()
        {
            try
            {
                Context.dmWeb.Message.SearchInbox(new MessagingModels.SearchInbox { PageNum = 1561, PageSize = 1561 }).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test]
        [Category("Search Inbox")]
        public void SearchInboxFalseFIDTest()
        {
            try
            {
                Context.dmWeb.Message.SearchInbox(new MessagingModels.SearchInbox { PageNum = 1, PageSize = 1, FolderId = 1565 }).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("401"));
            }
        }

        [Test]
        [Category("Search Inbox")]
        public void SearchInboxGetInboxUnReadOnlyTrueTest()
        {
            Context.dmWeb.Message.SearchInbox(new MessagingModels.SearchInbox { PageNum = 1, PageSize = 1, GetInboxUnReadOnly = true }).GetAwaiter().GetResult();
        }

        [Test]
        [Category("Search Inbox")]
        public void SearchInboxGetInboxUnReadOnlyFalseTest()
        {
            Context.dmWeb.Message.SearchInbox(new MessagingModels.SearchInbox { PageNum = 1, PageSize = 1, GetInboxUnReadOnly = false }).GetAwaiter().GetResult();
        }

        [Test]
        [Category("Search Inbox")]
        public void SearchInboxGetRetractedMsgsTrueTest()
        {
            Context.dmWeb.Message.SearchInbox(new MessagingModels.SearchInbox { PageNum = 1, PageSize = 1, GetRetractedMsgs = true }).GetAwaiter().GetResult();
        }

        [Test]
        [Category("Search Inbox")]
        public void SearchInboxGetRetractedMsgsFalseTest()
        {
            Context.dmWeb.Message.SearchInbox(new MessagingModels.SearchInbox { PageNum = 1, PageSize = 1, GetRetractedMsgs = false }).GetAwaiter().GetResult();
        }

        [Test]
        [Category("Search Inbox")]
        public void SearchInboxOrderDescTrueTest()
        {
            Context.dmWeb.Message.SearchInbox(new MessagingModels.SearchInbox { PageNum = 1, PageSize = 1, OrderDesc = true }).GetAwaiter().GetResult();
        }

        [Test]
        [Category("Search Inbox")]
        public void SearchInboxOrderDescFalseTest()
        {
            Context.dmWeb.Message.SearchInbox(new MessagingModels.SearchInbox { PageNum = 1, PageSize = 1, OrderDesc = false }).GetAwaiter().GetResult();
        }

        [Test]
        [Category("Send Mime Message")]
        public void SendMimeMessageNoToAddressTest()
        {
            try
            {
                string[] lines = System.IO.File.ReadAllLines("Data\\MessageData.txt");

                string str4 = lines[3];
                string[] linesplit4 = str4.Split(':');
                string fromAddress = linesplit4[1];

                string location = Path.Combine(TestContext.CurrentContext.TestDirectory, @"Test Documents\test.txt");
                Context.dmWeb.Message.SendMimeMessage(new MessagingModels.SendMessage { From = fromAddress, Subject = "No To Address", TextBody = "Mime Message Test" }, location).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                Assert.IsTrue(ex.Message.Contains("400"));
            }
        }

        [Test]
        [Category("Get Unread Messages")]
        public void GetUnreadMessagesLastMIDFalsePositiveTest()
        {
            Context.dmWeb.Message.GetUnreadMessages(LastMIDReceived: false, MID: 0.ToString()).GetAwaiter().GetResult();
        }

        [Test]
        [Category("Get Unread Messages")]
        public void GetUnreadMessagesLastMIDTrueMIDTrueTest()
        {
            Context.dmWeb.Message.GetUnreadMessages(LastMIDReceived: true, MID: 0.ToString()).GetAwaiter().GetResult();
        }

        [Test]
        [Category("Get Unread Messages")]
        public void GetUnreadMessagesLastMIDTrueMIDFalseTest()
        {
            try
            {
                Context.dmWeb.Message.GetUnreadMessages(LastMIDReceived: true, MID: 241.ToString()).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                //Does not return error
                //Instead MessageSummaries is empty
                Assert.IsTrue(ex.Message.Contains("200"));
            }
        }

        [OneTimeTearDown]
        [Category("LogOut")]
        public void LogOutWithSessionKey()
        {
            Context.dmWeb.Account.LogOut().GetAwaiter().GetResult();
        }
    }
}

