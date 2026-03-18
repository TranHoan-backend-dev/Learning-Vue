<template>
  <v-app>
    <v-main>
      <div class="main">
        <Header/>
        <Sidebar/>
      </div>
    </v-main>
  </v-app>
</template>

<script lang="ts" setup>
import Header from '@/components/layout/ms-header/Header.vue'
import SearchField from "@/components/ui/ms-input/SearchField.vue";
import {ref, watch} from "vue";
import NormalInput from "@/components/ui/ms-input/NormalInput.vue";
import Sidebar from "@/components/layout/ms-sidebar/Sidebar.vue";

let message = ref()
watch(message, (newValue, _) => {
  console.log('New: ', newValue)
})
</script>

<style scoped>
.main {
  margin: 160px;
}
</style>
