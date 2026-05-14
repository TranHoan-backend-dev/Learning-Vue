<template>
  <div class="ms-status-badge" :class="statusClass">
    <div class="status-dot"></div>
    <span class="status-text">{{ statusText }}</span>
  </div>
</template>

<script setup>
import {computed} from 'vue';

const props = defineProps({
  status: {
    type: Number,
    required: true
  }
});

const statusText = computed(() => {
  return props.status === 1 ? 'Đang theo dõi' : 'Ngừng theo dõi';
});

const statusClass = computed(() => {
  return props.status === 1 ? 'status-active' : 'status-inactive';
});
</script>

<style scoped>
.ms-status-badge {
  display: flex;
  align-items: center;
  gap: 8px;
}

.status-dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
}

.status-text {
  font-size: 14px;
}

/* Đang theo dõi */
.status-active .status-dot {
  background-color: var(--primary-green);
}

.status-active .status-text {
  color: var(--primary-green);
}

/* Ngừng theo dõi */
.status-inactive .status-dot {
  background-color: var(--misa-warning);
}

.status-inactive .status-text {
  color: var(--misa-warning);
}
</style>
