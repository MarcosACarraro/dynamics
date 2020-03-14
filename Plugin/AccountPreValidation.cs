using Microsoft.Xrm.Sdk;
using System;

namespace Plugin
{
    public class AccountPreValidation : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            Entity entidadeConexto = null;

            if (context.InputParameters.Contains("Target"))
                entidadeConexto = (Entity)context.InputParameters["Target"];

            if (entidadeConexto == null)
                return;

            if (!entidadeConexto.Contains("primarycontactid"))
                throw new InvalidPluginExecutionException(OperationStatus.Failed, "Contato relacionado obrigatório!!");

        }
    }
}
