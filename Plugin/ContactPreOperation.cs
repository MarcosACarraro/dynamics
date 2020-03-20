using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
namespace Plugin
{
    public class ContactPreOperation : IPlugin
    {

        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            if (context.MessageName.ToLower() == "delete" &&
                context.Mode == Convert.ToInt32(MeuEnum.Mode.Synchronous) &&
                context.Stage == Convert.ToInt32(MeuEnum.Stage.PosOperation))
            {
                IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

                var serviceProsxyUserService = factory.CreateOrganizationService(context.UserId);

                Entity entidadeContexto = null;
                Entity entidadePre = null;

                if (context.InputParameters.Contains("Target"))
                    entidadeContexto = (Entity)context.InputParameters["Target"];

                if (context.PreEntityImages.Contains("preImagem"))
                    entidadePre = (Entity)context.PreEntityImages["preImagem"];

                if (entidadeContexto == null || entidadePre == null)
                    return;

                if (entidadeContexto.Contains("primarycontactid") && entidadePre.Contains("primarycontactid")) 
                    throw new InvalidPluginExecutionException("Não é possivel excluir o contato primário");
            }
        }

        //public void Execute(IServiceProvider serviceProvider)
        //{
        //    IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

        //    if (context.MessageName.ToLower() == "delete" &&
        //        context.Mode == Convert.ToInt32(MeuEnum.Mode.Synchronous) &&
        //        context.Stage == Convert.ToInt32(MeuEnum.Stage.PosOperation))
        //    {
        //        IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

        //        var serviceProsxyUserService = factory.CreateOrganizationService(context.UserId);

        //        Entity entidadeContexto = null;

        //        if (context.InputParameters.Contains("Target"))
        //            entidadeContexto = (Entity)context.InputParameters["Target"];

        //        if (entidadeContexto != null)
        //        {

        //            QueryExpression queryExpression = new QueryExpression("account");
        //            queryExpression.Criteria.AddCondition("primarycontactid", ConditionOperator.Equal, entidadeContexto.Id);
        //            queryExpression.ColumnSet = new ColumnSet(true);
        //            EntityCollection colecaoEntidades = serviceProsxyUserService.RetrieveMultiple(queryExpression);

        //            if (colecaoEntidades.Entities != null && colecaoEntidades.Entities.Count > 0)
        //            {
        //                throw new InvalidPluginExecutionException(OperationStatus.Failed, "não pode excluir (Possui um contato primário)!!");
        //            }
        //        }
        //        else
        //        {
        //            return;
        //        }
        //    }
        //}
    }
}
