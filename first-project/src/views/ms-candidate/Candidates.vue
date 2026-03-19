<script setup lang="ts">

import CustomButton from "@/components/ui/ms-button/CustomButton.vue";
</script>

<template>
  <section class="content">
    <div class="content_header">
      <div class="content_header_left">
        <div class="content_header_title">Ứng viên</div>
      </div>
      <div class="content_header_right">
        <!--        Xoa-->
          <CustomButton
              class-name="content_header_right_btn_delete"
          >
            <div class="mi_icon_delete_user"></div>
            Xóa
          </CustomButton>
        <!--        Them-->
        <CustomButton
            variant="flat"
            class-name="content_header_right_btn_delete"
            style="background-color: var(--border-control-hover)"
        >
          <div class="content_header_action_add_icon_left"></div>
          <div class="content_header_action_add_icon_text">Thêm ứng viên</div>
        </CustomButton>
      </div>
    </div>
  </section>
</template>

<style scoped src="./style.css">

</style>