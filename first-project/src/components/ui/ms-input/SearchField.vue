<script setup lang="ts">
import {computed} from "vue";
import type {InputProps} from "@/components/ui/ms-input/model.ts";

const props = defineProps<InputProps>()

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

<style scoped src="./style.css">

</style>