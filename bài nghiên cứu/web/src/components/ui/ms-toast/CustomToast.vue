<script setup lang="ts">
import { watch } from "vue";
import { toast } from "@/services/toast.ts";
import MSIcon from "@/components/ui/ms-icon/MSIcon.vue";

/**
 * Hàm xóa toast khỏi danh sách
 */
const closeToast = (id: number) => {
  const index = toast.snackbars.value.findIndex(t => t.id === id);
  if (index !== -1) {
    toast.snackbars.value.splice(index, 1);
  }
};

/**
 * Lấy cấu hình theo loại toast
 */
const getToastConfig = (type: string) => {
  switch (type) {
    case 'success':
      return { icon: 'check-circle', color: 'var(--primary-green)', bg: '#ebf9eb' };
    case 'error':
      return { icon: 'error', color: 'var(--misa-error)', bg: '#fff0f0' };
    case 'warning':
      return { icon: 'warning', color: 'var(--misa-warning)', bg: '#fff9eb' };
    case 'info':
      return { icon: 'info', color: 'var(--primary-green)', bg: '#ebf9eb' }; // Based on image, info is also green
    default:
      return { icon: 'info', color: 'var(--primary-green)', bg: '#ebf9eb' };
  }
};

/**
 * Tự động đóng toast sau timeout
 */
watch(() => toast.snackbars.value.length, (newVal, oldVal) => {
  if (newVal > oldVal) {
    const newestToast = toast.snackbars.value[newVal - 1];
    setTimeout(() => {
      closeToast(newestToast.id);
    }, newestToast.timeout || 3000);
  }
});
</script>

<template>
  <div class="misa-toast-container">
    <TransitionGroup name="toast-list">
      <div 
        v-for="item in toast.snackbars.value" 
        :key="item.id" 
        class="misa-toast-item"
        :style="{ backgroundColor: getToastConfig(item.color).bg }"
      >
        <!-- Icon Area (40x40) -->
        <div 
          class="misa-toast-icon-area" 
          :style="{ backgroundColor: getToastConfig(item.color).color }"
        >
          <MSIcon :name="getToastConfig(item.color).icon" color="#fff" size="20" />
        </div>

        <!-- Content Area -->
        <div class="misa-toast-content">
          <div class="misa-toast-text">
            <span class="misa-toast-title">{{ item.title }}</span>
            <span class="misa-toast-message">{{ item.text }}</span>
          </div>
          <div class="misa-toast-close" @click="closeToast(item.id)">
            <MSIcon name="close" size="16" color="#666" />
          </div>
        </div>
      </div>
    </TransitionGroup>
  </div>
</template>

<style scoped>
.misa-toast-container {
  position: fixed;
  bottom: 24px;
  right: 24px;
  z-index: 9999;
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.misa-toast-item {
  min-height: 40px;
  height: auto;
  min-width: 300px;
  display: flex;
  align-items: stretch;
  border-radius: 4px;
  overflow: hidden;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  border: 1px solid rgba(0, 0, 0, 0.05);
}

.misa-toast-icon-area {
  width: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.misa-toast-content {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 8px 8px 8px 12px;
}

.misa-toast-text {
  font-size: 14px;
  color: #111;
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.misa-toast-title {
  font-weight: 700;
}

.misa-toast-message {
  color: #111;
  font-size: 13px;
}

.misa-toast-close {
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  border-radius: 4px;
  transition: background-color 0.2s;
  margin-left: 12px;
}

.misa-toast-close:hover {
  background-color: rgba(0, 0, 0, 0.05);
}

/* Animations */
.toast-list-enter-active,
.toast-list-leave-active {
  transition: all 0.3s ease;
}
.toast-list-enter-from {
  opacity: 0;
  transform: translateX(50px);
}
.toast-list-leave-to {
  opacity: 0;
  transform: translateX(50px);
}
</style>