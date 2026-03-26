<script setup lang="ts">
interface TextareaProps {
  modelValue?: string,
  placeholder?: string,
}

defineProps<TextareaProps>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: string): void
}>()
</script>

<template>
  <textarea
      @change="emit('update:modelValue', ($event.target as HTMLInputElement).value)"
      :placeholder="placeholder"
  ></textarea>
</template>

<style scoped>

</style>