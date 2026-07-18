#!/usr/bin/env bash

set -euo pipefail

if [[ -z "${UNITY_EDITOR_PATH:-}" ]]; then
  echo "UNITY_EDITOR_PATH must point to the Unity executable." >&2
  exit 2
fi

if [[ ! -x "$UNITY_EDITOR_PATH" ]]; then
  echo "UNITY_EDITOR_PATH is not executable: $UNITY_EDITOR_PATH" >&2
  exit 2
fi

project_path="${UNITY_PROJECT_PATH:-$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")/.." && pwd)}"
log_dir="${UNITY_COMPILE_LOG_DIR:-$(mktemp -d "${TMPDIR:-/tmp}/holokit-unity-compile.XXXXXX")}"
mkdir -p "$log_dir"

run_compile_check() {
  local target="$1"
  local log_file="$log_dir/${target}.log"

  echo "Compiling HoloKit boilerplate for ${target}..."
  if ! "$UNITY_EDITOR_PATH" \
    -batchmode \
    -nographics \
    -quit \
    -buildTarget "$target" \
    -projectPath "$project_path" \
    -logFile "$log_file"; then
    echo "Unity compilation failed for ${target}. Log: ${log_file}" >&2
    tail -n 80 "$log_file" >&2
    return 1
  fi

  echo "Unity compilation passed for ${target}. Log: ${log_file}"
}

run_compile_check StandaloneOSX
run_compile_check iOS

echo "HoloKit boilerplate compile checks passed. Logs: ${log_dir}"
