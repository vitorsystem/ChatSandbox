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