<script setup lang="ts">
import { ref, computed, watch, onMounted, nextTick } from 'vue';
import { PrismEditor } from 'vue-prism-editor';
import 'vue-prism-editor/dist/prismeditor.min.css';
import Prism from 'prismjs';
import 'prismjs/themes/prism.css';

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
  <div class="ms-formula-container">
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
      :placeholder="placeholder || 'Tự động gợi ý công thức và tham số khi gõ'"
    ></prism-editor>

    <!-- Dropdown gợi ý -->
    <div v-if="showSuggestions && filteredSuggestions.length > 0 && !disabled" class="ms-formula-suggestions">
      <div
        v-for="(param, index) in filteredSuggestions"
        :key="param"
        class="ms-formula-suggestion-item"
        :class="{ 'is-selected': index === selectedSuggestionIndex }"
        @mousedown.prevent="insertSuggestion(param)"
      >
        <span class="suggestion-icon">ƒ</span>
        <span class="suggestion-text">{{ param }}</span>
      </div>
    </div>
  </div>
</template>

<style>
/* CSS toàn cục cho Prism Editor vì component scoped có thể không tác động sâu được */
.ms-formula-editor .prism-editor__textarea {
  outline: none !important;
  padding: 8px 12px !important;
  font-family: 'Consolas', 'Monaco', 'Courier New', monospace !important;
  font-size: 13px !important;
  color: #111 !important;
  min-height: 80px;
}

.ms-formula-editor .prism-editor__code {
  padding: 8px 12px !important;
  font-family: 'Consolas', 'Monaco', 'Courier New', monospace !important;
  font-size: 13px !important;
}

/* Các token màu sắc cho công thức */
.token.function { color: #1565c0; font-weight: bold; }
.token.parameter { color: #2e7d32; }
.token.operator { color: #c62828; }
.token.punctuation { color: #666; }
.token.number { color: #ef6c00; }
</style>

<style scoped>
.ms-formula-container {
  position: relative;
  width: 100%;
}

.ms-formula-editor {
  width: 100%;
  border: 1px solid #e0e0e0;
  border-radius: 4px;
  background-color: #fff;
  transition: border-color 0.2s;
  box-sizing: border-box;
}

.ms-formula-editor:focus-within {
  border-color: #2ca01c;
}

.ms-formula-editor.has-error {
  border-color: #ff4d4f;
}

.ms-formula-editor.is-disabled {
  background-color: #f5f5f5;
  cursor: not-allowed;
}

.ms-formula-editor.is-disabled :deep(textarea) {
    color: #999 !important;
}

/* Suggestion dropdown */
.ms-formula-suggestions {
  position: absolute;
  top: 100%;
  left: 0;
  width: 300px;
  max-height: 200px;
  overflow-y: auto;
  background: #fff;
  border: 1px solid #e0e0e0;
  border-radius: 4px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.12);
  z-index: 100;
  margin-top: 2px;
}

.ms-formula-suggestion-item {
  display: flex;
  align-items: center;
  padding: 8px 12px;
  cursor: pointer;
  font-size: 13px;
  color: #333;
  transition: background 0.15s;
}

.ms-formula-suggestion-item:hover,
.ms-formula-suggestion-item.is-selected {
  background: #e8f5e9;
}

.suggestion-icon {
  width: 22px;
  height: 22px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #e3f2fd;
  border-radius: 4px;
  color: #1565c0;
  font-weight: 700;
  font-size: 12px;
  margin-right: 10px;
  flex-shrink: 0;
}

.suggestion-text {
  font-family: 'Consolas', 'Monaco', 'Courier New', monospace;
  font-size: 13px;
}
</style>
