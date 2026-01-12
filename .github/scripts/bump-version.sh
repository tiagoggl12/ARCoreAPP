#!/bin/bash
set -e

# Colors for output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

PROJECT_SETTINGS="ProjectSettings/ProjectSettings.asset"

# Check if ProjectSettings file exists
if [ ! -f "$PROJECT_SETTINGS" ]; then
    echo "Error: $PROJECT_SETTINGS not found!"
    exit 1
fi

# Extract current version
CURRENT_VERSION=$(grep "bundleVersion:" "$PROJECT_SETTINGS" | sed 's/.*bundleVersion: //')
echo -e "${BLUE}📦 Current version: ${YELLOW}${CURRENT_VERSION}${NC}"

# Parse version components
IFS='.' read -r MAJOR MINOR PATCH <<< "$CURRENT_VERSION"

# Get the last commit message
COMMIT_MSG=$(git log -1 --pretty=%B)
echo -e "${BLUE}📝 Last commit: ${NC}${COMMIT_MSG}"

# Determine version bump based on commit message (Conventional Commits)
if echo "$COMMIT_MSG" | grep -qiE "^(feat|feature)(\(.*\))?:"; then
    # Feature commit: increment MINOR version, reset PATCH
    MINOR=$((MINOR + 1))
    PATCH=0
    BUMP_TYPE="MINOR (feature)"
    echo -e "${GREEN}✨ Detected feature commit${NC}"
elif echo "$COMMIT_MSG" | grep -qiE "^(break|breaking)(\(.*\))?:"; then
    # Breaking change: increment MAJOR version, reset MINOR and PATCH
    MAJOR=$((MAJOR + 1))
    MINOR=0
    PATCH=0
    BUMP_TYPE="MAJOR (breaking change)"
    echo -e "${YELLOW}⚠️  Detected breaking change${NC}"
else
    # Default: increment PATCH version
    PATCH=$((PATCH + 1))
    BUMP_TYPE="PATCH (fix/chore)"
    echo -e "${GREEN}🔧 Detected fix/chore commit${NC}"
fi

# Create new version
NEW_VERSION="${MAJOR}.${MINOR}.${PATCH}"
echo -e "${GREEN}🚀 New version: ${YELLOW}${NEW_VERSION}${NC} (${BUMP_TYPE})"

# Update the ProjectSettings.asset file
sed -i.bak "s/bundleVersion: ${CURRENT_VERSION}/bundleVersion: ${NEW_VERSION}/" "$PROJECT_SETTINGS"
rm -f "${PROJECT_SETTINGS}.bak"

echo -e "${GREEN}✅ Version bumped successfully!${NC}"

# Output for GitHub Actions
if [ -n "$GITHUB_OUTPUT" ]; then
    echo "old_version=${CURRENT_VERSION}" >> "$GITHUB_OUTPUT"
    echo "new_version=${NEW_VERSION}" >> "$GITHUB_OUTPUT"
    echo "bump_type=${BUMP_TYPE}" >> "$GITHUB_OUTPUT"
fi