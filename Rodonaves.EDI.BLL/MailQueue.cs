using Rodonaves.Core.Bll;
using Rodonaves.Core.Interfaces;
using Rodonaves.Core.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text;

namespace Rodonaves.EDI.BLL
{
    public class MailQueue : Mail
    {
        private readonly GED _ged;

        public MailQueue( IMailQueue mailQueue, GED ged) : base(mailQueue)
        {
            _ged = ged;
        }

        protected override void SetAttachments(MailboxHeaderInfo sender, ref MailMessage mail)
        {
            foreach (var attachment in sender.Attachments)
            {
                var provider = sender.Origin.ToString().Equals("EDI") == false ? sender.Origin.ToString() : (attachment.Attachment.StartsWith(@"\\") ? "OUT" : "EDI");

                var obj = new object[,]
                {
                    { attachment.Attachment, attachment.Attachment, provider }
                };

                var localFile = _ged.Dowload(obj);

                var attachmentItem = new Attachment(localFile);

                if (Path.GetExtension(attachment.Attachment).ToLower() == ".xml" && Path.GetFileNameWithoutExtension(attachment.Attachment).Length > 48 && attachment.Attachment.Contains("-cte"))
                {
                    attachmentItem.Name = Path.GetFileNameWithoutExtension(attachment.Attachment).Substring(Path.GetFileNameWithoutExtension(attachment.Attachment).Length - 48, 48) + Path.GetExtension(attachment.Attachment);
                }
                else if (Path.GetExtension(attachment.Attachment).ToLower() == ".pdf" && Path.GetFileNameWithoutExtension(attachment.Attachment).Length > 20 && attachment.Attachment.Contains("DACTE"))
                {
                    attachmentItem.Name = Path.GetFileNameWithoutExtension(attachment.Attachment).Substring(Path.GetFileNameWithoutExtension(attachment.Attachment).IndexOf("DACTE", 0), Path.GetFileNameWithoutExtension(attachment.Attachment).Length - Path.GetFileNameWithoutExtension(attachment.Attachment).IndexOf("DACTE", 0)) + Path.GetExtension(attachment.Attachment);
                }

                mail.Attachments.Add(attachmentItem);
            }
        }
    }
}
