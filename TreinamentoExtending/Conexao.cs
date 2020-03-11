using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace TreinamentoExtending
{
    class Conexao
    {
        public OrganizationServiceProxy Obter()
        {
            Uri uri = new Uri("https://wayonsystem.api.crm2.dynamics.com/XRMServices/2011/Organization.svc");

            ClientCredentials clientCredentials = new ClientCredentials();
            clientCredentials.UserName.UserName = "marcos.antonio@wayonsystem.onmicrosoft.com";
            clientCredentials.UserName.Password = "@Americana27";

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
           

            OrganizationServiceProxy serviceProxy = new OrganizationServiceProxy(uri, null, clientCredentials, null);

            serviceProxy.EnableProxyTypes();

            return serviceProxy;
        }
    }
}
