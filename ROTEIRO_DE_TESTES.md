# Aula Prática: Testes de Jogos Digitais
Bem-vindo ao laboratório de testes do projeto **Dungeon of Algorithms**!

No ciclo de desenvolvimento de software (inclusive em jogos), garantir a qualidade (QA - Quality Assurance) é fundamental. Veremos na prática duas das principais metodologias de testes: a **Caixa Preta** e a **Caixa Branca**.

---

## 1. Testes de Caixa Branca (Estrutural)

### Conceito
No **Teste de Caixa Branca** (White-Box Testing), o testador (*você!*) tem acesso total ao código-fonte do jogo. Você conhece as engrenagens, variáveis, e a lógica matemática por trás das mecânicas.
O seu objetivo é garantir que os fluxos internos (if/else, loops, cálculo de dano) estejam se comportando exatamente conforme o programado.

### Na Prática (Testes Unitários)
Neste projeto, utilizamos a biblioteca **xUnit** para rodar testes automatizados na linguagem C#.

Abra o arquivo [`DungeonOfAlgorithms.Tests/PlayerTests.cs`](DungeonOfAlgorithms.Tests/PlayerTests.cs). Você notará métodos como `TakeDamage_DecreasesHealth`. 
Isso é um teste unitário! Ele simula a criação de um jogador, aplica um dano diretamente na memória (variável interna) e avalia (usando `Assert`) se a Vida (`Health`) calculou a matemática corretamente.

### 📝 Exercício de Caixa Branca
1. Abra o terminal na raiz do projeto e execute o comando:
   ```bash
   dotnet test
   ```
2. Observe o resultado. O console deve relatar que todos os testes "Passaram" (Passed).
3. **Desafio:** Abra o arquivo fonte principal do jogador (`Source/Entities/Player.cs`).
4. Encontre o método `TakeDamage(int amount)` (por volta da linha 50).
5. "Quebre" a lógica do jogo intencionalmente (Ex: Introduza um bug matemático trocando um sinal `-` por `+`).
6. Rode `dotnet test` novamente e veja o teste unitário capturar instantaneamente a quebra de contrato sem precisar sequer abrir o jogo!

---

## 2. Testes de Caixa Preta (Comportamental)

### Conceito
No **Teste de Caixa Preta** (Black-Box Testing), o testador *MUITAS* vezes não precisa saber programar. A "caixa" (o código do jogo) está opaca.
O testador atua exatamente como o jogador final atuaria. Ele valida **Requisitos**, **Experiência do Usuário (UX)**, **Colisões**, e as **Interfaces** através da observação e do uso da aplicação.

### 📝 Exercício de Caixa Preta
Sua missão agora é agir como um testador de QA contratado por uma desenvolvedora. Jogue o jogo completo!
Abra o jogo executando `dotnet run` no terminal. 

Durante o gameplay, avalie e documente (marcando com um 'X' ou escrevendo observações nos itens abaixo) o comportamento das seguintes heurísticas e mecânicas:

#### A. Testes de Colisão e Navegação Física:
- [ ] O jogador consegue atravessar as paredes externas da masmorra se andar forçando na diagonal?
- [ ] O jogador atravessa objetos de decoração (pedras/cristais maiores)?
- [ ] Ao mudar de tela pela porta e voltar rapidamente, o jogador fica preso no pixel da parede ("Limbo")?

#### B. Testes de Interface (HUD):
- [ ] Ao coletar uma moeda (Baú), o contador de moedas no HUD aumenta imediatamente em tempo real?
- [ ] O HUD da sala relata corretamente por quais salas você passou (Ex: Sala 1, Sala 2, Sala do Boss)?

#### C. Testes de Combate e Comportamento dos Inimigos:
- [ ] O Slime persegue o jogador ao chegar perto?
- [ ] O inimigo Fantasma (Ghost) respeita colisões com a parede, ou ele atravessa por um erro de cálculo?
- [ ] Você percebeu se existe um breve período de "Invencibilidade" (o jogador pisca) após tomar um dano, impedindo que os monstros o matem em 1 único milissegundo repetidas vezes?

#### D. Caso de Teste Livre (Bugs Variados):
Testadores Caixa-Preta realizam testes destrutivos. Tente executar comandos inesperados para tentar travar ("crashar") o jogo. 
- [ ] Descreva um bug ou comportamento não-ideal que você encontrou rodando o jogo que requer resolução antes do lançamento comercial: 
> Resposta: ____________________________________________________________________


---

### Conclusão
Ao concluir esta atividade, você experimentou como testes de Caixa Branca e Caixa Preta não são concorrentes, mas sim complementares para entregar um jogo digital livre de bugs severos para a indústria de entretenimento.
