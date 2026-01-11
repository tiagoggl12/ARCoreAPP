# CI/CD Setup Guide - ARCoreAPP

Este guia contém todas as instruções para configurar e usar o pipeline CI/CD do projeto ARCoreAPP.

## Índice

1. [Configuração Inicial](#configuração-inicial)
2. [Unity License Setup](#unity-license-setup)
3. [Android Keystore Setup](#android-keystore-setup)
4. [GitHub Secrets Configuration](#github-secrets-configuration)
5. [Xcode Cloud Setup (iOS)](#xcode-cloud-setup-ios)
6. [Usando os Workflows](#usando-os-workflows)
7. [Troubleshooting](#troubleshooting)

---

## Configuração Inicial

### Pré-requisitos

- Conta Unity (Personal, Plus, ou Pro)
- Conta GitHub com acesso ao repositório
- Para iOS: Apple Developer Account ($99/ano)
- Java Development Kit (JDK) para gerar Android keystore

### Estrutura do Projeto

O CI/CD está configurado com os seguintes workflows:

```
.github/workflows/
├── test-runner.yml           # Testes rápidos em PRs
├── android-build.yml         # Pipeline completo Android
└── generate-xcode-project.yml # Geração projeto iOS
```

---

## Unity License Setup

### Opção 1: Manual Activation (Recomendada para Unity Personal)

1. **Gerar arquivo de ativação (.alf)**

   Crie um workflow temporário ou use uma action para gerar o arquivo:

   ```yaml
   # .github/workflows/get-activation-file.yml
   name: Get Unity Activation File
   on: workflow_dispatch
   jobs:
     activation:
       runs-on: ubuntu-latest
       steps:
         - uses: actions/checkout@v4
         - uses: game-ci/unity-request-activation-file@v2
           id: getManualLicenseFile
           with:
             unityVersion: 2023.2.3f1
         - uses: actions/upload-artifact@v4
           with:
             name: Unity_ALF
             path: ${{ steps.getManualLicenseFile.outputs.filePath }}
   ```

2. **Execute o workflow** e baixe o arquivo `.alf` dos artifacts

3. **Ative manualmente**:
   - Acesse https://license.unity3d.com/manual
   - Faça upload do arquivo `.alf`
   - Baixe o arquivo de licença `.ulf` gerado

4. **Adicione como secret**:
   - Abra o arquivo `.ulf` em um editor de texto
   - Copie todo o conteúdo
   - No GitHub: Settings > Secrets > Actions > New repository secret
   - Nome: `UNITY_LICENSE`
   - Value: Cole o conteúdo do arquivo `.ulf`

### Opção 2: Credential-based Activation

Se você tem Unity Plus ou Pro:

1. Adicione os secrets:
   - `UNITY_EMAIL`: seu email Unity
   - `UNITY_PASSWORD`: sua senha Unity
   - `UNITY_SERIAL`: seu serial key Unity (apenas Pro/Plus)

### Renovação de Licença

- Licenças Unity Personal expiram após ~21 dias
- Monitore os logs dos workflows para avisos de expiração
- Repita o processo de ativação quando necessário

---

## Android Keystore Setup

### 1. Gerar Keystore

Use o keytool (incluído no JDK) para gerar seu keystore:

```bash
keytool -genkey -v -keystore user.keystore \
  -alias release \
  -keyalg RSA \
  -keysize 2048 \
  -validity 10000
```

**Informações necessárias**:
- **Keystore password**: Senha para proteger o keystore (lembre-se dela!)
- **Key password**: Senha para a chave específica (pode ser igual ao keystore)
- **Alias**: Nome da chave (use "release" como sugerido)
- **Informações pessoais**: Nome, organização, localização (serão parte do certificado)

**IMPORTANTE**:
- Guarde o keystore (`user.keystore`) em local seguro offline
- Nunca commite o keystore no repositório
- Se perder o keystore, não poderá mais atualizar o app na Play Store

### 2. Converter Keystore para Base64

```bash
# Linux/macOS
base64 user.keystore > user.keystore.base64

# Windows PowerShell
[Convert]::ToBase64String([IO.File]::ReadAllBytes("user.keystore")) > user.keystore.base64
```

### 3. Adicionar ao GitHub Secrets

Copie o conteúdo de `user.keystore.base64` e adicione como secret `ANDROID_KEYSTORE_BASE64`.

**Não confunda**: Adicione o arquivo .base64 codificado, não o .keystore original!

---

## GitHub Secrets Configuration

Navegue para: **Settings > Secrets and variables > Actions > New repository secret**

### Secrets Obrigatórios

| Secret Name | Description | Como Obter |
|------------|-------------|------------|
| `UNITY_LICENSE` | Conteúdo do arquivo .ulf | Ver [Unity License Setup](#unity-license-setup) |
| `UNITY_EMAIL` | Email da conta Unity | Suas credenciais Unity |
| `UNITY_PASSWORD` | Senha da conta Unity | Suas credenciais Unity |
| `ANDROID_KEYSTORE_BASE64` | Keystore em base64 | Ver [Android Keystore Setup](#android-keystore-setup) |
| `ANDROID_KEYSTORE_PASS` | Senha do keystore | Definida ao criar keystore |
| `ANDROID_KEY_ALIAS` | Alias da chave | Definido ao criar (ex: "release") |
| `ANDROID_KEY_PASS` | Senha da chave | Definida ao criar keystore |

### Secrets Opcionais

| Secret Name | Description | Quando Usar |
|------------|-------------|-------------|
| `CODECOV_TOKEN` | Token do Codecov | Se usar cobertura de código |
| `SLACK_WEBHOOK_URL` | Webhook Slack | Para notificações |

### Verificar Secrets

Depois de adicionar, você deve ver algo assim em Settings > Secrets:

```
UNITY_LICENSE               Updated X days ago
UNITY_EMAIL                 Updated X days ago
UNITY_PASSWORD              Updated X days ago
ANDROID_KEYSTORE_BASE64     Updated X days ago
ANDROID_KEYSTORE_PASS       Updated X days ago
ANDROID_KEY_ALIAS           Updated X days ago
ANDROID_KEY_PASS            Updated X days ago
```

**Nota**: Valores dos secrets nunca são mostrados após serem salvos.

---

## Xcode Cloud Setup (iOS)

### Pré-requisitos

1. **Apple Developer Account** ($99/ano)
   - Inscreva-se em https://developer.apple.com

2. **App Store Connect**:
   - Acesse https://appstoreconnect.apple.com
   - Crie uma nova app:
     - Nome: ARCoreAPP (ou seu nome escolhido)
     - Bundle ID: `com.unity.template.armobile` (ou personalize)
     - Platform: iOS

### Configuração do Xcode Cloud

#### 1. Gerar Projeto Xcode

Execute o workflow `generate-xcode-project.yml`:

```bash
# Via GitHub UI
Actions > Generate Xcode Project for iOS > Run workflow
```

Ou manualmente no Unity:
- File > Build Settings > iOS
- Click "Build" (não "Build and Run")
- Escolha pasta `ios-build/`

#### 2. Testar Localmente (macOS)

Se tiver acesso a um Mac:

```bash
# Abrir projeto no Xcode
open ios-build/ARCoreAPP.xcodeproj

# Build para verificar que não há erros
# Product > Build (Cmd+B)
```

#### 3. Commit para Branch xcode-project

O workflow automaticamente faz commit para o branch `xcode-project`, ou manualmente:

```bash
git checkout -b xcode-project
cp -r ios-build/* .
git add .
git commit -m "Add Xcode project for Xcode Cloud"
git push origin xcode-project
```

#### 4. Configurar Xcode Cloud

No **App Store Connect**:

1. Acesse sua app > Xcode Cloud

2. **Configure Source Control**:
   - Provider: GitHub
   - Repository: `tiagoggl12/ARCoreAPP`
   - Branch: `xcode-project`

3. **Create Workflow**:
   - Name: "TestFlight Distribution"
   - Environment: macOS 14 (Sonoma), Xcode 15+

4. **Start Conditions**:
   - Branch Changes: `xcode-project`
   - Files/Folders: Qualquer mudança

5. **Actions**:
   - ✅ Build
   - ✅ Test (opcional, se tiver UI tests)
   - ✅ Analyze (opcional)

6. **Archive**:
   - ✅ Enable
   - Distribution Method: TestFlight Internal Testing

7. **Post-Actions**:
   - ✅ TestFlight Internal Testing
   - ✅ Notify via email

8. **Environment Variables** (opcional):
   ```
   UNITY_VERSION=6000.3.3f1
   CI=true
   ```

#### 5. Provisioning Profiles e Signing

O Xcode Cloud gerencia automaticamente, mas verifique:

- **Automatic Signing**: Recomendado
- **Team**: Selecione seu Apple Developer Team
- **Bundle Identifier**: Deve bater com App Store Connect

### Workflow iOS

Quando quiser atualizar o app iOS:

1. Faça mudanças no Unity
2. Execute workflow `generate-xcode-project.yml` (automático no push para main)
3. Xcode Cloud detecta mudança em `xcode-project` branch
4. Build automático e deploy para TestFlight
5. Testadores recebem notificação de nova build

---

## Usando os Workflows

### Test Runner (Pull Requests)

**Trigger**: Automaticamente em PRs para `main` ou `develop`

**O que faz**:
- Lint C# code
- Roda Edit Mode tests
- Roda Play Mode tests

**Como usar**:
1. Crie um PR
2. Workflows executam automaticamente
3. Veja resultados na aba "Checks" do PR
4. Merge apenas se todos os checks passarem

### Android Build (Main Branch)

**Trigger**: Automaticamente ao fazer push para `main`

**O que faz**:
- Lint + Tests
- Valida configuração do projeto
- Build APK (desenvolvimento)
- Build AAB (produção)
- Cria GitHub Release com artifacts

**Como usar**:
1. Merge PR para `main`
2. Workflow executa automaticamente
3. Aguarde ~20-30 minutos
4. Baixe builds em:
   - Actions > Workflow run > Artifacts
   - Releases > Latest release

**Manual Trigger**:
- Actions > Android CI/CD Pipeline > Run workflow
- Escolha build type (APK, AAB, ou Both)

### Generate Xcode Project (iOS)

**Trigger**: Manual ou automático (push para main com mudanças em Assets/)

**O que faz**:
- Gera projeto Xcode do Unity
- Faz commit para branch `xcode-project`
- Upload como artifact

**Como usar**:
1. Actions > Generate Xcode Project for iOS > Run workflow
2. Aguarde ~15-20 minutos
3. Xcode Cloud detecta e builda automaticamente
4. Build aparece no TestFlight em ~30 minutos

---

## Troubleshooting

### Erro: "Unity license activation failed"

**Possíveis causas**:
- Licença expirada (>21 dias)
- Secret `UNITY_LICENSE` inválido ou mal formatado
- Muitas ativações simultâneas

**Solução**:
1. Verifique se o secret contém o arquivo `.ulf` completo
2. Regenere a licença seguindo [Unity License Setup](#unity-license-setup)
3. Certifique-se de que o workflow `cleanup` está retornando a licença

### Erro: "Keystore not found" ou "Wrong password"

**Possíveis causas**:
- Base64 do keystore corrompido
- Senha do keystore incorreta
- Alias da chave incorreto

**Solução**:
1. Verifique que `ANDROID_KEYSTORE_BASE64` foi copiado corretamente
2. Teste localmente decodificando: `echo "$BASE64" | base64 --decode > test.keystore`
3. Verifique senhas e alias usados na criação do keystore
4. Regenere keystore se necessário (mas não poderá atualizar apps existentes)

### Erro: "Tests failed"

**Possíveis causas**:
- Código com bugs
- Tests quebrados
- Mudanças que quebram funcionalidade

**Solução**:
1. Rode tests localmente no Unity: Window > General > Test Runner
2. Corrija erros de código
3. Verifique logs do workflow para detalhes
4. Re-run workflow após correção

### Erro: "Out of disk space"

**Possíveis causas**:
- Library/ cache muito grande
- Artifacts acumulados

**Solução**:
1. Limpe cache de workflows: Settings > Actions > Caches
2. Reduza retention de artifacts (já está em 30/90 dias)
3. Use `.gitignore` corretamente para não commitar Library/

### Builds Android muito lentos (>30min)

**Possíveis causas**:
- Cache não está funcionando
- Library/ não está sendo cacheada corretamente

**Solução**:
1. Verifique se cache hit está acontecendo nos logs
2. Limpe e recrie cache se necessário
3. Use IL2CPP apenas para production (AAB), Mono para development (APK)

### Xcode Cloud build failing

**Possíveis causas**:
- Projeto Xcode corrompido ou inválido
- Signing/provisioning issues
- Dependencies faltando

**Solução**:
1. Baixe artifact do projeto Xcode
2. Teste build localmente no Xcode (se tiver Mac)
3. Verifique logs no App Store Connect > Xcode Cloud
4. Certifique-se de que automatic signing está habilitado
5. Regenere projeto do Unity se necessário

### Como limpar e recomeçar

Se tudo falhar:

```bash
# 1. Limpe caches no GitHub
Settings > Actions > Caches > Delete all

# 2. Delete e recrie secrets problemáticos
Settings > Secrets > Actions > Delete > Add new

# 3. Re-run workflows from scratch
Actions > Select workflow > Re-run all jobs
```

---

## Manutenção e Boas Práticas

### Rotação de Credentials

Troque secrets a cada 90 dias:
- Unity password
- Android keystore (crie novo para novos apps)

### Backup de Keystore

- Guarde `user.keystore` em local seguro (Google Drive, 1Password, etc.)
- Guarde senhas em gerenciador de senhas
- Nunca commite no Git

### Monitoring

- Configure notificações: Settings > Notifications
- Revise workflow runs regularmente
- Monitore uso de Actions minutes: Settings > Billing

### Atualizações

Quando atualizar Unity:
- Atualize `UNITY_VERSION` nos workflows
- Teste localmente primeiro
- Verifique compatibilidade com Game CI

---

## Recursos Adicionais

- [Game CI Documentation](https://game.ci/docs)
- [Unity Test Framework](https://docs.unity3d.com/Packages/com.unity.test-framework@latest)
- [Xcode Cloud Documentation](https://developer.apple.com/xcode-cloud/)
- [GitHub Actions Documentation](https://docs.github.com/actions)

---

## Suporte

Para problemas específicos do projeto:
- Abra uma issue no GitHub
- Consulte logs dos workflows em Actions
- Revise este documento

Para problemas com Game CI:
- [Game CI Discord](https://game.ci/discord)
- [GitHub Discussions](https://github.com/game-ci/unity-builder/discussions)
