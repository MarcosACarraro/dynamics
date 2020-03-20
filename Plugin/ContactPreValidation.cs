using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;


namespace Plugin
{
    public class ContactPreValidation : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            var serviceProsxyUserService = factory.CreateOrganizationService(context.UserId);


            //if (context.MessageName.ToLower() == "create" &&
            //  context.Mode == Convert.ToInt32(MeuEnum.Mode.Synchronous) &&
            //  context.Stage == Convert.ToInt32(MeuEnum.Stage.PreValidation))
            //{

            //    Entity entidadeConexto = null;

            //    if (context.InputParameters.Contains("Target"))
            //        entidadeConexto = (Entity)context.InputParameters["Target"];
            //    if (entidadeConexto != null)
            //    {
            //        if (entidadeConexto.Contains("wayon_cpf"))
            //        {
            //            var cpf = entidadeConexto.Attributes["wayon_cpf"];

            //            QueryExpression queryExpression = new QueryExpression("contact");
            //            queryExpression.Criteria.AddCondition("wayon_cpf", ConditionOperator.BeginsWith, cpf);
            //            queryExpression.ColumnSet = new ColumnSet(true);
            //            EntityCollection colecaoEntidades = serviceProsxyUserService.RetrieveMultiple(queryExpression);

            //            if (colecaoEntidades.Entities != null && colecaoEntidades.Entities.Count > 0)
            //            {
            //                throw new InvalidPluginExecutionException(OperationStatus.Failed, "CPF existente !!");
            //            }
            //        }
            //        else
            //        {
            //            throw new InvalidPluginExecutionException(OperationStatus.Failed, "campo não  existente !!");
            //        }
            //    }
            //    else
            //    {
            //        return;
            //    }

            if (context.MessageName.ToLower() == "delete" &&
              context.Mode == Convert.ToInt32(MeuEnum.Mode.Synchronous) &&
              context.Stage == Convert.ToInt32(MeuEnum.Stage.PreValidation))
            {

                Entity entidadePre = null;


                if (context.PreEntityImages.Contains("preImagem"))
                    entidadePre = (Entity)context.PreEntityImages["preImagem"];
               
                if (entidadePre != null)
                {
                    QueryExpression queryExpression = new QueryExpression("account");
                    queryExpression.Criteria.AddCondition("primarycontactid", ConditionOperator.Equal, entidadePre.Id);
                    queryExpression.ColumnSet = new ColumnSet(true);
                    EntityCollection colecaoEntidades = serviceProsxyUserService.RetrieveMultiple(queryExpression);

                    if (colecaoEntidades.Entities != null && colecaoEntidades.Entities.Count > 0)
                    {
                        throw new InvalidPluginExecutionException(OperationStatus.Failed, "não pode excluir (Possui um contato primário)!!");
                    }
                }
            }
        }
    }
}

