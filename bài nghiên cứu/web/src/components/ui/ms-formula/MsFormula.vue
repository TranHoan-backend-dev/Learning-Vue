<script setup lang="ts">
import { ref, computed, watch, onMounted, nextTick } from 'vue';

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

const textareaRef = ref<HTMLTextAreaElement | null>(null);
const showSuggestions = ref(false);
const filterText = ref('');
const selectedSuggestionIndex = ref(0);

const filteredSuggestions = computed(() => {
  if (!filterText.value) return suggestedParams;
  const keyword = filterText.value.toUpperCase();
  return suggestedParams.filter(p => p.includes(keyword));
});

const handleInput = (e: Event) => {
  const target = e.target as HTMLTextAreaElement;
  const value = target.value;
  emit('update:modelValue', value);

  // Phân tích vị trí con trỏ để tìm từ đang gõ
  const cursorPos = target.selectionStart || 0;
  const textBeforeCursor = value.substring(0, cursorPos);

  // Tìm từ cuối cùng đang gõ (sau dấu =, +, -, *, /, (, ,, space)
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
  if (!textareaRef.value) return;

  const textarea = textareaRef.value;
  const cursorPos = textarea.selectionStart || 0;
  const value = props.modelValue || '';
  const textBeforeCursor = value.substring(0, cursorPos);

  // Tìm vị trí bắt đầu từ đang gõ
  const match = textBeforeCursor.match(/[=+\-*/,()\s]?([A-Za-z_][A-Za-z0-9_]*)$/);
  if (match && match[1]) {
    const startPos = cursorPos - match[1].length;
    const newValue = value.substring(0, startPos) + param + value.substring(cursorPos);
    emit('update:modelValue', newValue);

    // Đặt lại con trỏ
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
  // Delay để cho phép click vào suggestion
  setTimeout(() => {
    showSuggestions.value = false;
  }, 200);
};

const focus = () => {
  textareaRef.value?.focus();
};

defineExpose({ focus });
</script>

<template>
  <div class="ms-formula-container">
    <textarea
      ref="textareaRef"
      class="ms-formula-input"
      :class="{ 'has-error': hasError, 'is-disabled': disabled }"
      :value="modelValue"
      :placeholder="placeholder || 'Tự động gợi ý công thức và tham số khi gõ'"
      :disabled="disabled"
      :rows="rows || 3"
      @input="handleInput"
      @keydown="handleKeydown"
      @blur="handleBlur"
    ></textarea>

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

<style scoped>
.ms-formula-container {
  position: relative;
  width: 100%;
}

.ms-formula-input {
  width: 100%;
  padding: 8px 12px;
  border: 1px solid #e0e0e0;
  border-radius: 4px;
  font-family: 'Consolas', 'Monaco', 'Courier New', monospace;
  font-size: 13px;
  color: #111;
  outline: none;
  transition: border-color 0.2s;
  resize: vertical;
  box-sizing: border-box;
}

.ms-formula-input:focus {
  border-color: #2ca01c;
}

.ms-formula-input.has-error {
  border-color: #ff4d4f;
}

.ms-formula-input.is-disabled {
  background-color: #f5f5f5;
  color: #999;
  cursor: not-allowed;
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
