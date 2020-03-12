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

            //CriacaoRetornoAtualiza(serviceProxy);

            //Descoberta();

            //RetornarMultiploBasica(serviceProxy);

            //RetornarMultiplo(serviceProxy);

            //ConsultasLinq(serviceProxy);

            //CriacaoLinq(serviceProxy);

            //UpdateLinq(serviceProxy);

            //DeleteLinq(serviceProxy);

            //FetchXML(serviceProxy);

            FetchXMLAggregate(serviceProxy);

            Console.ReadKey();
        }

        static void FetchXMLAggregate(OrganizationServiceProxy serviceProxy)
        {
            StringBuilder query = new StringBuilder();

            query.Append("<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' aggregate='true'>");
            query.Append("   <entity name='opportunity'>");
            query.Append("      <attribute name='budgetamount' alias='budgetamount_soma' aggregate='avg'/>");
            query.Append("   </entity>");
            query.Append("</fetch>");

            EntityCollection colecao = serviceProxy.RetrieveMultiple(new FetchExpression(query.ToString()));

            foreach (var item in colecao.Entities)
            {
                var valor = ((AliasedValue)item["budgetamount_soma"]).Value;

                Console.WriteLine(String.Format("Média : {0}", ((Money)valor).Value.ToString()));
            }
        }
        static void FetchXML(OrganizationServiceProxy serviceProxy)
        {
            StringBuilder query = new StringBuilder();

            query.Append("<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>");
            query.Append("   <entity name='account'>");
            query.Append("      <attribute name='name' />");
            query.Append("      <attribute name='address1_city' />");
            query.Append("      <attribute name='primarycontactid' />");
            query.Append("      <attribute name='telephone1' />");
            query.Append("      <attribute name='accountid' />");
            query.Append("      <order attribute='name' descending='false' />");
            query.Append("      <filter type='and'>");
            query.Append("         <condition attribute='ownerid' operator='eq-userid' />");
            query.Append("         <condition attribute='statecode' operator='eq' value='0' />");
            query.Append("      </filter>");
            query.Append("      <link-entity name='contact' from='contactid' to='primarycontactid' visible='false' link-type='outer' alias='accountprimarycontactidcontactcontactid'>");
            query.Append("         <attribute name='emailaddress1' />");
            query.Append("      </link-entity>");
            query.Append("   </entity>");
            query.Append("</fetch>");

            EntityCollection colecao = serviceProxy.RetrieveMultiple(new FetchExpression(query.ToString()));

            foreach (var item in colecao.Entities)
            {
                Console.WriteLine(String.Format("Nome Conta : {0}", item["name"]));
            }
        }
        static void DeleteLinq(OrganizationServiceProxy serviceProxy)
        {
            OrganizationServiceContext context = new OrganizationServiceContext(serviceProxy);
            var resultados = from a in context.CreateQuery("account")
                             where ((string)a["name"]) == "Teste"
                             select a;

            foreach (var item in resultados)
            {
                context.DeleteObject(item);
            }
            context.SaveChanges();
        }
        static void UpdateLinq(OrganizationServiceProxy serviceProxy)
        {
            OrganizationServiceContext context = new OrganizationServiceContext(serviceProxy);
            var resultados = from a in context.CreateQuery("contact")
                             where ((string)a["firstname"]) == "Benno"
                             select a;

            foreach(var item in resultados)
            {
                item.Attributes["firstname"] = "Marcos";
                context.UpdateObject(item);
            }
            context.SaveChanges();
        }
        static void CriacaoLinq(OrganizationServiceProxy serviceProxy)
        {
            OrganizationServiceContext context = new OrganizationServiceContext(serviceProxy);

            for (int i = 0; i < 5; i++)
            {
                Entity account = new Entity("account");
                account["name"] = String.Format("Conta Linq {0}", i.ToString());
                context.AddObject(account);
            }
            context.SaveChanges();

        }
        static void ConsultasLinq(OrganizationServiceProxy serviceProxy)
        {
            OrganizationServiceContext context = new OrganizationServiceContext(serviceProxy);

            var resultados = from a in context.CreateQuery("contact")
                             join b in context.CreateQuery("account")
                                    on a["contactid"] equals b["primarycontactid"]
                             where ((string)b["address1_city"]) == "Seattle"
                             select new{ 
                                retorno = new { 
                                    FirstName = a["firstname"],
                                    LastName =a["lastname"],
                                    NomeConta = b["name"],
                                    Cidade = b["address1_city"]
                                }
                             };

            foreach (var item in resultados)
            {
                Console.WriteLine(String.Format("Nome       : {0}", item.retorno.FirstName));
                Console.WriteLine(String.Format("Sobrenome  : {0}", item.retorno.LastName));
                Console.WriteLine(String.Format("NomeConta  : {0}", item.retorno.NomeConta));
                Console.WriteLine(String.Format("Cidade     : {0}", item.retorno.Cidade));
                Console.WriteLine("---------------------------------------------------------------------------------");
            }
        }
        static void RetornarMultiploBasica(OrganizationServiceProxy serviceProxy)
        {
            QueryExpression queryExpression = new QueryExpression("account");
            queryExpression.Criteria.AddCondition("name", ConditionOperator.BeginsWith, "W");
            queryExpression.ColumnSet = new ColumnSet(true);
            EntityCollection colecaoEntidades = serviceProxy.RetrieveMultiple(queryExpression);

            if (colecaoEntidades.Entities != null && colecaoEntidades.Entities.Count > 0)
            {
                foreach (var item in colecaoEntidades.Entities)
                {
                    Console.WriteLine(item["name"]);
                }
            }
        }
        static void RetornarMultiplo(OrganizationServiceProxy serviceProxy)
        {
            QueryExpression queryExpression = new QueryExpression("account");
            queryExpression.ColumnSet = new ColumnSet(true);
            ConditionExpression condicao = new ConditionExpression("address1_city", ConditionOperator.Equal, "Seattle");
            queryExpression.Criteria.AddCondition(condicao);

            LinkEntity link = new LinkEntity("account", "contact", "primarycontactid", "contactid", JoinOperator.LeftOuter);
            link.Columns = new ColumnSet("firstname", "lastname");
            link.EntityAlias = "Contato";

            queryExpression.LinkEntities.Add(link);

            EntityCollection colecaoEntidades = serviceProxy.RetrieveMultiple(queryExpression);

            if (colecaoEntidades.Entities != null && colecaoEntidades.Entities.Count > 0)
            {
                foreach (var item in colecaoEntidades.Entities)
                {
                    Console.WriteLine(String.Format("ID                : {0}", item.Id));
                    Console.WriteLine(String.Format("Nome Conta        : {0}", item["name"]));
                    Console.WriteLine(String.Format("Cidade            : {0}", item["address1_city"]));
                    if (item.Attributes.Contains("Contato.firstname"))
                    {
                        Console.WriteLine(String.Format("Nome Contato      : {0}", ((AliasedValue)item["Contato.firstname"]).Value));
                        Console.WriteLine(String.Format("Sobrenome Contato : {0}", ((AliasedValue)item["Contato.lastname"]).Value));
                    }else
                    {
                        Console.WriteLine("Nome Contato      : ");
                        Console.WriteLine("Sobrenome Contato : ");
                    }
                    Console.WriteLine("---------------------------------------------------------------------------------");
                }
            }
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
