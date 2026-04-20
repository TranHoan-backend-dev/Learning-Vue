<script setup lang="ts">
import { computed } from 'vue';

const props = defineProps<{
  candidate: any;
}>();

defineEmits(['close', 'edit']);

const avatarText = computed(() => {
  if (!props.candidate?.name) return 'TT';
  const names = props.candidate.name.split(' ');
  if (names.length === 1) return names[0].charAt(0).toUpperCase();
  return (names[0].charAt(0) + names[names.length - 1].charAt(0)).toUpperCase();
});

const tabs = ['Hồ sơ ứng viên', 'Email', 'Tin nhắn', 'Đánh giá', 'Bình luận', 'Tài liệu', 'Công việc', 'Lịch sử'];
const subTabs = ['CV ứng viên', 'Thông tin ứng tuyển', 'Thông tin tiếp nhận'];
</script>

<template>
  <div class="detail_modal_content">
    <!-- Sidebar trái -->
    <div class="detail_sidebar">
      <div class="detail_sidebar_header">
        <div class="detail_sidebar_avatar">{{ avatarText }}</div>
        <div class="detail_sidebar_info_top">
          <div class="detail_sidebar_name_row">
            <span class="detail_sidebar_name">{{ candidate?.name }}</span>
            <div class="detail_sidebar_edit_icon" @click="$emit('edit')"></div>
          </div>
          <div class="detail_sidebar_sub_info">Ứng tuyển ngày {{ candidate?.hiringAt || '--' }}</div>
          <div class="detail_sidebar_sub_info">Ứng viên thuộc 1 chiến dịch tuyển dụng. <span class="detail_sidebar_link">Chi tiết</span></div>
          <div class="detail_sidebar_rating">
            <i class="bi bi-star-fill active"></i>
            <i class="bi bi-star-fill active"></i>
            <i class="bi bi-star-fill active"></i>
            <i class="bi bi-star-fill"></i>
            <i class="bi bi-star-fill"></i>
          </div>
        </div>
      </div>

      <div class="detail_sidebar_body">
        <div class="detail_sidebar_section">
          <div class="detail_sidebar_section_title">THÔNG TIN ỨNG TUYỂN (0)</div>
          <p class="detail_sidebar_sub_info">Ứng viên chưa thuộc tin tuyển dụng nào</p>
          <button class="btn_secondary_outline">
            <i class="bi bi-person-plus"></i> Chuyển ứng viên vào tin
          </button>
        </div>

        <div class="detail_sidebar_section">
          <div class="detail_sidebar_section_title" style="display: flex; gap: 8px; align-items: center;">
            Gắn thẻ <i class="bi bi-plus-square" style="color: #2970f6; cursor: pointer;"></i>
          </div>
        </div>

        <div class="detail-sidebar__divider"></div>

        <div class="detail_sidebar_section">
          <div class="detail_sidebar_section_title">THÔNG TIN CÁ NHÂN</div>
          <div class="detail-sidebar__row">
            <div class="detail-sidebar__label"><i class="bi bi-envelope"></i> Email</div>
            <div class="detail-sidebar__value">
              <span class="detail_sidebar_link">{{ candidate?.email }}</span>
            </div>
          </div>
          <div class="detail-sidebar__row">
            <div class="detail-sidebar__label"><i class="bi bi-telephone"></i> SĐT</div>
            <div class="detail-sidebar__value">{{ candidate?.phone }}</div>
          </div>
          <div class="detail-sidebar__row">
            <div class="detail-sidebar__label"><i class="bi bi-calendar"></i> Ngày sinh</div>
            <div class="detail-sidebar__value">{{ candidate?.dob || '--' }}</div>
          </div>
          <div class="detail-sidebar__row">
            <div class="detail-sidebar__label"><i class="bi bi-gender-ambiguous"></i> Giới tính</div>
            <div class="detail-sidebar__value">{{ candidate?.gender || '--' }}</div>
          </div>
        </div>

        <div class="detail_sidebar_section">
          <div class="detail_sidebar_section_title">HỌC VẤN</div>
          <div class="detail-sidebar__row">
            <div class="detail-sidebar__label" style="align-items: flex-start; padding-top: 4px;">• --</div>
            <div class="detail-sidebar__value">
              <div style="color: #65696e; font-size: 13px;">Nơi đào tạo</div>
              <div>{{ candidate?.educationLocation || '--' }}</div>
              <div style="color: #65696e; font-size: 13px; margin-top: 8px;">Chuyên ngành</div>
              <div style="font-weight: 700;">{{ candidate?.major || '--' }}</div>
            </div>
          </div>
        </div>
      </div>

      <div class="detail-sidebar__footer">
        <div class="btn-upload-small"><i class="bi bi-plus"></i></div>
        <div class="detail-sidebar__link-grey">
          <i class="bi bi-bookmark"></i> Theo dõi
        </div>
      </div>
    </div>

    <!-- Nội dung chính -->
    <div class="detail-main">
      <div class="detail-main__tabs">
        <div v-for="(tab, index) in tabs" :key="index" :class="['detail-main__tab', { active: index === 0 }]">
          {{ tab }}
        </div>
      </div>

      <div class="detail-main__sub-header">
        <div class="detail-main__sub-tabs">
          <div v-for="(sub, index) in subTabs" :key="index" :class="['detail-main__sub-tab', { active: index === 0 }]">
            {{ sub }}
          </div>
        </div>
        <button class="btn_upload">
          <i class="bi bi-upload"></i> Tải lên CV mới
        </button>
      </div>

      <div class="detail_main_body">
        <div class="detail_main_content_area">
          <div class="detail_main_placeholder_text">CV ỨNG VIÊN</div>
          <div style="text-align: center; color: #65696e;">
            Thông tin CV sẽ được hiển thị tại đây
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.detail_modal_content {
    background-color: #fff;
    width: 100%;
    height: 90vh;
    border-radius: 4px;
    display: flex;
    overflow: hidden;
    position: relative;
}

