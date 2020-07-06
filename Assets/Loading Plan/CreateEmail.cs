using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class CreateEmail : MonoBehaviour
{
    const string senderAddress = "optcapps001@gmail.com";
    const string senderPwd = "imanentdespair";
    const string receiverAddress = "mybleachemail@yahoo.com";   //Add variable to be set when function is called

    public static void SendAnEmail(string message, string company)
    {
        // Create mail
        MailMessage mail = new MailMessage();
        mail.From = new MailAddress(senderAddress);
        mail.To.Add(receiverAddress);
        mail.Subject = "Loading Plan " + company;   //Add variable to be set when function is called
        mail.Body = message;
        mail.Attachments.Add(new Attachment("Assets/Loading Plan/Loading Plan.html"));  //Add variable to be set when function is called

        // Setup server 
        SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
        smtpServer.Port = 587;
        smtpServer.Credentials = new NetworkCredential(
            senderAddress, senderPwd) as ICredentialsByHost;
        smtpServer.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback =
            delegate (object s, X509Certificate certificate,
            X509Chain chain, SslPolicyErrors sslPolicyErrors) {
                Debug.Log("Email success!");
                return true;
            };

        // Send mail to server, print results
        try
        {
            smtpServer.Send(mail);
        }
        catch (System.Exception e)
        {
            Debug.Log("Email error: " + e.Message);
        }
        finally
        {
            Debug.Log("Email sent!");
        }
    }
}