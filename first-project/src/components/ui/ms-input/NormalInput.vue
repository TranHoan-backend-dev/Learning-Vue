<script setup lang="ts">
import type {InputProps} from "@/components/ui/ms-input/model.ts";

const props = defineProps<InputProps>()

const value = defineModel<string>({
  default: ''
})
</script>

<template>
  <v-text-field
      :label="props.label"
      :placeholder="props.placeholder"
      v-model="value"
      :aria-label="props.label"
  ></v-text-field>
</template>

<style scoped src="./style.css">

</style>