#!/bin/bash
# ============================================================
# build-and-push.sh - Build & Push GishurFinanceConnect v4.0.0
# ============================================================
# Prerequisites: Docker Desktop with buildx enabled
# Usage: chmod +x build-and-push.sh && ./build-and-push.sh
# ============================================================

set -e

# ── Configuration ────────────────────────────────────────────
DOCKER_USERNAME="sivan67906"
API_IMAGE="${DOCKER_USERNAME}/gishurfinance-api"
CLIENT_IMAGE="${DOCKER_USERNAME}/gishurfinance-client"
VERSION="v4.0.0"

echo "==========================================="
echo " GishurFinanceConnect Docker Build v4.0.0"
echo "==========================================="
echo ""

# ── Step 1: Login to Docker Hub ──────────────────────────────
echo "[Step 1/6] Logging into Docker Hub..."
docker login
echo ""

# ── Step 2: Create/use buildx builder ────────────────────────
echo "[Step 2/6] Setting up buildx builder..."
docker buildx create --name gishur-builder --use 2>/dev/null || \
docker buildx use gishur-builder
docker buildx inspect --bootstrap
echo ""

# ── Step 3: Build & Push API Image ──────────────────────────
echo "[Step 3/6] Building & pushing API image..."
echo "  Image: ${API_IMAGE}:${VERSION}"
docker buildx build \
    --platform linux/amd64,linux/arm64 \
    --file Dockerfile.api \
    --tag "${API_IMAGE}:${VERSION}" \
    --tag "${API_IMAGE}:latest" \
    --push \
    .
echo "  ✅ API image pushed successfully!"
echo ""

# ── Step 4: Build & Push Client Image ───────────────────────
echo "[Step 4/6] Building & pushing Client image..."
echo "  Image: ${CLIENT_IMAGE}:${VERSION}"
docker buildx build \
    --platform linux/amd64,linux/arm64 \
    --file Dockerfile.client \
    --tag "${CLIENT_IMAGE}:${VERSION}" \
    --tag "${CLIENT_IMAGE}:latest" \
    --push \
    .
echo "  ✅ Client image pushed successfully!"
echo ""

# ── Step 5: Verify images on Docker Hub ─────────────────────
echo "[Step 5/6] Verifying images..."
docker buildx imagetools inspect "${API_IMAGE}:${VERSION}"
echo ""
docker buildx imagetools inspect "${CLIENT_IMAGE}:${VERSION}"
echo ""

# ── Step 6: Summary ─────────────────────────────────────────
echo "==========================================="
echo " ✅ BUILD & PUSH COMPLETE!"
echo "==========================================="
echo ""
echo " Docker Hub Images:"
echo "   🔹 ${API_IMAGE}:${VERSION}"
echo "   🔹 ${CLIENT_IMAGE}:${VERSION}"
echo ""
echo " Platforms: linux/amd64, linux/arm64"
echo ""
echo " Next: Share docker-compose.yml with your"
echo "       team and run: docker compose up -d"
echo "==========================================="
