#!/bin/bash

# Install TMG Hero git hooks
# Run this script to set up pre-commit hooks for the project

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"

echo "Installing TMG Hero git hooks..."

# Create hooks directory if it doesn't exist
mkdir -p "$PROJECT_ROOT/.git/hooks"

# Copy pre-commit hook
cat > "$PROJECT_ROOT/.git/hooks/pre-commit" << 'EOF'
#!/bin/bash

# TMG Hero Pre-commit Hook
# Runs build and tests before allowing commit

echo "Running pre-commit checks for TMG Hero..."

# Change to the solution directory
cd "$(git rev-parse --show-toplevel)/tmg-hero/tmg-hero" || exit 1

# Run dotnet build
echo "Building project..."
if ! dotnet build --configuration Release --verbosity quiet; then
    echo "❌ Build failed! Please fix build errors before committing."
    exit 1
fi

# Run tests
echo "Running tests..."
if ! dotnet test --configuration Release --verbosity quiet --no-build; then
    echo "❌ Tests failed! Please fix failing tests before committing."
    exit 1
fi

echo "✅ Pre-commit checks passed!"
exit 0
EOF

# Make it executable
chmod +x "$PROJECT_ROOT/.git/hooks/pre-commit"

echo "✅ Git hooks installed successfully!"
echo ""
echo "The pre-commit hook will now:"
echo "  - Build the project before each commit"
echo "  - Run all tests before each commit"
echo "  - Prevent commits if build or tests fail"
echo ""
echo "To bypass the hook (not recommended), use: git commit --no-verify"