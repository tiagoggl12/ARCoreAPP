# 🚀 Guia de Deploy iOS - BugabooXR

Este guia explica as diferentes maneiras de fazer deploy do seu app iOS para o Xcode Cloud, TestFlight e App Store.

---

## 📋 Opções de Deploy

### 1️⃣ **Xcode Cloud (Recomendado para simplicidade)**

**O que é:** Serviço de CI/CD nativo da Apple, integrado ao Xcode e App Store Connect.

**Vantagens:**
- ✅ Configuração visual no App Store Connect
- ✅ Integração nativa com TestFlight
- ✅ Máquinas Apple dedicadas
- ✅ Não precisa configurar certificates localmente
- ✅ Builds automáticos por branch/tag/PR

**Como configurar:**

1. **No App Store Connect:**
   - Vá para [App Store Connect](https://appstoreconnect.apple.com)
   - Selecione seu app → **Xcode Cloud**
   - Clique em **Get Started**
   - Conecte seu repositório GitHub

2. **Configurar Workflow no Xcode Cloud:**
   - Selecione a branch que quer monitorar (ex: `xcode-project`)
   - Configure quando fazer build:
     - Push to branch
     - Pull request
     - Tag
     - Schedule (agendado)
   - Configure ações pós-build:
     - TestFlight (beta testing)
     - Notificações

3. **Fazer push para a branch:**
   ```bash
   git push origin xcode-project
   ```

**Custo:** 25 horas/mês grátis, depois $14.99/mês por 25 horas adicionais.

---

### 2️⃣ **Fastlane + TestFlight (Recomendado para automação)**

**O que é:** Ferramenta de linha de comando para automação de deploys iOS/Android.

**Vantagens:**
- ✅ Controle total do processo
- ✅ Roda localmente ou em CI/CD
- ✅ Scripts reutilizáveis
- ✅ Comunidade grande
- ✅ Grátis

**Pré-requisitos:**

1. **Instalar Fastlane:**
   ```bash
   sudo gem install fastlane -NV
   ```

2. **Criar App Store Connect API Key:**
   - [App Store Connect](https://appstoreconnect.apple.com) → Users and Access → Keys
   - Clique em **+** para criar nova key
   - Baixe o arquivo `.p8`
   - Anote: Key ID, Issuer ID

3. **Configurar Secrets no GitHub:**
   - `APP_STORE_CONNECT_API_KEY_ID`: Key ID
   - `APP_STORE_CONNECT_API_ISSUER_ID`: Issuer ID
   - `APP_STORE_CONNECT_API_KEY`: Conteúdo do arquivo .p8 (base64)
   - `APPLE_TEAM_ID`: Seu Team ID
   - `PROVISIONING_PROFILE_NAME`: Nome do perfil de provisionamento
   - `MATCH_PASSWORD`: Senha para fastlane match (se usar)

**Uso local:**

```bash
# Build + Upload para TestFlight
cd /Users/tiago/Developer/BugabooXR
fastlane ios beta

# Apenas build (sem upload)
fastlane ios build

# Release para App Store
fastlane ios release
```

**Uso no GitHub Actions:**
- O workflow `ios-testflight.yml` já está configurado
- Execute manualmente em: Actions → iOS TestFlight Deploy

---

### 3️⃣ **xcrun altool (Linha de comando Apple)**

**O que é:** Ferramenta nativa da Apple para upload de builds.

**Vantagens:**
- ✅ Ferramenta oficial da Apple
- ✅ Não precisa instalar nada extra
- ✅ Simples e direto

**Como usar:**

```bash
# 1. Build com Unity
/Applications/Unity/Hub/Editor/*/Unity.app/Contents/MacOS/Unity \
  -quit -batchmode -nographics \
  -projectPath /Users/tiago/Developer/BugabooXR \
  -buildTarget iOS \
  -executeMethod BuildCommand.PerformBuild

# 2. Archive com Xcode
cd build/iOS
xcodebuild archive \
  -project Unity-iPhone.xcodeproj \
  -scheme Unity-iPhone \
  -configuration Release \
  -archivePath ./Unity-iPhone.xcarchive

# 3. Export IPA
xcodebuild -exportArchive \
  -archivePath ./Unity-iPhone.xcarchive \
  -exportPath ./output \
  -exportOptionsPlist ../../ExportOptions.plist

# 4. Upload para App Store Connect
xcrun altool --upload-app \
  --type ios \
  --file ./output/Unity-iPhone.ipa \
  --apiKey YOUR_API_KEY_ID \
  --apiIssuer YOUR_ISSUER_ID
```

---

### 4️⃣ **Transporter App (Upload manual)**

**O que é:** App oficial da Apple para upload de builds.

**Vantagens:**
- ✅ Interface gráfica
- ✅ Simples para uploads manuais
- ✅ Mostra erros de validação claramente

**Como usar:**

1. Baixe [Transporter](https://apps.apple.com/app/transporter/id1450874784) da Mac App Store
2. Faça login com sua Apple ID
3. Arraste o arquivo `.ipa` para o Transporter
4. Clique em **Deliver**

---

### 5️⃣ **Branch Deployment (Solução atual)**

**O que é:** Sua solução atual que faz push para branch `xcode-project`.

**Como funciona:**
- GitHub Actions faz build no Ubuntu
- Push do Xcode project para branch `xcode-project`
- Você clona essa branch no Mac e abre no Xcode

**Uso:**
```bash
# Clonar a branch xcode-project
git clone -b xcode-project https://github.com/YOUR_USERNAME/BugabooXR.git BugabooXR-Xcode

# Abrir no Xcode
cd BugabooXR-Xcode
open Unity-iPhone.xcodeproj

# Build manual no Xcode ou:
xcodebuild archive -project Unity-iPhone.xcodeproj -scheme Unity-iPhone
```

---

## 🎯 Qual método escolher?

| Método | Melhor para | Custo | Complexidade |
|--------|-------------|-------|--------------|
| **Xcode Cloud** | Times pequenos, integração Apple | $14.99/mês | Baixa ⭐ |
| **Fastlane** | Automação completa, CI/CD | Grátis | Média ⭐⭐ |
| **xcrun altool** | Scripts simples | Grátis | Média ⭐⭐ |
| **Transporter** | Upload manual ocasional | Grátis | Baixa ⭐ |
| **Branch Deploy** | Desenvolvimento, testar builds | Grátis | Baixa ⭐ |

---

## 🔐 Configuração de Signing

### Opção A: Manual (tradicional)
1. Criar App ID no Apple Developer Portal
2. Criar Certificates (Development + Distribution)
3. Criar Provisioning Profiles
4. Baixar e instalar no Keychain

### Opção B: Fastlane Match (recomendado)
```bash
# Inicializar match
fastlane match init

# Gerar certificados
fastlane match appstore

# Sync certificados (em outras máquinas)
fastlane match appstore --readonly
```

**Match** armazena certificados em repositório Git privado e sincroniza entre máquinas.

---

## 📝 Checklist antes do primeiro deploy

- [ ] App criado no App Store Connect
- [ ] Bundle ID configurado
- [ ] Certificados de distribuição criados
- [ ] Provisioning profile criado
- [ ] API Key criada (para upload automatizado)
- [ ] Secrets configurados no GitHub
- [ ] ExportOptions.plist atualizado com seu Team ID
- [ ] Build number incrementado
- [ ] Version number definido

---

## 🆘 Troubleshooting

### Erro: "No provisioning profile found"
- Verifique se o Bundle ID no Unity match o App ID no Apple Developer
- Baixe o provisioning profile e instale: duplo-clique no arquivo `.mobileprovision`

### Erro: "Code signing failed"
- Abra Keychain Access e verifique se o certificado está instalado
- Certifique-se de que o certificado não expirou

### Erro: "App Store Connect API authentication failed"
- Verifique se a API Key não foi revogada
- Confirme que Key ID e Issuer ID estão corretos
- Verifique se o arquivo .p8 está completo (base64)

### Erro: "Build processing failed"
- Aguarde 5-10 minutos (processamento pode demorar)
- Verifique no App Store Connect se há erros específicos

---

## 🔗 Links úteis

- [App Store Connect](https://appstoreconnect.apple.com)
- [Apple Developer Portal](https://developer.apple.com)
- [Fastlane Documentation](https://docs.fastlane.tools)
- [Xcode Cloud Documentation](https://developer.apple.com/xcode-cloud/)
- [Unity iOS Build Documentation](https://docs.unity3d.com/Manual/ios-BuildPlayer.html)

---

**Criado para BugabooXR** | Atualizado: Janeiro 2026
