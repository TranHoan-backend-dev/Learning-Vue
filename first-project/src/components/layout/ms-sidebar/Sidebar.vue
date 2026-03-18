<script setup lang="ts">
import type {SidebarItems} from "@/components/layout/ms-sidebar/model.ts";

const sidebarItems: SidebarItems[] = [
  {
    iconClassName: "sidebar_menu_item_recruitment_icon",
    text: "Tin tuyển dụng"
  },
  {
    iconClassName: "sidebar_menu_item_user_icon",
    text: "Ứng viên",
    parentClassName: "sidebar_menu_item_active"
  },
  {
    iconClassName: "sidebar_menu_item_calendar_icon",
    text: "Lịch"
  },
  {
    iconClassName: "sidebar_menu_item_tiem_nang_icon",
    text: "Kho tiềm năng"
  },
  {
    iconClassName: "sidebar_menu_item_hiring_campaign_icon",
    text: "Chiến dịch tuyển dụng"
  },
  {
    iconClassName: "sidebar_menu_item_job_icon",
    text: "Công việc"
  },
  {
    iconClassName: "sidebar_menu_item_ai_marketing_icon",
    text: "aiMarketing"
  },
  {
    iconClassName: "sidebar_menu_item_contact_icon",
    text: "Trao đổi với ứng viên"
  },
  {
    iconClassName: "sidebar_menu_item_report_icon",
    text: "Báo cáo"
  },
  {
    iconClassName: "sidebar_menu_item_setting_icon",
    text: "Thiết lập"
  },
  {
    iconClassName: "sidebar_menu_item_knowledge_icon",
    text: "Kiến thức hữu ích"
  },
];
</script>

<template>
  <div class="sidebar">
    <div class="sidebar_bg">
      <div class="sidebar_menu">
        <div
            class="sidebar_menu_item"
            v-for="(component, index) in sidebarItems"
            :class="component.parentClassName"
            :key="index"
        >
          <div :class="component.iconClassName"></div>
          <div class="sidebar_menu_item_text">{{ component.text }}</div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped src="./style.css">

</style>