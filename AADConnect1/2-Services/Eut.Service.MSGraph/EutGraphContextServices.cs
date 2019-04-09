/* 
*  Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. 
*  See LICENSE in the source repository root for complete license information. 
*/

using Eut.Entity.Common;
using Microsoft.Graph;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Eut.Service.MSGraph
{
    public partial class EutGraphContext: IEutServiceContext
    {
        // Load user's profile in formatted JSON.
        public async Task<string> GetUserJson(string email)
        {
            if (email == null) return JsonConvert.SerializeObject(new { Message = "Email address cannot be null." }, Formatting.Indented);

            try
            {
                // Load user profile.
                var user = await _graphClient.Users[email].Request().GetAsync();
                return JsonConvert.SerializeObject(user, Formatting.Indented);
            }
            catch (ServiceException e)
            {
                switch (e.Error.Code)
                {
                    case "Request_ResourceNotFound": throw new EutGraphException($"User '{email}' was not found.", e);
                    case "ResourceNotFound": throw new EutGraphException($"User '{email}' was not found.", e);
                    case "ErrorItemNotFound": throw new EutGraphException($"User '{email}' was not found.", e);
                    case "itemNotFound": throw new EutGraphException($"User '{email}' was not found.", e);
                    case "ErrorInvalidUser": throw new EutGraphException($"The requested user '{email}' is invalid.", e);
                    case "AuthenticationFailure": throw new EutGraphException(e.Error.Message, e);
                    case "TokenNotFound": throw new EutGraphException(e.Error.Message, e);
                    default: throw new EutGraphException("Error while getting user as JSON", e);
                }
            }
        }

        // Load user's profile picture in base64 string.
        public async Task<string> GetPictureBase64(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email)) { throw new ArgumentNullException("email"); }
                var pictureStream = await GetPictureStream(email);
                var pictureMemoryStream = new MemoryStream();
                await pictureStream.CopyToAsync(pictureMemoryStream);
                var pictureByteArray = pictureMemoryStream.ToArray();
                return "data:image/jpeg;base64," + Convert.ToBase64String(pictureByteArray);
            }
            catch (Exception e)
            {
                if(e.Message == "ResourceNotFound") { return "data:image/svg+xml;base64,PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiPz4NCjwhRE9DVFlQRSBzdmcgIFBVQkxJQyAnLS8vVzNDLy9EVEQgU1ZHIDEuMS8vRU4nICAnaHR0cDovL3d3dy53My5vcmcvR3JhcGhpY3MvU1ZHLzEuMS9EVEQvc3ZnMTEuZHRkJz4NCjxzdmcgd2lkdGg9IjQwMXB4IiBoZWlnaHQ9IjQwMXB4IiBlbmFibGUtYmFja2dyb3VuZD0ibmV3IDMxMi44MDkgMCA0MDEgNDAxIiB2ZXJzaW9uPSIxLjEiIHZpZXdCb3g9IjMxMi44MDkgMCA0MDEgNDAxIiB4bWw6c3BhY2U9InByZXNlcnZlIiB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciPg0KPGcgdHJhbnNmb3JtPSJtYXRyaXgoMS4yMjMgMCAwIDEuMjIzIC00NjcuNSAtODQzLjQ0KSI+DQoJPHJlY3QgeD0iNjAxLjQ1IiB5PSI2NTMuMDciIHdpZHRoPSI0MDEiIGhlaWdodD0iNDAxIiBmaWxsPSIjRTRFNkU3Ii8+DQoJPHBhdGggZD0ibTgwMi4zOCA5MDguMDhjLTg0LjUxNSAwLTE1My41MiA0OC4xODUtMTU3LjM4IDEwOC42MmgzMTQuNzljLTMuODctNjAuNDQtNzIuOS0xMDguNjItMTU3LjQxLTEwOC42MnoiIGZpbGw9IiNBRUI0QjciLz4NCgk8cGF0aCBkPSJtODgxLjM3IDgxOC44NmMwIDQ2Ljc0Ni0zNS4xMDYgODQuNjQxLTc4LjQxIDg0LjY0MXMtNzguNDEtMzcuODk1LTc4LjQxLTg0LjY0MSAzNS4xMDYtODQuNjQxIDc4LjQxLTg0LjY0MWM0My4zMSAwIDc4LjQxIDM3LjkgNzguNDEgODQuNjR6IiBmaWxsPSIjQUVCNEI3Ii8+DQo8L2c+DQo8L3N2Zz4NCg==";  }
                else { throw new EutGraphException("Error while getting picture as base 64", e);  }
            }
        }

        public async Task<Stream> GetPictureStream(string email)
        {
            if (email == null) throw new ArgumentNullException("email");
            Stream pictureStream = null;
            try
            {
                try { pictureStream = await _graphClient.Users[email].Photo.Content.Request().GetAsync(); }
                catch (ServiceException e)
                {
                    if (e.Error.Code == "GetUserPhoto") // User is using MSA, we need to use beta endpoint
                    {
                        await RunWithBetaVersion(async () => { pictureStream = await _graphClient.Users[email].Photo.Content.Request().GetAsync(); });
                    }
                }
            }
            catch (ServiceException e)
            {
                switch (e.Error.Code)
                {
                    case "Request_ResourceNotFound": throw new EutGraphException("Resource not found", e);
                    case "ResourceNotFound": throw new EutGraphException("Resource not found", e);
                    case "ErrorItemNotFound": throw new EutGraphException("Resource not found", e);
                    case "itemNotFound": throw new EutGraphException("Resource not found", e);
                    case "ErrorInvalidUser": throw new EutGraphException("Resource not found", e);
                    default: return null;
                }
            }

            return pictureStream;
        }
        public async Task<Stream> GetMyPictureStream()
        {
            Stream pictureStream = null;

            try
            {
                try { pictureStream = await _graphClient.Me.Photo.Content.Request().GetAsync(); }
                catch (ServiceException e)
                {
                    if (e.Error.Code == "GetUserPhoto") // User is using MSA, we need to use beta endpoint
                    {
                        await RunWithBetaVersion(async () => { pictureStream = await _graphClient.Me.Photo.Content.Request().GetAsync(); });
                    }
                }
            }
            catch (ServiceException e)
            {
                switch (e.Error.Code)
                {
                    case "Request_ResourceNotFound": throw new EutGraphException("Resource not found");
                    case "ResourceNotFound": throw new EutGraphException("Resource not found");
                    case "ErrorItemNotFound": throw new EutGraphException("Resource not found");
                    case "itemNotFound": throw new EutGraphException("Resource not found");
                    case "ErrorInvalidUser": throw new EutGraphException("Resource not found");
                    default: return null;
                }
            }
            return pictureStream;
        }

        // Send an email message from the current user.
        public async Task SendEmail(string bodyHtml, string recipients)
        {
            try
            {
                if (recipients == null) return;
                var attachments = new MessageAttachmentsCollectionPage();
                try
                {
                    var pictureStream = await GetMyPictureStream();
                    var pictureMemoryStream = new MemoryStream();
                    await pictureStream.CopyToAsync(pictureMemoryStream);
                    attachments.Add(new FileAttachment
                    {
                        ODataType = "#microsoft.graph.fileAttachment",
                        ContentBytes = pictureMemoryStream.ToArray(),
                        ContentType = "image/png",
                        Name = "me.png"
                    });
                }
                catch (Exception e) { if(e.Message != "ResourceNotFound") { throw; } }

                // Prepare the recipient list.
                var splitRecipientsString = recipients.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                var recipientList = splitRecipientsString.Select(recipient => new Recipient
                {
                    EmailAddress = new EmailAddress
                    {
                        Address = recipient.Trim()
                    }
                }).ToList();

                // Build the email message.
                var email = new Message
                {
                    Body = new ItemBody
                    {
                        Content = bodyHtml,
                        ContentType = BodyType.Html,
                    },
                    Subject = "Sent from the Microsoft Graph Connect sample",
                    ToRecipients = recipientList,
                    Attachments = attachments
                };

                await _graphClient.Me.SendMail(email, true).Request().PostAsync();
            }
            catch(Exception e) { throw new EutGraphException("Error while sending email", e); }
        }
    }
}
