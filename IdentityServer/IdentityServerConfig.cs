using System;
using IdentityServer4.Models;
using System.Collections.Generic;
using IdentityModel;

namespace IdentityServer
{
    public static class IdentityServerConfig
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>()
            {
                //new IdentityResources.OpenId(),
                new IdentityResource(//這段等同上方OpenId()，上方使用預設值
                    name: "openid",
                    userClaims: new[] { "sub" },
                    displayName: "Your user identifier"),
                //new IdentityResources.Profile()
                new IdentityResource(//這段等同上方Profile()，上方使用預設值
                    name: "profile",
                    userClaims: new[] { "name", "email", "website" },
                    displayName: "Your profile data")
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>() {
                new Client
                {
                    ClientId = "Admin",
                    // IdentityServer提供多種授權方式，這裡使用客戶授權模式
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    // from ApiScope, 這裡若ApiScope不同，取Token時會有invalid_scope錯誤
                    AllowedScopes = { "read", "write", "deltet" },
                    ClientSecrets = { new Secret("adminSecret".Sha256())},
                    // 因admin也要能使用user身份的api，故兩種角色都要加入
                    Claims = new List<ClientClaim>
                    {
                        new ClientClaim(JwtClaimTypes.Role, "admin"),
                        new ClientClaim(JwtClaimTypes.Role, "user")
                    },
                    /* 若無以下這行，Token回傳的欄位名稱會是client_role而不是role, 
                     * 這樣會因對應不到role而導致驗證失敗 */
                    ClientClaimsPrefix = string.Empty
                },
                new Client
                {
                    ClientId = "User",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = { "read" },
                    ClientSecrets = { new Secret("userSecret".Sha256())},
                    Claims = new List<ClientClaim>
                    {
                        new ClientClaim(JwtClaimTypes.Role, "user")
                    },
                    ClientClaimsPrefix = string.Empty
                }, new Client
                {
                    ClientId = "client",

                    AllowedScopes = { "openid", "profile" }
                }
            };
        }


        // 設定有哪些API可使用
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>()
            {
                new ApiResource("DevApi", "DEV Api", new List<string>{ JwtClaimTypes.Role }),
                new ApiResource("UatApi", "UAT Api", new List<string>{ JwtClaimTypes.Role })
            };
        }

        // 設定API範圍(for Client)
        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
            {
                new ApiScope(name: "read",   displayName: "Read your data."),
                new ApiScope(name: "write",  displayName: "Write your data."),
                new ApiScope(name: "delete", displayName: "Delete your data.")
            };
        }
    }

}

