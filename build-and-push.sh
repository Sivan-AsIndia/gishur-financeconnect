#!/bin/bash
# ============================================================
# build-and-push.sh - Build & Push GishurFinanceConnect
# ============================================================
# Prerequisites: Docker Desktop running, with buildx enabled
# Usage:
#   ./build-and-push.sh              # builds the DEFAULT_VERSION below
#   ./build-and-push.sh v10.0.10     # builds an explicit version
#   PUSH_LATEST=0 ./build-and-push.sh v10.0.10   # skip the :latest tag
# ============================================================

set -euo pipefail

# ── Configuration ────────────────────────────────────────────
DOCKER_USERNAME="sivan67906"
API_IMAGE="${DOCKER_USERNAME}/gishurfinance-api"
CLIENT_IMAGE="${DOCKER_USERNAME}/gishurfinance-client"
DEFAULT_VERSION="v10.0.10"
PLATFORMS="linux/amd64,linux/arm64"

VERSION="${1:-$DEFAULT_VERSION}"
PUSH_LATEST="${PUSH_LATEST:-1}"

# ── Validate version format (vMAJOR.MINOR.PATCH) ─────────────
if ! [[ "$VERSION" =~ ^v[0-9]+\.[0-9]+\.[0-9]+$ ]]; then
    echo "❌ Invalid version '$VERSION' — expected format vMAJOR.MINOR.PATCH (e.g. v10.0.10)"
    exit 1
fi

echo "==========================================="
echo " GishurFinanceConnect Docker Build ${VERSION}"
echo "==========================================="
echo ""

# ── Guard: refuse to overwrite an existing published tag ─────
echo "[Step 1/7] Checking that ${VERSION} is not already published..."
if docker buildx imagetools inspect "${API_IMAGE}:${VERSION}" >/dev/null 2>&1; then
    echo "❌ ${API_IMAGE}:${VERSION} already exists on Docker Hub."
    echo "   Bump the version — re-pushing a tag breaks anyone already running it."
    exit 1
fi
echo "  ✅ ${VERSION} is free"
echo ""

# ── Step 2: Login to Docker Hub ──────────────────────────────
echo "[Step 2/7] Logging into Docker Hub..."
docker login
echo ""

# ── Step 3: Create/use buildx builder ────────────────────────
echo "[Step 3/7] Setting up buildx builder..."
docker buildx create --name gishur-builder --use 2>/dev/null || \
docker buildx use gishur-builder
docker buildx inspect --bootstrap
echo ""

# ── Assemble tag arguments ───────────────────────────────────
api_tags=(--tag "${API_IMAGE}:${VERSION}")
client_tags=(--tag "${CLIENT_IMAGE}:${VERSION}")
if [[ "$PUSH_LATEST" == "1" ]]; then
    api_tags+=(--tag "${API_IMAGE}:latest")
    client_tags+=(--tag "${CLIENT_IMAGE}:latest")
fi

# ── Step 4: Build & Push API Image ──────────────────────────
echo "[Step 4/7] Building & pushing API image..."
echo "  Image: ${API_IMAGE}:${VERSION}"
docker buildx build \
    --platform "${PLATFORMS}" \
    --file Dockerfile.api \
    "${api_tags[@]}" \
    --push \
    .
echo "  ✅ API image pushed successfully!"
echo ""

# ── Step 5: Build & Push Client Image ───────────────────────
echo "[Step 5/7] Building & pushing Client image..."
echo "  Image: ${CLIENT_IMAGE}:${VERSION}"
docker buildx build \
    --platform "${PLATFORMS}" \
    --file Dockerfile.client \
    "${client_tags[@]}" \
    --push \
    .
echo "  ✅ Client image pushed successfully!"
echo ""

# ── Step 6: Verify images on Docker Hub ─────────────────────
echo "[Step 6/7] Verifying images..."
docker buildx imagetools inspect "${API_IMAGE}:${VERSION}"
echo ""
docker buildx imagetools inspect "${CLIENT_IMAGE}:${VERSION}"
echo ""

# ── Step 7: Summary ─────────────────────────────────────────
echo "==========================================="
echo " ✅ BUILD & PUSH COMPLETE!"
echo "==========================================="
echo ""
echo " Docker Hub Images:"
echo "   🔹 ${API_IMAGE}:${VERSION}"
echo "   🔹 ${CLIENT_IMAGE}:${VERSION}"
echo ""
echo " Platforms: ${PLATFORMS}"
echo ""
echo " NEXT STEP — pin the new version in docker-compose.yml:"
echo "   sed -i 's/:-v[0-9.]*}/:-${VERSION}}/g' docker-compose.yml"
echo ""
echo " Then hand docker-compose.yml + DEPLOY.md to the person"
echo " running it, and they run: docker compose up -d"
echo "==========================================="
