<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted, nextTick } from 'vue';

interface Option {
  value?: string | number,
  label: string
}

type SelectSize = 'sm' | 'md' | 'lg';

interface SelectProps {
  label?: string,
  modelValue?: any,
  options: Option[],
  required?: boolean,
  errorMessage?: string,
  placeholder?: string,
  size?: SelectSize,
  hideErrorSpace?: boolean
}

const props = withDefaults(defineProps<SelectProps>(), {
  placeholder: 'Chọn giá trị...',
  size: 'md',
  hideErrorSpace: false
});

const emit = defineEmits<{
  (e: 'update:modelValue', value: any): void
}>()

const isOpen = ref(false);
const placement = ref<'top' | 'bottom'>('bottom');
const selectRef = ref<HTMLElement | null>(null);

// Lấy label của option đang được chọn
const selectedLabel = computed(() => {
  const selected = props.options.find(opt => opt.value === props.modelValue);
  return selected ? selected.label : props.placeholder;
});

const toggleDropdown = async () => {
  if (!isOpen.value) {
    calculatePlacement();
  }
  isOpen.value = !isOpen.value;
};

// Tính toán hướng mở của menu
const calculatePlacement = () => {
  if (selectRef.value) {
    const rect = selectRef.value.getBoundingClientRect();
    const spaceBelow = window.innerHeight - rect.bottom;
    const menuHeight = 250; // Chiều cao tối đa của menu

    if (spaceBelow < menuHeight) {
      placement.value = 'top';
    } else {
      placement.value = 'bottom';
    }
  }
};

const selectOption = (option: Option) => {
  emit('update:modelValue', option.value);
  isOpen.value = false;
};

const handleClickOutside = (event: MouseEvent) => {
  if (selectRef.value && !selectRef.value.contains(event.target as Node)) {
    isOpen.value = false;
  }
};

onMounted(() => {
  window.addEventListener('click', handleClickOutside);
  window.addEventListener('scroll', calculatePlacement, true);
});

onUnmounted(() => {
  window.removeEventListener('click', handleClickOutside);
  window.removeEventListener('scroll', calculatePlacement, true);
});
</script>

<template>
  <div class="ms-select-container" ref="selectRef" :class="[`size-${size}`]">
    <label v-if="label" class="ms-select-label">
      {{ label }}
      <span v-if="required" class="required">*</span>
    </label>
    
    <div class="ms-select-wrapper">
      <div 
        class="ms-select-trigger" 
        :class="{ 'opened': isOpen, 'error': errorMessage }"
        @click="toggleDropdown"
      >
        <span class="selected-value" :class="{ 'is-placeholder': !modelValue }">
          {{ selectedLabel }}
        </span>
        <div class="chevron-icon" :class="{ 'rotated': isOpen }">
          <i class="bi bi-chevron-down"></i>
        </div>
      </div>

      <Transition :name="placement === 'top' ? 'fade-slide-up' : 'fade-slide'">
        <div 
          v-if="isOpen" 
          class="ms-select-menu"
          :class="[`placement-${placement}`]"
        >
          <div 
            v-for="(option, key) in options" 
            :key="key"
            class="ms-select-item"
            :class="{ 'active': option.value === modelValue }"
            @click="selectOption(option)"
          >
            {{ option.label }}
          </div>
        </div>
      </Transition>
    </div>

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
.ms-select-container {
  display: flex;
  flex-direction: column;
  width: 100%;
  position: relative;
}

/* Biến kích thước */
.size-sm .ms-select-trigger { min-height: 32px; padding: 0 12px; border-radius: 8px; font-size: 13px; }
.size-md .ms-select-trigger { min-height: 40px; padding: 0 16px; border-radius: 12px; font-size: 14px; }
.size-lg .ms-select-trigger { min-height: 48px; padding: 0 20px; border-radius: 14px; font-size: 16px; }

.ms-select-label {
  display: block;
  font-size: 14px;
  font-weight: 600;
  margin-bottom: 8px;
  color: #11181c;
}

.required {
  color: #f31260;
  margin-left: 2px;
}

.ms-select-wrapper {
  position: relative;
  width: 100%;
}

.ms-select-trigger {
  width: 100%;
  background-color: #f4f4f5;
  display: flex;
  align-items: center;
  justify-content: space-between;
  cursor: pointer;
  transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1);
  border: 2px solid transparent;
}

.ms-select-trigger:hover {
  background-color: #e4e4e7;
}

.ms-select-trigger.opened {
  background-color: #e4e4e7;
}

.ms-select-trigger.error {
  border-color: #f31260;
  background-color: #fee7ef;
}

.selected-value {
  color: #11181c;
  font-weight: 400;
}

.selected-value.is-placeholder {
  color: #71717a;
}

.chevron-icon {
  font-size: 12px;
  color: #71717a;
  transition: transform 0.3s ease;
  display: flex;
  align-items: center;
}

.chevron-icon.rotated {
  transform: rotate(180deg);
}

/* Dropdown Menu Style */
.ms-select-menu {
  position: absolute;
  left: 0;
  right: 0;
  background: #ffffff;
  border-radius: 14px;
  padding: 6px;
  box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05);
  border: 1px solid #f4f4f5;
  z-index: 1000;
  max-height: 250px;
  overflow-y: auto;
}

.placement-bottom {
  top: calc(100% + 8px);
}

.placement-top {
  bottom: calc(100% + 8px);
  box-shadow: 0 -10px 15px -3px rgba(0, 0, 0, 0.1), 0 -4px 6px -2px rgba(0, 0, 0, 0.05);
}

.ms-select-item {
  padding: 10px 12px;
  border-radius: 10px;
  font-size: 14px;
  color: #11181c;
  cursor: pointer;
  transition: background 0.2s ease;
}

.ms-select-item:hover {
  background-color: #f4f4f5;
}

.ms-select-item.active {
  background-color: #0070f3;
  color: #ffffff;
}

/* Animations */
.fade-slide-enter-active, .fade-slide-leave-active,
.fade-slide-up-enter-active, .fade-slide-up-leave-active {
  transition: all 0.2s ease;
}

.fade-slide-enter-from, .fade-slide-leave-to {
  opacity: 0;
  transform: translateY(-10px);
}

.fade-slide-up-enter-from, .fade-slide-up-leave-to {
  opacity: 0;
  transform: translateY(10px);
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