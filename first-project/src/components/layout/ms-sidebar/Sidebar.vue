<script setup lang="ts">
import {ref} from "vue";
import type {SidebarItems} from "@/components/layout/ms-sidebar/model.ts";
import CustomButton from "@/components/ui/ms-button/CustomButton.vue";

const props = defineProps({
  isCollapsed: Boolean
})
const emit = defineEmits(['toggle'])

const toggleSidebar = () => {
  emit('toggle')
}

const sidebarItems: SidebarItems[] = [
  {
    iconClassName: "sidebar_menu_item_recruitment_icon",
    text: "Tin tuyển dụng",
    to: "/hiring"
  },
  {
    iconClassName: "sidebar_menu_item_user_icon",
    text: "Ứng viên",
    to: "/candidates",
    parentClassName: "sidebar_menu_item_active"
  },
  {
    iconClassName: "sidebar_menu_item_calendar_icon",
    text: "Lịch",
    to: "/schedule"
  },
  {
    iconClassName: "sidebar_menu_item_tiem_nang_icon",
    text: "Kho tiềm năng",
    to: "/talent"
  },
  {
    iconClassName: "sidebar_menu_item_hiring_campaign_icon",
    text: "Chiến dịch tuyển dụng",
    to: "/media"
  },
  {
    iconClassName: "sidebar_menu_item_job_icon",
    text: "Công việc",
    to: "/job"
  },
  {
    iconClassName: "sidebar_menu_item_ai_marketing_icon",
    text: "aiMarketing",
    to: "/marketing"
  },
  {
    iconClassName: "sidebar_menu_item_contact_icon",
    text: "Trao đổi với ứng viên",
    to: "/conversation"
  },
  {
    iconClassName: "sidebar_menu_item_report_icon",
    text: "Báo cáo",
    to: "/report"
  },
  {
    iconClassName: "sidebar_menu_item_setting_icon",
    text: "Thiết lập",
    to: "/setting"
  },
  {
    iconClassName: "sidebar_menu_item_knowledge_icon",
    text: "Kiến thức hữu ích",
    to: "/knowledge"
  },
];
</script>

<template>
  <div class="sidebar" :class="{'sidebar_collapsed': isCollapsed}">
    <div class="sidebar_bg">
      <div class="sidebar_menu">
        <CustomButton
            v-for="(component, index) in sidebarItems"
            class="sidebar_menu_item"
            active-class="sidebar_menu_item_active"
            :key="index"
            :to="component.to"
            variant="text"
            :width="isCollapsed ? 36 : 209"
            :height="40"
            :is-loading="false"
            :style="{
              marginLeft: isCollapsed ? '5px' : '12px',
              justifyContent: isCollapsed ? 'center' : 'left'
            }"
        >
          <div :class="component.iconClassName"></div>
          <div class="sidebar_menu_item_text">{{ component.text }}</div>
          <v-tooltip
              activator="parent"
              location="right"
          >
            {{ component.text }}
          </v-tooltip>
        </CustomButton>
      </div>

      <CustomButton
          class-name="sidebar_toggle"
          :label="isCollapsed ? '' : 'Thu gọn'"
          :prepend-icon="isCollapsed ? 'mdi-chevron-double-right' : 'mdi-chevron-double-left'"
          @click="toggleSidebar"
          min-width="35"
          height="40"
          :width="isCollapsed ? 0 : 209"
          :is-loading="false"
          variant="outlined"
          style="color: white; border-color: #dee2e6;"
      />
    </div>
  </div>
</template>

<style scoped src="./style.css">
</style>
