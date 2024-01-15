# Desafio 2 - Bootcamp Programação C# com CRM Dynamics

![Imagem do Desafio 2](https://github.com/edvaldovitor250/ProjetoDesafio2/assets/116117189/f334fe27-83ca-4ecf-91a5-27b1a0ffbc66)

Realizei uma melhoria significativa no sistema. Agora, os alunos têm a flexibilidade de escolher dois cursos, mas com uma reviravolta interessante. Eles podem escolher um curso de 8h da manhã e outro de 18h da tarde, proporcionando uma experiência mais personalizada e adequada aos horários preferenciais dos alunos. Esta mudança visa garantir que os cursos escolhidos estejam alinhados com as preferências de disponibilidade dos alunos, proporcionando uma escolha mais direcionada e interativa.

Aqui está a atualização no código:

```csharp
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
