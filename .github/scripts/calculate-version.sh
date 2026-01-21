#!/bin/bash
set -e

# Ensure we have tags fetched
git fetch --tags --force > /dev/null 2>&1

# 1. Get the latest tag (e.g., v1.0.2), default to v0.0.0 if none
LAST_TAG=$(git describe --tags --abbrev=0 2>/dev/null || echo "v0.0.0")
echo "Previous Version: $LAST_TAG"

# Remove 'v' prefix
VERSION=${LAST_TAG#v}
IFS='.' read -r major minor patch <<< "$VERSION"

# Ensure defaults if parsing failed
major=${major:-0}
minor=${minor:-0}
patch=${patch:-0}

# 2. Analyze commits since last tag
LOGS=$(git log $LAST_TAG..HEAD --pretty=%B)

# Priority: 3=Major, 2=Minor, 1=Patch, 0=None
BUMP_LEVEL=0

if echo "$LOGS" | grep -qEi "(BREAKING CHANGE|!:)"; then
  BUMP_LEVEL=3
elif echo "$LOGS" | grep -qEi "^feat"; then
  BUMP_LEVEL=2
elif echo "$LOGS" | grep -qEi "^(fix|perf|refactor)"; then
  BUMP_LEVEL=1
fi

# 3. Increment logic
should_release="false"

if [ "$BUMP_LEVEL" -eq 3 ]; then
  major=$((major + 1))
  minor=0
  patch=0
  should_release="true"
  change_type="major"
elif [ "$BUMP_LEVEL" -eq 2 ]; then
  minor=$((minor + 1))
  patch=0
  should_release="true"
  change_type="minor"
elif [ "$BUMP_LEVEL" -eq 1 ]; then
  patch=$((patch + 1))
  should_release="true"
  change_type="patch"
else
  echo "No significant changes detected (docs/chore/test/style/ci). Skipping release."
  change_type="none"
fi

NEW_TAG="v$major.$minor.$patch"
echo "Calculated Result: $NEW_TAG ($change_type)"

# Output for next steps
echo "version=$NEW_TAG" >> $GITHUB_OUTPUT
echo "should_release=$should_release" >> $GITHUB_OUTPUT
