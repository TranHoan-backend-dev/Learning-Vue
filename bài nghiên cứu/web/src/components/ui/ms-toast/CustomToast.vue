<script setup lang="ts">
import {toast} from "@/services/toast.ts";
import {computed} from "vue";

const currentToast = computed(() => toast.snackbars.value[0] ?? null)

const getIcon = (color: string) => {
  switch (color) {
    case 'success':
      return 'mdi-check-circle'
    case 'error':
      return 'mdi-close-circle'
    case 'warning':
      return 'mdi-alert-circle'
    case 'info':
      return 'mdi-information'
    default:
      return 'mdi-bell'
  }
}

</script>

<template>
  <v-snackbar-queue
      v-model="toast.snackbars.value"
      location="bottom right"
      :timeout="currentToast?.timeout || 3000"
      :color="currentToast?.color || 'info'"
      :title="currentToast?.title"
      :text="currentToast?.text"
      contained
      timer="bottom"
      variant="outlined"
      min-height="75"
      :prepend-icon="getIcon(currentToast?.color || 'info')"
      closable
      :total-visible="3"
  >
    <template #actions="{ props }">
      <v-btn
          v-bind="props"
          variant="text"
          icon="mdi-close-circle"
      />
    </template>
  </v-snackbar-queue>
</template>

<style scoped>

</style>