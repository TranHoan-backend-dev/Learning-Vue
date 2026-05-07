<script setup lang="ts">

import type {CheckboxProps} from "@/components/ui/ms-checkbox/model.ts";

const props = defineProps<CheckboxProps>()
</script>

<template>
  <v-checkbox
      v-bind="$attrs"
      :label="props.label"
      :color="props.color"
  >

  </v-checkbox>
</template>

<style scoped>

</style>