# 🚀 Quick Deploy Reference

## Deploy Local (MacBook)

```bash
# Executar script interativo
./deploy-ios.sh
```

---

## Deploy via GitHub Actions

### Self-Hosted Runner (seu MacBook)
```bash
# No GitHub: Actions → Self-Hosted Build → Run workflow
# Selecione: iOS, Android, ou Both
```

### TestFlight Automático
```bash
# No GitHub: Actions → iOS TestFlight Deploy → Run workflow
```

---

## Deploy Manual Rápido

### Usando Fastlane (recomendado)
```bash
fastlane ios beta          # Build + upload TestFlight
fastlane ios build         # Apenas build
fastlane ios release       # Upload App Store
```

### Usando Transporter (GUI)
1. Build: `./deploy-ios.sh` → opção 2
2. Abrir Transporter.app
3. Arrastar o `.ipa` e clicar "Deliver"

### Usando xcrun (linha de comando)
```bash
xcrun altool --upload-app --type ios --file build/iOS/output/*.ipa \
  --apiKey $KEY_ID --apiIssuer $ISSUER_ID
```

---

## Xcode Cloud Setup

1. [App Store Connect](https://appstoreconnect.apple.com) → Seu App → Xcode Cloud
2. Conectar repositório GitHub
3. Criar workflow monitorando branch `xcode-project`
4. Configurar ação pós-build: TestFlight

**Pronto!** Push para `xcode-project` = Build automático no Xcode Cloud

---

## Secrets necessários (GitHub)

Para usar workflows automatizados, configure em Settings → Secrets:

```
APP_STORE_CONNECT_API_KEY_ID      # Key ID da API
APP_STORE_CONNECT_API_ISSUER_ID   # Issuer ID
APP_STORE_CONNECT_API_KEY         # Conteúdo .p8 em base64
APPLE_TEAM_ID                     # Seu Team ID
PROVISIONING_PROFILE_NAME         # Nome do perfil
MATCH_PASSWORD                    # Senha do fastlane match (opcional)
```

---

## 📖 Documentação completa

Ver `DEPLOY_GUIDE.md` para guia detalhado.

---

## 🆘 Troubleshooting rápido

**Erro signing:** Abra o projeto no Xcode e configure signing automático
**Erro Unity:** Ajuste `UNITY_PATH` no script
**Erro upload:** Verifique API key no App Store Connect
**Build lento:** Use self-hosted runner ou Xcode Cloud

---

## 📞 Links úteis

- [App Store Connect](https://appstoreconnect.apple.com)
- [Apple Developer](https://developer.apple.com)
- [Fastlane Docs](https://docs.fastlane.tools)
