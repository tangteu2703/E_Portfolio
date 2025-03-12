using System.Security.Cryptography;
using System.Text;

namespace WDI_CMS_Common
{
    public class E_Common
    {
        public static string sKey = "KEY_@wdi_mms_2024";
        //public string SetAuthCookie(HttpContext httpContext, string authenticationTicket, string cookieName, string key, DateTime Expiration)
        //{
        //    var encryptedTicket = Encrypt(authenticationTicket, key);
        //    var cookie = new HttpCookie(cookieName, encryptedTicket)
        //    {
        //        HttpOnly = true,
        //        Expires = Expiration
        //    };
        //    httpContext.Response.Cookies.Add(cookie);
        //    return encryptedTicket;
        //}
        //public void UserSignIn(Sec_UserLogin accountInfo, HttpContext curentHttpContext)
        //{
        //    var token = SetAuthCookie(curentHttpContext, JsonConvert.SerializeObject(accountInfo), "_WDIAUTH", "keyauthen", DateTime.Now.AddDays(15));
        //}
        //public void Logout(HttpContext httpContext)
        //{
        //    var cookie = new HttpCookie("_WDIAUTH");
        //    DateTime nowDateTime = DateTime.Now;
        //    cookie.Expires = nowDateTime.AddDays(-1);
        //    httpContext.Response.Cookies.Add(cookie);

        //    httpContext.Request.Cookies.Remove("_WDIAUTH");
        //    FormsAuthentication.SignOut();
        //}

        public static string Decrypt(string toDecrypt, bool useHashing = true)
        {
            var rt = "";
            string key = sKey;
            try
            {
                byte[] keyArray;
                byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);

                if (useHashing)
                {
                    MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                    keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                }
                else
                    keyArray = UTF8Encoding.UTF8.GetBytes(key);

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Key = keyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = tdes.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                rt = UTF8Encoding.UTF8.GetString(resultArray);
            }
            catch
            {
                //return "";
            }
            return rt;
        }

        public static string Encrypt(string toEncrypt, bool useHashing = true)
        {
            string key = sKey;
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
    }
}