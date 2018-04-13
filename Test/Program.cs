using System;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Messaging_Library.Models;
using DMWeb_REST;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            DMWeb dmWeb = new DMWeb();

            AccountModels.LogOn user = new AccountModels.LogOn();
            user.UserIdOrEmail = "christopherl@datamotion.com";
            user.Password = "32Endview!";

            string sessKey = dmWeb.Account.LogOn(user);

            for (int i = 0; i < 2; i++)
            {
                int sent = dmWeb.Message.Send(new MessagingModels.SendMessage { To = { "christopherl@datamotion.com" }, Subject = "Unit Testing is defeating me #" + i, TextBody = "Send help pls." });
                Console.WriteLine(sent);
            }
            Console.WriteLine(sessKey);
            Console.ReadLine();
        }
    }
}
