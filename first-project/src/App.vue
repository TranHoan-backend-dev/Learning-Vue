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
  </v-app>
</template>

<script lang="ts" setup>
import Header from '@/components/layout/ms-header/Header.vue'
import {ref} from "vue";
import Sidebar from "@/components/layout/ms-sidebar/Sidebar.vue";

const isCollapsed = ref(false);
</script>

<style scoped>
.main {
  padding-top: 48px; /* Height of Header */
  height: 100vh;
  transition: padding-left 0.3s ease;
}

.content_view {
  padding: 24px;
  height: calc(100vh - 48px);
  overflow-y: auto;
}
</style>
