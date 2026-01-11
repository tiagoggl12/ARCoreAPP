# Xcode Cloud Setup Guide - TestFlight Deployment

Guia completo para configurar Xcode Cloud e fazer deploy automático para TestFlight.

## Pré-requisitos

✅ Apple Developer Account ($99/ano) - **VOCÊ JÁ TEM**
✅ Projeto Unity configurado
✅ GitHub repositório configurado

## Visão Geral do Fluxo

```
Commit no GitHub (main branch)
    ↓
GitHub Actions: generate-xcode-project.yml
    ↓
Gera projeto Xcode do Unity
    ↓
Commit para branch 'xcode-project'
    ↓
Xcode Cloud detecta mudança
    ↓
Build automático no macOS da Apple
    ↓
Upload para TestFlight
    ↓
Testadores recebem notificação
```

---

## Parte 1: Criar App no App Store Connect

### 1.1 Acessar App Store Connect

1. Vá para https://appstoreconnect.apple.com
2. Faça login com sua conta Apple Developer
3. Clique em **"My Apps"** (ou "Meus Apps")

### 1.2 Criar Novo App

1. Clique no botão **"+"** (ou "Add Apps")
2. Selecione **"New App"**
3. Preencha os dados:

   **Platforms**: ☑️ iOS

   **Name**: `ARCoreAPP` (ou seu nome preferido)
   - Este é o nome que aparecerá na App Store
   - Pode ser mudado depois

   **Primary Language**: Portuguese (Brazil) ou English (U.S.)

   **Bundle ID**:
   - **IMPORTANTE**: Use o mesmo do Unity!
   - Verificar em: Unity > Project Settings > Player > iOS > Bundle Identifier
   - Provavelmente: `com.unity.template.armobile`
   - Se não existir na lista, crie um novo:
     1. Clique em "Register a new Bundle ID"
     2. ID: `com.unity.template.armobile` (ou seu custom)
     3. Description: "ARCore Mobile AR App"
     4. Capabilities: Nenhuma especial necessária

   **SKU**: `arcore-app-001` (código único interno, não visível publicamente)

   **User Access**: Full Access

4. Clique **"Create"**

### 1.3 Configurar App Information

Após criar, você estará na página do app. Configure:

1. **General Information**:
   - **Subtitle**: "AR Experience for Mobile" (opcional, 30 chars max)
   - **Category**: Games ou Education (escolha apropriada)
   - **Content Rights**: Marque se contém/não contém anúncios de terceiros

2. **Age Rating**:
   - Responda o questionário sobre conteúdo
   - Para app AR básico, geralmente será 4+

3. **Save Changes**

---

## Parte 2: Configurar GitHub no Xcode Cloud

### 2.1 Conectar Repositório

1. No App Store Connect, vá para **Xcode Cloud** na sidebar
2. Se for primeira vez:
   - Clique em **"Get Started"**
   - Aceite os termos de serviço

3. **Configure Source Control**:
   - Click **"Choose Repository"**
   - Select **"GitHub"**
   - Clique **"Connect to GitHub"**

4. **Autorizar GitHub**:
   - Uma janela do GitHub abrirá
   - Faça login na sua conta GitHub (tiagoggl12)
   - Autorize "Xcode Cloud" a acessar seus repositórios
   - Selecione:
     - ☑️ All repositories, OU
     - ☑️ Only select repositories → `ARCoreAPP`
   - Clique **"Install & Authorize"**

5. **Select Repository**:
   - Repository: `tiagoggl12/ARCoreAPP`
   - Clique **"Next"**

### 2.2 Escolher Branch e Projeto

1. **Select Branch**:
   - Branch: `xcode-project`
   - ⚠️ **IMPORTANTE**: Não use `main`!
   - Use `xcode-project` (branch que contém o projeto Xcode exportado)
   - Se o branch não existir ainda, clique "Next" e criaremos depois

2. **Select Xcode Project/Workspace**:
   - ⚠️ Xcode Cloud tentará detectar automaticamente
   - Se não detectar, **NÃO CONTINUE AINDA**
   - Precisamos gerar o projeto Xcode primeiro (Parte 3)

3. Por enquanto, **PARE AQUI** e vá para Parte 3

---

## Parte 3: Gerar Projeto Xcode do Unity

Antes de continuar no Xcode Cloud, precisamos gerar o projeto Xcode.

### 3.1 Executar Workflow de Geração

**Via GitHub UI**:

