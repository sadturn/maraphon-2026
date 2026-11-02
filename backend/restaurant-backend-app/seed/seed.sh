#!/usr/bin/env sh
# Seeds demo data into the deployed DynamoDB tables:
#   locations.json     — 3 locations
#   tables-dishes.json — 3 tables (2/4/6 seats) and 4 dishes per location
#   waiters.json       — 1 placeholder waiter per location (DynamoDB row only, no Cognito user)
# Idempotent: fixed ids, so a re-run overwrites the same items.
# Usage (from backend/restaurant-backend-app): sh seed/seed.sh [region]
set -e

REGION="${1:-eu-north-1}"
# relative file:// path: works the same for Linux/macOS aws and Windows aws.exe under Git Bash
cd "$(dirname "$0")"

for FILE in locations.json tables-dishes.json waiters.json; do
  echo "Seeding $FILE"
  aws dynamodb batch-write-item \
    --region "$REGION" \
    --request-items "file://$FILE" \
    --query UnprocessedItems
done
