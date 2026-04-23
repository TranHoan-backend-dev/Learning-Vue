<script setup lang="ts">

const props = defineProps<{
  label?: string,
  placeholder: string,
  className?: string,
  type?: 'text' | 'tel' | 'checkbox' | 'date' | 'email',
  appendInnerIcon?: string,
  errorMessage?: string,
  hideErrorSpace?: boolean,
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
    <div 
      v-if="!props.hideErrorSpace || props.errorMessage" 
      class="ms_error_message"
      :class="{ 'no-space': props.hideErrorSpace }"
    >
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
  height: 40px; /* Matching CustomSelect size-md */
  padding: 0 16px;
  border: 2px solid transparent;
  border-radius: 12px;
  font-size: 14px;
  outline: none;
  background-color: #f4f4f5;
  color: #11181c;
  font-family: inherit;
  transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1);
}

.ms_input:hover {
  background-color: #e4e4e7;
}

.ms_input:focus {
  background-color: #fff;
  border-color: #0070f3;
}

.ms-input-error {
  border-color: #f31260 !important;
  background-color: #fee7ef;
}

.ms_input_icon {
  position: absolute;
  right: 14px;
  color: #71717a;
  pointer-events: none;
  display: flex;
  align-items: center;
  justify-content: center;
  top: 0;
  height: 40px;
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

/* Specific styling for the date input icon if needed */
.ms_input[type="date"]::-webkit-calendar-picker-indicator {
  cursor: pointer;
  opacity: 0; /* Hide default icon if we want to show custom icon */
}
</style>