1. Vá para https://github.com/tiagoggl12/ARCoreAPP
2. Clique na aba **"Actions"**
3. No sidebar esquerdo, clique em **"Generate Xcode Project for iOS"**
4. Clique no botão **"Run workflow"** (lado direito)
5. Configurações:
   - Branch: `main` (use main para rodar o workflow)
   - Push to xcode-project branch: `true` ✅
6. Clique **"Run workflow"** (botão verde)

### 3.2 Aguardar Conclusão

1. Workflow levará **~15-20 minutos**
2. Você verá o status:
   - 🟡 Amarelo (In progress): Rodando
   - ✅ Verde (Completed): Sucesso!
   - ❌ Vermelho (Failed): Erro (veja logs)

3. **Se der erro de Unity License**:
   - Você precisa configurar os secrets primeiro
   - Veja seção "Troubleshooting" abaixo

### 3.3 Verificar Branch xcode-project

Após workflow completar:

1. No GitHub, mude para branch `xcode-project`:
   - No dropdown de branches (topo esquerdo), selecione `xcode-project`

2. Verifique que existe uma pasta `ios-build/` com:
   - `ios-build/ARCoreAPP.xcodeproj`
   - Arquivos `.h`, `.m`, `.mm`
   - Pasta `Data/`
   - Pasta `Libraries/`

3. Se tudo estiver lá, continue para Parte 4

---

## Parte 4: Criar Workflow no Xcode Cloud

### 4.1 Criar Primeiro Workflow

1. Volte para **App Store Connect** > **Xcode Cloud**
2. Agora que o projeto Xcode existe, Xcode Cloud deve detectá-lo
3. Clique **"Create Workflow"** ou **"Add Workflow"**

### 4.2 Configurar Workflow - General

1. **Workflow Name**: `TestFlight Deployment`
2. **Description**: "Automated build and TestFlight deployment from GitHub"
3. **Restrict Editing**: Off (você pode editar no App Store Connect)

### 4.3 Configurar Workflow - Environment

1. **Xcode Version**:
   - Selecione a versão mais recente disponível
   - Recomendado: Xcode 15.x ou mais novo
   - ⚠️ Certifique-se de que suporta Unity 6

2. **macOS Version**:
   - Selecione a versão mais recente
   - Recomendado: macOS 14 (Sonoma) ou macOS 15 (Sequoia)

3. **Environment Variables** (opcional):
   - Você pode adicionar variáveis customizadas se necessário
   - Para começar, deixe vazio

### 4.4 Configurar Workflow - Start Conditions

**Como o build deve ser disparado?**

1. Clique em **"Add Start Condition"**

2. **Option 1: Branch Changes** (RECOMENDADO):
   - Select: **"Branch Changes"**
   - Branch: `xcode-project`
   - Files and Folders: (deixe vazio para detectar qualquer mudança)
   - ✅ Esta configuração faz build automático quando você fizer push para xcode-project

3. **Option 2: Manual** (adicional, opcional):
   - Clique **"Add Start Condition"** novamente
   - Select: **"Manual"**
   - Permite disparar build manualmente no App Store Connect

4. **Option 3: Pull Request** (se quiser testar PRs):
   - Útil se você quiser testar antes de merge

**Recomendação**: Use **Branch Changes** no `xcode-project` + **Manual**

### 4.5 Configurar Workflow - Actions

Agora configuramos o que o workflow faz:

#### Action 1: Build

1. **Build Action** já está adicionado por padrão
2. Configure:
   - **Scheme**: `Unity-iPhone` (deve ser detectado automaticamente)
   - **Platform**: iOS
   - **Build Configuration**: Release (para TestFlight)
   - ⚠️ Se o scheme não for detectado, use "iOS"

#### Action 2: Test (Opcional)

- Se você tiver testes UI no Xcode, pode adicionar
- Por enquanto, **pule esta ação**

#### Action 3: Analyze (Opcional)

- Análise estática de código
- Por enquanto, **pule esta ação**

### 4.6 Configurar Workflow - Archive

**IMPORTANTE**: Esta é a parte que gera o IPA para TestFlight!

1. Clique em **"Archive"** (ou "Add Post-Action" > "Archive")

2. Configure:
   - **Archive Configuration**: Release
   - **Include**: ☑️ Include app archive
   - **Distribution Method**: TestFlight and App Store

3. **Signing & Capabilities**:
   - **Automatic Signing**: ☑️ Enabled (RECOMENDADO)
   - **Team**: Selecione seu Apple Developer Team
   - **Bundle Identifier**: `com.unity.template.armobile` (ou seu custom)
   - Xcode Cloud gerenciará certificados e provisioning profiles automaticamente

