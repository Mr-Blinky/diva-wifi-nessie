/**
 * Copyright (c) Crista Lopes (aka Diva) and Sam Kaufman. All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, 
 * are permitted provided that the following conditions are met:
 * 
 *     * Redistributions of source code must retain the above copyright notice, 
 *       this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright notice, 
 *       this list of conditions and the following disclaimer in the documentation 
 *       and/or other materials provided with the distribution.
 *     * Neither the name of the Organizations nor the names of Individual
 *       Contributors may be used to endorse or promote products derived from 
 *       this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES 
 * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL 
 * THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, 
 * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
 * GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED 
 * AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED 
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

using Nini.Config;
using log4net;
using OpenMetaverse;
using Nwc.XmlRpc;

using OpenSim.Framework;
using OpenSim.Framework.Servers.HttpServer;
using OpenSim.Services.Interfaces;
using OpenSim.Services.InventoryService;
using OpenSim.Services.GridService;

using Diva.Wifi.WifiScript;
using Environment = Diva.Utils.Environment;
using GridRegion = OpenSim.Services.Interfaces.GridRegion;

using Diva.Utils;
using Diva.OpenSimServices;

namespace Diva.Wifi
{
    public partial class Services
    {
        public string ForgotPasswordGetRequest(Environment env)
        {
            m_log.DebugFormat("[Wifi]: ForgotPasswordGetRequest");
            env.State = State.ForgotPassword;
            return m_WebApp.ReadFile(env, "index.html");
        }

        public string ForgotPasswordPostRequest(Environment env, string firstName, string lastName, string email)
        {
            // Several avatars may share one email address, so identify the avatar by name
            // and only proceed if the email matches the one registered for it.
            UserAccount account = null;
            if (!string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName) && !string.IsNullOrWhiteSpace(email))
                account = m_UserAccountService.GetUserAccount(UUID.Zero, firstName.Trim(), lastName.Trim());
            if (account != null && !string.Equals(account.Email?.Trim(), email.Trim(), StringComparison.OrdinalIgnoreCase))
                account = null;

            if (account != null)
            {
                string token = m_AuthenticationService.GetToken(account.PrincipalID, 60);
                if (token != string.Empty)
                {
                    string url = m_WebApp.WebAddress + "/wifi/recover/" + token + "?uid=" + account.PrincipalID;

                    string message = string.Format("\n{0}\n{1}\n{2}",
                        string.Format(_("Your account is {0} {1}", env), account.FirstName, account.LastName),
                        _("Click here to reset your password:", env),
                        url);
                    SendEMail(email, _("Password Reset", env), message);
                    NotifyWithoutButton(env, _("Check your email. You must reset your password within 60 minutes.", env));

                    return m_WebApp.ReadFile(env, "index.html");
                }
            }

            // Same response for unknown accounts to avoid disclosing registered details.
            NotifyWithoutButton(env, _("Check your email. You must reset your password within 60 minutes.", env));
            return m_WebApp.ReadFile(env, "index.html");
        }


        public string RecoverPasswordGetRequest(Environment env, string userID, string token)
        {
            UserAccount account = null;
            if (IsValidToken(userID, token, out account))
            {
                PasswordRecoveryData precovery = new PasswordRecoveryData(account.PrincipalID.ToString(), token);
                env.Data = new List<object>();
                env.Data.Add(precovery);
                env.State = State.RecoveringPassword;
                return m_WebApp.ReadFile(env, "index.html");
            }
            else
            {
                return "<p>" + _("Invalid token.", env) + "</p>";
            }
        }

        public string RecoverPasswordPostRequest(Environment env, string userID, string token, string newPassword)
        {
            if (newPassword == null || newPassword.Length < 3)
            {
                env.State = State.RecoveringPassword;
                return m_WebApp.ReadFile(env, "index.html");
            }

            if (ResetPassword(userID, token, newPassword))
                NotifyWithoutButton(env, _("Your password has been reset.", env));
            else
                NotifyWithoutButton(env, _("Invalid token.", env));

            return m_WebApp.ReadFile(env, "index.html");
        }

        private bool ResetPassword(string userID, string token, string newPassword)
        {
            UserAccount account = null;
            if (!IsValidToken(userID, token, out account))
            {
                m_log.WarnFormat("[ForgotPasswordService]: Invalid or expired recovery token for account uuid:{0}.", userID);
                return false;
            }

            bool success = m_AuthenticationService.SetPassword(account.PrincipalID, newPassword);
            if (!success)
                m_log.ErrorFormat("[ForgotPasswordService]: Unable to reset password for account uuid:{0}.", account.PrincipalID);
            else
                m_log.InfoFormat("[ForgotPasswordService]: Password reset for account uuid:{0}", account.PrincipalID);
            return success;
        }

        // The recovery link identifies the account by UUID, because an email address
        // can belong to several avatars.
        private bool IsValidToken(string userID, string token, out UserAccount account)
        {
            account = null;
            UUID principalID;
            if (!UUID.TryParse(userID, out principalID))
                return false;

            account = m_UserAccountService.GetUserAccount(UUID.Zero, principalID);
            if (account != null)
                return (m_AuthenticationService.Verify(account.PrincipalID, token, 15));

            return false;
        }

    }

    class PasswordRecoveryData
    {
        public string UserID;
        public string Token;

        public PasswordRecoveryData(string u, string t)
        {
            UserID = u; Token = t;
        }
    }
}
