using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Description;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Discovery;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System.Net;

namespace TreinamentoExtending {
    class Program {
        static void Main(string[] args) {
            Conexao conexao = new Conexao();
            var serviceProxy = conexao.Obter();

            //Create(serviceProxy);

            CriacaoRetornoAtualiza(serviceProxy);

            //Descoberta();

            //RetornarMultiplo(serviceProxy);

            Console.ReadKey();
        }

        static void CriacaoRetornoAtualiza(OrganizationServiceProxy serviceProxy)
        {
            for (int i = 0; i < 4; i++)
            {
                var entidade = new Entity("account");
                Entity RegistroRetorno = new Entity();
                entidade.Attributes.Add("name", "Treinamento" + i.ToString());
                entidade.Id = serviceProxy.Create(entidade);


                entidade.Attributes.Add("telephone1", "0000-0000");
                entidade.Attributes.Add("address1_city", "São Paulo");
                serviceProxy.Update(entidade);

                RegistroRetorno = serviceProxy.Retrieve("account", entidade.Id, new ColumnSet(true));

                if (RegistroRetorno.Attributes.Contains("name"))
                {
                    RegistroRetorno.Attributes["name"] = "Treinamento" + (i+1).ToString();
                }
                else
                {
                    RegistroRetorno.Attributes.Add("name", "meu valor");
                }

                if(RegistroRetorno.Attributes.Contains("parentaccountid"))
                {
                    Guid parentaccountid = new Guid("d2a19cdd-88df-e311-b8e5-6c3be5a8b200");
                    EntityReference reference = new EntityReference("account", parentaccountid);
                    RegistroRetorno.Attributes["parentaccountid"] = reference;
                }
                else
                {
                    Guid parentaccountid = new Guid("d2a19cdd-88df-e311-b8e5-6c3be5a8b200");
                    EntityReference reference = new EntityReference("account", parentaccountid);
                    RegistroRetorno.Attributes.Add("parentaccountid", reference);
                }
                serviceProxy.Update(RegistroRetorno);
            }
        }
        static void Create(OrganizationServiceProxy serviceProxy)
        {
            //QueryExpression queryExpression = new QueryExpression("contact");
            //QueryExpression condicao = new QueryExpression("");

            for (int i = 0; i < 4; i++)
            {
                Guid registro = new Guid();
                Entity entidade = new Entity("account");
                entidade.Attributes.Add("name","Treinamento"+ i.ToString());
                entidade.Attributes.Add("telephone1", "0000-0000");
                entidade.Attributes.Add("address1_city", "São Paulo");
                Guid contactid = new Guid("4ba0e5b9-88df-e311-b8e5-6c3be5a8b200");

                EntityReference reference = new EntityReference("contact",contactid);

                entidade.Attributes.Add("primarycontactid", reference);
                

                registro = serviceProxy.Create(entidade);

                if(registro !=Guid.Empty)
                {
                    Console.WriteLine(registro.ToString());
                }
            }
        }
        
        static void Descoberta()
        {

            Uri local = new Uri("https://disco.crm2.dynamics.com/XRMServices/2011/Discovery.svc");

            ClientCredentials clientecred = new ClientCredentials();
            clientecred.UserName.UserName = "marcos.antonio@wayonsystem.onmicrosoft.com";
            clientecred.UserName.Password = "@Americana27";

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            DiscoveryServiceProxy dsp = new DiscoveryServiceProxy(local, null, clientecred, null);
            dsp.Authenticate();

            RetrieveOrganizationsRequest rosreq = new RetrieveOrganizationsRequest();
            rosreq.AccessType = EndpointAccessType.Default;
            rosreq.Release = OrganizationRelease.Current;

            RetrieveOrganizationsResponse response = (RetrieveOrganizationsResponse)dsp.Execute(rosreq);

            foreach (var item in response.Details)
            {
                Console.WriteLine("Unique " + item.UniqueName);
                Console.WriteLine("Friendly " + item.FriendlyName);
                foreach (var endpoints in item.Endpoints)
                {
                    Console.WriteLine(endpoints.Key);
                    Console.WriteLine(endpoints.Value);
                }
            }
        }
    }
}
