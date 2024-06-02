using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ApiProyectoAWS.Helpers
{
    public class HelperActionServicesOAuth
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecretKey { get; set; }

        public HelperActionServicesOAuth(string issuer, string audience, string secretKey)
        {
            this.Issuer = issuer;
            this.Audience = audience;
            this.SecretKey = secretKey;
        }

        //Necesitamos un metodo para generar el token 
        //Que se basa en el secret key
        public SymmetricSecurityKey GetKeyToken()
        {
            //Convertimos el secret key a Bytes[]
            byte[] data =
                Encoding.UTF8.GetBytes(this.SecretKey);
            //Devolvemos la key generada mediante los bytes[]
            return new SymmetricSecurityKey(data);
        }

        //Hemos creado esta clase para quitar codigo dentro 
        //De program en los services.
        //Metodo para la configuracion de la validacion
        //Del token
        public Action<JwtBearerOptions> GetJwtBearerOptions()
        {
            Action<JwtBearerOptions> options =
                new Action<JwtBearerOptions>(options =>
                {
                    //Indicamos que deseamos validar de 
                    //Nuestro token, issuer, audience
                    //, Time
                    options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = false,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = this.Issuer,
                        ValidAudience = this.Audience,
                        IssuerSigningKey = this.GetKeyToken()
                    };
                });
            return options;
        }

        //Metodo para indicar el esquema de la validacion
        public Action<AuthenticationOptions>
            GetAuthenticateSchema()
        {
            Action<AuthenticationOptions> options =
                new Action<AuthenticationOptions>(options =>
                {
                    options.DefaultScheme =
                    JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
                });
            return options;
        }
    }
}
