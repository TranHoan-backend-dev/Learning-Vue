<script setup lang="ts">
interface Option {
  value?: string,
  label: string
}

interface SelectProps {
  label?: string,
  modelValue?: any,
  options: Option[],
  required?: boolean,
  errorMessage?: string
}

const props = defineProps<SelectProps>();

const emit = defineEmits<{
  (e: 'update:modelValue', value: any): void
}>()
</script>

<template>
  <div class="ms-select-container">
    <label v-if="label" class="ms-select-label">
      {{ label }}
      <span v-if="required" class="required">*</span>
    </label>
    <select
        :value="modelValue"
        @change="emit('update:modelValue', ($event.target as HTMLSelectElement).value)"
        class="ms-select"
        :class="{'ms-select-error': errorMessage}"
    >
      <option
          v-for="(option, key) in options"
          :key="key"
          :value="option.value ?? ''"
      >
        {{ option.label }}
      </option>
    </select>
    <div v-if="errorMessage" class="ms_error_message">
      {{ errorMessage }}
    </div>
  </div>
</template>

<style scoped>
.ms-select-container {
    display: flex;
    flex-direction: column;
    width: 100%;
}

.ms-select-label {
    display: block;
    font-size: 14px;
    font-weight: 600;
    margin-bottom: 8px;
    color: #212121;
}

.required {
    color: #ff4d4f;
    margin-left: 2px;
}

.ms-select {
    width: 100%;
    height: 36px;
    padding: 8px 12px;
    border: 1px solid #dddde4;
    border-radius: 4px;
    font-size: 14px;
    outline: none;
    background-color: #fff;
    font-family: inherit;
    cursor: pointer;
}

.ms-select:focus {
    border-color: #2970f6;
}

.ms-select-error {
    border-color: #ff4d4f !important;
}

.ms_error_message {
    font-size: 12px;
    color: #ff4d4f;
    margin-top: 4px;
    min-height: 16px;
}
</style>