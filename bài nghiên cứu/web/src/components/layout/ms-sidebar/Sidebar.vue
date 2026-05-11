<script setup lang="ts">
import CustomButton from "@/components/ui/ms-button/CustomButton.vue";
import type { SidebarItems } from "./model";

defineProps({
  isCollapsed: Boolean
})
const emit = defineEmits(['toggle'])

const toggleSidebar = () => {
  emit('toggle')
}

const sidebarItems: SidebarItems[] = [
  {
    iconClassName: "sidebar_menu_item_recruitment_icon",
    text: "Tổng quan",
    to: "/overview"
  },
  {
    iconClassName: "sidebar_menu_item_user_icon",
    text: "Thành phần lương",
    to: "/compositions",
    parentClassName: "sidebar_menu_item_active"
  },
  {
    iconClassName: "sidebar_menu_item_calendar_icon",
    text: "Mẫu bảng lương",
    to: "/sample"
  },
  {
    iconClassName: "sidebar_menu_item_tiem_nang_icon",
    text: "Dữ liệu tính lương",
    to: "/data"
  },
  {
    iconClassName: "sidebar_menu_item_hiring_campaign_icon",
    text: "Tính lương",
    to: "/calculate"
  },
  {
    iconClassName: "sidebar_menu_item_job_icon",
    text: "Chi trả",
    to: "/pay"
  },
  {
    iconClassName: "sidebar_menu_item_ai_marketing_icon",
    text: "Báo cáo",
    to: "/report"
  },
  {
    iconClassName: "sidebar_menu_item_setting_icon",
    text: "Thiết lập",
    to: "/setting"
  }
];
</script>

<template>
  <div class="sidebar" :class="{ 'sidebar_collapsed': isCollapsed }">
    <div class="sidebar_bg">
      <div class="sidebar_menu">
        <CustomButton v-for="(component, index) in sidebarItems" class="sidebar_menu_item" :key="index"
          :to="component.to" variant="text" :width="isCollapsed ? 36 : 209" :height="40" :is-loading="false" :style="{
            marginLeft: isCollapsed ? '5px' : '12px',
            justifyContent: isCollapsed ? 'center' : 'left'
          }" tooltip-location="right" :tooltip-content="component.text">
          <div :class="component.iconClassName"></div>
          <div class="sidebar_menu_item_text">{{ component.text }}</div>
        </CustomButton>
      </div>

      <CustomButton class-name="sidebar_toggle" @click="toggleSidebar" :width="isCollapsed ? 36 : 209" :height="44"
        :is-loading="false" variant="text" tooltip-location="right" tooltip-content="Thu gọn">
        <div class="sidebar_toggle_icon" :style="{ transform: isCollapsed ? 'rotate(180deg)' : 'none' }"></div>
        <div v-if="!isCollapsed" class="sidebar_toggle_text">Thu gọn</div>
      </CustomButton>
    </div>
  </div>
</template>

<style scoped src="./style.css"></style>
