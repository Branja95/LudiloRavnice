﻿using System;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System.Configuration;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security;
using RentApp.Providers;
using RentApp.Persistance;
using Microsoft.Owin.Security.DataProtection;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace RentApp
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }
        internal static IDataProtectionProvider DataProtectionProvider { get; private set; }
        public static string PublicClientId { get; private set; }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            ConfigureOAuthTokenGeneration(app);
            ConfigureOAuthTokenConsumption(app);

            var oabao = new OAuthBearerAuthenticationOptions();
            oabao.Provider = new QueryStringOAuthBearerProvider("token");

            app.UseOAuthBearerAuthentication(oabao);
            DataProtectionProvider = app.GetDataProtectionProvider();
        }

        private void ConfigureOAuthTokenGeneration(IAppBuilder app)
        {
            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext(RADBContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(() => DependencyResolver.Current.GetService<ApplicationUserManager>());

            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                //For Dev enviroment only (on production should be AllowInsecureHttp = false)
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/oauth/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = new CustomOAuthProvider(),
                AccessTokenFormat = new CustomJwtFormat("http://localhost:51680") //TODO Kad promenis port, menjaj i ovde. ---> Premesteno u UnityConfig!
            };

            // OAuth 2.0 Bearer Access Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
        }

        private void ConfigureOAuthTokenConsumption(IAppBuilder app)
        {

            var issuer = "http://localhost:51680";
            string audienceId = ConfigurationManager.AppSettings["as:AudienceId"];
            byte[] audienceSecret = TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings["as:AudienceSecret"]);

            // Api controllers with an [Authorize] attribute will be validated with JWT
            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    Provider = new QueryStringOAuthBearerProvider("token"),//Dodato rukovanje tokenom kroz query string
                    AuthenticationMode = AuthenticationMode.Active,
                    AllowedAudiences = new[] { audienceId },
                    IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
                    {
                        new SymmetricKeyIssuerSecurityTokenProvider(issuer, audienceSecret)
                    }
                });
        }
    }
    //Klasa koja omogucava vadjenje tokena iz query string-a
    public class QueryStringOAuthBearerProvider : OAuthBearerAuthenticationProvider
    {
        readonly string _name;

        public QueryStringOAuthBearerProvider(string name)
        {
            _name = name;
        }

        public override Task RequestToken(OAuthRequestTokenContext context)
        {
            var value = context.Request.Query.Get(_name);

            if (!string.IsNullOrEmpty(value))
            {
                value = value.Split(' ')[1];
                context.Token = value;
            }

            return Task.FromResult<object>(null);
        }
    }
}
