<script setup lang="ts">
import { computed } from 'vue';

interface Props {
  modelValue: number; // Trang hiện tại
  total: number;      // Tổng số bản ghi
  pageSize: number;   // Số bản ghi trên trang
  color?: string;     // Màu sắc chủ đạo (tùy chọn)
}

const props = withDefaults(defineProps<Props>(), {
  color: '#0070f3'
});

const emit = defineEmits(['update:modelValue', 'change']);

// Tính tổng số trang
const totalPages = computed(() => Math.ceil(props.total / props.pageSize) || 1);

// Logic tạo dải số trang có dấu ba chấm (...)
const pages = computed(() => {
  const total = totalPages.value;
  const current = props.modelValue;
  const delta = 1; // Số trang hiển thị bên cạnh trang hiện tại (tổng cộng 3 trang ở giữa)
  
  // Nếu tổng số trang nhỏ hơn hoặc bằng 7, hiển thị tất cả
  if (total <= 7) {
    return Array.from({ length: total }, (_, i) => i + 1);
  }

  const range: (number | string)[] = [];
  const left = current - delta;
  const right = current + delta;
  
  // Luôn hiển thị trang 1
  range.push(1);

  if (left > 2) {
    range.push('...');
  }

  // Hiển thị các trang xung quanh trang hiện tại
  // Điều chỉnh để luôn có 3 số ở giữa
  let start = Math.max(2, left);
  let end = Math.min(total - 1, right);

  // Đảm bảo luôn hiện đủ số lượng nếu ở gần đầu hoặc cuối
  if (current <= 3) {
    end = 4;
  } else if (current >= total - 2) {
    start = total - 3;
  }

  for (let i = start; i <= end; i++) {
    range.push(i);
  }

  if (end < total - 1) {
    range.push('...');
  }

  // Luôn hiển thị trang cuối
  range.push(total);

  return range;
});

const setPage = (page: number | string) => {
  // Nếu là dấu ba chấm, không làm gì cả
  if (page === '...') return;

  const targetPage = typeof page === 'string' ? parseInt(page) : page;

  // Kiểm tra điều kiện: Trang đích phải nằm trong khoảng [1, totalPages] và khác trang hiện tại
  if (
    targetPage >= 1 && 
    targetPage <= totalPages.value && 
    targetPage !== props.modelValue
  ) {
    emit('update:modelValue', targetPage);
    emit('change', targetPage);
  }
};
</script>

<template>
  <div class="ms-pagination">
    <!-- Nút Prev -->
    <div 
      class="pagination-item nav-btn" 
      :class="{ 'disabled': modelValue <= 1 }"
      @click="setPage(modelValue - 1)"
    >
      <i class="bi bi-chevron-left"></i>
    </div>

    <!-- Danh sách trang -->
    <div 
      v-for="(page, index) in pages" 
      :key="index"
      :class="[
        'pagination-item', 
        { 'active': page === modelValue },
        { 'ellipsis': page === '...' }
      ]"
      @click="setPage(page)"
      :style="page === modelValue ? { backgroundColor: color } : {}"
    >
      {{ page }}
    </div>

    <!-- Nút Next -->
    <div 
      class="pagination-item nav-btn" 
      :class="{ 'disabled': modelValue >= totalPages }"
      @click="setPage(modelValue + 1)"
    >
      <i class="bi bi-chevron-right"></i>
    </div>
  </div>
</template>

<style scoped>
.ms-pagination {
  display: flex;
  gap: 8px;
  align-items: center;
  user-select: none;
}

.pagination-item {
  min-width: 36px;
  height: 36px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 12px; /* Tăng bo góc cho giống ảnh */
  background-color: #f4f4f5;
  color: #11181c;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1);
}

.pagination-item:hover:not(.active):not(.ellipsis):not(.disabled) {
  background-color: #e4e4e7;
  transform: scale(1.05);
}

.pagination-item.active {
  color: #ffffff;
  box-shadow: 0 4px 12px 0 rgba(0, 0, 0, 0.1);
}

.pagination-item.ellipsis {
  background: transparent;
  cursor: default;
  color: #a1a1aa;
}

.pagination-item.disabled {
  opacity: 0.5;
  cursor: not-allowed;
  background-color: #fafafa;
}

.nav-btn {
  font-size: 12px;
  background-color: #f4f4f5;
}

/* Hiệu ứng khi nhấn */
.pagination-item:active:not(.ellipsis):not(.disabled) {
  transform: scale(0.95);
}

/* Icon style */
.nav-btn i {
  display: flex;
  align-items: center;
  justify-content: center;
}
</style>
