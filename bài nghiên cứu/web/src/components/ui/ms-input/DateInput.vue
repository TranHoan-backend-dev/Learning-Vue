<script setup lang="ts">
import NormalInput from "@/components/ui/ms-input/NormalInput.vue";

const model = defineModel<string>()

defineProps<{
  className?: string,
  errorMessage?: string,
  hideErrorSpace?: boolean,
  disabled?: boolean,
}>()
</script>

<template>
  <NormalInput
      v-model="model"
      type="date"
      append-inner-icon="mdi-calendar-month-outline"
      :error-message="errorMessage"
      :hide-error-space="hideErrorSpace"
      :disabled="disabled"
      placeholder="dd/MM/yyyy"
      :class-name="className"
  />
</template>

<style scoped>

</style>