/* Sidebar Styling */
.detail_sidebar {
    width: 320px;
    border-right: 1px solid #dddde4;
    display: flex;
    flex-direction: column;
    background-color: #fff;
}

.detail_sidebar_header {
    padding: 24px;
    display: flex;
    gap: 16px;
    position: relative;
}

.detail_sidebar_avatar {
    width: 48px;
    height: 48px;
    background-color: #2970f6;
    color: #fff;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 18px;
    font-weight: 700;
}

.detail_sidebar_info_top {
    flex: 1;
}

.detail_sidebar_name_row {
    display: flex;
    align-items: center;
    gap: 8px;
    margin-bottom: 4px;
}

.detail_sidebar_name {
    font-size: 16px;
    font-weight: 700;
    color: #1f1f1f;
}

.detail_sidebar_edit_icon {
    width: 16px;
    height: 16px;
    background: url(https://amisplatform.misacdn.net/apps/recruit/assets/images/ICON.svg) no-repeat -182px -658px;
    cursor: pointer;
}

.detail_sidebar_sub_info {
    font-size: 13px;
    color: #65696e;
    margin-bottom: 4px;
}

.detail_sidebar_link {
    color: #2970f6;
    text-decoration: none;
    cursor: pointer;
}

.detail_sidebar_rating {
    margin-top: 8px;
    color: #ddd;
    display: flex;
    gap: 4px;
}

.detail_sidebar_rating .active {
    color: #f1c40f;
}

.detail_sidebar_body {
    flex: 1;
    overflow-y: auto;
    padding: 0 24px 24px 24px;
}

.detail_sidebar_section {
    margin-top: 24px;
}

.detail_sidebar_section_title {
    font-size: 12px;
    font-weight: 700;
    color: #65696e;
    text-transform: uppercase;
    margin-bottom: 12px;
}

.detail-sidebar__row {
    display: flex;
    margin-bottom: 16px;
    font-size: 14px;
}

.detail-sidebar__label {
    width: 100px;
    color: #65696e;
    display: flex;
    align-items: center;
    gap: 8px;
}

.detail-sidebar__value {
    flex: 1;
    color: #1f1f1f;
}

.detail-sidebar__divider {
    height: 1px;
    background: #eee;
    margin: 16px 0;
}

.detail-sidebar__footer {
    padding: 16px 24px;
    border-top: 1px solid #dddde4;
    display: flex;
    justify-content: space-between;
    align-items: center;
}

.btn-upload-small {
    padding: 4px 8px;
    border-radius: 4px;
    border: 1px solid #dddde4;
    cursor: pointer;
}

.detail-sidebar__link-grey {
    display: flex;
    gap: 4px;
    align-items: center;
    color: #65696e;
    font-size: 13px;
    cursor: pointer;
}

/* Main Content Styling */
.detail-main {
    flex: 1;
    display: flex;
    flex-direction: column;
    background-color: #f0f2f5;
}

.detail-main__tabs {
    background-color: #fff;
    display: flex;
    padding: 0 16px;
    border-bottom: 1px solid #dddde4;
}

.detail-main__tab {
    padding: 12px 16px;
    font-size: 14px;
    font-weight: 600;
    color: #65696e;
    cursor: pointer;
    text-transform: uppercase;
    border-bottom: 3px solid transparent;
}

.detail-main__tab.active {
    color: #2970f6;
    border-bottom-color: #2970f6;
}

.detail-main__sub-header {
    background-color: #fff;
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 8px 16px;
}

.detail-main__sub-tabs {
    display: flex;
    gap: 8px;
}

.detail-main__sub-tab {
    padding: 8px 16px;
    font-size: 14px;
    color: #65696e;
    cursor: pointer;
    border-radius: 4px;
}

.detail-main__sub-tab.active {
    color: #2970f6;
    background-color: #e2efff;
}

.detail_main_body {
    flex: 1;
    padding: 16px;
    display: flex;
    flex-direction: column;
}

.detail_main_content_area {
    flex: 1;
    background-color: #fff;
    border-radius: 4px;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    color: #1f1f1f;
    border: 1px solid #dddde4;
}

.detail_main_placeholder_text {
    font-weight: 700;
    font-size: 18px;
    margin-bottom: 24px;
}

.btn_upload {
    background-color: #fff;
    border: 1px solid #dddde4;
    padding: 8px 16px;
    border-radius: 4px;
    display: flex;
    align-items: center;
    gap: 8px;
    cursor: pointer;
    font-size: 14px;
}

.btn_upload:hover {
    background-color: #f8f9fa;
}

.btn_secondary_outline {
    background: transparent;
    border: 1px solid #2970f6;
    color: #2970f6;
    padding: 6px 12px;
    border-radius: 4px;
    font-weight: 600;
    font-size: 13px;
    display: flex;
    align-items: center;
    gap: 8px;
    cursor: pointer;
}

.btn_secondary_outline:hover {
    background-color: #f1f6ff;
}
</style>
