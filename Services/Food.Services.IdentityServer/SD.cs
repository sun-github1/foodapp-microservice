﻿using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Food.Services.IdentityServer
{
    public static class SD
    {
        public const string Admin = "Admin";
        public const string Customer = "Customer";

        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),
                new IdentityResources.Profile()
            };

        public static IEnumerable<ApiScope> ApiScopes=>
            new List<ApiScope> { 
                new ApiScope("food","food server"),
                new ApiScope(name: "read",displayName: "Read your data"),
                new ApiScope(name: "write",displayName: "Write your data"),
                new ApiScope(name: "delete", displayName: "Delete your data")
            };

        public static IEnumerable<Client> Clients => 
            new List<Client> {
                new Client{ 
                    ClientId="client",
                    ClientSecrets= { new Secret("secret".Sha256())},
                    AllowedGrantTypes= GrantTypes.ClientCredentials,
                    AllowedScopes={ "read", "write", "profile" }
                },
                new Client{
                    ClientId="food",
                    ClientSecrets= { new Secret("secret".Sha256())},
                    AllowedGrantTypes= GrantTypes.Code,
                    RedirectUris={"https://localhost:44340/signin-oidc", "https://localhost:7137/signin-oidc"},
                    PostLogoutRedirectUris={"https://localhost:44340/signout-callback-oidc", "https://localhost:7137/signout-callback-oidc" },
                    AllowedScopes=new List<string>{ 
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "food"
                    }
                }
            };
    }
}
