using DocumentFormat.OpenXml.Spreadsheet;
using E_Common;
using E_Contract.Repository;
using E_Contract.Service;
using E_Contract.Service.Authentication;
using E_Model.Authentication;
using E_Model.Request.Token;
using E_Model.Response.Authentication;
using E_Model.Table_SQL.User;
using Microsoft.AspNetCore.Http;
using Novell.Directory.Ldap;
using System.Data;
using System.Security.Claims;

namespace E_Service.Authentication
{
    public class TokenService : ServiceBase<data_user>, ITokenService
    {
        private readonly JwtHelper _jwtHelper;

        public TokenService(IRepositoryWrapper RepositoryWrapper, JwtHelper jwtHelper) : base(RepositoryWrapper)
        {
            this._repositoryBase = RepositoryWrapper.Token;
            this._jwtHelper = jwtHelper;
        }

        public async Task<UserAuthenticationItemResponse> AuthenticateAsync(LoginItemRequest data)
        {
            try
            {
                var userAuthen = new UserAuthenticationItemResponse();
                data.password = _jwtHelper.Encrypt(data.password);

                if (!data.is_ldap)
                {
                    var user = await _repositoryWrapper.DataUser.SelectByUserAsync(data);
                    if (user == null)
                        return new UserAuthenticationItemResponse();

                    var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, user?.user_code),
                            new Claim(ClaimTypes.Name, user?.email),
                            new Claim(ClaimTypes.Role, ""),
                        };

                    userAuthen.access_token = _jwtHelper.GenerateAccessToken(claims);
                    userAuthen.refresh_token = _jwtHelper.GenerateRefreshToken();
                    userAuthen.user_info = new DataUserCardItemResponse
                    {
                        user_code = user?.user_code,
                        avatar = user?.avatar_url,
                        full_name = user.full_name,
                        email = user.email,
                        phone = user.phone_number,
                        card_color = user.card_color,
                    };
                    //userAuthen.list_application = await _repositoryWrapper.DataApplication.SelectByUserIdAsync(user.id);

                    return userAuthen;
                }
                else
                {
                    //logic check LDAP
                    var user = await _repositoryWrapper.DataUser.SelectByUserAsync(data.email, data.password);
                    if (!string.IsNullOrEmpty(user.ldap_server) && !string.IsNullOrEmpty(user.ldap_dc) && user.ldap_port > 0)
                        userAuthen.user_info = await CheckLoginAndResponseToken(user);

                    var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                            new Claim(ClaimTypes.Name, user.email),
                            new Claim(ClaimTypes.Role, user?.role_name),
                        };

                    userAuthen.access_token = _jwtHelper.GenerateAccessToken(claims);
                    userAuthen.refresh_token = _jwtHelper.GenerateRefreshToken();
                    userAuthen.list_application = await _repositoryWrapper.DataApplication.SelectByUserIdAsync(user.id);

