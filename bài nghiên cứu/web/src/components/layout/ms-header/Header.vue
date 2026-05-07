<script setup lang="ts">
import {ref, watch} from "vue";
import type {LeftHeaderComponents, RightHeaderComponents} from "@/components/layout/ms-header/model.ts";
import SearchField from "@/components/ui/ms-input/SearchField.vue";

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
      <div class="navbar_left_search">
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
    </section>
  </div>
</template>

<style scoped src="./style.css">

</style>