4. **Prepare for Distribution**:
   - ☑️ Manage Version and Build Number (auto-incrementa)
   - ☑️ Include Bitcode: Off (iOS não usa mais)
   - ☑️ Upload Symbols: On (para crash reports)

### 4.7 Configurar Workflow - Post-Actions

#### Post-Action 1: TestFlight Internal Testing

1. Clique **"Add Post-Action"**
2. Selecione **"TestFlight Internal Testing"**
3. Configure:
   - **Submit for Review**: ❌ Off (para testes rápidos)
   - **Automatically notify testers**: ☑️ On
   - **What to Test**: "New build from automated CI/CD"
   - **Internal Testing Groups**:
     - Selecione o grupo (App Store Connect Users - Internal)
     - Ou crie um grupo novo

#### Post-Action 2: Notificações (Opcional)

1. Clique **"Add Post-Action"**
2. Selecione **"Notify"**
3. Configure:
   - Email: seu@email.com
   - Notificar em: Build Success e Build Failure

### 4.8 Salvar Workflow

1. Revise todas as configurações
2. Clique **"Create"** ou **"Save"**
3. Workflow está pronto! 🎉

---

## Parte 5: Configurar TestFlight e Testadores

### 5.1 Adicionar Testadores Internos

1. No App Store Connect, vá para **"TestFlight"**
2. Clique em **"Internal Testing"** (sidebar)
3. Clique no **"+"** para adicionar testadores
4. Adicione usuários da sua equipe (até 100 testadores internos)
5. Eles precisam ter conta no App Store Connect

### 5.2 Criar Grupo de Teste (Opcional)

1. Em **"Internal Testing"**, clique **"Create Group"**
2. Nome: "QA Team" ou "Dev Team"
3. Adicione testadores ao grupo
4. Este grupo receberá builds automaticamente

### 5.3 Configurar Informações do App para TestFlight

1. Vá para **"App Store"** > seu app > **"TestFlight"** tab
2. Preencha:
   - **What to Test**: Descrição das mudanças na build
   - **Feedback Email**: seu@email.com (para receber feedback)
   - **Marketing URL**: (opcional)
   - **Privacy Policy URL**: (se app coleta dados)

---

## Parte 6: Primeiro Deploy de Teste

### 6.1 Trigger Manual (Primeira Build)

Para testar tudo:

1. Vá para **App Store Connect** > **Xcode Cloud**
2. Selecione seu workflow **"TestFlight Deployment"**
3. Clique **"Start Build"** (botão no topo direito)
4. Confirme: **"Start"**

### 6.2 Acompanhar Build

1. Build aparecerá na lista com status:
   - 🔵 Preparing
   - 🟡 Building
   - 🟢 Succeeded
   - 🔴 Failed

2. Clique na build para ver logs detalhados

3. **Tempo estimado**: ~30-40 minutos
   - Cloning: 2-3 min
   - Building: 20-30 min
   - Archiving: 5 min
   - Upload TestFlight: 5 min

### 6.3 Verificar TestFlight

Após build completar:

1. Vá para **TestFlight** tab no App Store Connect
2. Aguarde ~5-10 minutos para processamento
3. Build aparecerá em **"Internal Testing"**
4. Status:
   - "Processing" → Aguarde
   - "Ready to Submit" → Pronto!
   - "Missing Compliance" → Precisa responder questionário

### 6.4 Responder Export Compliance

Se aparecer "Missing Compliance":

1. Clique na build
2. **"Provide Export Compliance Information"**
3. Perguntas:
   - Does your app use encryption?
     - **No** (se só usa HTTPS padrão)
     - **Yes** (se usa criptografia customizada)
   - Responda honestamente baseado no seu app
4. Salve

5. Build ficará "Ready to Test"

### 6.5 Testar no Dispositivo

1. Instale **TestFlight** app no seu iPhone/iPad
   - Download: App Store

2. Faça login com seu Apple ID (mesmo da Developer Account)

3. O app **"ARCoreAPP"** aparecerá automaticamente

4. Clique **"Install"**

5. Teste o app! 🎉

---

## Parte 7: Deploy Automático (Após Primeira Build)

### 7.1 Fluxo Automático

Agora que está configurado, o fluxo é:

```bash
# 1. Faça mudanças no Unity
git checkout main
# ... faça mudanças em Assets/ ...

# 2. Commit e push
git add .
git commit -m "Update AR features"
git push origin main

# 3. GitHub Actions gera Xcode project automaticamente
# (workflow generate-xcode-project.yml detecta mudança em Assets/)

# 4. Xcode Cloud detecta mudança em xcode-project branch

# 5. Build automático

# 6. TestFlight recebe nova build

# 7. Testadores notificados via email
```

