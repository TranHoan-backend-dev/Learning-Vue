<script setup lang="ts">

const props = defineProps<{
  label?: string,
  className?: string,
  variant?: 'outlined' | 'tonal' | 'text' | 'plain' | 'flat',
  density?: 'compact' | 'comfortable' | 'default',
  size?: 'x-small' | 'small' | 'large' | 'x-large',
  tooltipLocation?: 'top' | 'bottom' | 'start' | 'end' | 'left' | 'right',
  tooltipContent?: string
}>()
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
    <v-tooltip v-if="tooltipLocation && tooltipContent" activator="parent" :location="tooltipLocation">{{props.tooltipContent}}</v-tooltip>
  </v-btn>
</template>

<style scoped>
:deep(.v-btn__content) {
  justify-content: center;
}

:deep(.v-btn__prepend) {
  position: absolute;
  left: 12px;
}
</style>