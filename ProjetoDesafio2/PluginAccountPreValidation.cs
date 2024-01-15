using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System.Collections.Generic;

namespace PluginsTreinamento
{
    public class WFValidaLimiteInscricoesAluno : CodeActivity
    {
        #region Parametros
        // Recebe o usuário do contexto
        [Input("Usuario")]
        [ReferenceTarget("systemuser")]
        public InArgument<EntityReference> usuarioEntrada { get; set; }

        // Recebe o contexto
        [Input("AlunoXCursoDisponivel")]
        [ReferenceTarget("curso_alunoxcursodisponvel")]
        public InArgument<EntityReference> RegistroContexto { get; set; }

        [Output("Saida")]
        public OutArgument<string> saida { get; set; }
        #endregion

        protected override void Execute(CodeActivityContext executionContext)
        {
            // Cria o contexto
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            ITracingService trace = executionContext.GetExtension<ITracingService>();

            // Informação para o Log de Rastreamento de Plugin
            trace.Trace("curso_alunoxcursodisponvel: " + context.PrimaryEntityId);

            // Declara variável com o Guid da entidade primária em uso
            Guid guidAlunoXCurso = context.PrimaryEntityId;

            // Informação para o Log de Rastreamento de Plugin
            trace.Trace("guidAlunoXCurso: " + guidAlunoXCurso);

            // Construção da consulta FetchXML para obter informações sobre cursos associados ao aluno (todos os cursos)
            String fetchAlunoXCursos = "<fetch distinct='false' mapping='logical' output-format='xml-platform' version='1.0'>";
            fetchAlunoXCursos += "<entity name='curso_alunoxcursodisponvel' >";
            fetchAlunoXCursos += "<attribute name='curso_alunoxcursodisponvelid' />";
            fetchAlunoXCursos += "<attribute name='curso_name' />";
            fetchAlunoXCursos += "<attribute name='curso_emcurso' />";
            fetchAlunoXCursos += "<attribute name='createdon' />";
            fetchAlunoXCursos += "<attribute name='curso_aluno' />";
            fetchAlunoXCursos += "<order descending= 'false' attribute = 'curso_name' />";
            fetchAlunoXCursos += "<filter type= 'and' >";
            fetchAlunoXCursos += "<condition attribute = 'curso_alunoxcursodisponvelid' value = '" + guidAlunoXCurso + "' uitype = 'curso_alunoxcursodisponvel'  operator= 'eq' />";
            fetchAlunoXCursos += "</filter> ";
            fetchAlunoXCursos += "</entity>";
            fetchAlunoXCursos += "</fetch> ";
            trace.Trace("fetchAlunoXCursos: " + fetchAlunoXCursos);

            // Consulta FetchXML para obter informações sobre cursos associados ao aluno (todos os cursos)
            var entityAlunoXCursos = service.RetrieveMultiple(new FetchExpression(fetchAlunoXCursos));
            trace.Trace("entityAlunoXCursos: " + entityAlunoXCursos.Entities.Count);

            Guid guidAluno = Guid.Empty;
            HashSet<string> cursosManha = new HashSet<string>(); // Usado para rastrear os cursos de manhã
            HashSet<string> cursosTarde = new HashSet<string>(); // Usado para rastrear os cursos de tarde

            // Loop sobre todos os cursos associados ao aluno
            foreach (var item in entityAlunoXCursos.Entities)
            {
                // Acessa informações do curso
                string nomeCurso = item.Attributes["curso_name"].ToString();
                trace.Trace("nomeCurso: " + nomeCurso);

                // Obtém o Id do aluno associado ao curso
                var entityAluno = ((EntityReference)item.Attributes["curso_aluno"]).Id;
                guidAluno = ((EntityReference)item.Attributes["curso_aluno"]).Id;
                trace.Trace("entityAluno: " + entityAluno);

                // Verifica se o curso é de manhã ou tarde
                if (nomeCurso.ToLower().Contains("8h"))
                {
                    cursosManha.Add(nomeCurso);
                }
                else if (nomeCurso.ToLower().Contains("18h"))
                {
                    cursosTarde.Add(nomeCurso);
                }
            }

            // Verifica se o aluno escolheu exatamente dois cursos (um de manhã e um de tarde)
            if (cursosManha.Count == 1 && cursosTarde.Count == 1)
            {
                saida.Set(executionContext, "Aluno dentro do limite de cursos ativos.");
                trace.Trace("Aluno dentro do limite de cursos ativos.");
            }
            else
            {
                saida.Set(executionContext, "Selecione exatamente um curso de 8h da manhã e um curso de 18h da tarde.");
                trace.Trace("Aluno não selecionou cursos corretos.");
            }
        }
    }
}
