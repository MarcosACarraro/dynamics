using Microsoft.Xrm.Sdk;
using System;

namespace Plugin
{
    public class AccountPosOperation : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            if (context.MessageName.ToLower() == "update" &&
                context.Mode == Convert.ToInt32(MeuEnum.Mode.Synchronous) &&
                context.Stage == Convert.ToInt32(MeuEnum.Stage.PosOperation))
            {
                IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

                var serviceAdmin = factory.CreateOrganizationService(null);

                ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

                trace.Trace("Inicio Plugin");
                Entity entidadeContexto = null;
                Entity entidadePre = null;

                if (context.InputParameters.Contains("Target"))
                    entidadeContexto = (Entity)context.InputParameters["Target"];

                if (context.PreEntityImages.Contains("preImagen"))
                    entidadePre = (Entity)context.PreEntityImages["preImagen"];

                if (entidadeContexto == null || entidadePre == null)
                    return;

                if ((entidadeContexto.Contains("primarycontactid") && entidadePre.Contains("primarycontactid")) &&
                    (entidadeContexto["primarycontactid"]) != (entidadePre["primarycontactid"]))
                    throw new InvalidPluginExecutionException("Não é possivel altear o contato primário");
            }
        }
    }
}
