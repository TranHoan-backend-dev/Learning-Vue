<script setup lang="ts">
interface TextareaProps {
  modelValue?: string,
  placeholder?: string,
  errorMessage?: string,
  hideErrorSpace?: boolean,
}

const props = withDefaults(defineProps<TextareaProps>(), {
  hideErrorSpace: false
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: string): void
}>()
</script>

<template>
  <div class="ms-textarea-container">
    <textarea
        class="ms-textarea"
        :class="{ 'error': errorMessage }"
        @input="emit('update:modelValue', ($event.target as HTMLInputElement).value)"
        :value="modelValue"
        :placeholder="placeholder"
    ></textarea>
    <div 
      v-if="!hideErrorSpace || errorMessage" 
      class="ms_error_message"
      :class="{ 'no-space': hideErrorSpace }"
    >
      {{ errorMessage }}
    </div>
  </div>
</template>

<style scoped>
.ms-textarea-container {
  display: flex;
  flex-direction: column;
  width: 100%;
}

.ms-textarea {
  width: 100%;
  min-height: 100px;
  padding: 12px 16px;
  border: 2px solid transparent;
  border-radius: 12px;
  font-size: 14px;
  outline: none;
  background-color: #f4f4f5;
  color: #11181c;
  font-family: inherit;
  transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1);
  resize: vertical;
}

.ms-textarea:hover {
  background-color: #e4e4e7;
}

.ms-textarea:focus {
  background-color: #fff;
  border-color: #0070f3;
}

.ms-textarea.error {
  border-color: #f31260;
  background-color: #fee7ef;
}

.ms-textarea::placeholder {
  color: #71717a;
}

.ms_error_message {
  font-size: 12px;
  color: #f31260;
  margin-top: 4px;
  min-height: 18px;
}

.ms_error_message.no-space {
  min-height: 0;
}
</style>