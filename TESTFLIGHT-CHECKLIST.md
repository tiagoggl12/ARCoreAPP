# TestFlight Deploy Checklist

Checklist rápido para primeiro deploy no TestFlight via Xcode Cloud.

## ☐ Pré-requisitos (5 min)

- [ ] Apple Developer Account ativo ($99/ano)
- [ ] Acesso ao App Store Connect
- [ ] GitHub repositório configurado
- [ ] Unity License secrets configurados no GitHub

## ☐ App Store Connect Setup (10 min)

- [ ] Criar app no App Store Connect
  - Nome: BugabooXR
  - Bundle ID: `com.unity.template.armobile`
  - SKU: arcore-app-001
- [ ] Configurar Age Rating
- [ ] Configurar General Information

## ☐ GitHub Connection (5 min)

- [ ] App Store Connect > Xcode Cloud > "Connect to GitHub"
- [ ] Autorizar Xcode Cloud no GitHub
- [ ] Selecionar repositório: `tiagoggl12/BugabooXR`
- [ ] Selecionar branch: `xcode-project` ✅ **JÁ EXISTE**

## ☐ Generate Xcode Project (15-20 min)

- [ ] GitHub > Actions > "Generate Xcode Project for iOS"
- [ ] Run workflow (branch: main, push to xcode-project: true)
- [ ] Aguardar conclusão (~15 min)
- [ ] Verificar que branch `xcode-project` contém `ios-build/`

## ☐ Xcode Cloud Workflow (10 min)

- [ ] App Store Connect > Xcode Cloud > "Create Workflow"
- [ ] Nome: "TestFlight Deployment"
- [ ] Environment: macOS 14+, Xcode 15+
- [ ] Start Condition: Branch Changes (`xcode-project`)
- [ ] Build Action: Scheme `Unity-iPhone`, Platform iOS, Config Release
- [ ] Archive: Automatic Signing, Include app archive
- [ ] Post-Action: TestFlight Internal Testing (notify testers: ON)
- [ ] Salvar workflow

## ☐ TestFlight Setup (5 min)

- [ ] TestFlight > Internal Testing
- [ ] Adicionar testadores (email)
- [ ] Criar grupo de teste (opcional)
- [ ] Configurar "What to Test" message

## ☐ First Build (30-40 min)

- [ ] Xcode Cloud > Workflow > "Start Build"
- [ ] Aguardar build completar (~30 min)
- [ ] Verificar logs se falhar
- [ ] Aguardar processamento TestFlight (~5-10 min)

## ☐ Export Compliance (2 min)

- [ ] TestFlight > Build > "Provide Export Compliance"
- [ ] Responder questionário sobre criptografia
- [ ] Salvar

## ☐ Test on Device (5 min)

- [ ] Instalar app TestFlight no iPhone/iPad
- [ ] Fazer login com Apple ID (mesmo da Developer Account)
- [ ] Verificar que BugabooXR aparece
- [ ] Install e testar! 🎉

---

## Tempo Total Estimado

- **Setup inicial**: ~45 minutos
- **Primeira build**: ~30-40 minutos
- **Total**: ~1h30min

---

## Próxima Build (Automático)

Após setup inicial, próximas builds são automáticas:

1. Commit mudanças no Unity (branch `main`)
2. Push para GitHub
3. GitHub Actions gera Xcode project automaticamente
4. Xcode Cloud detecta mudança em `xcode-project` branch
5. Build automático
6. TestFlight recebe nova build
7. Testadores notificados

⏱️ **Tempo**: 40-50 min total (automático!)

---

## Troubleshooting Rápido

| Problema | Solução |
|----------|---------|
| Workflow GitHub falha | Configure Unity License secrets (ver CI-CD-SETUP.md) |
| Xcode Cloud não detecta projeto | Re-execute workflow generate-xcode-project |
| Code signing failed | Use Automatic Signing no Xcode Cloud |
| TestFlight stuck in processing | Aguarde até 1 hora, normal na primeira build |
| Testadores não recebem notificação | Verifique emails e re-envie convites |

---

## Guias Detalhados

- **Setup Completo**: [XCODE-CLOUD-SETUP.md](XCODE-CLOUD-SETUP.md)
- **CI/CD Geral**: [CI-CD-SETUP.md](CI-CD-SETUP.md)
- **Arquitetura**: [CLAUDE.md](CLAUDE.md)

---

**Boa sorte! 🚀**
