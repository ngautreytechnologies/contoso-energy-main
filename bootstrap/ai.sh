#!/usr/bin/env bash
set -euo pipefail

echo "🚀 Bootstrapping Contoso Energy AI Module (Flask)..."

AI_DIR="$(cd "$(dirname "$0")/../src/ai" && pwd)"
mkdir -p "$AI_DIR"
cd "$AI_DIR"

# Python venv
[ ! -d "venv" ] && python -m venv venv
source venv/bin/activate || source venv/Scripts/activate

# Requirements
cat > requirements.txt <<'EOF'
Flask==2.3.3
SQLAlchemy==2.0.22
EOF
pip install -r requirements.txt

# Sample app.py
cat > app.py <<'EOF'
from flask import Flask, jsonify

app = Flask(__name__)

@app.route("/ai/job")
def job():
    return jsonify({"jobId": 1, "status": "ready", "result": None})

if __name__ == "__main__":
    app.run(host="0.0.0.0", port=5001)
EOF

# Dockerfile
cat > Dockerfile <<'EOF'
FROM python:3.12-alpine
WORKDIR /app
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt
COPY . .
EXPOSE 5001
CMD ["python", "app.py"]
EOF

echo "✅ AI module bootstrap complete!"
