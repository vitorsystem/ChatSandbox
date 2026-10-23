# ChatSandBox

Um ecossistema completo de atendimento automatizado via WhatsApp, utilizando Inteligência Artificial, orquestração visual e microsserviços.

## Arquitetura e Tecnologias
* **Backend:** C# / ASP.NET Core (Gestão de Webhooks, regras de negócio).
* **Integração WhatsApp:** Evolution API v2 (Baileys) rodando em Docker.
* **Orquestração e IA:** n8n processando fluxos lógicos com LLMs via Groq/Gemini.
* **Banco de Dados & Cache:** PostgreSQL 15 e Redis.
* **Infraestrutura:** Docker & Docker Compose (Rede interna, DNS customizado).

## Como Executar
1. Clone o repositório.
2. Configure o arquivo `.env` com as chaves (Evolution API, Groq/Gemini, Postgres).
3. Execute `docker-compose up -d` para subir a infraestrutura completa.
4. Escaneie o QR Code via requisição HTTP e inicie a automação.

## Roadmap
- [ ] Implementar memória
- [ ] Adicionar system prompt
- [ ] Controle de tokens

## n8n
Há uma segunda branch nesse repositório, com uma versão pronta para rodar com um sistema n8n, que é uma ferramenta de automação de fluxos de trabalho. Essa versão permite a integração com o ChatSandBox, facilitando a criação e gerenciamento de fluxos lógicos para atendimento automatizado via WhatsApp.