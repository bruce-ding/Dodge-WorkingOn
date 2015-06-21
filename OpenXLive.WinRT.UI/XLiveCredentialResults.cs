using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenXLive.WinRT.UI
{
    /// <summary>
    /// Describes the results of the dialog box operation.
    /// </summary>
    public sealed class XLiveCredentialResults
    {
        public XLiveCredentialResults()
        {
        }

        public XLiveCredentialResults(string userName, string password, bool? remember)
        {
            this.CredentialUserName = userName;
            this.CredentialPassword = password;

            if (remember != null)
                this.CredentialSaved = (bool)remember;
            else
                this.CredentialSaved = false;
        }

        public XLiveCredentialResults(string email, string password, string confirmPassword, string name)
        {
            this.NewAccountEmail = email;
            this.NewAccountPassword = password;
            this.NewAccountConfirmPassword = confirmPassword;
            this.NewAccountUserName = name;
        }

        /// <summary>
        /// Gets the user name of the unpacked credential.
        /// </summary>
        public string CredentialUserName { get; private set; }
        /// <summary>
        /// Gets the password portion of the unpacked credential.
        /// </summary>
        public string CredentialPassword { get; private set; }
        /// <summary>
        /// Gets the status of the credential save operation. 
        /// </summary>
        public bool CredentialSaved { get; private set; }
        /// <summary>
        /// Gets the new account email of the unpacked credential
        /// </summary>
        public string NewAccountEmail { get; private set; }
        /// <summary>
        /// Gets the new account password of the unpacked credential
        /// </summary>
        public string NewAccountPassword { get; private set; }
        /// <summary>
        /// Gets the new account confirm password of the unpacked credential
        /// </summary>
        public string NewAccountConfirmPassword { get; private set; }
        /// <summary>
        /// Gets the new account user name of the unpacked credential
        /// </summary>
        public string NewAccountUserName { get; private set; }

        public MyPlayer player { get; set; }
    }
}
