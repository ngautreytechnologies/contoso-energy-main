#!/usr/bin/env bash
set -euo pipefail

echo "🚀 Bootstrapping Contoso Energy Frontend..."

FRONTEND_DIR="$(cd "$(dirname "$0")/../src/frontend" && pwd)"
mkdir -p "$FRONTEND_DIR"
cd "$FRONTEND_DIR"

# Init Vite + React + TS
[ ! -d "$FRONTEND_DIR/node_modules" ] && npm create vite@latest . -- --template react-ts

# TailwindCSS
npm install -D tailwindcss postcss autoprefixer
npx tailwindcss init -p

# shadcn/ui (example)
npm install @shadcn/ui

# Dockerfile
cat > Dockerfile <<'EOF'
FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
EOF

echo "✅ Frontend bootstrap complete!"
