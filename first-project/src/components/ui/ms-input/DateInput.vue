<script setup lang="ts">
import NormalInput from "@/components/ui/ms-input/NormalInput.vue";

interface DateInputProps {
  className?: string,
  errorMessage?: string,
}
const props = defineProps<DateInputProps>()
</script>

<template>
  <NormalInput
      type="date"
      append-inner-icon="mdi-calendar-month-outline"
      hide-details
      density="compact"
      variant="outlined"
      :error-message="errorMessage"
      placeholder="dd/MM/yyyy"
      :class-name="className"
  />
</template>

<style scoped>

</style>