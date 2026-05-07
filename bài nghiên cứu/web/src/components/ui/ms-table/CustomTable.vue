<script setup lang="ts">
import type {BodyProps, HeaderProps} from "@/components/ui/ms-table/model.ts";

const props = defineProps<{
  headerProps: HeaderProps[],
  bodyProps: BodyProps[][]
}>()
</script>

<template>
  <table id="table_user">
    <thead>
    <tr>
      <th
          v-for="(component, index) in props.headerProps"
          :key="index"
          :class="component.className"
      >
        <slot :name="'header-' + (component.slotName || component.className)" :column="component">
          {{ component.value }}
        </slot>
      </th>
    </tr>
    </thead>
    <tbody>
    <tr
        v-for="(component, index) in props.bodyProps"
        :key="index"
    >
      <td
          v-for="(cell, i) in component"
          :key="i"
          :class="cell.tdClassName"
      >
        <slot :name="'cell-' + (cell.slotName || cell.tdClassName)" :cell="cell">
          {{ cell.value }}
        </slot>
      </td>
    </tr>
    </tbody>
  </table>
</template>

<style scoped>

</style>