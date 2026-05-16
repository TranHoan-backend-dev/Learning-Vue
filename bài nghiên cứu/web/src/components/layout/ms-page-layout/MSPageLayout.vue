<script setup lang="ts">
/**
 * MSPageLayout - Component layout chung cho các trang trong hệ thống
 * Đảm bảo tính nhất quán về padding, spacing và cấu trúc header/body
 */
</script>

<template>
  <div class="ms-page-layout">
    <!-- Header của trang -->
    <header class="ms-page-header">
      <div class="ms-page-header-left">
        <slot name="header-left"></slot>
      </div>
      <div class="ms-page-header-right">
        <slot name="header-right"></slot>
      </div>
    </header>

    <!-- Nội dung chính của trang -->
    <main class="ms-page-body">
      <slot name="body"></slot>
    </main>
  </div>
</template>

<style scoped>
.ms-page-layout {
  height: 100%;
  display: flex;
  flex-direction: column;
  background-color: #f4f5f8;
  width: 100%;
}

.ms-page-header {
  height: auto;
  min-height: 48px;
  padding: 24px 24px 12px 24px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.ms-page-body {
  flex: 1;
  padding: 12px 24px 24px 24px;
  overflow: hidden;
  display: flex;
  flex-direction: column;
}

/* Các style bổ trợ cho slot để đảm bảo căn lề đúng */
.ms-page-header-left,
.ms-page-header-right {
  display: flex;
  align-items: center;
  gap: 12px;
}
</style>
