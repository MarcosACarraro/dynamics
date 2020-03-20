using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WorkflowAsincrono
{
    public class WfAssinc:CodeActivity
    {

        [Input("Nome Contato")]
        public InArgument<string> nomeContato { get; set; }

        [Input("Sobrenome Contato")]
        public InArgument<string> sobreNomeContato { get; set; }

        [Input("Usuario")]
        [ReferenceTarget("systemuser")]
        public InArgument<EntityReference> usuarioEntrada { get; set; }

        [Input("Conta")]
        [ReferenceTarget("account")]
        public InArgument<EntityReference> conta { get; set; }

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
            }
            else
            {
                usuario = contextWF.InitiatingUserId;
            }

            Entity entidade = new Entity("contact");
            entidade["firstname"] = nomeContato.Get<string>(context);
            entidade["lastname"] = sobreNomeContato.Get<string>(context);

            if (conta.Get<EntityReference>(context) != null)
            {
                account = conta.Get<EntityReference>(context).Id;
                entidade["parentcustomerid"] = new EntityReference("account", account);
;           }

            IOrganizationService service = factory.CreateOrganizationService(usuario);
            trace.Trace("Guid usuario"+ usuario.ToString());
            service.Create(entidade);
            trace.Trace("contato criado");
            saida.Set(context, "Teste realizado com sucesso!");
            trace.Trace("Fim");
        }
    }
}
