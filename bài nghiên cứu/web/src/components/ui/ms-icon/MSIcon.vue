<script setup lang="ts">
import { computed } from 'vue';

interface Props {
  name: 'pin' | 'check-circle' | 'minus-circle' | 'copy' | 'edit' | 'trash' | 'drag-handle' | 'info' | 'warning' | 'error' | 'close' | 'plus';
  size?: string | number;
  color?: string;
  strokeWidth?: string | number;
}

const props = withDefaults(defineProps<Props>(), {
  size: 18,
  color: 'currentColor',
  strokeWidth: 2
});

const isFillIcon = computed(() => ['pin', 'drag-handle'].includes(props.name));
</script>

<template>
  <div class="ms-icon" :style="{ width: size + 'px', height: size + 'px' }">
    <svg v-if="!isFillIcon" viewBox="0 0 24 24" :width="size" :height="size" :stroke="color" :stroke-width="strokeWidth"
      fill="none" stroke-linecap="round" stroke-linejoin="round">
      <!-- check-circle -->
      <template v-if="name === 'check-circle'">
        <circle cx="12" cy="12" r="10"></circle>
        <polyline points="8 12 11 15 16 9"></polyline>
      </template>
      <!-- minus-circle -->
      <template v-else-if="name === 'minus-circle'">
        <circle cx="12" cy="12" r="10"></circle>
        <line x1="8" y1="12" x2="16" y2="12"></line>
      </template>

      <!-- info -->
      <template v-else-if="name === 'info'">
        <circle cx="12" cy="12" r="10"></circle>
        <line x1="12" y1="16" x2="12" y2="12"></line>
        <line x1="12" y1="8" x2="12.01" y2="8"></line>
      </template>

      <!-- warning -->
      <template v-else-if="name === 'warning'">
        <circle cx="12" cy="12" r="10"></circle>
        <line x1="12" y1="8" x2="12" y2="12"></line>
        <line x1="12" y1="16" x2="12.01" y2="16"></line>
      </template>

      <!-- error -->
      <template v-else-if="name === 'error'">
        <circle cx="12" cy="12" r="10"></circle>
        <line x1="15" y1="9" x2="9" y2="15"></line>
        <line x1="9" y1="9" x2="15" y2="15"></line>
      </template>

      <!-- close -->
      <template v-else-if="name === 'close'">
        <line x1="18" y1="6" x2="6" y2="18"></line>
        <line x1="6" y1="6" x2="18" y2="18"></line>
      </template>

      <!-- plus -->
      <template v-else-if="name === 'plus'">
        <line x1="12" y1="5" x2="12" y2="19"></line>
        <line x1="5" y1="12" x2="19" y2="12"></line>
      </template>

      <!-- copy / Nhân bản -->
      <template v-else-if="name === 'copy'">
        <rect x="9" y="9" width="13" height="13" rx="2" ry="2"></rect>
        <path d="M5 15H4a2 2 0 0 1-2-2V4a2 2 0 0 1 2-2h9a2 2 0 0 1 2 2v1"></path>
      </template>

      <!-- edit / Sửa -->
      <template v-else-if="name === 'edit'">
        <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"></path>
        <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"></path>
      </template>

      <!-- trash / Xóa -->
      <template v-else-if="name === 'trash'">
        <polyline points="3 6 5 6 21 6"></polyline>
        <path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path>
      </template>
    </svg>

    <!-- Icons that use fill -->
    <svg v-else viewBox="0 0 24 24" :width="size" :height="size" :fill="color">
      <!-- pin -->
      <path v-if="name === 'pin'" d="M16 9V4l1 1V2H7v3l1-1v5L6 12v2h5v7l1 1 1-1v-7h5v-2l-2-3z" />

      <!-- drag-handle -->
      <path v-else-if="name === 'drag-handle'"
        d="M11 18c0 1.1-.9 2-2 2s-2-.9-2-2 .9-2 2-2 2 .9 2 2zm-2-8c-1.1 0-2 .9-2 2s.9 2 2 2 2-.9 2-2-.9-2-2-2zm0-8c-1.1 0-2 .9-2 2s.9 2 2 2 2-.9 2-2-.9-2-2-2zm6 4c1.1 0 2-.9 2-2s-.9-2-2-2-2 .9-2 2 .9 2 2 2zm0 2c-1.1 0-2 .9-2 2s.9 2 2 2 2-.9 2-2-.9-2-2-2zm0 6c-1.1 0-2 .9-2 2s.9 2 2 2 2-.9 2-2-.9-2-2-2z" />
    </svg>
  </div>
</template>

<style scoped>
.ms-icon {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
}

.ms-icon svg {
  display: block;
}
</style>