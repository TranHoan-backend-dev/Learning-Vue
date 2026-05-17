<template>
  <v-app>
    <v-main>
      <div class="main" :style="{ '--sidebar-width': isCollapsed ? '60px' : '233px', paddingLeft: isCollapsed ? '60px' : '233px' }">
        <Header/>
        <Sidebar :is-collapsed="isCollapsed" @toggle="isCollapsed = !isCollapsed"/>
        <div class="content_view">
          <router-view/>
        </div>
      </div>
    </v-main>
    <CustomToast/>
  </v-app>
</template>

<script lang="ts" setup>
import Header from '@/components/layout/ms-header/Header.vue'
import {ref, onMounted, onUnmounted} from "vue";
import Sidebar from "@/components/layout/ms-sidebar/Sidebar.vue";
import CustomToast from "@/components/ui/ms-toast/CustomToast.vue";

const isCollapsed = ref(false);

const handleResize = () => {
  // Sử dụng breakpoint chuẩn 1280px (dành cho laptop/tablet ngang) thay vì tỷ lệ screen.width
  // vì screen.width sẽ bị sai khi dùng giả lập Responsive của trình duyệt
  // và gây lỗi trên các màn hình quá to (như màn 4K).
  if (window.innerWidth <= 1280) {
    isCollapsed.value = true;
  } else {
    isCollapsed.value = false;
  }
};

onMounted(() => {
  window.addEventListener('resize', handleResize);
  handleResize(); // trigger on mount
});

onUnmounted(() => {
  window.removeEventListener('resize', handleResize);
});
</script>

<style scoped>
.main {
  padding-top: 48px; /* Height of Header */
  height: 100vh;
  transition: padding-left 0.3s ease;
}

.content_view {
  height: calc(100vh - 48px);
  overflow-y: auto;
}
</style>