                    return userAuthen;
                }
            }
            catch (Exception ex)
            {
                return new UserAuthenticationItemResponse();
            }
        }

        public async Task<bool> ChangePassword(ChangePasswordItemRequest request)
        {
            request.code = null;
            var user = await _repositoryWrapper.DataUser.SelectUserByEmailAsync(request.email);
            if (user != null)
            {
                var encrypt_pass = _jwtHelper.Encrypt(request.new_password);

                user.password = encrypt_pass;
                user.note = "ChangePassword - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                user.SetUpdateInfo(user.user_code);

                var status = await _repositoryWrapper.DataUser.UpdateAsync(user, "E_PortalConnection");
                return status;
            }
            else
                return false;
        }

        public async Task<DataUserCardItemResponse> CheckLoginAndResponseToken(DataUserItemResponse user, bool is_refresh = false)
        {
            var userCard = new DataUserCardItemResponse();
            try
            {
                using (var ldapConnection = new Novell.Directory.Ldap.LdapConnection())
                {
                    var list_dc = user.ldap_dc.Trim().Split(',');
                    var username_ldap = $"{user.username}@{list_dc[0]}";
                    var baseDN = $"dc={list_dc[0]}";
                    for (int i = 1; i < list_dc.Length; i++)
                    {
                        username_ldap += $".{list_dc[i]}";
                        baseDN += $",dc={list_dc[i]}";
                    }
                    ldapConnection.Connect(user.ldap_server, user.ldap_port);
                    if (is_refresh)
                    {
                        user.password = _jwtHelper.Decrypt(user.password);
                        ldapConnection.Bind(username_ldap, user.password);
                    }
                    else
                    {
                        ldapConnection.Bind(username_ldap, user.password);
                    }
                    LdapSearchResults searchResults = (LdapSearchResults)ldapConnection.Search(
                        baseDN,
                        Novell.Directory.Ldap.LdapConnection.ScopeSub,
                        $"(sAMAccountName={user.username})",
                        null,
                        false
                    );

                    if (searchResults.HasMore())
                    {
                        LdapEntry userEntry = searchResults.Next();
                        LdapAttributeSet attributeSet = userEntry.GetAttributeSet();
                        LdapAttribute displayNameAttribute = attributeSet.GetAttribute("displayName");
                        LdapAttribute emailAttribute = attributeSet.GetAttribute("mail");
                        LdapAttribute phoneAttribute = attributeSet.GetAttribute("telephoneNumber");
                        LdapAttribute dnAttribute = attributeSet.GetAttribute("distinguishedName");
                        LdapAttribute titleAttribute = attributeSet.GetAttribute("title");
                        LdapAttribute departmentAttribute = attributeSet.GetAttribute("department");
                        if (displayNameAttribute != null)
                        {
                            user.full_name = displayNameAttribute.StringValue;
                        }
                        if (emailAttribute != null)
                        {
                            user.email = emailAttribute.StringValue;
                        }
                        if (phoneAttribute != null)
                        {
                            user.phone = phoneAttribute.StringValue;
                        }
                        if (titleAttribute != null)
                        {
                            var checkExistTitle = await _repositoryWrapper.DataTitle.SelectByTitleNameAsync(titleAttribute.StringValue.Trim());
                            if (checkExistTitle != null)
                            {
                                user.title_id = checkExistTitle.id;
                            }
                            else
                            {
                                var new_title = new data_title
                                {
                                    title = titleAttribute.StringValue,
                                };
                                new_title.SetInsertInfo(user.id + "");
                                var check_insert = await _repositoryWrapper.DataTitle.InsertAsync(new_title);
                                if (check_insert > 0)
                                {
                                    user.title_id = check_insert;
                                }
                            }
                            user.title = titleAttribute.StringValue;
                        }
                        if (departmentAttribute != null)
                        {
                            var checkExistDepartment = await _repositoryWrapper.DataDepartment.SelectByDepartmentNameAsync(departmentAttribute.StringValue.Trim());
                            if (checkExistDepartment != null)
                            {
                                user.department_id = checkExistDepartment.id;
                            }
                            else
                            {
                                var new_department = new data_department
                                {
                                    department = departmentAttribute.StringValue,
                                };
                                new_department.SetInsertInfo(user.id + "");
                                var check_insert = await _repositoryWrapper.DataDepartment.InsertAsync(new_department);
                                if (check_insert > 0)
                                {
                                    user.department_id = check_insert;
                                }
                            }
                            user.department = departmentAttribute.StringValue;
                        }
                        var encrypt_pass = _jwtHelper.Encrypt(user.password);
                        var new_user_info = new UserData
                        {
                            user_id = user.id,
                            email = user.email,
                            phone_number = user.phone,
                            full_name = user.full_name,
                            avatar_url = user.avatar,
                            is_ldap = user.is_ldap,
                            password = encrypt_pass,
                            title_id = user.title_id,
                            position_id = user.position_id,
                            department_id = user.department_id,
                            card_color = user.card_color.Trim(),
                        };
                        new_user_info.SetUpdateInfo(user.id + "");
                        await _repositoryWrapper.DataUser.UpdateAsync(new_user_info);

                        var ldap_setting = await _repositoryWrapper.SysLdapSetting.SelectByIdAsync(user.ldap_setting_id);
                        if (ldap_setting != null && dnAttribute != null)
                        {
                            ldap_setting.ldap_dn = dnAttribute.StringValue;
                            ldap_setting.SetUpdateInfo(user.id + "");
                            await _repositoryWrapper.SysLdapSetting.UpdateAsync(ldap_setting);
                        }
                        userCard = new DataUserCardItemResponse
                        {
                            avatar = user.avatar,
                            full_name = user.full_name,
                            email = user.email,
                            phone = user.phone,
                            position = user.position,
                            title = user.title,
                            department = user.department,
                            card_color = user.card_color.Trim(),
                        };
                    }
                    else
                    {
                        Console.WriteLine("Không tìm thấy người dùng.");
                    }
                    return userCard;
                }
            }
            catch (Exception ex)
            {
                return userCard;
            }
        }

        public async Task<UserAuthenticationItemResponse> RefreshAsync(string email, string refresh_token)
        {
            var userAuthen = new UserAuthenticationItemResponse();

            var saved_refresh_token = await _repositoryWrapper.SysRefreshToken.SelectByToken(refresh_token);
            if (saved_refresh_token.token != refresh_token || saved_refresh_token.expired_date < DateTime.Now)
            {
                return userAuthen;
            }
            else
            {
                var user = await _repositoryWrapper.DataUser.SelectByUserAsync(email, "");
                if (user != null)
                {
                    if (user.is_ldap)
                    {
                        if (!string.IsNullOrEmpty(user.ldap_server) && !string.IsNullOrEmpty(user.ldap_dc) && user.ldap_port > 0)
                        {
                            userAuthen.user_info = await CheckLoginAndResponseToken(user, true);
                        }
                    }
                    else
                    {
                        userAuthen.user_info = new DataUserCardItemResponse
                        {
                            username = user.username,
                            avatar = user.avatar,
                            full_name = user.full_name,
                            email = user.email,
                            department = user.department,
                            phone = user.phone,
                            position = user.position,
                            title = user.title,
                            card_color = user.card_color.Trim(),
                        };
                    }
                    if (!string.IsNullOrEmpty(userAuthen.user_info.full_name))
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, email),
                        };
                        userAuthen.access_token = _jwtHelper.GenerateAccessToken(claims);
                        userAuthen.refresh_token = _jwtHelper.GenerateRefreshToken();
                    }
                    return userAuthen;
                }
                else
                {
                    return new UserAuthenticationItemResponse();
                }
            }
        }

        public async Task<int> SignUp(SignUpItemRequest request)
        {
            var user_new = new UserData
            {
                full_name = request.full_name,
                password = _jwtHelper.Encrypt(request.password),
                phone_number = request.phone,
                email = request.email,
                card_color = "White",
            };

            if (!string.IsNullOrEmpty(request.title))
            {
                var checkExistTitle = await _repositoryWrapper.DataTitle.SelectByTitleNameAsync(request.title.Trim());
                if (checkExistTitle != null)
                {
                    user_new.title_id = checkExistTitle.id;
                }
                else
                {
                    var new_title = new data_title
                    {
                        title = request.title,
                    };
                    new_title.SetInsertInfo("");
                    var check_insert = await _repositoryWrapper.DataTitle.InsertAsync(new_title);
                    if (check_insert > 0)
                    {
                        user_new.title_id = check_insert;
                    }
                }
            }
            user_new.SetInsertInfo("");
            return await _repositoryWrapper.DataUser.InsertAsync(user_new);
        }

    }
}
