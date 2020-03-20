using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WorkflowAsincrono
{
    public class WFContato : CodeActivity
    {

        [Input("Usuario")]
        [ReferenceTarget("systemuser")]
        public InArgument<EntityReference> usuarioEntrada { get; set; }

        [Input("ContatoID")]
        [ReferenceTarget("contact")]
        public InArgument<EntityReference> ContatoID { get; set; }

        [Input("CNPJ")]
        public InArgument<string> CNPJ { get; set; }

        [Output("Saida")]
        public OutArgument<string> saida { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IWorkflowContext contextWF = context.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory factory = context.GetExtension<IOrganizationServiceFactory>();
            ITracingService trace = context.GetExtension<ITracingService>();

            Guid usuario = Guid.Empty;
            Guid account = Guid.Empty;

            trace.Trace("Inicio");


            if (usuarioEntrada.Get<EntityReference>(context) != null)
            {
                usuario = usuarioEntrada.Get<EntityReference>(context).Id;
                trace.Trace("01");
            }
            else
            {
                usuario = contextWF.InitiatingUserId;
                trace.Trace("02");
            }

            IOrganizationService service = factory.CreateOrganizationService(usuario);

            string cnpj = CNPJ.Get<string>(context);
            trace.Trace(cnpj);

            QueryExpression queryExpression = new QueryExpression("account");
            queryExpression.Criteria.AddCondition("wayon_cnpj", ConditionOperator.BeginsWith, cnpj);
            queryExpression.ColumnSet = new ColumnSet(true);

            trace.Trace("03");

            EntityCollection colecaoEntidades = service.RetrieveMultiple(queryExpression);

            if (colecaoEntidades.Entities != null && colecaoEntidades.Entities.Count > 0)
            {
                trace.Trace("04");
                foreach (var item in colecaoEntidades.Entities)
                {
                    trace.Trace("05");
                    if (item.Contains("primarycontactid"))
                    {
                        trace.Trace("06");
                        trace.Trace("contato atualizado");
                        EntityReference reference = ContatoID.Get<EntityReference>(context);
                        trace.Trace(reference.ToString());
                        item.Attributes["primarycontactid"] = reference;
                        trace.Trace("setou");
                        service.Update(item);
                        trace.Trace("atualizou");
                    }
                }
            }
            else
            {
                trace.Trace("naõ achou nada");
            }

            //throw new InvalidPluginExecutionException(OperationStatus.Failed, "Errado");
           saida.Set(context, "Teste realizado com sucesso!");
        }
    }
}

//https://wayonsystem.crm2.dynamics.com/api/data/v9.0/contacts(4ba0e5b9-88df-e311-b8e5-6c3be5a8b200)?$select=fullname