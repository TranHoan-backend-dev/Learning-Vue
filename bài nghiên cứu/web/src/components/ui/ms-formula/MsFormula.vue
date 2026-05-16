<script setup lang="ts">
import { ref, computed, nextTick } from 'vue';
import { PrismEditor } from 'vue-prism-editor';
import 'vue-prism-editor/dist/prismeditor.min.css';
import Prism from 'prismjs';

const props = defineProps<{
  modelValue: string;
  placeholder?: string;
  disabled?: boolean;
  rows?: number;
  hasError?: boolean;
}>();

const emit = defineEmits(['update:modelValue']);

// Danh sách tham số gợi ý (mock)
const suggestedParams = [
  'LUONG_CO_BAN', 'TONG_CONG', 'NGAY_CONG_THUC_TE', 'HE_SO_LUONG',
  'BHXH', 'BHYT', 'BHTN', 'PC_AN_TRUA', 'PC_DIEN_THOAI',
  'TONG_CONG_LUONG', 'TONG_KHAU_TRU', 'THUC_LINH',
  'ROUND', 'IF', 'SUM', 'MIN', 'MAX'
];

// Định nghĩa ngôn ngữ Excel-like cho Prism
Prism.languages.formula = {
  'function': {
    pattern: /\b(ROUND|IF|SUM|MIN|MAX|AND|OR|NOT|COUNT|AVERAGE)\b/i,
    alias: 'important'
  },
  'parameter': {
    pattern: /\b(LUONG_CO_BAN|TONG_CONG|NGAY_CONG_THUC_TE|HE_SO_LUONG|BHXH|BHYT|BHTN|PC_AN_TRUA|PC_DIEN_THOAI|TONG_CONG_LUONG|TONG_KHAU_TRU|THUC_LINH)\b/,
    alias: 'variable'
  },
  'operator': /[+\-*/=<>&|]/,
  'punctuation': /[(),]/,
  'number': /\b\d+(\.\d+)?\b/
};

const formulaDescriptions: Record<string, string> = {
  SUM: '(X1, X2, ...)',
  IF: '(Logical_test, [value_if_true], [value_if_false])',
  IFS: '(logical_test1, value_if_true1, ...)',
  ROUND: '(number, num_digits)',
  MIN: '(X1, X2, ...)',
  MAX: '(X1, X2, ...)',
};

const getFormulaSignature = (name: string) => {
  return formulaDescriptions[name] || '';
};

const editorRef = ref<any>(null);
const showSuggestions = ref(false);
const filterText = ref('');
const selectedSuggestionIndex = ref(0);

const filteredSuggestions = computed(() => {
  if (!filterText.value) return suggestedParams;
  const keyword = filterText.value.toUpperCase();
  return suggestedParams.filter(p => p.includes(keyword));
});

const highlighter = (code: string) => {
  return Prism.highlight(code, Prism.languages.formula, 'formula');
};

const handleInput = (value: string) => {
  emit('update:modelValue', value);

  // Tìm textarea bên trong PrismEditor
  const textarea = editorRef.value?.$el.querySelector('textarea');
  if (!textarea) return;

  const cursorPos = textarea.selectionStart || 0;
  const textBeforeCursor = value.substring(0, cursorPos);

  const match = textBeforeCursor.match(/[=+\-*/,()\s]?([A-Za-z_][A-Za-z0-9_]*)$/);

  if (match && match[1] && match[1].length >= 1) {
    filterText.value = match[1];
    showSuggestions.value = true;
    selectedSuggestionIndex.value = 0;
  } else {
    showSuggestions.value = false;
    filterText.value = '';
  }
};

const insertSuggestion = (param: string) => {
  const textarea = editorRef.value?.$el.querySelector('textarea');
  if (!textarea) return;

  const cursorPos = textarea.selectionStart || 0;
  const value = props.modelValue || '';
  const textBeforeCursor = value.substring(0, cursorPos);

  const match = textBeforeCursor.match(/[=+\-*/,()\s]?([A-Za-z_][A-Za-z0-9_]*)$/);
  if (match && match[1]) {
    const startPos = cursorPos - match[1].length;
    const newValue = value.substring(0, startPos) + param + value.substring(cursorPos);
    emit('update:modelValue', newValue);

    nextTick(() => {
      const newCursorPos = startPos + param.length;
      textarea.setSelectionRange(newCursorPos, newCursorPos);
      textarea.focus();
    });
  }

  showSuggestions.value = false;
  filterText.value = '';
};

const handleKeydown = (e: KeyboardEvent) => {
  if (!showSuggestions.value || filteredSuggestions.value.length === 0) return;

  if (e.key === 'ArrowDown') {
    e.preventDefault();
    selectedSuggestionIndex.value = Math.min(
      selectedSuggestionIndex.value + 1,
      filteredSuggestions.value.length - 1
    );
  } else if (e.key === 'ArrowUp') {
    e.preventDefault();
    selectedSuggestionIndex.value = Math.max(selectedSuggestionIndex.value - 1, 0);
  } else if (e.key === 'Enter' && showSuggestions.value) {
    e.preventDefault();
    insertSuggestion(filteredSuggestions.value[selectedSuggestionIndex.value]);
  } else if (e.key === 'Escape') {
    showSuggestions.value = false;
  }
};

const handleBlur = () => {
  setTimeout(() => {
    showSuggestions.value = false;
  }, 200);
};

const focus = () => {
  const textarea = editorRef.value?.$el.querySelector('textarea');
  textarea?.focus();
};

