#!/bin/bash

# 🚀 Script de Deploy iOS - BugabooXR
# Automatiza build Unity → Archive → Export → Upload TestFlight

set -e  # Exit on error

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Função para print colorido
print_step() {
    echo -e "${BLUE}▶ $1${NC}"
}

print_success() {
    echo -e "${GREEN}✓ $1${NC}"
}

print_error() {
    echo -e "${RED}✗ $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠ $1${NC}"
}

# Banner
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "🚀  BugabooXR iOS Deploy Script"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""

# Verificar se estamos no diretório correto
if [ ! -f "Assets" ] && [ ! -d "Assets" ]; then
    print_error "Este script deve ser executado na raiz do projeto Unity!"
    exit 1
fi

# Configurações
PROJECT_PATH=$(pwd)
BUILD_PATH="$PROJECT_PATH/build/iOS"
UNITY_PATH="/Applications/Unity/Hub/Editor/*/Unity.app/Contents/MacOS/Unity"
UNITY_PATH=$(echo $UNITY_PATH | head -n1)

# Menu de opções
echo "Selecione o que deseja fazer:"
echo ""
echo "1) 🔨 Build Unity (apenas)"
echo "2) 📦 Archive + Export IPA (requer build Unity pronto)"
echo "3) 🚀 Upload para TestFlight (requer IPA pronto)"
echo "4) 🎯 COMPLETO: Build + Archive + Upload"
echo "5) 🧹 Limpar builds antigos"
echo ""
read -p "Escolha uma opção (1-5): " option

case $option in
    1|4)
        print_step "Iniciando build Unity..."

        if [ ! -f "$UNITY_PATH" ]; then
            print_error "Unity não encontrado em: $UNITY_PATH"
            print_warning "Por favor, ajuste o caminho do Unity no script"
            exit 1
        fi

        # Limpar build anterior
        rm -rf "$BUILD_PATH"

        # Build com Unity
        "$UNITY_PATH" \
            -quit \
            -batchmode \
            -nographics \
            -silent-crashes \
            -logFile "$PROJECT_PATH/build-ios.log" \
            -projectPath "$PROJECT_PATH" \
            -buildTarget iOS \
            -executeMethod BuildCommand.PerformBuild

        if [ $? -eq 0 ]; then
            print_success "Build Unity concluído!"
        else
            print_error "Build Unity falhou. Verifique o log:"
            tail -n 50 "$PROJECT_PATH/build-ios.log"
            exit 1
        fi

        if [ "$option" != "4" ]; then
            exit 0
        fi
        ;;
esac

