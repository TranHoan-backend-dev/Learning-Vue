<script setup lang="ts">
import type {CheckboxProps} from "@/components/ui/ms-checkbox/model.ts";

const props = defineProps<CheckboxProps>()
</script>

<template>
<v-checkbox-btn
    v-bind="$attrs"
    :label="props.label"
    :color="props.color"
>

</v-checkbox-btn>
</template>

<style scoped>

</style>