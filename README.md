# Simplex (Persymplex)

[Clique aqui](http://persymplex.azurewebsites.net/) para acessar a aplicação.

**Projeto de Pesquisa Operacional** - 5º semestre BSI - UNIVEM

### Integrantes

1. Claudio de Jesus Junior - RA: 555835
2. Marcelo Guilherme Sassá - RA: 478504
3. Marcus Vinícius de Souza - RA: 388149
4. Willy Rogério de Souza - RA: 552526

## Introdução
Este documento provê uma visão geral da versão do aplicativo Persymplex que está sendo liberada. 
Aqui descreveremos as funcionalidades do aplicativo, bem como seus problemas e limitações conhecidos. 
Por último são descritas as demandas e os problemas que foram resolvidos para liberação da versão atual.

O projeto é um algoritmo de implementação do método simplex, com o objetivo de resolver modelos de programação 
linear com inúmeras variáveis e restrições, realizando a maximização ou minimização do resultado a fim de encontrar a solução ideal.

## Nota de release

### Simplex

* Resolução de problemas de maximização e minimização;
* Quantidade ilimitada de restrições e variáveis;
* Quantidade limitada de iterações;
* Passo a passo das tabelas de iteração;
* Passo a passo das explicações;
* Passo a passo dos cálculos;
* Análise de sensibilidade;
* Tratativa de erros de soluções infinitas e valores inválidos;
* Opção de gerar solução em .pdf.

## Problemas conhecidos e limitações

* As restrições são estritamente menores ou iguais à zero (<= 0);
* Os campos só aceitam números inteiros.

## Datas importantes

| Data  | Evento    |
|-------|-----------|
| 10/05/2017    | Início do projeto   |
| 12/05/2017    | Definição das funcionalidades   |
| 17/05/2017    | Criação do mockup da aplicação   |
| 24/05/2017    | Criação do template inicial  |
| 01/06/2017    | Resolução de problemas de maximização e minimização    |
| 01/06/2017    | Primeiros testes    |
| 10/06/2017    | Resolução da análise de sensibilidade   |
| 12/06/2017    | Hospedagem da aplicação  |
| 12/06/2017    | Criação da versão para impressão em .pdf    |
| 14/06/2017    | Testes finais   |
| 14/06/2017    | Entrega da versão final do projeto   |

## Compatibilidade

| Requisitos    | Ferramentas   |
|---------------|---------------|
| Navegadores   | Google Chrome, Mozilla Firefox, Internet Explorer 9+, Microsoft Edge, Opera e Safari   |
| Sistemas Operacionais     | Windows, Ubuntu e Android    |

Tecnologias

| Tecnologias   | Descrição |
|---------------|-----------|
| Linguagem de programação | C# |
| Framework Web  | ASP.NET e Bootstrap  |
| IDE    | Microsoft Visual Studio     |
| Design Pattern    | MVC   |
| Servidor  | IIS    |

## Procedimento e alteração de configuração do ambiente
Para alteração no ambiente é necessário possuir o Git e o Microsoft Visual Studio instalados. Para publicar o projeto, inicialmente é necessário clonar a aplicação que está no Git para o Microsoft Visual Studio e realizar as mudanças no projeto. A partir daí é preciso efetuar o login no Microsoft Azure e publicar a aplicação a partir do Microsoft Visual Studio. Após o projeto modificado, é necessario atualizar a aplicação no Git.

## Atividades realizadas no período

| Código    | Título    | Tarefa    | Situação  | Observação  |
|-----------|-----------|-----------|-----------|-------------|
| 1 | Maximização   | Permitir ao usuário que selecione a opção de maximização e apresentar o resultado    | Concluído |  Apenas restrições de <= |
| 2 | Minimização   | Permitir ao usuário que selecione a opção de minimização e apresentar o resultado   | Concluído |  Apenas restrições de <= |
| 3 | Tabelas de iteração   | Apresentar ao usuário cada tabela de iteração após a realização dos cálculos  | Concluído |  
| 4 | Soluções indeterminas    | Limitar a quantidade de iterações a fim de evitar loops infinitos     | Concluído     |  
| 5 | Análise de sensibilidade     | Apresentar ao usuário o Preço Sombra e o Limite de Restrições     | Concluído     |  
| 6 | Gerar .pdf     | Disponibilizar solução em .pdf     | Concluído     |  
