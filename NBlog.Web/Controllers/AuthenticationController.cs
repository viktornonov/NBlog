﻿using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
using DotNetOpenAuth.OpenId.RelyingParty;
using NBlog.Web.Application;
using NBlog.Web.Application.Infrastructure;
using NBlog.Web.Application.Service;
using NBlog.Web.Models;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;

namespace NBlog.Web.Controllers
{
    public partial class AuthenticationController : LayoutController
    {
        public AuthenticationController(IServices services)
            : base(services)
        {
        }

        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            var model = new Login { ReturnUrl = returnUrl.AsNullIfEmpty() ?? Url.Action("Index", "Home") };
            return View(model);
        }

        [HttpGet]
        public ActionResult Logout(string returnUrl)
        {
            FormsAuthentication.SignOut();

            var url = returnUrl.AsNullIfEmpty() ?? Url.Action("Index", "Home");
            return Redirect(url);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OpenId(Login model)
        {
            Identifier id;
            if (Identifier.TryParse(model.OpenID_Identifier, out id))
            {
                try
                {
                    var openId = new OpenIdRelyingParty();
                    var returnToUrl = new Uri(Url.Action("OpenIdCallback", "Authentication", new { ReturnUrl = model.ReturnUrl }, Request.Url.Scheme), UriKind.Absolute);
                    var request = openId.CreateRequest(id, Realm.AutoDetect, returnToUrl);

                    request.AddExtension(new ClaimsRequest
                    {
                        Email = DemandLevel.Require,
                        FullName = DemandLevel.Require,
                        Nickname = DemandLevel.Require
                    });

                    // also add AX request
                    var axRequest = new FetchRequest();
                    axRequest.Attributes.AddRequired(WellKnownAttributes.Name.FullName);
                    axRequest.Attributes.AddRequired(WellKnownAttributes.Name.First);
                    axRequest.Attributes.AddRequired(WellKnownAttributes.Name.Last);
                    axRequest.Attributes.AddRequired(WellKnownAttributes.Contact.Email);
                    request.AddExtension(axRequest);

                    return request.RedirectingResponse.AsActionResult();
                }
                catch (ProtocolException ex)
                {
                    model.Message = ex.Message;
                    return View("Login", model);
                }
            }
            else
            {
                model.Message = "Invalid identifier";
                return View("Login", model);
            }
        }

        [HttpGet]
        [ValidateInput(false)]
        public ActionResult OpenIdCallback(string returnUrl)
        {
            var model = new Login { ReturnUrl = returnUrl };
            var openId = new OpenIdRelyingParty();
            var openIdResponse = openId.GetResponse();

            if (openIdResponse.Status == AuthenticationStatus.Authenticated)
            {
                var friendlyName = GetFriendlyName(openIdResponse);

                var isPersistentCookie = true;
                SetAuthCookie(openIdResponse.ClaimedIdentifier, isPersistentCookie, friendlyName);

                return Redirect(returnUrl.AsNullIfEmpty() ?? Url.Action("Index", "Home"));
            }

            model.Message = "Sorry, login failed.";
            return View("Login", model);
        }

        private void SetAuthCookie(string username, bool createPersistentCookie, string userData)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException("username");

            var authenticationConfig =
                (AuthenticationSection)WebConfigurationManager.GetWebApplicationSection("system.web/authentication");

            var timeout = (int)authenticationConfig.Forms.Timeout.TotalMinutes;
            var expiry = DateTime.Now.AddMinutes((double)timeout);

            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(2,
              username,
              DateTime.Now,
              expiry,
              createPersistentCookie,
              userData,
              FormsAuthentication.FormsCookiePath);

            string encryptedTicket = FormsAuthentication.Encrypt(ticket);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName)
            {
                Value = encryptedTicket,
                HttpOnly = true,
                Secure = authenticationConfig.Forms.RequireSSL
            };

            if (ticket.IsPersistent)
                cookie.Expires = ticket.Expiration;

            Response.Cookies.Add(cookie);
        }

        private string GetFriendlyName(IAuthenticationResponse authResponse)
        {
            string friendlyName = "";

            var sregResponse = authResponse.GetExtension<ClaimsResponse>();
            var axResponse = authResponse.GetExtension<FetchResponse>();

            if (sregResponse != null)
            {
                friendlyName =
                    sregResponse.FullName.AsNullIfEmpty() ??
                    sregResponse.Nickname.AsNullIfEmpty() ??
                    sregResponse.Email;
            }
            else if (axResponse != null)
            {
                var fullName = axResponse.GetAttributeValue(WellKnownAttributes.Name.FullName);
                var firstName = axResponse.GetAttributeValue(WellKnownAttributes.Name.First);
                var lastName = axResponse.GetAttributeValue(WellKnownAttributes.Name.Last);
                var email = axResponse.GetAttributeValue(WellKnownAttributes.Contact.Email);

                friendlyName =
                    fullName.AsNullIfEmpty() ??
                    ((!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName)) ? firstName + " " + lastName : null) ??
                    email;
            }

            if (string.IsNullOrEmpty(friendlyName))
                friendlyName = authResponse.FriendlyIdentifierForDisplay;

            return friendlyName;
        }
    }
}