<script setup lang="ts">
import {ref, watch, onMounted, onUnmounted} from "vue";
import type {LeftHeaderComponents, RightHeaderComponents} from "@/components/layout/ms-header/model.ts";
import SearchField from "@/components/ui/ms-input/SearchField.vue";

const isMobileMode = ref(false);
const isMobileMenuOpen = ref(false);
const isSearchHidden = ref(false);

const handleResize = () => {
  // Thay đổi breakpoint xuống 992px để tránh bị gom sớm trên màn hình laptop thông thường (1280px, 1366px)
  isMobileMode.value = window.innerWidth <= 992;
  // Chỉ ẩn thanh tìm kiếm khi màn hình thực sự quá nhỏ (dưới 700px) không đủ chứa logo + search + menu
  isSearchHidden.value = window.innerWidth <= 700;
  
  if (!isMobileMode.value) {
    isMobileMenuOpen.value = false;
  }
};

onMounted(() => {
  window.addEventListener('resize', handleResize);
  handleResize();
});

onUnmounted(() => {
  window.removeEventListener('resize', handleResize);
});

let message = ref()
watch(message, (newValue, _) => {
  console.log('New: ', newValue)
})

const leftComponents: LeftHeaderComponents[] = [
  {
    // Nen tet
    className: "navbar_left_logo_bg_tet",
    label: ""
  },
  {
    // App
    className: "navbar_left_logo_launcher",
    label: ""
  },
  {
    // Logo
    className: "navbar_left_logo_icon",
    label: ""
  },
  {
    // App name
    className: "navbar_left_logo_name",
    label: "Tuyển dụng"
  },
]

const navbarRightClassNamePrefix = "navbar_right_"
const rightComponents: RightHeaderComponents[] = [
  {
    parentTitle: "Thông báo",
    childClassName: navbarRightClassNamePrefix.concat("notify_icon")
  },
  {
    parentTitle: "Chat",
    childClassName: navbarRightClassNamePrefix.concat("chat_icon")
  },
  {
    parentTitle: "Help",
    childClassName: navbarRightClassNamePrefix.concat("help_icon")
  },
  {
    parentTitle: "Option",
    childClassName: navbarRightClassNamePrefix.concat("option_icon")
  },
  {
    parentTitle: "Avatar",
    childClassName: navbarRightClassNamePrefix.concat("avatar_icon")
  },
];

</script>

<template>
  <div class="navbar">
    <!-- Left header -->
    <section class="navbar_left">
      <div class="navbar_left_logo">
        <div v-for="(component, index) in leftComponents"
             :key="index"
             :class="component.className"
        >
          {{ component.label }}
        </div>
      </div>
      <!--    Thanh tìm kiếm-->
      <div class="navbar_left_search" v-if="!isSearchHidden">
        <!--      icon-->
        <div class="navbar_left_search_icon"></div>
        <SearchField
            className="navbar_left_search_input"
            placeholder="tin tuyển dụng, ứng viên, nhân tài,..."
            label=""
            v-model="message"
        />
      </div>
    </section>

    <!-- Right header -->
    <section class="navbar_right">
      <template v-if="!isMobileMode">
        <div class="navbar_right_website">
          <div class="navbar_right_website_icon"></div>
          <div class="navbar_right_website_text">
            Website tuyen dung
          </div>
        </div>
        <div class="navbar_right_item"
             v-for="(component, index) in rightComponents"
             :key="index"
             :title="component.parentTitle.toString()"
        >
          <div :class="component.childClassName"></div>
        </div>
      </template>

      <!-- Hamburger Menu cho màn hình nhỏ -->
      <div v-else class="navbar_right_hamburger" @click="isMobileMenuOpen = !isMobileMenuOpen">
        <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="#7a8188" stroke-width="2" style="cursor: pointer;">
          <line x1="3" y1="12" x2="21" y2="12"></line>
          <line x1="3" y1="6" x2="21" y2="6"></line>
          <line x1="3" y1="18" x2="21" y2="18"></line>
        </svg>

        <div v-if="isMobileMenuOpen" class="mobile_dropdown_menu">
          <div class="navbar_right_website">
            <div class="navbar_right_website_icon"></div>
            <div class="navbar_right_website_text">
              Website
            </div>
          </div>
          <div class="navbar_right_item"
               v-for="(component, index) in rightComponents"
               :key="index"
               :title="component.parentTitle.toString()"
          >
            <div :class="component.childClassName"></div>
            <span class="mobile_dropdown_text">{{ component.parentTitle }}</span>
          </div>
        </div>
      </div>
    </section>
  </div>
</template>

<style scoped src="./style.css">

</style>