defineExpose({ focus });
</script>

<template>
  <div class="formula-wrapper">
    <!-- Editor -->
    <div
        class="formula-editor-shell"
        :class="{ focused: showSuggestions }"
    >
      <prism-editor
          ref="editorRef"
          class="ms-formula-editor"
          :class="{ 'has-error': hasError, 'is-disabled': disabled }"
          :model-value="modelValue"
          :highlight="highlighter"
          :line-numbers="false"
          :readonly="disabled"
          @update:model-value="handleInput"
          @keydown="handleKeydown"
          @blur="handleBlur"
          :placeholder="placeholder || 'Nhập công thức...'"
      />
    </div>

    <!-- Suggestion Panel -->
    <div
        v-if="showSuggestions && !disabled"
        class="formula-suggestion-panel"
    >
      <!-- Tabs -->
      <div class="formula-tabs">
        <div class="formula-tab active">Công thức</div>
        <div class="formula-tab">Tham số</div>
      </div>

      <!-- List -->
      <div class="formula-list">
        <div
            v-for="(param, index) in filteredSuggestions"
            :key="param"
            class="formula-item"
            :class="{ active: index === selectedSuggestionIndex }"
            @mousedown.prevent="insertSuggestion(param)"
        >
          <div class="formula-icon">ƒx</div>

          <div class="formula-content">
            <div class="formula-name">
              {{ param }}
              <span class="formula-signature">
                {{ getFormulaSignature(param) }}
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.formula-wrapper {
  width: 100%;
  position: relative;
}

/* =========================
   EDITOR
========================= */

.formula-editor-shell {
  display: flex;
  align-items: flex-start;
  border: 1px solid var(--misa-border-color);
  border-radius: 4px;
  background: var(--misa-white);
  min-height: 110px;
  overflow: hidden;
  transition: all 0.2s ease;
}

.formula-editor-shell.focused {
  border-color: var(--primary-green);
  box-shadow: 0 0 0 1px rgba(44, 160, 28, 0.13);
}

.ms-formula-editor {
  flex: 1;
  background: transparent;
}

/* textarea thật */
:deep(.prism-editor__textarea) {
  position: absolute !important;
  top: 0 !important;
  left: 0 !important;          /* ← đổi lại 0 */
  right: 0 !important;
  bottom: 0 !important;

  padding-left: 10px !important;  /* ← thêm dòng này */
  padding-top: 8px !important;    /* ← khớp với container */

  color: transparent !important;
  caret-color: var(--misa-text-primary) !important;
  background: transparent !important;
  resize: none !important;
  outline: none !important;
  border: none !important;
  overflow: hidden !important;
  z-index: 2;
}

/* layer highlight */
:deep(.prism-editor__code) {
  position: relative;
  z-index: 1;

  color: #222 !important;

  pointer-events: none;
}

.ms-formula-editor {
  position: relative;
}

:deep(pre),
:deep(code) {
  margin: 0 !important;
  background: transparent !important;
  text-shadow: none !important;
  font-family: Consolas, Monaco, monospace !important;
}

/* fix prism default opacity */
:deep(.token) {
  background: none !important;
  text-shadow: none !important;
}
:deep(.prism-editor__textarea::placeholder) {
  font-size: 13px;
  color: #aaa;
}

/* Thêm vào sau block :deep(.token) */
:deep(.prism-editor__container) {
  padding-left: 10px;
  padding-top: 8px;
}

/* =========================
   SUGGESTION PANEL
========================= */

.formula-suggestion-panel {
  margin-top: 12px;
  background: #f7f7f7;
  border-radius: 8px;
  border: 1px solid #e4e4e4;
  overflow: hidden;
}

/* Tabs */

.formula-tabs {
  display: flex;
  gap: 28px;
  padding: 12px 24px 0;
  background: #f7f7f7;
}

.formula-tab {
  position: relative;
  padding-bottom: 12px;
  font-size: 13px;
  color: var(--misa-text-secondary);
  cursor: pointer;
}

.formula-tab.active {
  color: var(--primary-green);
  font-weight: 600;
}

.formula-tab.active::after {
  content: '';
  position: absolute;
  left: 0;
  bottom: 0;

  width: 100%;
  height: 3px;
  border-radius: 999px;

  background: var(--primary-green);
}

/* List */

.formula-list {
  max-height: 260px;
  overflow-y: auto;
  padding: 10px 0;
}

.formula-item {
  display: flex;
  align-items: flex-start;
  padding: 10px 24px;  /* từ 16px 24px → 10px 24px */
  gap: 10px;
  cursor: pointer;
  transition: background 0.15s ease;
}

.formula-item:hover,
.formula-item.active {
  background: #ececec;
}

.formula-icon {
  font-size: 20px;
  color: var(--misa-text-secondary);
  font-family: serif;
  line-height: 1;
  margin-top: 2px;
}

.formula-content {
  flex: 1;
}

.formula-name {
  font-size: 13px;
  color: var(--misa-text-primary);
  font-weight: 700;
}

.formula-signature {
  font-weight: 400;
  color: var(--misa-text-secondary);
  margin-left: 6px;
  font-size: 13px;
}

/* =========================
   TOKENS
========================= */

:deep(.token.function) {
  color: #1565c0;
  font-weight: 700;
}

:deep(.token.parameter) {
  color: #2e7d32;
}

:deep(.token.operator) {
  color: #c62828;
}

:deep(.token.number) {
  color: #ef6c00;
}

:deep(.token.punctuation) {
  color: var(--misa-text-secondary);
}
</style>
