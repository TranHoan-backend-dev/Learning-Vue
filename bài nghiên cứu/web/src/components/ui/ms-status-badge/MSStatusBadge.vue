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
  padding: 0 10px;
  height: 22px;
  line-height: 22px;
  border-radius: 11px;
  border: 1px solid transparent;
  width: fit-content;
  white-space: nowrap;
}

.status-dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
}

.status-text {
  font-size: 13px;
  font-weight: 400;
}

/* Đang theo dõi (Active) */
.status-active {
  background-color: #ebf9eb;
  border-color: #b3ebb3;
}

.status-active .status-dot {
  background-color: var(--primary-green);
}

.status-active .status-text {
  color: var(--primary-green);
}

/* Ngừng theo dõi (Inactive) */
.status-inactive {
  background-color: #fff7e6;
  border-color: #ffcc99;
}

.status-inactive .status-dot {
  background-color: #ff8c00;
}

.status-inactive .status-text {
  color: #ff8c00;
}
</style>
