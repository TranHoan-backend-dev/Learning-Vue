<script setup lang="ts">
import MSIcon from "@/components/ui/ms-icon/MSIcon.vue";

defineProps<{
  isAddDropdownVisible: boolean;
}>();

const emit = defineEmits(['add', 'toggle-dropdown', 'add-from-system', 'open-system']);
</script>

<template>
  <div class="content_header_right">
    <button class="misa-btn-outline" @click="() => { console.log('Header button clicked'); emit('open-system'); }">
      <MSIcon name="edit" size="20" color="#5a5a5a" stroke-width="2.5" />
      <span>Danh mục của hệ thống</span>
    </button>

    <div class="split_button_container">
      <button class="misa-btn-primary-left" @click="emit('add')">
        <div class="mi_icon_add_white"></div>
        Thêm mới
      </button>
      <div class="split_button_arrow" @click="emit('toggle-dropdown', $event)">
        <div class="mi_icon_arrow_down_white"></div>
      </div>
      <!-- Dropdown menu -->
      <div v-if="isAddDropdownVisible" class="misa-dropdown-menu">
        <div class="misa-dropdown-item" @click="emit('add-from-system')">
          Chọn từ danh mục của hệ thống
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped src="../style.css"></style>
