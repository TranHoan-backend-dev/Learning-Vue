<script setup lang="ts">
import {computed} from "vue";

const props = defineProps<{
  label?: string,
  placeholder: string,
  className?: string,
}>()

const message = defineModel<string>({
  default: ''
})

const placeholderPrefix = 'Tìm kiếm '

const mergedPlaceholder = computed(() => {
  return placeholderPrefix + (props.placeholder ?? '')
})
</script>

<template>
  <v-text-field
      :label="props.label"
      :placeholder="mergedPlaceholder"
      v-model="message"
      :aria-label="props.label"
      :class="props.className"
      variant="solo"
      density="compact"
      hide-details
      type="text"
      clearable
  ></v-text-field>
</template>

<style scoped>
:deep(.v-field) {
  border-radius: 10px;
}
</style>