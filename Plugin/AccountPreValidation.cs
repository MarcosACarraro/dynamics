using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Plugin
{
    public class AccountPreValidation : IPlugin
    {

        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            var serviceProsxyUserService = factory.CreateOrganizationService(context.UserId);

            Entity entidadeConexto = null;

            if (context.InputParameters.Contains("Target"))
                entidadeConexto = (Entity)context.InputParameters["Target"];
            if (entidadeConexto != null)
            {
                if (entidadeConexto.Contains("wayon_cnpj"))
                {
                    var cnpj = entidadeConexto.Attributes["wayon_cnpj"];

                    QueryExpression queryExpression = new QueryExpression("account");
                    queryExpression.Criteria.AddCondition("wayon_cnpj", ConditionOperator.BeginsWith, cnpj);
                    queryExpression.ColumnSet = new ColumnSet(true);
                    EntityCollection colecaoEntidades = serviceProsxyUserService.RetrieveMultiple(queryExpression);

                    if (colecaoEntidades.Entities != null && colecaoEntidades.Entities.Count > 0)
                    {
                        throw new InvalidPluginExecutionException(OperationStatus.Failed, "CNPJ existente !!");
                    }
                }
                else {
                    throw new InvalidPluginExecutionException(OperationStatus.Failed, "campo não  existente !!");
                }
            }
            else
            {
                return;
            }
             

        }

        //static void RetornarMultiploBasica(OrganizationServiceProxy serviceProxy)
        //{
        //    QueryExpression queryExpression = new QueryExpression("account");
        //    queryExpression.Criteria.AddCondition("name", ConditionOperator.BeginsWith, "W");
        //    queryExpression.ColumnSet = new ColumnSet(true);
        //    EntityCollection colecaoEntidades = serviceProxy.RetrieveMultiple(queryExpression);

        //    if (colecaoEntidades.Entities != null && colecaoEntidades.Entities.Count > 0)
        //    {
        //        foreach (var item in colecaoEntidades.Entities)
        //        {
        //            Console.WriteLine(item["name"]);
        //        }
        //    }
        //}

        //public void Execute(IServiceProvider serviceProvider)
        //{
        //    IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

        //    IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

        //    ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

        //    Entity entidadeConexto = null;

        //    if (context.InputParameters.Contains("Target"))
        //        entidadeConexto = (Entity)context.InputParameters["Target"];

        //    if (entidadeConexto == null)
        //        return;

        //    if (!entidadeConexto.Contains("primarycontactid"))
        //        throw new InvalidPluginExecutionException(OperationStatus.Failed, "Contato relacionado obrigatório!!");

        //}


    }
}