### 7.2 Monitorar Builds

1. Acesse **App Store Connect** > **Xcode Cloud**
2. Veja histórico de builds
3. Configure notificações por email/Slack

### 7.3 Gerenciar Versões

**Build Numbers**: Auto-incrementados pelo Xcode Cloud
- 1.0.0 (1)
- 1.0.0 (2)
- 1.0.0 (3)
- etc.

**Version Numbers**: Atualize manualmente no Unity
- Unity > Project Settings > Player > iOS > Version
- Use Semantic Versioning: 1.0.0 → 1.1.0 → 2.0.0

---

## Troubleshooting

### Erro: "Unity License Required"

**Problema**: Workflow `generate-xcode-project.yml` falha sem licença Unity

**Solução**:
1. Veja **CI-CD-SETUP.md** seção "Unity License Setup"
2. Configure secrets:
   - `UNITY_LICENSE`
   - `UNITY_EMAIL`
   - `UNITY_PASSWORD`

### Erro: "Repository Not Found"

**Problema**: Xcode Cloud não consegue acessar repositório

**Solução**:
1. Re-autorize GitHub:
   - GitHub Settings > Applications > Xcode Cloud
   - Revoke and re-authorize
2. Certifique-se que repositório é público ou Xcode Cloud tem acesso

### Erro: "Xcode Project Not Found"

**Problema**: Branch `xcode-project` não contém projeto válido

**Solução**:
1. Re-execute workflow `generate-xcode-project.yml`
2. Verifique que `ios-build/ARCoreAPP.xcodeproj` existe
3. Certifique-se que Unity Editor está fechado durante geração

### Erro: "Code Signing Failed"

**Problema**: Certificados ou provisioning profiles inválidos

**Solução**:
1. Use **Automatic Signing** no Xcode Cloud (recomendado)
2. Certifique-se que Bundle ID bate com App Store Connect
3. Revise Team ID no Xcode Cloud settings

### Build Fica em "Processing" Forever

**Problema**: TestFlight demora muito para processar

**Solução**:
1. Normal até 1 hora em primeira build
2. Atualize a página periodicamente
3. Se >2 horas, contate Apple Developer Support

### Testadores Não Recebem Notificação

**Problema**: Notificações não chegam

**Solução**:
1. Verifique email dos testadores no App Store Connect
2. Testadores precisam aceitar convite primeiro
3. Check spam folder
4. Re-envie convite: TestFlight > Internal Testing > Testers > Resend Invite

---

## Custos

### Xcode Cloud
- **Free Tier**: 25 compute hours/mês
- **Build Time**: ~30 min/build
- **Estimativa**: ~50 builds grátis/mês
- **Excedente**: $0.40/compute hour

### Apple Developer
- **$99/ano** (você já tem)

### GitHub Actions
- **Free**: 2000 minutos/mês
- Workflow `generate-xcode-project.yml` usa ~15 min/build
- **Estimativa**: ~130 gerações grátis/mês

**Total**: $99/ano (já pago)

---

## Próximos Passos

### Imediato (Agora)
1. ✅ Configure App no App Store Connect (Parte 1)
2. ✅ Conecte GitHub ao Xcode Cloud (Parte 2)
3. ✅ Execute workflow para gerar Xcode project (Parte 3)
4. ✅ Configure workflow no Xcode Cloud (Parte 4)
5. ✅ Teste primeira build (Parte 6)

### Curto Prazo (Próximos Dias)
- Adicione mais testadores internos
- Teste em múltiplos dispositivos
- Configure External Testing (até 10,000 testadores)
- Adicione screenshots e descrição no TestFlight

### Longo Prazo (Produção)
- Quando estiver pronto, submeta para App Store Review
- Configure In-App Purchases (se necessário)
- Setup analytics e crash reporting

---

## Referências

- [Xcode Cloud Documentation](https://developer.apple.com/xcode-cloud/)
- [TestFlight Documentation](https://developer.apple.com/testflight/)
- [App Store Connect Help](https://help.apple.com/app-store-connect/)
- [Unity iOS Build Guide](https://docs.unity3d.com/Manual/iphone-GettingStarted.html)

---

## Suporte

- **Apple Developer Support**: https://developer.apple.com/support/
- **Xcode Cloud Forums**: https://developer.apple.com/forums/tags/xcode-cloud
- **Projeto GitHub Issues**: https://github.com/tiagoggl12/ARCoreAPP/issues

---

**Boa sorte com o deploy! 🚀**
