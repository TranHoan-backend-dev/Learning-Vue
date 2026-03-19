<script setup lang="ts">
import type {ButtonProps} from "@/components/ui/ms-button/model.ts";

const props = defineProps<ButtonProps>()
const emit = defineEmits(['click'])
</script>

<template>
  <v-btn
      v-bind="$attrs"
      :class="props.className"
      :density="props.density ?? 'compact'"
      :size="props.size ?? 'small'"
      :variant="props.variant"
      @click="emit('click')"
  >
    {{ props.label }}
    <slot/>
  </v-btn>
</template>

<style scoped src="./style.css">

</style>