<script setup lang="ts">
interface Option {
  value?: string,
  label: string
}

interface SelectProps {
  label?: string,
  modelValue?: string,
  options: Option[],
  required?: boolean
}

defineProps<SelectProps>();

const emit = defineEmits<{
  // phat 1 su kien co ten la 'update:modelValue', gia tri truyen vao la value, va khong nhan ve gia tri nao
  (e: 'update:modelValue', value: string): void
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
    >
      <option
          v-for="(option, key) in options"
          :key="key"
          :value="option.value ?? ''"
      >
        {{ option.label }}
      </option>
    </select>
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
</style>