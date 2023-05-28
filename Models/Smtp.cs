using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace travelManagement.Models
{
    public class Smtp
    {
        static bool mailSent = false;
        [Obsolete]
        public bool SendEmail(string email, string msg)
        {
            SmtpClient smtp = new SmtpClient();
            MailMessage message = new MailMessage();
            try
            {
                //設定smtp主機
                string smtpAddress = "smtp.gmail.com";
                //設定Port
                int portNumber = 587;
                bool enableSSL = true;
                //填入寄送方email和密碼
                string emailFrom = "george00157127@gmail.com";
                string password = "George831119";
                //收信方email 可以用逗號區分多個收件人
                string emailTo = email;
                //主旨
                string subject = "小菜旅遊會員驗證信";
                //內容
                string body = msg;

                MailAddress fromAddress = new MailAddress(emailFrom);
                MailAddress toAddress = new MailAddress(emailTo);
                message.From = fromAddress;
                message.To.Add(emailTo);
                message.Subject = subject;
                message.Body = body;
                // 若你的內容是HTML格式，則為True
                message.IsBodyHtml = false;

                //如果需要夾帶檔案
                //mail.Attachments.Add(new Attachment("C:\\SomeFile.txt"));
                //mail.Attachments.Add(new Attachment("C:\\SomeZip.zip"));
                //using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                //{
                    smtp.Host = smtpAddress;
                    smtp.Port = portNumber;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(emailFrom, password);
                    smtp.EnableSsl = enableSSL;
                    smtp.Send(message);
                //}
            }
            catch (SmtpFailedRecipientsException sfrEx)
            {
                // TODO: Handle exception
                // When email could not be delivered to all receipients.
                Console.WriteLine("Exception caught in CreateTestMessage2(): {0}",
                    sfrEx.ToString());
                return false;
            }
            catch (SmtpException sEx)
            {
                // TODO: Handle exception
                // When SMTP Client cannot complete Send operation.
                Console.WriteLine("Exception caught in CreateTestMessage2(): {0}",
                    sEx.ToString());
                return false;
            }
            catch (Exception ex)
            {
                // TODO: Handle exception
                // Any exception that may occur during the send process.
                Console.WriteLine("Exception caught in CreateTestMessage2(): {0}",
                    ex.ToString());
                return false;
            }
            return true;
        }


        public bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public bool checkDNS(string host, string recType = "MX")
        {
            bool result = true;
            try
            {
                using (Process proc = new Process())
                {
                    proc.StartInfo.FileName = "nslookup";
                    proc.StartInfo.Arguments = string.Format("-type={0} {1}", recType, host);
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.ErrorDialog = false;
                    proc.StartInfo.RedirectStandardError = true;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    proc.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
                    {
                        if ((e.Data != null) && (!result))
                            result = e.Data.StartsWith(host);
                    };
                    proc.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
                    {
                        if (e.Data != null)
                        {
                            //read error output here, not sure what for?
                        }
                    };
                    proc.Start();
                    proc.BeginErrorReadLine();
                    proc.BeginOutputReadLine();
                    proc.WaitForExit(30000); //timeout after 30 seconds.
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }
    }
}