case $option in
    2|4)
        print_step "Verificando projeto Xcode..."

        if [ ! -d "$BUILD_PATH" ] || [ ! -f "$BUILD_PATH/Unity-iPhone.xcodeproj/project.pbxproj" ]; then
            print_error "Projeto Xcode não encontrado em: $BUILD_PATH"
            print_warning "Execute primeiro o build do Unity (opção 1)"
            exit 1
        fi

        cd "$BUILD_PATH"

        print_step "Criando arquivo Archive..."

        # Clean build folder
        rm -rf ./DerivedData
        rm -rf ./*.xcarchive

        # Archive
        xcodebuild archive \
            -project Unity-iPhone.xcodeproj \
            -scheme Unity-iPhone \
            -configuration Release \
            -archivePath ./Unity-iPhone.xcarchive \
            -derivedDataPath ./DerivedData \
            CODE_SIGN_STYLE=Automatic \
            DEVELOPMENT_TEAM="" \
            -allowProvisioningUpdates

        if [ $? -eq 0 ]; then
            print_success "Archive criado com sucesso!"
        else
            print_error "Falha ao criar archive"
            exit 1
        fi

        print_step "Exportando IPA..."

        # Export IPA
        mkdir -p ./output
        rm -rf ./output/*

        xcodebuild -exportArchive \
            -archivePath ./Unity-iPhone.xcarchive \
            -exportPath ./output \
            -exportOptionsPlist "$PROJECT_PATH/ExportOptions.plist" \
            -allowProvisioningUpdates

        if [ $? -eq 0 ] && [ -f ./output/*.ipa ]; then
            print_success "IPA exportado com sucesso!"
            IPA_PATH=$(ls ./output/*.ipa | head -n1)
            print_success "IPA: $IPA_PATH"

            # Mostrar tamanho
            IPA_SIZE=$(du -h "$IPA_PATH" | cut -f1)
            echo "   Tamanho: $IPA_SIZE"
        else
            print_error "Falha ao exportar IPA"
            exit 1
        fi

        cd "$PROJECT_PATH"

        if [ "$option" != "4" ]; then
            exit 0
        fi
        ;;
esac

case $option in
    3|4)
        print_step "Preparando upload para TestFlight..."

        # Encontrar IPA
        IPA_PATH=$(find "$BUILD_PATH/output" -name "*.ipa" -type f | head -n1)

        if [ ! -f "$IPA_PATH" ]; then
            print_error "IPA não encontrado!"
            print_warning "Execute primeiro o build e export (opção 2)"
            exit 1
        fi

        print_success "IPA encontrado: $IPA_PATH"

        # Verificar se tem as credenciais
        if [ -z "$APP_STORE_CONNECT_API_KEY_ID" ] || [ -z "$APP_STORE_CONNECT_API_ISSUER_ID" ]; then
            print_warning "Credenciais do App Store Connect não configuradas!"
            echo ""
            echo "Opções de upload:"
            echo ""
            echo "A) 🖥️  Abrir Transporter (manual)"
            echo "B) 📋 Usar xcrun altool (linha de comando)"
            echo "C) 🚀 Usar Fastlane"
            echo ""
            read -p "Escolha (A/B/C): " upload_option

            case $upload_option in
                [Aa])
                    print_step "Abrindo Transporter..."
                    open -a Transporter "$IPA_PATH"
                    print_success "Arraste o IPA para o Transporter e clique em 'Deliver'"
                    ;;
                [Bb])
                    print_warning "Para usar altool, configure as variáveis:"
                    echo "export APP_STORE_CONNECT_API_KEY_ID='seu-key-id'"
                    echo "export APP_STORE_CONNECT_API_ISSUER_ID='seu-issuer-id'"
                    echo ""
                    echo "Depois execute:"
                    echo "xcrun altool --upload-app --type ios --file '$IPA_PATH' \\"
                    echo "  --apiKey \$APP_STORE_CONNECT_API_KEY_ID \\"
                    echo "  --apiIssuer \$APP_STORE_CONNECT_API_ISSUER_ID"
                    ;;
                [Cc])
                    if command -v fastlane &> /dev/null; then
                        print_step "Fazendo upload com Fastlane..."
                        fastlane pilot upload --ipa "$IPA_PATH" --skip_waiting_for_build_processing
                    else
                        print_error "Fastlane não instalado!"
                        echo "Instale com: sudo gem install fastlane -NV"
                    fi
                    ;;
            esac
        else
            print_step "Fazendo upload com xcrun altool..."
            xcrun altool --upload-app \
                --type ios \
                --file "$IPA_PATH" \
                --apiKey "$APP_STORE_CONNECT_API_KEY_ID" \
                --apiIssuer "$APP_STORE_CONNECT_API_ISSUER_ID"

            if [ $? -eq 0 ]; then
                print_success "Upload para TestFlight concluído!"
                print_warning "O processamento pode levar 5-15 minutos"
                echo ""
                echo "🔗 Verifique em: https://appstoreconnect.apple.com"
            else
                print_error "Falha no upload"
                exit 1
            fi
        fi
        ;;
    5)
        print_step "Limpando builds antigos..."
        rm -rf "$BUILD_PATH"
        rm -rf "$PROJECT_PATH/build-ios.log"
        rm -rf "$PROJECT_PATH/build-android.log"
        print_success "Builds removidos!"
        ;;
    *)
        print_error "Opção inválida!"
        exit 1
        ;;
esac

echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
print_success "Processo concluído!"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
