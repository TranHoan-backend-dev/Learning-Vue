<script setup lang="ts">

const props = defineProps<{
  label?: string,
  placeholder: string,
  className?: string,
  type?: 'text' | 'tel' | 'checkbox' | 'date' | 'email',
  appendInnerIcon?: string,
  errorMessage?: string,
}>()

const value = defineModel<string>({
  default: ''
})
</script>

<template>
  <div class="ms-input-container" :class="props.className">
    <div class="ms_input_wrapper">
      <input
          :type="props.type ?? 'text'"
          :placeholder="props.placeholder"
          v-model="value"
          class="ms_input"
          :class="{'ms-input-error': props.errorMessage}"
          :aria-label="props.label"
      />
      <div v-if="props.appendInnerIcon" class="ms_input_icon">
        <i :class="props.appendInnerIcon.startsWith('mdi-') ? 'mdi ' + props.appendInnerIcon : props.appendInnerIcon"></i>
      </div>
    </div>
    <div v-if="props.errorMessage" class="ms_error_message">
      {{ props.errorMessage }}
    </div>
  </div>
</template>

<style scoped>
.ms-input-container {
  display: flex;
  flex-direction: column;
  width: 100%;
}

.ms_input_wrapper {
  position: relative;
  display: flex;
  align-items: center;
}

.ms_input {
  width: 100%;
  height: 36px;
  padding: 8px 12px;
  border: 1px solid #dddde4;
  border-radius: 4px; /* Default for consistency, can be overridden */
  font-size: 14px;
  outline: none;
  background-color: #fff;
  font-family: inherit;
  transition: border-color 0.2s;
}

.ms_input:focus {
  border-color: #2970f6;
}

.ms-input-error {
  border-color: #ff4d4f !important;
}

.ms_input_icon {
  position: absolute;
  right: 10px;
  color: #65696e;
  pointer-events: none;
  display: flex;
  align-items: center;
  justify-content: center;
}

.ms_error_message {
  font-size: 12px;
  color: #ff4d4f;
  margin-top: 4px;
  min-height: 16px;
}

/* Specific styling for the date input icon if needed */
.ms_input[type="date"]::-webkit-calendar-picker-indicator {
  cursor: pointer;
  opacity: 0; /* Hide default icon if we want to show custom icon */
}
</style>